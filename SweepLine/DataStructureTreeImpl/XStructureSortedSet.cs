using SweepLine.DataStructures;
using SweepLine.Primitives;

namespace SweepLine.DataStructureTreeImpl;

public class XStructureSortedSet<TSegment> : IXStructure<TSegment>
    where TSegment : Segment
{
    private class EventPoint : IEventPoint<TSegment>
    {
        public Point Value { get; init; }
        public YStructureNodeBase<TSegment>? MinNode { get; set; }
        public YStructureNodeBase<TSegment>? MaxNode { get; set; }
    }

    private SortedSet<EventPoint> Set { get; } =
        new(Comparer<EventPoint>.Create((pointA, pointB) => pointA.Value.CompareTo(pointB.Value)));
    
    public IEventPoint<TSegment> Insert(Point point)
    {
        var eventPoint = new EventPoint
        {
            Value = point,
        };
        
        if (Set.TryGetValue(eventPoint, out var actualValue))
        {
            return actualValue;
        }

        Set.Add(eventPoint);
        return eventPoint;
    }

    public bool Take(out IEventPoint<TSegment> eventPoint)
    {
        var min = Set.Min;
        if (min is null)
        {
            eventPoint = null!;
            return false;
        }
        
        Set.Remove(min);
        eventPoint = min;
        return true;
    }

    public IEventPoint<TSegment>? FindOrDefault(Point point)
    {
        var found = Set.TryGetValue(new EventPoint
        {
            Value = point,
        }, out var eventPoint);

        return found ? eventPoint : null;
    }
}
