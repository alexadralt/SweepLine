using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IYStructure<TYStructureNode, in TEventPoint>
    where TYStructureNode : IYStructureNode
    where TEventPoint : IEventPoint
{
    public (TYStructureNode Start, TYStructureNode End) FindIntersectingSegments(TEventPoint eventPoint);

    public void InsertSegment(Segment segment, SegmentComparator<TYStructureNode> cmp);

    public void RemoveSegment(TYStructureNode node);
}