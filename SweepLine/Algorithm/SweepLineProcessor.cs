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
            //TODO: make finding a subsequence a job of the SweepLineProcessor
            var subsequence = yStructure.FindIntersectingSegments(eventPoint);

            if (subsequence.HasValue)
            {
                RemoveSegmentsEndingInPoint(eventPoint, subsequence.Value, visitor);

                subsequence = yStructure.FindIntersectingSegments(eventPoint);
                if (subsequence.HasValue)
                {
                    subsequence = ReverseSubsequence(eventPoint, subsequence.Value, visitor);
                }
            }

            AddStartingSegments(eventPoint, visitor);

            if (subsequence.HasValue)
            {
                FindIntersections(subsequence.Value);
            }
        }
    }

    private void RemoveSegmentsEndingInPoint(
        TEventPoint eventPoint,
        (TYStructureNode Start, TYStructureNode End) subsequence,
        ISweepLineVisitor<TEventPoint, TYStructureNode> visitor)
    {
        var segmentsToRemove = new SubsequenceIterator<TYStructureNode, TEventPoint>(subsequence)
            .Where(node => node.Value.EndPoint == eventPoint.Value)
            .ToList();

        visitor.VisitEndingSegments(eventPoint, segmentsToRemove);
                
        foreach (var segment in segmentsToRemove)
        {
            yStructure.RemoveSegment(segment);
        }
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

    private void AddStartingSegments(TEventPoint eventPoint, ISweepLineVisitor<TEventPoint, TYStructureNode> visitor)
    {
        var segmentsToAdd = SegmentStart[eventPoint.Value];
            
        var segmentComparator = new SegmentComparator(eventPoint.Value);
        var insertedNodes = new List<TYStructureNode>(segmentsToAdd.Count);
        foreach (var segment in segmentsToAdd)
        {
            var node = yStructure.InsertSegment(segment, segmentComparator);
            insertedNodes.Add(node);

            InsertEventPoint(segment.EndPoint, node);
        }
            
        visitor.VisitStartingSegments(eventPoint, insertedNodes);
    }

    private void FindIntersections((TYStructureNode Start, TYStructureNode End) subsequence)
    {
        var (start, end) = subsequence;

        var prev = start.Previous;
        var next = end.Next;

        if (prev is not null)
        {
            AddIntersectionEvent(prev, start);
        }

        if (next is not null)
        {
            AddIntersectionEvent(end, next);
        }
    }

    private void AddIntersectionEvent(TYStructureNode bottom, TYStructureNode top)
    {
        var intersection = Segment.FindIntersection(bottom.Value, top.Value);
        if (intersection is { Type: IntersectionType.Point })
        {
            InsertEventPoint(intersection.Point, bottom);
        }
        else if (intersection is { Type: IntersectionType.SubSegment })
        {
            throw new UnreachableException("Parallel segments must merge into a single TYStructureNode when inserted");
        }
    }

    private void InsertEventPoint(Point point, TYStructureNode node)
    {
        var eventPoint = xStructure.FindOrDefault(point);
        if (eventPoint is not null)
        {
            // TODO: handle segments ending in point here because comparator does not handle them
            var comparator = new SegmentComparator(point);
                    
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