using SweepLine.Primitives;

namespace SweepLine.Algorithm;

public interface ISweepLineVisitor<TSegment>
    where TSegment : Segment
{
    public void VisitIntersectingSegments(Point point, IEnumerable<IEnumerable<TSegment>> segments);
}