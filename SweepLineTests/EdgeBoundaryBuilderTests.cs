using System.Diagnostics;
using System.Drawing;
using EdgeBoundary;
using SweepLine.Primitives;
using Point = SweepLine.Primitives.Point;

namespace SweepLineTests;

public class EdgeBoundaryBuilderTests
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
        
        var result = new EdgeBoundaryBuilder(segments).ComputePlaneSubdivision();
        DumpBitmap(result, "out-test-1.png");
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
                    X = 4, Y = 4,
                },
                EndPoint = new Point
                {
                    X = 5, Y = 2
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
                    X = 4, Y = 4,
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
                    X = 1, Y = 4,
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
                    X = 0, Y = 3,
                },
                EndPoint = new Point
                {
                    X = 1, Y = 4,
                }
            },
        };

        var result = new EdgeBoundaryBuilder(segments).ComputePlaneSubdivision();
        DumpBitmap(result, "out-test-2.png");
    }

    private void DumpBitmap(List<HalfEdge> result, string fileName)
    {
#pragma warning disable CA1416
        var bitmap = new Bitmap(1000, 1000);

        var colors = new[]
        {
            Color.Cyan,
            Color.Blue,
            Color.BlueViolet,
            Color.Green,
            Color.LightGreen,
            Color.Red,
            Color.White,
            Color.Yellow,
            Color.DeepPink,
            Color.Purple,
            Color.Orange,
            Color.Tomato,
            Color.LightGray,
            Color.Brown,
            Color.Fuchsia,
            Color.Olive,
            Color.OliveDrab,
            Color.SteelBlue,
            Color.Gold,
            Color.PaleVioletRed,
            Color.Teal,
            Color.SaddleBrown,
            Color.Goldenrod,
            Color.Aquamarine,
        };
        var colorIndex = 0;

        using (var g = Graphics.FromImage(bitmap))
        {
            var pen = new Pen(Color.Cyan);
            var visited = new HashSet<int>();

            var currentIndex = 0;
            while (true)
            {
                var halfEdge = result[currentIndex];
                visited.Add(currentIndex);

                var ox = halfEdge.OriginPoint.X;
                var oy = halfEdge.OriginPoint.Y;

                var dx = halfEdge.DestinationPoint.X;
                var dy = halfEdge.DestinationPoint.Y;

                var directionX = dx - ox;
                var directionY = dy - oy;
                var mag = Math.Sqrt(directionX * directionX + directionY * directionY);
                directionX /= mag;
                directionY /= mag;

                var displacementX = -directionY;
                var displacementY = directionX;
                displacementX *= 0.025;
                displacementY *= 0.025;

                g.DrawLine(pen,
                    new PointF(300 + (float)(ox + displacementX) * 100, 800 + (float)(oy + displacementY) * -100),
                    new PointF(300 + (float)(dx + displacementX) * 100, 800 + (float)(dy + displacementY) * -100));

                currentIndex = halfEdge.NextIndex;

                if (currentIndex < 0)
                {
                    throw new UnreachableException();
                }

                if (visited.Count == result.Count)
                {
                    break;
                }

                if (!visited.Contains(currentIndex))
                {
                    continue;
                }

                currentIndex = Enumerable.Range(0, result.Count).First(index => !visited.Contains(index));
                colorIndex = (colorIndex + 1) % colors.Length;
                pen = new Pen(colors[colorIndex]);
            }
        }

        bitmap.Save(fileName);
#pragma warning restore CA1416
    }
}