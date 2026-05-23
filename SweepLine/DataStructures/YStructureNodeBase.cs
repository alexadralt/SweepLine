using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public abstract class YStructureNodeBase<TSegment> : NodeWithId
    where TSegment : Segment
{
    public List<TSegment> Value { get; } = [];
    
    public abstract YStructureNodeBase<TSegment>? Next { get; }
    
    public abstract YStructureNodeBase<TSegment>? Previous { get; }
}

public abstract class NodeWithId
{
    public int UniqueId { get; }
    
    protected NodeWithId()
    {
        UniqueId = IdCounter;
        IdCounter = (IdCounter + 1) & ((1 << 30) - 1);
    }

    private static int IdCounter { get; set; }
}
