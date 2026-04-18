using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IYStructureNode
{
    public List<Segment>? Value { get; set; }
    
    public IYStructureNode? Next { get; }
    
    public IYStructureNode? Previous { get; }
}
