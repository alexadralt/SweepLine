using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.Algorithm;

public interface ISweepLineVisitor<TEventPoint, TYStructureNode>
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
{
    public void VisitEndingSegments(TEventPoint eventPoint, IEnumerable<TYStructureNode> segments);
    
    public void VisitSubsequence(TEventPoint eventPoint, IEnumerable<TYStructureNode> subsequence);

    public void VisitStartingSegments(TEventPoint eventPoint, IEnumerable<TYStructureNode> segments);
}