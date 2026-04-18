using SweepLine.Algorithm;
using SweepLine.DataStructuresLinkedListImpl;
using SweepLine.Primitives;

namespace SweepLineTests;

public class SweepLineProcessorTests
{
    private static Dictionary<Point, List<Segment>> SegmentEvents { get; } = [];
    
    private class SweepLineTestVisitor : ISweepLineVisitor
    {
        public void VisitIntersectingSegments(Point point, IEnumerable<IEnumerable<Segment>> segments)
        {
            var segmentList = segments.SelectMany(list => list).ToList();
            SegmentEvents[point] = segmentList;

            Console.Write($"Intersecting segments in point {point}: ");
            foreach (var segment in segmentList)
            {
                Console.Write($"{segment}; ");
            }
            Console.WriteLine("|");
        }
    }

    private void CheckIntersections(List<Segment> segments)
    {
        var expectedIntersections = new Dictionary<Point, HashSet<Segment>>();
        
        for (var i = 0; i < segments.Count; i++)
        {
            AddSegment(expectedIntersections, segments[i].StartPoint, segments[i]);
            AddSegment(expectedIntersections, segments[i].EndPoint, segments[i]);
            
            for (var j = i + 1; j < segments.Count; j++)
            {
                var intersectiomResult = Segment.FindIntersection(segments[i], segments[j]);

                if (intersectiomResult.Type == IntersectionType.Point)
                {
                    AddSegment(expectedIntersections, intersectiomResult.Point, segments[i]);
                    AddSegment(expectedIntersections, intersectiomResult.Point, segments[j]);
                }
            }
        }

        foreach (var intersection in expectedIntersections)
        {
            var segmentList = SegmentEvents[intersection.Key];
            foreach (var segment in intersection.Value)
            {
                if (!segmentList.Contains(segment))
                {
                    Assert.Fail();
                }
            }
        }
        
        return;

        void AddSegment(Dictionary<Point, HashSet<Segment>> intersections, Point point, Segment segment)
        {
            var intersecting = intersections.GetValueOrDefault(point);
            if (intersecting is not null)
            {
                intersecting.Add(segment);
            }
            else
            {
                expectedIntersections[point] = [segment];
            }
        }
    }

    [Test]
    public void Test1()
    {
        var processor = new SweepLineProcessor(new XStructure(), new YStructure());

        var segments = new List<Segment>
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
            new()
            {
                StartPoint = new Point
                {
                    X = 3.5, Y = 0,
                },
                EndPoint = new Point
                {
                    X = 3.5, Y = 3.5,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 3.5, Y = 3,
                },
                EndPoint = new Point
                {
                    X = 3.5, Y = 5,
                }
            },
        };
        
        processor.AddSegments(segments);
        
        processor.Process(new SweepLineTestVisitor());
        
        CheckIntersections(segments);
    }
}