using SweepLine.Primitives;

namespace FaceBoundary;

public struct HalfEdge()
{
    public int NextIndex { get; set; } = -1;

    public int TwinIndex { get; init; } = -1;
    
    public Point OriginPoint { get; set; } = default;

    public Point DestinationPoint { get; set; } = default;
}