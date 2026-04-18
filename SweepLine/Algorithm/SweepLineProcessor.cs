using System.Diagnostics;
using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.Algorithm;

public class SweepLineProcessor<TXStructure, TYStructure, TEventPoint, TYStructureNode>(
    TXStructure xStructure,
    TYStructure yStructure)
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
    where TXStructure : class, IXStructure<TEventPoint, TYStructureNode>
    where TYStructure : class, IYStructure<TYStructureNode, TEventPoint>
{
    private Dictionary<Point, List<Segment>> SegmentStart { get; } = new();
    
    public void AddSegments(IEnumerable<Segment> segments)
    {
        foreach (var segment in segments)
        {
            var startingSegments = SegmentStart.GetValueOrDefault(segment.StartPoint);
            startingSegments?.Add(segment);
            SegmentStart[segment.StartPoint] = startingSegments ?? [segment];
            
            xStructure.Insert(segment.StartPoint);
        }
    }

    public void Process(ISweepLineVisitor<TEventPoint, TYStructureNode> visitor)
    {
        while (xStructure.Take(out var eventPoint))
        {
            var subsequence = FindSubsequence(eventPoint);

            var intersectingSegments = new List<List<Segment>>();
            
            // note(shevyrin): we need to defer removal of overlapping segments that end in current point, so that we can pass them correctly to the visitor
            var overlappingSegmentsToRemove = new List<(TYStructureNode, List<int>)>();

            if (subsequence.HasValue)
            {
                var nodesToRemove = new List<TYStructureNode>();
                var nodesToKeep = new List<TYStructureNode>();

                foreach (var node in new SubsequenceIterator<TYStructureNode, TEventPoint>(subsequence.Value))
                {
                    if (node.Value![0].EndPoint == eventPoint.Value)
                    {
                        nodesToRemove.Add(node);
                    }
                    else
                    {
                        nodesToKeep.Add(node);
                    }
                }

                if (nodesToRemove.Count > 0)
                {
                    foreach (var node in nodesToRemove)
                    {
                        intersectingSegments.Add(node.Value!);
                        yStructure.RemoveNode(node);
                    }
                }

                subsequence = nodesToKeep.Count > 0 ? (nodesToKeep[0], nodesToKeep[^1]) : null;

                if (subsequence.HasValue)
                {
                    yStructure.ReverseSubSequence(subsequence.Value);

                    // note(shevyrin): after reversal of the subsequence start and end pointers are backwards
                    subsequence = (subsequence.Value.End, subsequence.Value.Start);

                    eventPoint.Referenced = subsequence.Value.Start;

                    foreach (var node in new SubsequenceIterator<TYStructureNode, TEventPoint>(subsequence.Value))
                    {
                        var indicesToRemove = new List<int>();
                        
                        for (var i = 0; i < node.Value!.Count; i++)
                        {
                            var segment = node.Value![i];
                            if (segment.EndPoint == eventPoint.Value)
                            {
                                indicesToRemove.Add(i);
                            }
                        }

                        if (indicesToRemove.Count > 0)
                        {
                            overlappingSegmentsToRemove.Add((node, indicesToRemove));
                        }
                        
                        intersectingSegments.Add(node.Value!);
                    }
                }
                else
                {
                    eventPoint.Referenced = null;
                }
            }

            var segmentComparator = new SegmentComparator(eventPoint.Value);
            var segmentsToAdd = SegmentStart.GetValueOrDefault(eventPoint.Value);
            if (segmentsToAdd is not null)
            {
                var insertedNodes = new List<TYStructureNode>();
                
                foreach (var segment in segmentsToAdd)
                {
                    var node = yStructure.FindOrCreateNode(segment, segmentComparator);
                    if (node.Value is null)
                    {
                        node.Value = [segment];
                        insertedNodes.Add(node);
                    }
                    else
                    {
                        node.Value.Add(segment);
                        if (segment.EndPoint > node.Value[0].EndPoint)
                        {
                            (node.Value[0], node.Value[^1]) = (node.Value[^1], node.Value[0]);
                        }
                    }

                    InsertEventPoint(segment.EndPoint, node, segmentComparator);

                    // note(shevyrin): update references here so that we can check for intersections correctly in a later step
                    if (eventPoint.Referenced == null ||
                        segmentComparator.Compare(eventPoint.Referenced.Value![0], segment) == SegmentComparison.BBeforeA)
                    {
                        eventPoint.Referenced = node;
                    }
                }
            
                intersectingSegments.AddRange(insertedNodes.Select(node => node.Value!));
            }
            
            subsequence = FindSubsequence(eventPoint);

            if (subsequence.HasValue)
            {
                var (start, end) = subsequence.Value;

                var prev = start.Previous;
                var next = end.Next;

                if (prev is not null)
                {
                    AddIntersectionEvent(prev, start, eventPoint, segmentComparator);
                }

                if (next is not null)
                {
                    AddIntersectionEvent(end, next, eventPoint, segmentComparator);
                }
            }
            
            visitor.VisitIntersectingSegments(eventPoint.Value, intersectingSegments);

            foreach (var (node, indicesToRemove) in overlappingSegmentsToRemove)
            {
                foreach (var i in indicesToRemove)
                {
                    node.Value!.RemoveAt(i);
                }
            }
        }
    }

    private (TYStructureNode Start, TYStructureNode End)? FindSubsequence(TEventPoint eventPoint)
    {
        var start = eventPoint.Referenced;
        if (start is null)
        {
            return null;
        }

        var current = start;
        var end = current;
        while (true)
        {
            current = current.Next;
            
            if (!current?.Value![0].ContainsPoint(eventPoint.Value) ?? true)
            {
                break;
            }

            end = current;
        }

        return (start, end);
    }

    private void AddIntersectionEvent(
        TYStructureNode bottom,
        TYStructureNode top,
        TEventPoint eventPoint,
        SegmentComparator comparator)
    {
        var intersection = Segment.FindIntersection(bottom.Value![0], top.Value![0]);
        if (intersection is { Type: IntersectionType.Point } && intersection.Point > eventPoint.Value)
        {
            InsertEventPoint(intersection.Point, bottom, comparator);
        }
        else if (intersection is { Type: IntersectionType.SubSegment })
        {
            throw new UnreachableException(
                "Overlapping segments must merge into a single TYStructureNode when inserted");
        }
    }

    private void InsertEventPoint(Point point, TYStructureNode node, SegmentComparator comparator)
    {
        var eventPoint = xStructure.FindOrDefault(point);
        if (eventPoint is not null)
        {
            var comparison = eventPoint.Referenced != null
                ? comparator.Compare(node.Value![0], eventPoint.Referenced.Value![0])
                : SegmentComparison.ABeforeB;
                    
            if (comparison == SegmentComparison.ABeforeB)
            {
                eventPoint.Referenced = node;
            }
        }
        else
        {
            var endPoint = xStructure.Insert(point);
            endPoint.Referenced = node;
        }
    }
}