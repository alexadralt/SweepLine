using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IEventPoint
{
    public Point Value { get; }
    
    public IYStructureNode? MinNode { get; set; }
    
    public IYStructureNode? MaxNode { get; set; }
}