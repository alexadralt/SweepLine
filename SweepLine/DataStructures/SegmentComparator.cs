using System.Diagnostics;
using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public class SegmentComparator(Point point)
{
    public SegmentComparison Compare(Segment segmentA, Segment segmentB)
    {
        var parallelCheck = Segment.FindIntersection(segmentA, segmentB);
        if (parallelCheck is { Type: IntersectionType.SubSegment })
        {
            return SegmentComparison.Overlapping;
        }

        var maxY = Math.Max(segmentA.EndPoint.Y, segmentB.EndPoint.Y);
        var minY = Math.Min(segmentA.StartPoint.Y, segmentB.StartPoint.Y);

        // TODO: maybe instead of creating a segment we should make a method for checking intersection of a segment and a line
        var sweepLineSegment = new Segment
        {
            StartPoint = point with { Y = minY - 10 },
            EndPoint = point with { Y = maxY + 10 }
        };

        var intersectionA = Segment.FindIntersection(sweepLineSegment, segmentA);
        var intersectionB = Segment.FindIntersection(sweepLineSegment, segmentB);

        if (intersectionA.Type == IntersectionType.None || intersectionB.Type == IntersectionType.None)
        {
            throw new ArgumentException("Provided segments must intersect the sweep line");
        }

        if (intersectionA is { Type: IntersectionType.Point } && intersectionB is { Type: IntersectionType.Point })
        {
            var pointA = intersectionA.Point;
            var pointB = intersectionB.Point;
            
            if (Math.Abs(pointA.Y - pointB.Y) < Point.Eps)
            {
                if (!intersectionA.Point.Equals(point))
                {
                    throw new ArgumentException(
                        "Two segments intersecting in one point must intersect at the current event point");
                }

                var ax = segmentA.EndPoint.X - segmentA.StartPoint.X;
                var ay = segmentA.EndPoint.Y - segmentA.StartPoint.Y;

                var bx = segmentB.EndPoint.X - segmentB.StartPoint.X;
                var by = segmentB.EndPoint.Y - segmentB.StartPoint.Y;

                if (Math.Abs(ax) < Point.Eps || Math.Abs(bx) < Point.Eps)
                {
                    throw new UnreachableException(
                        "One of the segments is vertical, but that should not be the case here");
                }

                var coeffA = ay / ax;
                var coeffB = by / bx;

                return coeffA > coeffB ? SegmentComparison.BBeforeA : SegmentComparison.ABeforeB;
            }

            return pointA.Y > pointB.Y
                ? SegmentComparison.BBeforeA
                : SegmentComparison.ABeforeB;
        }

        if (intersectionA is { Type: IntersectionType.Point } && intersectionB is { Type: IntersectionType.SubSegment })
        {
            return IsAbove(intersectionA.Point)
                ? SegmentComparison.BBeforeA
                : SegmentComparison.ABeforeB;
        }

        if (intersectionA is { Type: IntersectionType.SubSegment } && intersectionB is { Type: IntersectionType.Point })
        {
            return IsAbove(intersectionB.Point)
                ? SegmentComparison.ABeforeB
                : SegmentComparison.BBeforeA;
        }

        throw new UnreachableException("Both segments are vertical, but we must have caught that at the start");
    }

    private bool IsAbove(Point intersectionPoint)
        => !intersectionPoint.Equals(point) && intersectionPoint.Y > point.Y;
}

public enum SegmentComparison
{
    Overlapping,
    ABeforeB,
    BBeforeA,
}