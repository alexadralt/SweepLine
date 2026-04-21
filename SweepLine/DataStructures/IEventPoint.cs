using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IEventPoint<TSegment>
    where TSegment : Segment
{
    public Point Value { get; }
    
    public YStructureNodeBase<TSegment>? MinNode { get; set; }
    
    public YStructureNodeBase<TSegment>? MaxNode { get; set; }
}