using System.Globalization;
using System.Text;
using MinkowskiSum;
using SweepLine.Primitives;

class Program
{
  public static void Main(String[] args)
  {
    var figureA = GetPointsFromFile(args[0]);
    var figureB = GetPointsFromFile(args[1]);

    var sum = MinkowskiSumComputer.ComputeMinkowskiSum(figureA, figureB);

    var sumAsString = ResultAsString(sum);

    File.WriteAllText(args[2], sumAsString);

    return;

    string ResultAsString((List<Segment> Boundary, List<List<Segment>> Holes) result)
    {
      var sb = new StringBuilder();

      foreach (var segment in result.Boundary)
      {
        sb.Append($"{segment.StartPoint.X.ToString(CultureInfo.InvariantCulture)} {segment.StartPoint.Y.ToString(CultureInfo.InvariantCulture)}\n");
      }

      sb.Append('\n');

      foreach (var hole in result.Holes)
      {
        foreach (var segment in hole)
        {
          sb.Append($"{segment.StartPoint.X.ToString(CultureInfo.InvariantCulture)} {segment.StartPoint.Y.ToString(CultureInfo.InvariantCulture)}\n");
        }

        sb.Append('\n');
      }

      return sb.ToString();
    }

    List<Point> GetPointsFromFile(string fileName)
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
}
