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
        
        var rightEdge = subsequence.end.Next;
        var leftEdge = subsequence.start.Previous;

        subsequence.end.Previous = leftEdge;
        
        var newEnd = subsequence.start;
        newEnd.Next = rightEdge;
        newEnd.Previous = subsequence.end;
        
        var current = subsequence.start.Next;
        while (current != null && current != subsequence.end)
        {
            current.Next = newEnd;
            newEnd = current;
            newEnd.Previous = subsequence.end;
            subsequence.end.Next = newEnd;
        }
    }

    public YStructureNode InsertSegment(Segment segment, SegmentComparator cmp)
    {
        if (Head is null)
        {
            Head = new YStructureNode
            {
                Value = segment,
            };
            return Head;
        }

        var current = Head;
        YStructureNode? prev = null;
        while (true)
        {
            var cmpResult = cmp.Compare(current.Value, segment);
            
            if (cmpResult == SegmentComparison.Overlapping && segment.EndPoint > current.Value.EndPoint)
            {
                if (prev is null)
                {
                    var headNext = Head.Next;
                    Head = new YStructureNode
                    {
                        Value = segment,
                        Next = headNext,
                    };
                    headNext?.Previous = Head;
                    return Head;
                }

                var next = current.Next;
                prev.Next = new YStructureNode
                {
                    Value = segment,
                    Next = next,
                    Previous = prev,
                };
                next?.Previous = prev.Next;
                return prev.Next;
            }

            if (cmpResult == SegmentComparison.BBeforeA)
            {
                if (prev is null)
                {
                    var oldHead = Head;
                    Head = new YStructureNode
                    {
                        Value = segment,
                        Next = oldHead,
                    };
                    oldHead.Previous = Head;

                    return Head;
                }

                prev.Next = new YStructureNode
                {
                    Value = segment,
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
                    Value = segment,
                    Previous = prev,
                };
                return prev.Next;
            }
        }
    }

    public void RemoveSegment(YStructureNode segment)
    {
        var prev = segment.Previous;
        var next = segment.Next;
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
    public Segment Value { get; set; }
    
    public XStructureNode? Referenced { get; }
    
    public YStructureNode? Next { get; set; }
    
    public YStructureNode? Previous { get; set; }
}
