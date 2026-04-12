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
                subsequence = RemoveSegmentsEndingInPoint(eventPoint, subsequence.Value, visitor);

                if (subsequence.HasValue)
                {
                    subsequence = ReverseSubsequence(eventPoint, subsequence.Value, visitor);
                }
            }

            var segmentComparator = new SegmentComparator(eventPoint.Value);
            AddStartingSegments(eventPoint, visitor, segmentComparator);

            if (subsequence.HasValue)
            {
                FindIntersections(subsequence.Value, eventPoint, segmentComparator);
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

    private (TYStructureNode Start, TYStructureNode End)? RemoveSegmentsEndingInPoint(
        TEventPoint eventPoint,
        (TYStructureNode Start, TYStructureNode End) subsequence,
        ISweepLineVisitor<TEventPoint, TYStructureNode> visitor)
    {
        var segmentsToRemove = new List<TYStructureNode>();
        var segmentsToKeep = new List<TYStructureNode>();

        foreach (var node in new SubsequenceIterator<TYStructureNode, TEventPoint>(subsequence))
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

        return segmentsToKeep.Count > 0 ? (segmentsToKeep[0], segmentsToKeep[^1]) : null;
    }

    private (TYStructureNode Start, TYStructureNode End) ReverseSubsequence(
        TEventPoint eventPoint,
        (TYStructureNode Start, TYStructureNode End) subsequence,
        ISweepLineVisitor<TEventPoint, TYStructureNode> visitor)
    {
        yStructure.ReverseSubSequence(subsequence);

        // note(shevyrin): after reversal of the subsequence start and end pointers are backwards
        subsequence = (subsequence.End, subsequence.Start);

        visitor.VisitSubsequence(eventPoint,
            new SubsequenceIterator<TYStructureNode, TEventPoint>(subsequence));

        return subsequence;
    }

    private void AddStartingSegments(
        TEventPoint eventPoint,
        ISweepLineVisitor<TEventPoint, TYStructureNode> visitor,
        SegmentComparator segmentComparator)
    {
        var segmentsToAdd = SegmentStart[eventPoint.Value];
            
        var insertedNodes = new List<TYStructureNode>(segmentsToAdd.Count);
        foreach (var segment in segmentsToAdd)
        {
            var node = yStructure.InsertSegment(segment, segmentComparator);
            insertedNodes.Add(node);

            InsertEventPoint(segment.EndPoint, node, segmentComparator);
        }
            
        visitor.VisitStartingSegments(eventPoint, insertedNodes);
    }

    private void FindIntersections(
        (TYStructureNode Start, TYStructureNode End) subsequence,
        TEventPoint eventPoint,
        SegmentComparator comparator)
    {
        var (start, end) = subsequence;

        var prev = start.Previous;
        var next = end.Next;

        if (prev is not null)
        {
            AddIntersectionEvent(prev, start, eventPoint, comparator);
        }

        if (next is not null)
        {
            AddIntersectionEvent(end, next, eventPoint, comparator);
        }
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