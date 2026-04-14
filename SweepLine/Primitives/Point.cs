namespace SweepLine.Primitives;

public readonly struct Point : IEquatable<Point>, IComparable<Point>
{
    public const double Eps = 1e-6;
    
    public double X { get; init; }
    
    public double Y { get; init; }
    
    public static double DistanceSquared(Point a, Point b)
    {
        var x = a.X - b.X;
        var y = a.Y - b.Y;
        return x * x + y * y;
    }

    public bool Equals(Point other) => DistanceSquared(this, other) < Eps * Eps;

    public int CompareTo(Point other)
        => Math.Abs(X - other.X) > Eps ? X.CompareTo(other.X) : Y.CompareTo(other.Y);

    public override bool Equals(object? obj) => obj is Point other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString()
    {
        return $"({X}; {Y})";
    }

    public static bool operator ==(Point left, Point right) => left.Equals(right);

    public static bool operator !=(Point left, Point right) => !left.Equals(right);

    public static bool operator >(Point left, Point right) => left.CompareTo(right) > 0;

    public static bool operator <(Point left, Point right) => left.CompareTo(right) < 0;

    public static bool operator >=(Point left, Point right) => left.CompareTo(right) >= 0;
    
    public static bool operator <=(Point left, Point right) => left.CompareTo(right) <= 0;
}