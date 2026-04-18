using SweepLine.Algorithm;
using SweepLine.DataStructuresLinkedListImpl;
using SweepLine.Primitives;

namespace SweepLineTests;

public class SweepLineProcessorTests
{
    private static Dictionary<Point, List<Segment>> SegmentSubsequenceEvents { get; } = [];
    
    private class SweepLineTestVisitor : ISweepLineVisitor<XStructureNode, YStructureNode>
    {
        public void VisitIntersectingSegments(Point point, IEnumerable<IEnumerable<Segment>> segments)
        {
            var segmentList = segments.SelectMany(list => list).ToList();
            SegmentSubsequenceEvents[point] = segmentList;

            Console.Write($"Intersecting segments in point {point}: ");
            foreach (var segment in segmentList)
            {
                Console.Write($"{segment}; ");
            }
            Console.WriteLine("|");
        }
    }

    [Test]
    public void Test1()
    {
        var processor = new SweepLineProcessor<XStructure, YStructure, XStructureNode, YStructureNode>(
                new XStructure(), new YStructure());
        
        processor.AddSegments(new List<Segment>
        {
            new()
            {
                StartPoint = new Point
                {
                    X = -1, Y = 0,
                },
                EndPoint = new Point
                {
                    X = 6, Y = 6,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 0, Y = 7,
                },
                EndPoint = new Point
                {
                    X = 6, Y = -3,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = -1, Y = 5,
                },
                EndPoint = new Point
                {
                    X = 6, Y = 2,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = -2, Y = 2.9433962264150946,
                },
                EndPoint = new Point
                {
                    X = 3, Y = 2.9433962264150946,
                },
            },
            new()
            {
                StartPoint = new Point
                {
                    X = -1, Y = 5,
                },
                EndPoint = new Point
                {
                    X = 2.5, Y = 3.5,
                },
            },
        });
        
        processor.Process(new SweepLineTestVisitor());
    }
}