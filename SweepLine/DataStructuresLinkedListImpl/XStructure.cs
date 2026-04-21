using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.DataStructuresLinkedListImpl;

public class XStructure<TSegment> : IXStructure<TSegment>
    where TSegment : Segment
{
    private XStructureNode<TSegment>? Head { get; set; }

    public IEventPoint<TSegment> Insert(Point point)
    {
        if (Head is null)
        {
            Head = new XStructureNode<TSegment>
            {
                Value = point,
            };
            return Head;
        }

        var current = Head;
        XStructureNode<TSegment>? prev = null;
        while (true)
        {
            if (current.Value == point)
            {
                return current;
            }

            if (current.Value > point)
            {
                if (prev is null)
                {
                    var nextNode = Head;
                    Head = new XStructureNode<TSegment>
                    {
                        Value = point,
                        Next = nextNode,
                    };

                    return Head;
                }

                prev.Next = new XStructureNode<TSegment>
                {
                    Value = point,
                    Next = current,
                };
                return prev.Next;
            }

            prev = current;
            current = current.Next;

            if (current is null)
            {
                prev.Next = new XStructureNode<TSegment>
                {
                    Value = point,
                };
                return prev.Next;
            }
        }
    }

    public bool Take(out IEventPoint<TSegment> eventPoint)
    {
        var head = Head;
        var next = Head?.Next;
        Head = next;
        
        eventPoint = head!;
        return head is not null;
    }

    public IEventPoint<TSegment>? FindOrDefault(Point point)
    {
        var current = Head;
        while (current is not null)
        {
            if (current.Value == point)
            {
                return current;
            }

            current = current.Next;
        }

        return null;
    }
}

public class XStructureNode<TSegment> : IEventPoint<TSegment>
    where TSegment : Segment
{
    public Point Value { get; init; }
    
    public YStructureNodeBase<TSegment>? MinNode { get; set; }
    
    public YStructureNodeBase<TSegment>? MaxNode { get; set; }

    public XStructureNode<TSegment>? Next { get; set; }
}