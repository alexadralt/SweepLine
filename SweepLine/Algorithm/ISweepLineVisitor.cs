using SweepLine.Primitives;

namespace SweepLine.Algorithm;

public interface ISweepLineVisitor
{
    public void VisitIntersectingSegments(Point point, IEnumerable<IEnumerable<Segment>> segments);
}