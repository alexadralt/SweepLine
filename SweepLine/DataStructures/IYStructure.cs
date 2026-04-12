using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IYStructure<TYStructureNode, TEventPoint>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
{
    public void ReverseSubSequence((TYStructureNode start, TYStructureNode end) subsequence);

    public TYStructureNode InsertSegment(Segment segment, SegmentComparator cmp);

    public void RemoveSegment(TYStructureNode segment);
}