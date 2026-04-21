using SweepLine.Algorithm;
using SweepLine.DataStructuresLinkedListImpl;
using SweepLine.Primitives;

namespace SweepLineTests;

public class SweepLineProcessorTests
{
    private static Dictionary<Point, List<Segment>> SegmentEvents { get; } = [];
    
    private class SweepLineTestVisitor : ISweepLineVisitor<Segment>
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
        var foundIndices = new Dictionary<Point, HashSet<int>>();

        for (var i = 0; i < segments.Count; i++)
        {
            AddSegment(segments[i].StartPoint, segments, i);
            AddSegment(segments[i].EndPoint, segments, i);
            
            for (var j = i + 1; j < segments.Count; j++)
            {
                var intersectiomResult = Segment.FindIntersection(segments[i], segments[j]);

                if (intersectiomResult.Type == IntersectionType.Point)
                {
                    AddSegment(intersectiomResult.Point, segments, i);
                    AddSegment(intersectiomResult.Point, segments, j);
                }
                else if (intersectiomResult.Type == IntersectionType.SubSegment)
                {
                    var a = intersectiomResult.SubSegment.StartPoint;
                    var b = intersectiomResult.SubSegment.EndPoint;

                    if (SegmentContainsPoint(segments[i], a))
                    {
                        AddSegment(a, segments, i);
                    }

                    if (SegmentContainsPoint(segments[i], b))
                    {
                        AddSegment(b, segments, i);
                    }
                    
                    if (SegmentContainsPoint(segments[j], a))
                    {
                        AddSegment(a, segments, j);
                    }

                    if (SegmentContainsPoint(segments[j], b))
                    {
                        AddSegment(b, segments, j);
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

        return;
        
        void AddSegment(Point point, List<Segment> allSegments, int segmentIndex)
        {
            var intersecting = expectedIntersections.GetValueOrDefault(point);
            if (intersecting is not null)
            {
                var isNew = foundIndices[point].Add(segmentIndex);
                if (isNew)
                {
                    intersecting.Add(allSegments[segmentIndex]);
                }
            }
            else
            {
                foundIndices[point] = [segmentIndex];
                expectedIntersections[point] = [allSegments[segmentIndex]];
            }
        }
    }

    private bool SegmentContainsPoint(Segment segment, Point point)
    {
        if (point == segment.StartPoint)
        {
            return true;
        }
        
        var segmentToCheck = new Segment
        {
            StartPoint = segment.StartPoint,
            EndPoint = point
        };
        
        var intersectionResult = Segment.FindIntersection(segmentToCheck, segment);
        
        return intersectionResult.Type == IntersectionType.SubSegment &&
               intersectionResult.SubSegment.EndPoint >= point;
    }

    [Test]
    public void Test1()
    {
        var processor = new SweepLineProcessor<Segment>(new XStructure<Segment>(), new YStructure<Segment>());

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
            new()
            {
                StartPoint = new Point
                {
                    X = 1.3333333333333, Y = 2,
                },
                EndPoint = new Point
                {
                    X = 3.6666666666667, Y = 4,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 1.5, Y = 4.5,
                },
                EndPoint = new Point
                {
                    X = 3, Y = 2,
                },
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 1.8, Y = 2.4,
                },
                EndPoint = new Point
                {
                    X = 3.2, Y = 3.6,
                },
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 0.9, Y = 5.5,
                },
                EndPoint = new Point
                {
                    X = 2.1, Y = 3.5,
                },
            },
        };
        
        processor.AddSegments(segments);
        
        processor.Process(new SweepLineTestVisitor());
        
        CheckIntersections(segments);
    }
}