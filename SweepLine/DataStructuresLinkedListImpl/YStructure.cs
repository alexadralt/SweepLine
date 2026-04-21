using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.DataStructuresLinkedListImpl;

public class YStructure<TSegment> : IYStructure<TSegment>
    where TSegment : Segment
{
    private YStructureNode<TSegment>? Head { get; set; }
    
    public void ReverseSubSequence((YStructureNodeBase<TSegment> start, YStructureNodeBase<TSegment> end) subsequence)
    {
        if (subsequence.start == subsequence.end)
        {
            return;
        }

        if (subsequence.start == Head)
        {
            Head = (YStructureNode<TSegment>)subsequence.end;
        }
        
        var rightEdge = ((YStructureNode<TSegment>)subsequence.end).NextNode;
        var leftEdge = ((YStructureNode<TSegment>)subsequence.start).PreviousNode;

        leftEdge?.NextNode = (YStructureNode<TSegment>)subsequence.end;

        var accumulator = rightEdge;
        var current = (YStructureNode<TSegment>)subsequence.start;

        while (current != rightEdge)
        {
            var nextCurrent = current!.NextNode;
            
            current.PreviousNode = leftEdge;
            current.NextNode = accumulator;
            accumulator?.PreviousNode = current;

            accumulator = current;
            current = nextCurrent;
        }
    }

    public YStructureNodeBase<TSegment> FindOrCreateNode(Segment segment, SegmentComparator cmp)
    {
        if (Head is null)
        {
            Head = new YStructureNode<TSegment>();
            return Head;
        }

        var current = Head;
        YStructureNode<TSegment>? prev = null;
        while (true)
        {
            var cmpResult = cmp.Compare(current.Value[0], segment);
            
            if (cmpResult == SegmentComparison.Overlapping)
            {
                return current;
            }

            if (cmpResult == SegmentComparison.BBeforeA)
            {
                if (prev is null)
                {
                    var oldHead = Head;
                    Head = new YStructureNode<TSegment>
                    {
                        NextNode = oldHead,
                    };
                    oldHead.PreviousNode = Head;

                    return Head;
                }

                prev.NextNode = new YStructureNode<TSegment>
                {
                    NextNode = current,
                    PreviousNode = prev,
                };
                current.PreviousNode = prev.NextNode;
                return prev.NextNode;
            }

            prev = current;
            current = current.NextNode;

            if (current is null)
            {
                prev.NextNode = new YStructureNode<TSegment>
                {
                    PreviousNode = prev,
                };
                return prev.NextNode;
            }
        }
    }

    public void RemoveNode(YStructureNodeBase<TSegment> nodeBase)
    {
        var yStructureNode = (YStructureNode<TSegment>)nodeBase;
        var prev = yStructureNode.PreviousNode;
        var next = yStructureNode.NextNode;
        if (prev is null)
        {
            Head = next;
            next?.PreviousNode = null;
            return;
        }

        prev.NextNode = next;
        next?.PreviousNode = prev;
    }
}

public class YStructureNode<TSegment> : YStructureNodeBase<TSegment>
    where TSegment : Segment
{
    public YStructureNode<TSegment>? NextNode { get; set; }
    
    public YStructureNode<TSegment>? PreviousNode { get; set; }
    
    public override YStructureNodeBase<TSegment>? Next => NextNode;

    public override YStructureNodeBase<TSegment>? Previous => PreviousNode;
}
