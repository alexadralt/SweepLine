using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using SweepLine.Algorithm;
using SweepLine.DataStructuresLinkedListImpl;
using SweepLine.Primitives;
using Point = SweepLine.Primitives.Point;

namespace EdgeBoundary;

public class EdgeBoundaryBuilder
{
    private class SegmentWithReference : Segment
    {
        public SegmentWithReference(Segment segment)
        {
            StartPoint = segment.StartPoint;
            EndPoint = segment.EndPoint;
        }

        public int HalfEdgeIndex { get; set; } = -1;
    }

    private class EdgeBoundaryBuilderVisitor : ISweepLineVisitor<SegmentWithReference>
    {
        public List<HalfEdge> HalfEdges { get; } = [];

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
                else if (groupAsList.All(segment => segment.StartPoint == point))
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
                var segment = group.First(segment => segment.HalfEdgeIndex != -1);
                group[0].HalfEdgeIndex = segment.HalfEdgeIndex;
                segment.HalfEdgeIndex = -1;

                var halfEdge = HalfEdges[group[0].HalfEdgeIndex];
                halfEdge.DestinationPoint = group[0].EndPoint;
                HalfEdges[group[0].HalfEdgeIndex] = halfEdge;
                
                var twinIndex = GetTwinHalfEdgeIndex(group);
                var twin = HalfEdges[twinIndex];
                twin.OriginPoint = group[0].EndPoint;
                HalfEdges[twinIndex] = twin;
            }

            if (intersectingGroups.Count > 1) // note(shevyrin): don't split if only overlapping segments intersect here
            {
                foreach (var segment in intersectingGroups.Select(group => group[0]))
                {
                    var halfEdgeIndex = segment.HalfEdgeIndex;
                    var twinIndex = HalfEdges[halfEdgeIndex].TwinIndex;

                    var halfEdge = HalfEdges[halfEdgeIndex];
                    var farPoint = halfEdge.DestinationPoint;
                    halfEdge.DestinationPoint = point;
                    HalfEdges[halfEdgeIndex] = halfEdge;

                    var twin = HalfEdges[twinIndex];
                    twin.OriginPoint = point;
                    HalfEdges[twinIndex] = twin;

                    outgoingHalfEdges.Add(twinIndex);

                    var newIndex = HalfEdges.Count;

                    HalfEdges.Add(new HalfEdge
                    {
                        DestinationPoint = farPoint,
                        OriginPoint = point,
                        TwinIndex = newIndex + 1,
                        NextIndex = newIndex + 1,
                    });

                    HalfEdges.Add(new HalfEdge
                    {
                        DestinationPoint = point,
                        OriginPoint = farPoint,
                        TwinIndex = newIndex,
                    });

                    outgoingHalfEdges.Add(newIndex);

                    segment.HalfEdgeIndex = newIndex;
                }
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
                    NextIndex = newIndex + 1,
                });

                HalfEdges.Add(new HalfEdge
                {
                    OriginPoint = segment.EndPoint,
                    DestinationPoint = segment.StartPoint,
                    TwinIndex = newIndex,
                    NextIndex = newIndex,
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

                    var angleA = ClockwisePolarAngle(ax, ay);
                    var angleB = ClockwisePolarAngle(bx, by);

                    return angleA.CompareTo(angleB);
                }));

            if (outgoingHalfEdges.Count > 1)
            {
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
            
#if DEBUG
#pragma warning disable CA1416
            var bitmap = new Bitmap(1000, 1000);

            var colorIndex = 0;

            using (var g = Graphics.FromImage(bitmap))
            {
                var pen = new Pen(Color.Cyan);
                var visited = new HashSet<int>();
            
                var currentIndex = 0;
                while (true)
                {
                    var halfEdge = HalfEdges[currentIndex];
                    visited.Add(currentIndex);

                    var ox = halfEdge.OriginPoint.X;
                    var oy = halfEdge.OriginPoint.Y;

                    var dx = halfEdge.DestinationPoint.X;
                    var dy = halfEdge.DestinationPoint.Y;

                    var directionX = dx - ox;
                    var directionY = dy - oy;
                    var mag = Math.Sqrt(directionX * directionX + directionY * directionY);
                    directionX /= mag;
                    directionY /= mag;

                    var displacementX = -directionY;
                    var displacementY = directionX;
                    displacementX *= 0.025;
                    displacementY *= 0.025;
                
                    g.DrawLine(pen,
                        new PointF(300 + (float)(ox + displacementX) * 100, 800 + (float)(oy + displacementY) * -100),
                        new PointF(300 + (float)(dx + displacementX) * 100, 800 + (float)(dy + displacementY) * -100));

                    currentIndex = halfEdge.NextIndex;
                    
                    if (currentIndex < 0)
                    {
                        throw new UnreachableException();
                    }
                    
                    if (visited.Count == HalfEdges.Count)
                    {
                        break;
                    }
                    
                    if (!visited.Contains(currentIndex))
                    {
                        continue;
                    }

                    currentIndex = Enumerable.Range(0, HalfEdges.Count).First(index => !visited.Contains(index));
                    colorIndex = (colorIndex + 1) % _colors.Length;
                    pen = new Pen(_colors[colorIndex]);
                }
                
                g.DrawEllipse(new Pen(Color.Red, 5), 295 + (float)point.X * 100, 795 + (float)point.Y * -100, 10, 10);
            }
        
            bitmap.Save($"out-{_step++}.png");
#pragma warning restore CA1416
#endif
        }
#if DEBUG
        private int _step;
        
        private static Color[] _colors =
        [
            Color.Cyan,
            Color.Blue,
            Color.BlueViolet,
            Color.Green,
            Color.LightGreen,
            Color.Red,
            Color.White,
            Color.Yellow,
            Color.DeepPink,
            Color.Purple,
            Color.Orange,
            Color.Tomato,
            Color.LightGray,
            Color.Brown,
            Color.Fuchsia,
            Color.Olive,
            Color.OliveDrab,
            Color.SteelBlue,
            Color.Gold,
            Color.PaleVioletRed,
            Color.Teal,
            Color.SaddleBrown,
            Color.Goldenrod,
            Color.Aquamarine
        ];
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetTwinHalfEdgeIndex(List<SegmentWithReference> group)
        {
            return HalfEdges[group[0].HalfEdgeIndex].TwinIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ClockwisePolarAngle(double x, double y)
        {
            return -Math.Atan2(y, x);
        }
    }

    private SweepLineProcessor<SegmentWithReference> SweepLineProcessor { get; } =
        new(new XStructure<SegmentWithReference>(), new YStructure<SegmentWithReference>());
    
    public EdgeBoundaryBuilder(List<Segment> segments)
    {
        SweepLineProcessor.AddSegments(segments.Select(segment => new SegmentWithReference(segment)));
    }

    public List<HalfEdge> ComputePlaneSubdivision()
    {
        var visitor = new EdgeBoundaryBuilderVisitor();
        SweepLineProcessor.Process(visitor);
        return visitor.HalfEdges;
    }
}
