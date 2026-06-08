using System.Text;
using FaceBoundary;
using SweepLine.Algorithm;
using SweepLine.DataStructuresLinkedListImpl;
using SweepLine.DataStructureTreeImpl;
using SweepLine.Primitives;

namespace SweepLineTests;

public class FaceWindingNumberTests
{
    [Test]
    public void Test1()
    {
        var segments = new List<Segment>
        {
            new()
            {
                StartPoint = new Point
                {
                    X = 0, Y = 3,
                },
                EndPoint = new Point
                {
                    X = 1, Y = 1,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 1, Y = 1,
                },
                EndPoint = new Point
                {
                    X = 3, Y = 1,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 3, Y = 1,
                },
                EndPoint = new Point
                {
                    X = 5, Y = 2,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 5, Y = 2
                },
                EndPoint = new Point
                {
                    X = 4, Y = 4,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 4, Y = 4,
                },
                EndPoint = new Point
                {
                    X = 2, Y = 3
                }
            },
            new()
            {
                StartPoint = new Point
                {
                   X = 2, Y = 3
                },
                EndPoint = new Point
                {
                    X = 3, Y = 2,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 3, Y = 2
                },
                EndPoint = new Point
                {
                    X = 3, Y = 5
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 3, Y = 5
                },
                EndPoint = new Point
                {
                    X = 1, Y = 4,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 1, Y = 4,
                },
                EndPoint = new Point
                {
                    X = 0, Y = 3,
                }
            },
        };
        
        var halfEdges = GetPlanarSubdivision(segments);
        var result = FaceWindingNumberComputer.ComputeWindingNumbers(segments, halfEdges);
        Assert.That(InsideFacesAsString(result.InsideFaces), Is.EqualTo("1: { [(0; 3) - (1; 1)] [(1; 1) - (3; 1)] [(3; 1) - (5; 2)] [(5; 2) - (4; 4)] [(4; 4) - (3; 3,5)] [(3; 3,5) - (3; 2)] [(3; 2) - (2; 3)] [(2; 3) - (3; 3,5)] [(3; 3,5) - (3; 5)] [(3; 5) - (1; 4)] [(1; 4) - (0; 3)] }\n2: { [(2; 3) - (3; 2)] [(3; 2) - (3; 3,5)] [(3; 3,5) - (2; 3)] }\n"));
    }

    [Test]
    public void Test2()
    {
        var segments = new List<Segment>
        {
            new()
            {
                StartPoint = new Point
                {
                    X = 0, Y = 4,
                },
                EndPoint = new Point
                {
                    X = 3, Y = 2,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 3, Y = 2,
                },
                EndPoint = new Point
                {
                    X = 1, Y = 0,
                },
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 1, Y = 0,
                },
                EndPoint = new Point
                {
                    X = 5, Y = 0,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 5, Y = 0,
                },
                EndPoint = new Point
                {
                    X = 4, Y = 5,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 4, Y = 5,
                },
                EndPoint = new Point
                {
                    X = 0, Y = 4,
                }
            }
        };

        var halfEdges = GetPlanarSubdivision(segments);
        var result = FaceWindingNumberComputer.ComputeWindingNumbers(segments, halfEdges);
        Assert.That(InsideFacesAsString(result.InsideFaces), Is.EqualTo("1: { [(0; 4) - (3; 2)] [(3; 2) - (1; 0)] [(1; 0) - (5; 0)] [(5; 0) - (4; 5)] [(4; 5) - (0; 4)] }\n"));
    }

    [Test]
    public void Test3()
    {
        var segments = new List<Segment>
        {
            new()
            {
                StartPoint = new Point
                {
                    X = 0, Y = 0,
                },
                EndPoint = new Point
                {
                    X = 3, Y = 0
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 3, Y = 0,
                },
                EndPoint = new Point
                {
                    X = 6, Y = 4,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 6, Y = 4,
                },
                EndPoint = new Point
                {
                    X = 4, Y = 5
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 4, Y = 5,
                },
                EndPoint = new Point
                {
                    X = 2, Y = 3,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 2, Y = 3,
                },
                EndPoint = new Point
                {
                    X = 5, Y = 0,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 5, Y = 0,
                },
                EndPoint = new Point
                {
                    X = 5, Y = 6,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 5, Y = 6
                },
                EndPoint = new Point
                {
                    X = 1, Y = 6,
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 1, Y = 6
                },
                EndPoint = new Point
                {
                    X = 0, Y = 0
                }
            }
        };

        var halfEdges = GetPlanarSubdivision(segments);
        var result = FaceWindingNumberComputer.ComputeWindingNumbers(segments, halfEdges);
        Assert.That(InsideFacesAsString(result.InsideFaces), Is.EqualTo("1: { [(0; 0) - (3; 0)] [(3; 0) - (3,857142857142857; 1,1428571428571428)] [(3,857142857142857; 1,1428571428571428) - (2; 3)] [(2; 3) - (4; 5)] [(4; 5) - (5; 4,5)] [(5; 4,5) - (5; 6)] [(5; 6) - (1; 6)] [(1; 6) - (0; 0)] }\n1: { [(5; 4,5) - (5; 2,6666666666666665)] [(5; 2,6666666666666665) - (6; 4)] [(6; 4) - (5; 4,5)] }\n2: { [(5; 2,6666666666666665) - (5; 4,5)] [(5; 4,5) - (4; 5)] [(4; 5) - (2; 3)] [(2; 3) - (3,857142857142857; 1,1428571428571428)] [(3,857142857142857; 1,1428571428571428) - (5; 2,6666666666666665)] }\n1: { [(5; 0) - (5; 2,6666666666666665)] [(5; 2,6666666666666665) - (3,857142857142857; 1,1428571428571428)] [(3,857142857142857; 1,1428571428571428) - (5; 0)] }\n"));
    }

    [Test]
    public void Test4()
    {
        var segments = VerticesToSegments([
            (-2, 0),
            (7, 0),
            (8, 1),
            (6, 6),
            (5, 7),
            (4, 7),
            (3, 6),
            (4.5, 4),
            (6, 3),
            (6.5, 4),
            (6, 5),
            (-1, 5),
            (-2, 4),
            (-1, 3),
            (0, 3),
            (4, 6),
            (3, 7),
            (-2, 7),
            (-3, 6),
            (-3, 1)
        ]);

        var halfEdges = GetPlanarSubdivision(segments);
        var result = FaceWindingNumberComputer.ComputeWindingNumbers(segments, halfEdges);
        Assert.That(InsideFacesAsString(result.InsideFaces), Is.EqualTo("1: { [(6; 6) - (5; 7)] [(5; 7) - (4; 7)] [(4; 7) - (3,5; 6,5)] [(3,5; 6,5) - (4; 6)] [(4; 6) - (3,36; 5,52)] [(3,36; 5,52) - (3,75; 5)] [(3,75; 5) - (6; 5)] [(6; 5) - (6,5; 4)] [(6,5; 4) - (6; 3)] [(6; 3) - (4,5; 4)] [(4,5; 4) - (3,75; 5)] [(3,75; 5) - (2,6666666666666665; 5)] [(2,6666666666666665; 5) - (0; 3)] [(0; 3) - (-1; 3)] [(-1; 3) - (-2; 4)] [(-2; 4) - (-1; 5)] [(-1; 5) - (2,6666666666666665; 5)] [(2,6666666666666665; 5) - (3,36; 5,52)] [(3,36; 5,52) - (3; 6)] [(3; 6) - (3,5; 6,5)] [(3,5; 6,5) - (3; 7)] [(3; 7) - (-2; 7)] [(-2; 7) - (-3; 6)] [(-3; 6) - (-3; 1)] [(-3; 1) - (-2; 0)] [(-2; 0) - (7; 0)] [(7; 0) - (8; 1)] [(8; 1) - (6; 6)] }\n2: { [(4,5; 4) - (6; 3)] [(6; 3) - (6,5; 4)] [(6,5; 4) - (6; 5)] [(6; 5) - (3,75; 5)] [(3,75; 5) - (4,5; 4)] }\n0: { [(3,75; 5) - (3,36; 5,52)] [(3,36; 5,52) - (2,6666666666666665; 5)] [(2,6666666666666665; 5) - (3,75; 5)] }\n2: { [(-2; 4) - (-1; 3)] [(-1; 3) - (0; 3)] [(0; 3) - (2,6666666666666665; 5)] [(2,6666666666666665; 5) - (-1; 5)] [(-1; 5) - (-2; 4)] }\n2: { [(3,5; 6,5) - (3; 6)] [(3; 6) - (3,36; 5,52)] [(3,36; 5,52) - (4; 6)] [(4; 6) - (3,5; 6,5)] }\n"));
    }

    [Test]
    public void Test5()
    {
        var segments = VerticesToSegments([
            (0, 0),
            (2, 0),
            (0, 2),
            (0.833333333, 0.333333333)
        ]);
        
        var halfEdges = GetPlanarSubdivision(segments);
        var result = FaceWindingNumberComputer.ComputeWindingNumbers(segments, halfEdges);
        Assert.That(InsideFacesAsString(result.InsideFaces), Is.EqualTo("1: { [(0; 0) - (2; 0)] [(2; 0) - (0; 2)] [(0; 2) - (0,833333333; 0,333333333)] [(0,833333333; 0,333333333) - (0; 0)] }\n"));
    }

    [Test, Order(6)]
    public void Test6()
    {
        var segments = VerticesToSegments([
            (0, 0),
            (2, 2),
            (0, 0),
            (4, 0),
            (4, 4),
            (0, 4)
        ]);

        var halfEdges = GetPlanarSubdivision(segments);
        var result = FaceWindingNumberComputer.ComputeWindingNumbers(segments, halfEdges);
        Assert.That(InsideFacesAsString(result.InsideFaces), Is.EqualTo("1: { [(0; 0) - (2; 2)] [(2; 2) - (0; 0)] [(0; 0) - (4; 0)] [(4; 0) - (4; 4)] [(4; 4) - (0; 4)] [(0; 4) - (0; 0)] }\n"));
    }

    private static List<Segment> VerticesToSegments(List<(double x, double y)> vertices)
    {
        var segments = new List<Segment>();
        
        for (var i = 0; i < vertices.Count; i++)
        {
            var vertex1 = vertices[i];
            var vertex2 = vertices[(i + 1) % vertices.Count];
            
            segments.Add(new Segment
            {
                StartPoint = new Point
                {
                    X = vertex1.x, Y = vertex1.y,
                },
                EndPoint = new Point
                {
                    X = vertex2.x, Y = vertex2.y,
                }
            });
        }

        return segments;
    }
    
    private static List<HalfEdge> GetPlanarSubdivision(List<Segment> segments)
    {
        var sweepLineProcessor = new SweepLineProcessor<SegmentWithReference>(
            new XStructureSortedSet<SegmentWithReference>(),
            new YStructure<SegmentWithReference>());
        
        sweepLineProcessor
            .AddSegments(segments
                .Select(EnsureSegmentOrientation)
                .Select(segment => new SegmentWithReference(segment)));

        var faceBoundaryBuilderVisitor = new FaceBoundaryBuilderVisitor();
        sweepLineProcessor.Process(faceBoundaryBuilderVisitor);

        return faceBoundaryBuilderVisitor.HalfEdges;
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

    private static string InsideFacesAsString(List<FaceWindingNumberComputer.FaceWithWindingNumber> insideFaces)
    {
        var sb = new StringBuilder();
        foreach (var faceWithWindingNumber in insideFaces)
        {
            sb.Append($"{faceWithWindingNumber.WindingNumber}: {{ ");
            foreach (var segment in faceWithWindingNumber.FaceBoundary)
            {
                sb.Append($"{segment} ");
            }
            sb.Append("}\n");
        }

        return sb.ToString();
    }
}