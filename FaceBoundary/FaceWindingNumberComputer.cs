//#define DRAW_FACE_POINTS

using System.Drawing;
using SweepLine.Primitives;
using Point = SweepLine.Primitives.Point;

namespace FaceBoundary;

public class FaceWindingNumberComputer
{
    public class FaceWithWindingNumber
    {
        public required int WindingNumber { get; init; }
        public required List<Segment> FaceBoundary { get; init; }
    }
    
    private List<Segment> ConvolutionCycle { get; }
    private FaceBoundaryBuilder BoundaryBuilder { get; }
    
    public FaceWindingNumberComputer(IEnumerable<Segment> convolutionCycle)
    {
        ConvolutionCycle = convolutionCycle.ToList();
        BoundaryBuilder = new FaceBoundaryBuilder(ConvolutionCycle.Select(segment =>
        {
            if (segment.StartPoint > segment.EndPoint)
            {
                return new Segment
                {
                    StartPoint = segment.EndPoint,
                    EndPoint = segment.StartPoint,
                };
            }

            return segment;
        }));
    }

    public (List<FaceWithWindingNumber> InsideFaces, List<Segment> OuterFace) ComputeWindingNumbers(bool onlyFacesWithZero = false)
    {
        var faces = new List<FaceWithWindingNumber>();
        List<Segment>? outerFace = null;

        var halfEdges = BoundaryBuilder.ComputePlaneSubdivision();

#if DRAW_FACE_POINTS
        var bitmap = new Bitmap(1100, 1000);
        using var g = Graphics.FromImage(bitmap);
        var colors = new[]
        {
            Color.DodgerBlue,
            Color.BlueViolet,
            Color.LightGreen,
            Color.Red,
            Color.Orange,
            Color.Tomato,
            Color.Fuchsia,
            Color.Olive,
            Color.OliveDrab,
            Color.SteelBlue,
            Color.Gold,
            Color.PaleVioletRed,
            Color.Teal,
            Color.SaddleBrown,
            Color.Goldenrod,
        };
        var colorIndex = 0;
        var pen = new Pen(colors[colorIndex]);
#endif
        
        foreach (var faceBoundary in new FaceBoundaryIterator(halfEdges))
        {
            // separate outer face
            if (outerFace is null)
            {
                var minSegmentIndex = Enumerable.Range(0, faceBoundary.Count)
                    .MinBy(index => faceBoundary[index].EndPoint);
                
                var minSegment = faceBoundary[minSegmentIndex];
                var ax = minSegment.EndPoint.X - minSegment.StartPoint.X;
                var ay = minSegment.EndPoint.Y - minSegment.StartPoint.Y;

                var nextSegment = faceBoundary[(minSegmentIndex + 1) % faceBoundary.Count];
                var bx = nextSegment.EndPoint.X - nextSegment.StartPoint.X;
                var by = nextSegment.EndPoint.Y - nextSegment.StartPoint.Y;

                var semiCross = ax * by - ay * bx;
                if (semiCross <= Point.Eps)
                {
                    outerFace = new List<Segment>(faceBoundary); // note(shevyrin): copy list because iterator reuses it
                    continue;
                }
            }
            
            // find a point inside the face
            var internalFacePoint = new Point();

            var vertices = faceBoundary.Select(segment => segment.StartPoint).ToList();
            var currentVertexIndex = 0;
            
            while (true)
            {
                var vertex1 = vertices[currentVertexIndex];
                var vertex2 = vertices[(currentVertexIndex + 1) % vertices.Count];
                var vertex3 = vertices[(currentVertexIndex + 2) % vertices.Count];

                var ax = vertex2.X - vertex1.X;
                var ay = vertex2.Y - vertex1.Y;

                var bx = vertex3.X - vertex2.X;
                var by = vertex3.Y - vertex2.Y;

                var semiCross = ax * by - ay * bx;
                if (Math.Abs(semiCross) < Point.Eps) // note(shevyrin): skip invalid triangles
                {
                    currentVertexIndex = (currentVertexIndex + 1) % vertices.Count;
                    continue;
                }
                
                internalFacePoint = new Point
                {
                    X = (vertex1.X + vertex2.X + vertex3.X) / 3.0,
                    Y = (vertex1.Y + vertex2.Y + vertex3.Y) / 3.0,
                };
                
                if (ComputeWindingNumber(faceBoundary, internalFacePoint, checkPointOnBoundary: true) > 0)
                {
                    break;
                }
                
                currentVertexIndex = (currentVertexIndex + 1) % vertices.Count;
            }

            // compute winding number
            var windingNumber = ComputeWindingNumber(ConvolutionCycle, internalFacePoint);

            if (!onlyFacesWithZero || windingNumber == 0)
            {
                faces.Add(new FaceWithWindingNumber
                {
                    FaceBoundary = new List<Segment>(faceBoundary), // note(shevyrin): copy list because iterator reuses it
                    WindingNumber = windingNumber,
                });
            }
            
#if DRAW_FACE_POINTS
            foreach (var segment in faceBoundary)
            {
                var ox = segment.StartPoint.X;
                var oy = segment.StartPoint.Y;

                var dx = segment.EndPoint.X;
                var dy = segment.EndPoint.Y;

                var directionX = dx - ox;
                var directionY = dy - oy;
                var mag = Math.Sqrt(directionX * directionX + directionY * directionY);
                directionX /= mag;
                directionY /= mag;

                var displacementX = -directionY;
                var displacementY = directionX;
                displacementX *= 0.015;
                displacementY *= 0.015;

                g.DrawLine(pen,
                    new PointF(300 + (float)(ox + displacementX) * 100, 800 + (float)(oy + displacementY) * -100),
                    new PointF(300 + (float)(dx + displacementX) * 100, 800 + (float)(dy + displacementY) * -100));
            }
            
            //g.DrawEllipse(new Pen(colors[colorIndex], 5), 295 + (float)internalFacePoint.X * 100, 795 + (float)internalFacePoint.Y * -100, 10, 10);
            g.DrawString($"{windingNumber}", new Font(FontFamily.GenericMonospace, 20), new SolidBrush(colors[colorIndex]),
                290 + (float)internalFacePoint.X * 100, 785 + (float)internalFacePoint.Y * -100);
            colorIndex = (colorIndex + 1) % colors.Length;
            pen = new Pen(colors[colorIndex]);
#endif
        }
#if DRAW_FACE_POINTS        
        bitmap.Save("debug-face-points.png");
#endif
        return (faces, outerFace!);
    }

    private static int ComputeWindingNumber(List<Segment> boundary, Point point, bool checkPointOnBoundary = false)
    {
        var quarterRevelationsCount = 0;
        var prevVertex = boundary[0].StartPoint;
        var prevQuadrant = GetQuadrant(point, prevVertex);
        foreach (var segment in boundary)
        {
            if (checkPointOnBoundary)
            {
                var ax = segment.EndPoint.X - segment.StartPoint.X;
                var ay = segment.EndPoint.Y - segment.StartPoint.Y;
                
                var bx = point.X - segment.StartPoint.X;
                var by = point.Y - segment.StartPoint.Y;

                var semiCross = ax * by - ay * bx;
                if (Math.Abs(semiCross) < Point.Eps)
                {
                    var magA = Math.Sqrt(ax * ax + ay * ay);
                    var scalarProjection = (ax * bx + ay * by) / magA;
                    
                    if (scalarProjection >= -Point.Eps && scalarProjection <= magA + Point.Eps)
                    {
                        return 0; // note(shevyrin): point lying on a boundary segment is considered an outside point
                    }
                }
            }
            
            var vertex = segment.EndPoint;
            var currentQuadrant = GetQuadrant(point, vertex);
            var diff = currentQuadrant - prevQuadrant;

            if (diff == 1 || diff == -3)
            {
                quarterRevelationsCount += 1;
            }
            else if (diff == -1 || diff == 3)
            {
                quarterRevelationsCount -= 1;
            }
            else if (diff == 2 || diff == -2)
            {
                var ax = prevVertex.X - point.X;
                var ay = prevVertex.Y - point.Y;

                var bx = vertex.X - point.X;
                var by = vertex.Y - point.Y;

                var semiCross = ax * by - ay * bx;
                quarterRevelationsCount += semiCross >= -Point.Eps ? 2 : -2;
            }

            prevQuadrant = currentQuadrant;
            prevVertex = vertex;
        }

        return quarterRevelationsCount / 4;
    }

    private static int GetQuadrant(Point internalFacePoint, Point vertex)
    {
        var right = vertex.X > internalFacePoint.X;
        var top = vertex.Y > internalFacePoint.Y;
        
        if (right && top)
        {
            return 0;
        }

        if (!right && top)
        {
            return 1;
        }

        if (!right && !top)
        {
            return 2;
        }

        return 3;
    }
}