using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IYStructure<TSegment>
    where TSegment : Segment
{
    public void ReverseSubSequence((YStructureNodeBase<TSegment> start, YStructureNodeBase<TSegment> end) subsequence);

    public YStructureNodeBase<TSegment> FindOrCreateNode(Segment segment, SegmentComparator cmp);

    public void RemoveNode(YStructureNodeBase<TSegment> nodeBase);
}