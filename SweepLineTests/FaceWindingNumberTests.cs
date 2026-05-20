using System.Text;
using FaceBoundary;
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
        
        var result = new FaceWindingNumberComputer(segments).ComputeWindingNumbers();
        var sb = new StringBuilder();
        foreach (var faceWithWindingNumber in result.InsideFaces)
        {
            sb.Append($"{faceWithWindingNumber.WindingNumber}: {{ ");
            foreach (var segment in faceWithWindingNumber.FaceBoundary)
            {
                sb.Append($"{segment} ");
            }
            sb.Append("}\n");
        }
        
        Assert.That(sb.ToString(), Is.EqualTo("1: { [(0; 3) - (1; 1)] [(1; 1) - (3; 1)] [(3; 1) - (5; 2)] [(5; 2) - (4; 4)] [(4; 4) - (3; 3,5)] [(3; 3,5) - (3; 2)] [(3; 2) - (2; 3)] [(2; 3) - (3; 3,5)] [(3; 3,5) - (3; 5)] [(3; 5) - (1; 4)] [(1; 4) - (0; 3)] }\n2: { [(3; 3,5) - (2; 3)] [(2; 3) - (3; 2)] [(3; 2) - (3; 3,5)] }\n"));
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
        
        var result = new FaceWindingNumberComputer(segments).ComputeWindingNumbers();
        var sb = new StringBuilder();
        foreach (var faceWithWindingNumber in result.InsideFaces)
        {
            sb.Append($"{faceWithWindingNumber.WindingNumber}: {{ ");
            foreach (var segment in faceWithWindingNumber.FaceBoundary)
            {
                sb.Append($"{segment} ");
            }
            sb.Append("}\n");
        }

        Assert.That(sb.ToString(), Is.EqualTo("1: { [(0; 4) - (3; 2)] [(3; 2) - (1; 0)] [(1; 0) - (5; 0)] [(5; 0) - (4; 5)] [(4; 5) - (0; 4)] }\n"));
    }
}