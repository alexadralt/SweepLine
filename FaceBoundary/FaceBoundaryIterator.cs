using System.Collections;
using SweepLine.Primitives;

namespace FaceBoundary;

public class FaceBoundaryIterator(List<HalfEdge> halfEdges) : IEnumerable<List<Segment>>
{
    public IEnumerator<List<Segment>> GetEnumerator()
    {
        var visited = new bool[halfEdges.Count];
        var currentFaceBoundary = new List<Segment>();
        var currentIndex = 0;

        while (currentIndex != -1)
        {
            var halfEdge = halfEdges[currentIndex];
            visited[currentIndex] = true;
            
            currentFaceBoundary.Add(halfEdge.AsSegment());

            currentIndex = halfEdge.NextIndex;
            if (!visited[currentIndex])
            {
                continue;
            }

            yield return currentFaceBoundary;
            currentFaceBoundary.Clear();
            
            currentIndex = Enumerable.Range(0, halfEdges.Count).FirstOrDefault(index => !visited[index], -1);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}