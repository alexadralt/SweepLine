using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IEventPoint<TEventPoint, TYStructureNode>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
{
    public Point Value { get; }
    
    public TYStructureNode Referenced { get; set; }
}