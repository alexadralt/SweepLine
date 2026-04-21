using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IXStructure<TSegment>
    where TSegment : Segment
{
    public IEventPoint<TSegment> Insert(Point point);

    public bool Take(out IEventPoint<TSegment> eventPoint);

    public IEventPoint<TSegment>? FindOrDefault(Point point);
}