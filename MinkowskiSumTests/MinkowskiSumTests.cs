using System.Drawing;
using System.Text;
using MinkowskiSum;
using SweepLine.Primitives;
using Point = SweepLine.Primitives.Point;

namespace MinkowskiSumTests;

public class MinkowskiSumTests
{
    [Test]
    public void Test1()
    {
        var segmentsA = VerticesToSegments([
            (0, 0),
            (2.5, 0),
            (2.5, 2),
            (1.25, 1),
            (0, 2),
        ]);

        var segmentsB = VerticesToSegments([
            (0, 0),
            (2, 0),
            (2, 2),
            (0, 2),
        ]);

        var result = MinkowskiSumComputer.ComputeMinkowskiSum(
            segmentsA.Select(segment => segment.StartPoint).ToList(),
            segmentsB.Select(segment => segment.StartPoint).ToList());

       DumpBitmap("minkowski-sum-test-1.png", segmentsA, segmentsB, result);
        
        var resultString = ResultAsString(result);
        Assert.That(resultString, Is.EqualTo("Boundary: { [(4,5; 0) - (0; 0)] [(0; 0) - (0; 4)] [(0; 4) - (2; 4)] [(2; 4) - (2,25; 3,8)] [(2,25; 3,8) - (2,5; 4)] [(2,5; 4) - (4,5; 4)] [(4,5; 4) - (4,5; 0)] }\nHoles:\n"));
    }
    
    [Test]
    public void Test2()
    {
        var segmentsA = VerticesToSegments([
            (0, 0),
            (6, 0),
            (5, 5),
            (4, 5),
            (5, 1),
            (1, 1),
            (2, 5),
            (0, 5)
        ]);

        var segmentsB = VerticesToSegments([
            (0, 0),
            (2, 0),
            (2, 2),
            (0, 2),
        ]);

        var result = MinkowskiSumComputer.ComputeMinkowskiSum(
            segmentsA.Select(segment => segment.StartPoint).ToList(),
            segmentsB.Select(segment => segment.StartPoint).ToList());

        DumpBitmap("minkowski-sum-test-2.png", segmentsA, segmentsB, result);

        var resultAsString = ResultAsString(result);
        Assert.That(resultAsString, Is.EqualTo("Boundary: { [(8; 0) - (0; 0)] [(0; 0) - (0; 7)] [(0; 7) - (4; 7)] [(4; 7) - (7; 7)] [(7; 7) - (8; 2)] [(8; 2) - (8; 0)] }\nHoles:\n0: { [(3,5; 3) - (4,5; 3)] [(4,5; 3) - (4; 5)] [(4; 5) - (3,5; 3)] }\n"));
    }
    
    private static List<Segment> VerticesToSegments(List<(double x, double y)> vertices)
    {
        var segments = new List<Segment>(vertices.Count);
        
        for (var i = 0; i < vertices.Count; i++)
        {
            var vertex1 = vertices[i];
            var vertex2 = vertices[(i + 1) % vertices.Count];
            
            segments.Add(new Segment
            {
                StartPoint = new Point
                {
                    X = vertex1.x, Y = vertex1.y,
                },
                EndPoint = new Point
                {
                    X = vertex2.x, Y = vertex2.y,
                }
            });
        }

        return segments;
    }

    private static void DumpBitmap(string fileName, List<Segment> segmentsA, List<Segment> segmentsB,
        (List<Segment> Boundary, List<List<Segment>> Holes) result)
    {
        var bitmap = new Bitmap(1000, 1000);
        using var g = Graphics.FromImage(bitmap);
        
        DrawSegments(segmentsA, Color.LawnGreen, g);
        DrawSegments(segmentsB, Color.CornflowerBlue, g);
        
        DrawSegments(result.Boundary, Color.Red, g);
        foreach (var hole in result.Holes)
        {
            DrawSegments(hole, Color.MediumPurple, g);
        }
        
        bitmap.Save(fileName);
    }

    private static void DrawSegments(List<Segment> segments, Color color, Graphics g)
    {
        var pen = new Pen(color);
        
        foreach (var segment in segments)
        {
            g.DrawLine(pen,
                new PointF(100 + (float)segment.StartPoint.X * 100, 800 + (float)segment.StartPoint.Y * -100),
                new PointF(100 + (float)segment.EndPoint.X * 100, 800 + (float)segment.EndPoint.Y * -100));
        }
    }

    private static string ResultAsString((List<Segment> Boundary, List<List<Segment>> Holes) result)
    {
        var sb = new StringBuilder();

        sb.Append("Boundary: { ");
        foreach (var segment in result.Boundary)
        {
            sb.Append($"{segment} ");
        }
        
        sb.Append("}\nHoles:\n");

        var i = 0;
        foreach (var hole in result.Holes)
        {
            sb.Append($"{i++}: {{ ");
            foreach (var segment in hole)
            {
                sb.Append($"{segment} ");
            }
            sb.Append("}\n");
        }

        return sb.ToString();
    }
}