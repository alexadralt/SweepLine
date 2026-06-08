using System.Runtime.CompilerServices;
using SweepLine.Primitives;

namespace FaceBoundary;

public static class PointWindingNumberComputer
{
    public static int ComputeWindingNumber(List<Segment> boundary, Point point, bool checkPointOnBoundary = false)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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