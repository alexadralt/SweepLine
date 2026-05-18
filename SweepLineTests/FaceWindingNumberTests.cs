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
    }
}