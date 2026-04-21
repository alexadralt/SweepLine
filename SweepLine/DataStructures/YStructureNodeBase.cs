using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public abstract class YStructureNodeBase<TSegment>
    where TSegment : Segment
{
    public List<TSegment> Value { get; } = [];
    
    public abstract YStructureNodeBase<TSegment>? Next { get; }
    
    public abstract YStructureNodeBase<TSegment>? Previous { get; }
}
