//#define DRAW_DEBUG

#if DRAW_DEBUG        
using System.Drawing;
#endif
using FaceBoundary;
using SweepLine.Algorithm;
using SweepLine.DataStructuresLinkedListImpl;
using SweepLine.DataStructureTreeImpl;
using SweepLine.Primitives;
using Point = SweepLine.Primitives.Point;

namespace MinkowskiSum;

public static class MinkowskiSumComputer
{
    public static (List<Segment> Boundary, List<List<Segment>> Holes) ComputeMinkowskiSum(List<Point> figureA, List<Point> figureB)
    {
        var convolutionCycle = ConvolutionComputer.ComputeConvolutionCycle(figureA, figureB);

#if DRAW_DEBUG        
        DumpBitmap("convolution.png", convolutionCycle);
#endif
        
        var sweepLineProcessor = new SweepLineProcessor<SegmentWithReference>(
            new XStructureSortedSet<SegmentWithReference>(),
            new YStructure<SegmentWithReference>());
        
        sweepLineProcessor
            .AddSegments(convolutionCycle
                .Select(EnsureSegmentOrientation)
                .Select(segment => new SegmentWithReference(segment)));

        var faceBoundaryBuilderVisitor = new FaceBoundaryBuilderVisitor();
        sweepLineProcessor.Process(faceBoundaryBuilderVisitor);
        
        var result = FaceWindingNumberComputer.ComputeWindingNumbers(
            convolutionCycle, faceBoundaryBuilderVisitor.HalfEdges, onlyFacesWithZero: true);
        
        return (result.OuterFace, result.InsideFaces.Select(face => face.FaceBoundary).ToList());
    }

    private static Segment EnsureSegmentOrientation(Segment segment)
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
}