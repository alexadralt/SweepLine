using System.Diagnostics;
using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.Algorithm;

public class SweepLineProcessor(IXStructure xStructure, IYStructure yStructure)
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

    public void Process(ISweepLineVisitor visitor)
    {
        while (xStructure.Take(out var eventPoint))
        {
            var subsequence = (eventPoint.MinNode, eventPoint.MaxNode);

            var intersectingSegments = new List<List<Segment>>();
            
            // note(shevyrin): we need to defer removal of overlapping segments that end in current point, so that we can pass them correctly to the visitor
            var overlappingSegmentsToRemove = new List<(YStructureNodeBase, List<int>)>();

            if (subsequence.MinNode is not null)
            {
                var nodesToRemove = new List<YStructureNodeBase>();
                var nodesToKeep = new List<YStructureNodeBase>();

                foreach (var node in new SubsequenceIterator(subsequence!))
                {
                    if (node.Value[0].EndPoint == eventPoint.Value)
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
                        intersectingSegments.Add(node.Value);
                        yStructure.RemoveNode(node);
                    }
                }

                subsequence = nodesToKeep.Count > 0 ? (nodesToKeep[0], nodesToKeep[^1]) : (null, null);

                if (subsequence.MinNode is not null)
                {
                    yStructure.ReverseSubSequence(subsequence!);
                    subsequence = (subsequence.MaxNode, subsequence.MinNode);

                    eventPoint.MinNode = subsequence.MinNode;
                    eventPoint.MaxNode = subsequence.MaxNode;

                    foreach (var node in new SubsequenceIterator(subsequence!))
                    {
                        var indicesToRemove = new List<int>();
                        
                        for (var i = 0; i < node.Value.Count; i++)
                        {
                            var segment = node.Value[i];
                            if (segment.EndPoint == eventPoint.Value)
                            {
                                indicesToRemove.Add(i);
                            }
                        }

                        if (indicesToRemove.Count > 0)
                        {
                            overlappingSegmentsToRemove.Add((node, indicesToRemove));
                        }
                        
                        intersectingSegments.Add(node.Value);
                    }
                }
                else
                {
                    eventPoint.MinNode = null;
                    eventPoint.MaxNode = null;
                }
            }

            var segmentComparator = new SegmentComparator(eventPoint.Value);
            var segmentsToAdd = SegmentStart.GetValueOrDefault(eventPoint.Value);
            if (segmentsToAdd is not null)
            {
                var insertedNodes = new HashSet<YStructureNodeBase>(
                    EqualityComparer<YStructureNodeBase>.Create(
                        (nodeA, nodeB) => nodeA?.Value[0] == nodeB?.Value[0],
                        node => node.Value[0].GetHashCode()));
                
                foreach (var segment in segmentsToAdd)
                {
                    var node = yStructure.FindOrCreateNode(segment, segmentComparator);
                    
                    node.Value.Add(segment);
                    insertedNodes.Add(node);
                    
                    if (segment.EndPoint > node.Value[0].EndPoint)
                    {
                        //note(shevyrin): since we use first segment as a key, we have to reinsert this node when we swap first segment with new one
                        insertedNodes.Remove(node);
                        (node.Value[0], node.Value[^1]) = (node.Value[^1], node.Value[0]);
                        insertedNodes.Add(node);
                    }

                    InsertEventPoint(segment.EndPoint, node, node, segmentComparator);

                    // note(shevyrin): update references here so that we can check for intersections correctly in a later step
                    if (eventPoint.MinNode == null ||
                        segmentComparator.Compare(eventPoint.MinNode.Value[0], segment) == SegmentComparison.BBeforeA)
                    {
                        eventPoint.MinNode = node;
                    }

                    if (eventPoint.MaxNode == null ||
                        segmentComparator.Compare(eventPoint.MaxNode.Value[0], segment) == SegmentComparison.ABeforeB)
                    {
                        eventPoint.MaxNode = node;
                    }
                }
            
                intersectingSegments.AddRange(insertedNodes.Select(node => node.Value));
            }
            
            subsequence = (eventPoint.MinNode, eventPoint.MaxNode);

            if (subsequence.MinNode is not null)
            {
                var (start, end) = subsequence;

                var prev = start.Previous;
                var next = end!.Next;

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

    private void AddIntersectionEvent(
        YStructureNodeBase bottom,
        YStructureNodeBase top,
        IEventPoint eventPoint,
        SegmentComparator comparator)
    {
        var intersection = Segment.FindIntersection(bottom.Value[0], top.Value[0]);
        if (intersection is { Type: IntersectionType.Point } && intersection.Point > eventPoint.Value)
        {
            InsertEventPoint(intersection.Point, bottom, top, comparator);
        }
        else if (intersection is { Type: IntersectionType.SubSegment })
        {
            throw new UnreachableException(
                "Overlapping segments must merge into a single TYStructureNode when inserted");
        }
    }

    private void InsertEventPoint(Point point, YStructureNodeBase bottom, YStructureNodeBase top, SegmentComparator comparator)
    {
        var eventPoint = xStructure.FindOrDefault(point);
        if (eventPoint is not null)
        {
            var comparison = eventPoint.MinNode != null
                ? comparator.Compare(bottom.Value[0], eventPoint.MinNode.Value[0])
                : SegmentComparison.ABeforeB;
                    
            if (comparison == SegmentComparison.ABeforeB)
            {
                eventPoint.MinNode = bottom;
            }

            comparison = eventPoint.MaxNode != null
                ? comparator.Compare(top.Value[0], eventPoint.MaxNode.Value[0])
                : SegmentComparison.BBeforeA;

            if (comparison == SegmentComparison.BBeforeA)
            {
                eventPoint.MaxNode = top;
            }
        }
        else
        {
            var endPoint = xStructure.Insert(point);
            endPoint.MinNode = bottom;
            endPoint.MaxNode = top;
        }
    }
}