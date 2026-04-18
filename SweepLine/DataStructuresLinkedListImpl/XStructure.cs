using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.DataStructuresLinkedListImpl;

public class XStructure : IXStructure
{
    private XStructureNode? Head { get; set; }

    public IEventPoint Insert(Point point)
    {
        if (Head is null)
        {
            Head = new XStructureNode
            {
                Value = point,
            };
            return Head;
        }

        var current = Head;
        XStructureNode? prev = null;
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
                    Head = new XStructureNode
                    {
                        Value = point,
                        Next = nextNode,
                    };

                    return Head;
                }

                prev.Next = new XStructureNode
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
                prev.Next = new XStructureNode
                {
                    Value = point,
                };
                return prev.Next;
            }
        }
    }

    public bool Take(out IEventPoint eventPoint)
    {
        var head = Head;
        var next = Head?.Next;
        Head = next;
        
        eventPoint = head!;
        return head is not null;
    }

    public IEventPoint? FindOrDefault(Point point)
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

public class XStructureNode : IEventPoint
{
    public Point Value { get; init; }
    
    public IYStructureNode? MinNode { get; set; }
    
    public IYStructureNode? MaxNode { get; set; }

    public XStructureNode? Next { get; set; }
}