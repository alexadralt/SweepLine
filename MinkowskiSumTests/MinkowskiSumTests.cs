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
        (-1, -1),
            (1, -1),
            (1, 1),
            (-1, 1),
        ]);

    var result = MinkowskiSumComputer.ComputeMinkowskiSum(
        segmentsA.Select(segment => segment.StartPoint).ToList(),
        segmentsB.Select(segment => segment.StartPoint).ToList());

    DumpBitmap("minkowski-sum-test-2.png", segmentsA, segmentsB, result);

    var resultAsString = ResultAsString(result);
    Assert.That(resultAsString, Is.EqualTo("Boundary: { [(7; -1) - (-1; -1)] [(-1; -1) - (-1; 6)] [(-1; 6) - (3; 6)] [(3; 6) - (6; 6)] [(6; 6) - (7; 1)] [(7; 1) - (7; -1)] }\nHoles:\n0: { [(2,5; 2) - (3,5; 2)] [(3,5; 2) - (3; 4)] [(3; 4) - (2,5; 2)] }\n"));
  }

  [Test]
  public void Test3()
  {
    var segmentsA = VerticesToSegments([
        (0, 0),
            (2, 0),
            (3, 1),
            (1, 2),
        ]);

    var segmentsB = VerticesToSegments([
        (-2, -1),
            (-1, -4),
            (0, 0),
        ]);

    var result = MinkowskiSumComputer.ComputeMinkowskiSum(
        segmentsA.Select(segment => segment.StartPoint).ToList(),
        segmentsB.Select(segment => segment.StartPoint).ToList());

    DumpBitmap("minkowski-sum-test-3.png", segmentsA, segmentsB, result, xOffset: 400, yOffset: 500);

    var resultAsString = ResultAsString(result);
    Assert.That(resultAsString, Is.EqualTo("Boundary: { [(-1; -4) - (-2; -1)] [(-2; -1) - (-1; 1)] [(-1; 1) - (1; 2)] [(1; 2) - (3; 1)] [(3; 1) - (2; -3)] [(2; -3) - (1; -4)] [(1; -4) - (-1; -4)] }\nHoles:\n"));
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
          X = vertex1.x,
          Y = vertex1.y,
        },
        EndPoint = new Point
        {
          X = vertex2.x,
          Y = vertex2.y,
        }
      });
    }

    return segments;
  }

  private static void DumpBitmap(string fileName, List<Segment> segmentsA, List<Segment> segmentsB,
      (List<Segment> Boundary, List<List<Segment>> Holes) result, float xOffset = 200, float yOffset = 800)
  {
#if WINDOWS
        var bitmap = new Bitmap(1000, 1000);
        using var g = Graphics.FromImage(bitmap);
        
        DrawSegments(segmentsA, Color.LawnGreen, g, xOffset, yOffset);
        DrawSegments(segmentsB, Color.CornflowerBlue, g, xOffset, yOffset);
        
        DrawSegments(result.Boundary, Color.Red, g, xOffset, yOffset);
        foreach (var hole in result.Holes)
        {
            DrawSegments(hole, Color.MediumPurple, g, xOffset, yOffset);
        }
        
        bitmap.Save(fileName);
#else
    Console.WriteLine("Drawing is supported only under Windows!");
#endif
  }

  private static void DrawSegments(List<Segment> segments, Color color, Graphics g, float xOffset, float yOffset)
  {
#if WINDOWS
    var pen = new Pen(color);

    foreach (var segment in segments)
    {
      g.DrawLine(pen,
          new PointF(xOffset + (float)segment.StartPoint.X * 100, yOffset + (float)segment.StartPoint.Y * -100),
          new PointF(xOffset + (float)segment.EndPoint.X * 100, yOffset + (float)segment.EndPoint.Y * -100));
    }
#endif
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