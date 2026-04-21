using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public abstract class YStructureNodeBase
{
    public List<Segment> Value { get; } = [];
    
    public abstract YStructureNodeBase? Next { get; }
    
    public abstract YStructureNodeBase? Previous { get; }
}
