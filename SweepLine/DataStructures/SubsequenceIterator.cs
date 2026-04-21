using System.Collections;
using SweepLine.Primitives;

namespace SweepLine.DataStructures;

public class SubsequenceIterator<TSegment>(
    (YStructureNodeBase<TSegment> Start, YStructureNodeBase<TSegment> End) subsequence,
    bool reversed = false)
    : IEnumerable<YStructureNodeBase<TSegment>>
    where TSegment : Segment
{
    public IEnumerator<YStructureNodeBase<TSegment>> GetEnumerator()
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