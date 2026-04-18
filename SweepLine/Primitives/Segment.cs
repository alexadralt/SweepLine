namespace SweepLine.Primitives;

public readonly struct Segment : IEquatable<Segment>
{
    public required Point StartPoint { get; init; }
    
    public required Point EndPoint { get; init; }

    public static SegmentIntersection FindIntersection(Segment a, Segment b)
    {
        var x1 = a.StartPoint.X;
        var x2 = a.EndPoint.X;
        var x3 = b.StartPoint.X;
        var x4 = b.EndPoint.X;

        var y1 = a.StartPoint.Y;
        var y2 = a.EndPoint.Y;
        var y3 = b.StartPoint.Y;
        var y4 = b.EndPoint.Y;

        var denom = (x2 - x1) * (y4 - y3) - (x4 - x3) * (y2 - y1);

        if (Math.Abs(denom) < Point.Eps)
        {
            var cross = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);
            if (Math.Abs(cross) < Point.Eps)
            {
                var maxStart = a.StartPoint > b.StartPoint ? a.StartPoint : b.StartPoint;
                var minEnd = a.EndPoint > b.EndPoint ? b.EndPoint : a.EndPoint;
                if (maxStart < minEnd)
                {
                    return new SegmentIntersection
                    {
                        Type = IntersectionType.SubSegment,
                        SubSegment = new Segment
                        {
                            StartPoint = maxStart,
                            EndPoint = minEnd,
                        },
                    };
                }
            }

            return new SegmentIntersection
            {
                Type = IntersectionType.None,
            };
        }

        var t = ((x3 - x1) * (y4 - y3) - (x4 - x3) * (y3 - y1)) / denom;
        var s = -((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1)) / denom;

        if (t is >= 0 and <= 1 && s is >= 0 and <= 1)
        {
            return new SegmentIntersection
            {
                Type = IntersectionType.Point,
                Point = new Point
                {
                    X = x1 + t * (x2 - x1),
                    Y = y1 + t * (y2 - y1),
                },
            };
        }

        return new SegmentIntersection
        {
            Type = IntersectionType.None,
        };
    }

    public bool Equals(Segment other) => 
        StartPoint.Equals(other.StartPoint) && EndPoint.Equals(other.EndPoint);

    public override bool Equals(object? obj) => obj is Segment other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(StartPoint, EndPoint);

    public override string ToString()
    {
        return $"[{StartPoint} - {EndPoint}]";
    }

    public static bool operator ==(Segment left, Segment right) => left.Equals(right);

    public static bool operator !=(Segment left, Segment right) => !left.Equals(right);
}

public enum IntersectionType
{
    None,
    Point,
    SubSegment,
}

public struct SegmentIntersection
{
    public IntersectionType Type { get; init; }
    
    public Point Point { get; init; }
    
    public Segment SubSegment { get; init; }
}