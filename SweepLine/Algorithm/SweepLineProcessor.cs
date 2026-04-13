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

            if (subsequence.HasValue)
            {
                var segmentsToRemove = new List<TYStructureNode>();
                var segmentsToKeep = new List<TYStructureNode>();

                foreach (var node in new SubsequenceIterator<TYStructureNode, TEventPoint>(subsequence.Value))
                {
                    if (node.Value.EndPoint == eventPoint.Value)
                    {
                        segmentsToRemove.Add(node);
                    }
                    else
                    {
                        segmentsToKeep.Add(node);
                    }
                }

                visitor.VisitEndingSegments(eventPoint, segmentsToRemove);
        
                foreach (var segment in segmentsToRemove)
                {
                    yStructure.RemoveSegment(segment);
                }

                subsequence = segmentsToKeep.Count > 0 ? (segmentsToKeep[0], segmentsToKeep[^1]) : null;

                if (subsequence.HasValue)
                {
                    yStructure.ReverseSubSequence(subsequence.Value);

                    // note(shevyrin): after reversal of the subsequence start and end pointers are backwards
                    subsequence = (subsequence.Value.End, subsequence.Value.Start);

                    visitor.VisitSubsequence(eventPoint,
                        new SubsequenceIterator<TYStructureNode, TEventPoint>(subsequence.Value));
                }
            }

            var segmentComparator = new SegmentComparator(eventPoint.Value);
            var segmentsToAdd = SegmentStart.GetValueOrDefault(eventPoint.Value);
            if (segmentsToAdd is null)
            {
                return;
            }
            
            var insertedNodes = new List<TYStructureNode>(segmentsToAdd.Count);
            foreach (var segment in segmentsToAdd)
            {
                var node = yStructure.InsertSegment(segment, segmentComparator);
                insertedNodes.Add(node);

                InsertEventPoint(segment.EndPoint, node, segmentComparator);
            }
            
            visitor.VisitStartingSegments(eventPoint, insertedNodes);

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
            
            if (!current?.Value.ContainsPoint(eventPoint.Value) ?? true)
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
        var intersection = Segment.FindIntersection(bottom.Value, top.Value);
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
                ? comparator.Compare(node.Value, eventPoint.Referenced.Value)
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