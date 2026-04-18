using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.DataStructuresLinkedListImpl;

public class YStructure : IYStructure<YStructureNode, XStructureNode>
{
    private YStructureNode? Head { get; set; }
    
    public void ReverseSubSequence((YStructureNode start, YStructureNode end) subsequence)
    {
        if (subsequence.start == subsequence.end)
        {
            return;
        }

        if (subsequence.start == Head)
        {
            Head = subsequence.end;
        }
        
        var rightEdge = subsequence.end.Next;
        var leftEdge = subsequence.start.Previous;

        leftEdge?.Next = subsequence.end;

        var accumulator = rightEdge;
        var current = subsequence.start;

        while (current != rightEdge)
        {
            var nextCurrent = current!.Next;
            
            current.Previous = leftEdge;
            current.Next = accumulator;
            accumulator?.Previous = current;

            accumulator = current;
            current = nextCurrent;
        }
    }

    public YStructureNode FindOrCreateNode(Segment segment, SegmentComparator cmp)
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
            var cmpResult = cmp.Compare(current.Value![0], segment);
            
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
                        Next = oldHead,
                    };
                    oldHead.Previous = Head;

                    return Head;
                }

                prev.Next = new YStructureNode
                {
                    Next = current,
                    Previous = prev,
                };
                current.Previous = prev.Next;
                return prev.Next;
            }

            prev = current;
            current = current.Next;

            if (current is null)
            {
                prev.Next = new YStructureNode
                {
                    Previous = prev,
                };
                return prev.Next;
            }
        }
    }

    public void RemoveNode(YStructureNode node)
    {
        var prev = node.Previous;
        var next = node.Next;
        if (prev is null)
        {
            Head = next;
            next?.Previous = null;
            return;
        }

        prev.Next = next;
        next?.Previous = prev;
    }
}

public class YStructureNode : IYStructureNode<YStructureNode, XStructureNode>
{
    public List<Segment>? Value { get; set; }
    
    public YStructureNode? Next { get; set; }
    
    public YStructureNode? Previous { get; set; }
}
