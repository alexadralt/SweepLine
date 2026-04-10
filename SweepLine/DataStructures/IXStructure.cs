using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IXStructure<out TEventPoint> where TEventPoint : IEventPoint
{
    public void Insert(Point point);

    public TEventPoint Take();
}