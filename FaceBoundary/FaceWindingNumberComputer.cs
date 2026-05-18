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

    public (List<FaceWithWindingNumber> InsideFaces, List<Segment> OuterFace) ComputeWindingNumbers()
    {
        var faces = new List<FaceWithWindingNumber>();
        List<Segment>? outerFace = null;

        var halfEdges = BoundaryBuilder.ComputePlaneSubdivision();

#if DRAW_FACE_POINTS
        var bitmap = new Bitmap(1000, 1000);
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
                if (semiCross <= 0)
                {
                    outerFace = new List<Segment>(faceBoundary); // note(shevyrin): copy list because iterator reuses it
                    continue;
                }
            }
            
            // find a point inside the face
            var point = new Point();

            var vertices = faceBoundary.Select(segment => segment.StartPoint).ToList();
            var currentVertexIndex = 0;
            
            while (true)
            {
                var index1 = currentVertexIndex;
                var index2 = (currentVertexIndex + 1) % vertices.Count;
                var index3 = (currentVertexIndex + 2) % vertices.Count;
                
                if (HasToTheLeft(vertices, index1, index2, currentVertexIndex) &&
                    HasToTheLeft(vertices, index2, index3, currentVertexIndex) &&
                    HasToTheLeft(vertices, index3, index1, currentVertexIndex))
                {
                    currentVertexIndex = (currentVertexIndex + 1) % vertices.Count;
                    continue;
                }
                
                var vertex1 = vertices[index1];
                var vertex2 = vertices[index2];
                var vertex3 = vertices[index3];
                point = new Point
                {
                    X = (vertex1.X + vertex2.X + vertex3.X) / 3.0,
                    Y = (vertex1.Y + vertex2.Y + vertex3.Y) / 3.0,
                };
                break;
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
            
            g.DrawEllipse(new Pen(colors[colorIndex], 5), 295 + (float)point.X * 100, 795 + (float)point.Y * -100, 10, 10);
            colorIndex = (colorIndex + 1) % colors.Length;
            pen = new Pen(colors[colorIndex]);
#endif
        }
#if DRAW_FACE_POINTS        
        bitmap.Save("debug-face-points.png");
#endif
        return (faces, outerFace!);
    }

    private bool HasToTheLeft(List<Point> vertices, int index1, int index2, int currentVertexIndex)
    {
        var vertex1 = vertices[index1];
        var vertex2 = vertices[index2];

        var ax = vertex2.X - vertex1.X;
        var ay = vertex2.Y - vertex1.Y;

        var hasToTheLeft = false;
        for (var i = (currentVertexIndex + 3) % vertices.Count; i != currentVertexIndex; i = (i + 1) % vertices.Count)
        {
            var vertexToTest = vertices[i];
            var bx = vertexToTest.X - vertex1.X;
            var by = vertexToTest.Y - vertex1.Y;

            var semiCross = ax * by - ay * bx;
            if (semiCross > 0)
            {
                hasToTheLeft = true;
                break;
            }
        }

        return hasToTheLeft;
    }
}