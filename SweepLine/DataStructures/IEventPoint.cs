using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IEventPoint
{
    public Point Value { get; }
    
    public YStructureNodeBase? MinNode { get; set; }
    
    public YStructureNodeBase? MaxNode { get; set; }
}