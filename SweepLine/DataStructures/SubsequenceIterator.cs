using System.Collections;

namespace SweepLine.DataStructures;

public class SubsequenceIterator(
    (YStructureNodeBase Start, YStructureNodeBase End) subsequence,
    bool reversed = false)
    : IEnumerable<YStructureNodeBase>
{
    public IEnumerator<YStructureNodeBase> GetEnumerator()
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