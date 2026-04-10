using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IYStructureNode
{
    public Segment Value { get; }
    
    public IYStructureNode Next { get; }
    
    public IEventPoint Referenced { get; }
}