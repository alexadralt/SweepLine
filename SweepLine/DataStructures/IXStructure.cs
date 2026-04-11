using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IXStructure<TEventPoint, TYStructureNode>
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
{
    public TEventPoint Insert(Point point, TYStructureNode? referenced);

    public bool Take(out TEventPoint eventPoint);
}