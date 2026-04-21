using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.DataStructuresLinkedListImpl;

public class YStructure : IYStructure
{
    private YStructureNode? Head { get; set; }
    
    public void ReverseSubSequence((YStructureNodeBase start, YStructureNodeBase end) subsequence)
    {
        if (subsequence.start == subsequence.end)
        {
            return;
        }

        if (subsequence.start == Head)
        {
            Head = (YStructureNode)subsequence.end;
        }
        
        var rightEdge = ((YStructureNode)subsequence.end).NextNode;
        var leftEdge = ((YStructureNode)subsequence.start).PreviousNode;

        leftEdge?.NextNode = (YStructureNode)subsequence.end;

        var accumulator = rightEdge;
        var current = (YStructureNode)subsequence.start;

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

    public YStructureNodeBase FindOrCreateNode(Segment segment, SegmentComparator cmp)
    {
        if (Head is null)
        {
            Head = new YStructureNode();
            return Head;
        }

        var current = Head;
        YStructureNode? prev = null;
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
                    Head = new YStructureNode
                    {
                        NextNode = oldHead,
                    };
                    oldHead.PreviousNode = Head;

                    return Head;
                }

                prev.NextNode = new YStructureNode
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
                prev.NextNode = new YStructureNode
                {
                    PreviousNode = prev,
                };
                return prev.NextNode;
            }
        }
    }

    public void RemoveNode(YStructureNodeBase nodeBase)
    {
        var yStructureNode = (YStructureNode)nodeBase;
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

public class YStructureNode : YStructureNodeBase
{
    public YStructureNode? NextNode { get; set; }
    
    public YStructureNode? PreviousNode { get; set; }
    
    public override YStructureNodeBase? Next => NextNode;

    public override YStructureNodeBase? Previous => PreviousNode;
}
