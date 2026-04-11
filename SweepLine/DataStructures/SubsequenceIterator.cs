using System.Collections;

namespace SweepLine.DataStructures;

public class SubsequenceIterator<TYStructureNode, TEventPoint>(
    (TYStructureNode Start, TYStructureNode End) subsequence,
    bool reversed = false)
    : IEnumerable<TYStructureNode>
    where TYStructureNode : class, IYStructureNode<TYStructureNode, TEventPoint>
    where TEventPoint : class, IEventPoint<TEventPoint, TYStructureNode>
{
    public IEnumerator<TYStructureNode> GetEnumerator()
    {
        var start = reversed ? subsequence.End : subsequence.Start;
        var end = reversed ? subsequence.Start : subsequence.End;
        
        var current = start;
        
        while(true)
        {
            yield return current;
            
            if (current == end)
            {
                break;
            }
            
            current = reversed ? current.Previous! : current.Next!;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}