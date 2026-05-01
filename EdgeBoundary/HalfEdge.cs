using SweepLine.Primitives;

namespace EdgeBoundary;

public struct HalfEdge
{
    public int NextIndex { get; set; }
    
    public int TwinIndex { get; set; }
    
    public Point OriginPoint { get; set; }
    
    public Point DestinationPoint { get; set; }
}