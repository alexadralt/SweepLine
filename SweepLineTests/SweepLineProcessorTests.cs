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
        var expectedIntersections = new Dictionary<Point, List<Segment>>();

        var visitedPoints = new HashSet<Point>();
        var addedIndices = new Dictionary<Point, HashSet<int>>();
        
        for (var i = 0; i < segments.Count; i++)
        {
            var startPoint = segments[i].StartPoint;
            var endPoint = segments[i].EndPoint;
            
            foreach (var segment in segments)
            {
                if (!visitedPoints.Contains(startPoint) && SegmentContainsPoint(segment, startPoint))
                {
                    AddSegment(expectedIntersections, startPoint, segment);
                }
                
                if (!visitedPoints.Contains(endPoint) && SegmentContainsPoint(segment, endPoint))
                {
                    AddSegment(expectedIntersections, endPoint, segment);
                }
            }

            visitedPoints.Add(startPoint);
            visitedPoints.Add(endPoint);

            for (var j = i + 1; j < segments.Count; j++)
            {
                var intersectionResult = Segment.FindIntersection(segments[i], segments[j]);

                if (intersectionResult.Type == IntersectionType.Point)
                {
                    var indices = addedIndices.GetValueOrDefault(intersectionResult.Point);
                    if (indices is null)
                    {
                        indices = new HashSet<int>();
                        addedIndices[intersectionResult.Point] = indices;
                    }
                    
                    if (intersectionResult.Point != segments[i].StartPoint
                        && intersectionResult.Point != segments[i].EndPoint)
                    {
                        if (!indices.Contains(i))
                        {
                            AddSegment(expectedIntersections, intersectionResult.Point, segments[i]);
                            indices.Add(i);
                        }
                    }

                    if (intersectionResult.Point != segments[j].StartPoint
                        && intersectionResult.Point != segments[j].EndPoint)
                    {
                        if (!indices.Contains(j))
                        {
                            AddSegment(expectedIntersections, intersectionResult.Point, segments[j]);
                            indices.Add(j);
                        }
                    }
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

        Assert.That(SegmentEvents.Count, Is.EqualTo(expectedIntersections.Count));
        foreach (var (point, expectation) in expectedIntersections)
        {
            Assert.That(SegmentEvents[point].Count, Is.EqualTo(expectation.Count));
        }
    }
    
    private void AddSegment(Dictionary<Point, List<Segment>> intersections, Point point, Segment segment)
    {
        var intersecting = intersections.GetValueOrDefault(point);
        if (intersecting is not null)
        {
            intersecting.Add(segment);
        }
        else
        {
            intersections[point] = [segment];
        }
    }

    private bool SegmentContainsPoint(Segment segment, Point point)
    {
        if (point == segment.StartPoint)
        {
            return true;
        }
        
        var segmentToCheck = segment with { EndPoint = point };
        var intersectionResult = Segment.FindIntersection(segmentToCheck, segment);
        
        return intersectionResult.Type == IntersectionType.SubSegment &&
               intersectionResult.SubSegment.EndPoint >= point;
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
            new()
            {
                StartPoint = new Point
                {
                    X = 3.2, Y = 3.2,
                },
                EndPoint = new Point
                {
                    X = 6, Y = 2,
                },
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 0, Y = 4,
                },
                EndPoint = new Point
                {
                    X = 5, Y = 4,
                },
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 0, Y = 4,
                },
                EndPoint = new Point
                {
                    X = 5, Y = 4,
                },
            }, // same twice. intentional
        };
        
        processor.AddSegments(segments);
        
        processor.Process(new SweepLineTestVisitor());
        
        CheckIntersections(segments);
    }
}