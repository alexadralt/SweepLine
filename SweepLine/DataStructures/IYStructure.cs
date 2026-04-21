using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IYStructure
{
    public void ReverseSubSequence((YStructureNodeBase start, YStructureNodeBase end) subsequence);

    public YStructureNodeBase FindOrCreateNode(Segment segment, SegmentComparator cmp);

    public void RemoveNode(YStructureNodeBase nodeBase);
}