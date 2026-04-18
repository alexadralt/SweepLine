using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IYStructure
{
    public void ReverseSubSequence((IYStructureNode start, IYStructureNode end) subsequence);

    public IYStructureNode FindOrCreateNode(Segment segment, SegmentComparator cmp);

    public void RemoveNode(IYStructureNode node);
}