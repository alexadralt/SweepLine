//#define DRAW_DEBUG

#if DRAW_DEBUG        
using System.Drawing;
#endif
using FaceBoundary;
using SweepLine.Primitives;
using Point = SweepLine.Primitives.Point;

namespace MinkowskiSum;

public static class MinkowskiSumComputer
{
    public static (List<Segment> Boundary, List<List<Segment>> Holes) ComputeMinkowskiSum(List<Point> figureA, List<Point> figureB)
    {
        var convolutionCycle = ComputeConvolutionCycle(figureA, figureB);
#if DRAW_DEBUG        
        DumpBitmap("convolution.png", convolutionCycle);
#endif
        var result = new FaceWindingNumberComputer(convolutionCycle).ComputeWindingNumbers(onlyFacesWithZero: true);
        
        return (result.OuterFace, result.InsideFaces.Select(face => face.FaceBoundary).ToList());
    }
    
#if DRAW_DEBUG
    private static void DumpBitmap(string fileName, List<Segment> segmentsA,
        float xOffset = 200, float yOffset = 800)
    {
        var bitmap = new Bitmap(1000, 1000);
        using var g = Graphics.FromImage(bitmap);
        
        DrawSegments(segmentsA, Color.LawnGreen, g, xOffset, yOffset);
        
        bitmap.Save(fileName);
    }

    private static void DrawSegments(List<Segment> segments, Color color, Graphics g, float xOffset, float yOffset)
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

    private static List<Segment> ComputeConvolutionCycle(List<Point> figureA, List<Point> figureB)
    {
        var normalsA = GetNormals(figureA);
        var normalsB = GetNormals(figureB);

        var i = 0;
        var j = 0;

        var semiCross1 = SemiCross(normalsB[0], normalsA[0]);
        var semiCross2 = SemiCross(normalsB[1], normalsA[0]);

        while (semiCross1 < -Point.Eps || semiCross2 >= -Point.Eps) {
            j++;
            
            semiCross1 = SemiCross(normalsB[j % normalsB.Length], normalsA[0]);
            semiCross2 = SemiCross(normalsB[(j + 1) % normalsB.Length], normalsA[0]);
        }

        var convexHull = new List<Point>(figureA.Count + figureB.Count);

        var offset = j;

        do
        {
            var semiCross = SemiCross(normalsA[i % normalsA.Length], normalsB[j % normalsB.Length]);
            var convexCheck = SemiCross(normalsA[(i + normalsA.Length - 1) % normalsA.Length], normalsA[i % normalsA.Length]);
            
            semiCross1 = SemiCross(normalsA[i % normalsA.Length], normalsB[j % normalsB.Length]);
            semiCross2 = SemiCross(normalsA[i % normalsA.Length], normalsB[(j + normalsB.Length - 1) % normalsB.Length]);

            convexHull.Add(figureA[i % figureA.Count] + figureB[j % figureB.Count]);

            if (convexCheck >= -Point.Eps)
            {
                if (semiCross > Point.Eps)
                {
                    i++;
                }
                else if (semiCross < -Point.Eps)
                {
                    j++;
                }
                else
                {
                    i++;
                    j++;
                }
            }
            else
            {
                if (semiCross1 > Point.Eps && semiCross2 < -Point.Eps)
                {
                    i++;
                }
                else if (semiCross1 >= -Point.Eps && semiCross2 < -Point.Eps)
                {
                    i++;
                    j++;
                }
                else
                {
                    j++;
                }
            }

        } while (i % normalsA.Length != 0 || (j - offset) % normalsB.Length != 0);

        return convexHull.Select(
            (_, it) => new Segment
                {
                    StartPoint = convexHull[it],
                    EndPoint = convexHull[(it + 1) % convexHull.Count]
                }).ToList();
    }

    private static Point[] GetNormals(List<Point> figure)
    {
        var normals = new Point[figure.Count];
        for (int i = 0; i < figure.Count; i++) {
            var edge = figure[(i + 1) % figure.Count] - figure[i];
            
            normals[i] = new Point
            {
                X = edge.Y,
                Y = -edge.X,
            };
        }
        return normals;
    }

    private static double SemiCross(Point vectorA, Point vectorB)
    {
        return vectorA.X * vectorB.Y - vectorA.Y * vectorB.X;
    }
}