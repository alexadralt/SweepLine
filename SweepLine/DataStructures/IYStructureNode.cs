using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IYStructureNode<TYStructureNode, TEventPoint>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
{
    public Segment Value { get; }
    
    public TYStructureNode? Next { get; }
    
    public TYStructureNode? Previous { get; }
    
    public int UniqueId { get; }
}
