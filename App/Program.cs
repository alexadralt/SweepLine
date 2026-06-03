#if WINDOWS
using System.Drawing;
#endif

using System.Globalization;
using System.Text;
using MinkowskiSum;
using SweepLine.Primitives;
using Point = SweepLine.Primitives.Point;

class Program
{
  public static void Main(string[] args)
  {
    if (args.Length < 2)
    {
      Console.WriteLine("Need at least two arguments, which are the names of files with the summands (the first one is possibly non-convex)");
      return;
    }
    if (args.Length > 3)
    {
      Console.WriteLine("Need at most three arguments, which are the names of files with the summands (the first one is possibly non-convex) and the sum");
      return;
    }

    var figureA = GetPointsFromFile(args[0]);
    var figureB = GetPointsFromFile(args[1]);

    var sum = MinkowskiSumComputer.ComputeMinkowskiSum(figureA, figureB);

    var sumAsString = ResultAsString(sum);

    File.WriteAllText(args.Length > 2 ? args[2] : "output.txt", sumAsString);
#if WINDOWS
    DumpBitmap("output.png", figureA, figureB, sum);
#endif
  }

  private static string ResultAsString((List<Segment> Boundary, List<List<Segment>> Holes) result)
  {
    var sb = new StringBuilder();

    foreach (var segment in result.Boundary)
    {
      sb.Append(
        $"{segment.StartPoint.X.ToString(CultureInfo.InvariantCulture)} {segment.StartPoint.Y.ToString(CultureInfo.InvariantCulture)}\n");
    }

    sb.Append('\n');

    foreach (var hole in result.Holes)
    {
      foreach (var segment in hole)
      {
        sb.Append(
          $"{segment.StartPoint.X.ToString(CultureInfo.InvariantCulture)} {segment.StartPoint.Y.ToString(CultureInfo.InvariantCulture)}\n");
      }

      sb.Append('\n');
    }

    return sb.ToString();
  }
  
#if DRAW_BITMAP
  private static void DumpBitmap(string fileName, List<Point> figureA, List<Point> figureB,
    (List<Segment> Boundary, List<List<Segment>> Holes) result, float xOffset = 200, float yOffset = 800)
  {
    var bitmap = new Bitmap(1000, 1000);
    using var g = Graphics.FromImage(bitmap);
        
    DrawSegments(PointsToSegments(figureA), Color.LawnGreen, g, xOffset, yOffset);
    DrawSegments(PointsToSegments(figureB), Color.CornflowerBlue, g, xOffset, yOffset);
        
    DrawSegments(result.Boundary, Color.Red, g, xOffset, yOffset);
    foreach (var hole in result.Holes)
    {
      DrawSegments(hole, Color.MediumPurple, g, xOffset, yOffset);
    }
    
    bitmap.Save(fileName);
  }

  private static IEnumerable<Segment> PointsToSegments(List<Point> figure)
    => figure.Select((_, i) => new Segment { StartPoint = figure[i], EndPoint = figure[(i + 1) % figure.Count] });
  
  private static void DrawSegments(IEnumerable<Segment> segments, Color color, Graphics g, float xOffset, float yOffset)
  {
    var pen = new Pen(color);
        
    foreach (var segment in segments)
    {
      g.DrawLine(pen,
        new PointF(xOffset + (float)segment.StartPoint.X * 100, yOffset + (float)segment.StartPoint.Y * -100),
        new PointF(xOffset + (float)segment.EndPoint.X * 100, yOffset + (float)segment.EndPoint.Y * -100));
    }
  }
#endif
  
  private static List<Point> GetPointsFromFile(string fileName)
  {
    var lines = File.ReadAllLines(fileName);
    var figure = new List<Point>();

    var index = 0;
    foreach (var line in lines)
    {
      var xy = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
      if (xy.Length != 2)
        throw new ArgumentException($"Invalid values at line {index}");

      figure.Add(new Point
      {
        X = double.Parse(xy[0], CultureInfo.InvariantCulture),
        Y = double.Parse(xy[1], CultureInfo.InvariantCulture),
      });

      index++;
    }

    if (figure.Count < 3)
      throw new ArgumentException("Figure must have at least 3 vertices");

    return figure;
  }
}
