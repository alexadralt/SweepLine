using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IEventPoint
{
    public Point Value { get; }
    
    public IYStructureNode? Referenced { get; set; }
}