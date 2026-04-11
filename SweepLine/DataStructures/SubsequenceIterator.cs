using System.Collections;

namespace SweepLine.DataStructures;

public class SubsequenceIterator<TYStructureNode, TEventPoint>((TYStructureNode Start, TYStructureNode End) subsequence)
    : IEnumerable<TYStructureNode>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
{
    public IEnumerator<TYStructureNode> GetEnumerator()
    {
        var current = subsequence.Start;
        while (current != subsequence.End)
        {
            yield return current;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}