using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.Algorithm;

public interface ISweepLineVisitor<TEventPoint, TYStructureNode>
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
{
    public void VisitIntersectingSegments(Point point, IEnumerable<IEnumerable<Segment>> segments);
}