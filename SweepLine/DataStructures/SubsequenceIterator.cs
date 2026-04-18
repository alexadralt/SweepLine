using System.Collections;

namespace SweepLine.DataStructures;

public class SubsequenceIterator(
    (IYStructureNode Start, IYStructureNode End) subsequence,
    bool reversed = false)
    : IEnumerable<IYStructureNode>
{
    public IEnumerator<IYStructureNode> GetEnumerator()
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