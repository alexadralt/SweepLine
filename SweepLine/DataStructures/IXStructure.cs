using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public interface IXStructure
{
    public IEventPoint Insert(Point point);

    public bool Take(out IEventPoint eventPoint);

    public IEventPoint? FindOrDefault(Point point);
}