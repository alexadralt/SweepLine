using System.Collections;
using SweepLine.Primitives;

namespace FaceBoundary;

public class FaceBoundaryIterator(List<HalfEdge> halfEdges) : IEnumerable<List<Segment>>
{
    private struct NotVisitedArray
    {
        private (int value, int movedTo)[] Array { get; }
        private int Remaining { get; set; }

        public NotVisitedArray(int count)
        {
            Array = new (int value, int movedTo)[count];
            Remaining = count;

            for (var i = 0; i < count; i++)
            {
                Array[i] = (i, -1);
            }
        }

        public void Remove(int index)
        {
            var item = Array[index];
            while (item.value == -1)
            {
                index = item.movedTo;
                item = Array[index];
            }
            
            Array[index] = Array[Remaining - 1];
            Array[Remaining - 1] = (-1, index);
            Remaining -= 1;
        }

        public int GetNotVisited()
        {
            return Remaining > 0 ? Array[0].value : -1;
        }
    }
    
    public IEnumerator<List<Segment>> GetEnumerator()
    {
        var visited = new bool[halfEdges.Count];
        var notVisited = new NotVisitedArray(halfEdges.Count);
        
        var currentFaceBoundary = new List<Segment>();
        var currentIndex = 0;

        while (currentIndex != -1)
        {
            var halfEdge = halfEdges[currentIndex];
            visited[currentIndex] = true;
            notVisited.Remove(currentIndex);
            
            currentFaceBoundary.Add(halfEdge.AsSegment());

            currentIndex = halfEdge.NextIndex;
            if (!visited[currentIndex])
            {
                continue;
            }

            yield return currentFaceBoundary;
            currentFaceBoundary.Clear();

            currentIndex = notVisited.GetNotVisited();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}