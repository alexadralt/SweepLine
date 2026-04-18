using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IYStructureNode<TYStructureNode, TEventPoint>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
{
    public List<Segment>? Value { get; set; }
    
    public TYStructureNode? Next { get; }
    
    public TYStructureNode? Previous { get; }
}
