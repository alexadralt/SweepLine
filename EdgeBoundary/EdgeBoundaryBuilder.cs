using System.Diagnostics;
using System.Runtime.CompilerServices;
using SweepLine.Algorithm;
using SweepLine.DataStructuresLinkedListImpl;
using SweepLine.Primitives;

namespace EdgeBoundary;

public class EdgeBoundaryBuilder : ISweepLineVisitor<EdgeBoundaryBuilder.SegmentWithReference>
{
    public class SegmentWithReference : Segment
    {
        public SegmentWithReference(Segment segment)
        {
            StartPoint = segment.StartPoint;
            EndPoint = segment.EndPoint;
        }

        public int HalfEdgeIndex { get; set; } = -1;
    }
    
    private List<HalfEdge> HalfEdges { get; } = [];

    private SweepLineProcessor<SegmentWithReference> SweepLineProcessor { get; } =
        new(new XStructure<SegmentWithReference>(), new YStructure<SegmentWithReference>());
    
    public EdgeBoundaryBuilder(List<Segment> segments)
    {
        SweepLineProcessor.AddSegments(segments.Select(segment => new SegmentWithReference(segment)));
    }

    public List<HalfEdge> ComputePlaneSubdivision()
    {
        SweepLineProcessor.Process(this);
        return HalfEdges;
    }
    
    public void VisitIntersectingSegments(Point point, IEnumerable<IEnumerable<SegmentWithReference>> segments)
    {
        var endingGroups = new List<List<SegmentWithReference>>();
        var intersectingGroups = new List<List<SegmentWithReference>>();
        var startingGroups = new List<List<SegmentWithReference>>();

        foreach (var group in segments)
        {
            var groupAsList = group.ToList();
            
            if (groupAsList[0].EndPoint == point)
            {
                endingGroups.Add(groupAsList);
            }
            else if (groupAsList[0].StartPoint == point)
            {
                startingGroups.Add(groupAsList);
            }
            else
            {
                intersectingGroups.Add(groupAsList);
            }
        }
        
        var outgoingHalfEdges = endingGroups.Select(GetTwinHalfEdgeIndex).ToList();

        foreach (var group in intersectingGroups.Where(group => group[0].HalfEdgeIndex == -1))
        {
#if DEBUG
            var found = false;
#endif
            
            foreach (var segment in group.Where(segment => segment.HalfEdgeIndex != -1))
            {
                group[0].HalfEdgeIndex = segment.HalfEdgeIndex;
                segment.HalfEdgeIndex = -1;
#if DEBUG
                found = true;
#endif
                
                break;
            }

#if DEBUG
            if (!found)
            {
                throw new UnreachableException();
            }
#endif
        }
        
        foreach (var halfEdgeIndex in intersectingGroups.Select(GetHalfEdgeIndex))
        {
            var twinIndex = HalfEdges[halfEdgeIndex].TwinIndex;

            var halfEdge = HalfEdges[halfEdgeIndex];
            halfEdge.DestinationPoint = point;
            HalfEdges[halfEdgeIndex] = halfEdge;

            var twin = HalfEdges[twinIndex];
            twin.OriginPoint = point;
            HalfEdges[twinIndex] = twin;
            
            outgoingHalfEdges.Add(twinIndex);
        }
        
        foreach (var group in startingGroups)
        {
            var newIndex = HalfEdges.Count;
            var segment = group[0];

            // note(shevyrin): always reference the half edge that matches direction of the segment
            segment.HalfEdgeIndex = newIndex;
            
            HalfEdges.Add(new HalfEdge
            {
                OriginPoint = segment.StartPoint,
                DestinationPoint = segment.EndPoint,
                TwinIndex = newIndex + 1,
            });
            
            HalfEdges.Add(new HalfEdge
            {
                OriginPoint = segment.EndPoint,
                DestinationPoint = segment.StartPoint,
                TwinIndex = newIndex,
            });
            
            outgoingHalfEdges.Add(newIndex);
        }

        outgoingHalfEdges.Sort(
            Comparer<int>.Create((indexA, indexB) =>
            {
                var halfEdgeA = HalfEdges[indexA];
                var halfEdgeB = HalfEdges[indexB];

                var ax = halfEdgeA.DestinationPoint.X - halfEdgeA.OriginPoint.X;
                var ay = halfEdgeA.DestinationPoint.Y - halfEdgeA.OriginPoint.Y;
                
                var bx = halfEdgeB.DestinationPoint.X - halfEdgeB.OriginPoint.X;
                var by = halfEdgeB.DestinationPoint.Y - halfEdgeB.OriginPoint.Y;

                var cmp = ax * by - ay * bx;

                if (Math.Abs(cmp) < Point.Eps)
                {
                    if (halfEdgeA.DestinationPoint > halfEdgeB.DestinationPoint)
                    {
                        return -1;
                    }

                    return 1;
                }

                if (cmp < 0)
                {
                    return -1;
                }

                return 1;
            }));

        if (outgoingHalfEdges.Count == 1)
        {
            var current = HalfEdges[outgoingHalfEdges[0]];
            
            var currentTwin = HalfEdges[current.TwinIndex];
            currentTwin.NextIndex = outgoingHalfEdges[0];
            HalfEdges[current.TwinIndex] = currentTwin;
            
            return;
        }

        for (var i = 0; i < outgoingHalfEdges.Count; i++)
        {
            var currentEdgeIndex = outgoingHalfEdges[i];
            var nextEdgeIndex = outgoingHalfEdges[(i + 1) % outgoingHalfEdges.Count];

            var current = HalfEdges[currentEdgeIndex];

            var currentTwin = HalfEdges[current.TwinIndex];
            currentTwin.NextIndex = nextEdgeIndex;
            HalfEdges[current.TwinIndex] = currentTwin;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetHalfEdgeIndex(List<SegmentWithReference> group)
    {
        return group[0].HalfEdgeIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetTwinHalfEdgeIndex(List<SegmentWithReference> group)
    {
        return HalfEdges[group[0].HalfEdgeIndex].TwinIndex;
    }
}
