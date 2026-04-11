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
            
            xStructure.Insert(segment.StartPoint, null);
        }
    }

    public void Process(ISweepLineVisitor<TEventPoint, TYStructureNode> visitor)
    {
        while (xStructure.Take(out var eventPoint))
        {
            var subsequence = yStructure.FindIntersectingSegments(eventPoint);

            var segmentsToRemove = new SubsequenceIterator<TYStructureNode, TEventPoint>(subsequence)
                .Where(node => node.Value.EndPoint == eventPoint.Value)
                .ToList();
            
            visitor.VisitEndingSegments(eventPoint, segmentsToRemove);
            
            yStructure.ReverseSubSequence(subsequence);
            
            foreach (var segment in segmentsToRemove)
            {
                yStructure.RemoveSegment(segment);
            }

            var toVisit = new SubsequenceIterator<TYStructureNode, TEventPoint>(subsequence)
                .Except(segmentsToRemove)
                .ToList();
            
            visitor.VisitSubsequence(eventPoint, toVisit);

            var segmentsToAdd = SegmentStart[eventPoint.Value];
            
            var segmentComparator = new SegmentComparator(eventPoint.Value);
            var insertedNodes = new List<TYStructureNode>(segmentsToAdd.Count);
            foreach (var segment in segmentsToAdd)
            {
                var node = yStructure.InsertSegment(segment, segmentComparator);
                insertedNodes.Add(node);
                
                xStructure.Insert(segment.EndPoint, node);
            }
            
            visitor.VisitStartingSegments(eventPoint, insertedNodes);

            // TODO: find intersections
        }
    }
}