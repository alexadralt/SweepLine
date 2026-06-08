using System.Drawing;
using FaceBoundary;
using SweepLine.Algorithm;
using SweepLine.DataStructuresLinkedListImpl;
using SweepLine.DataStructureTreeImpl;
using SweepLine.Primitives;
using Point = SweepLine.Primitives.Point;

namespace SweepLineTests;

public class FaceBoundaryBuilderTests
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
        
        var result = GetPlanarSubdivision(segments);
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

        var result = GetPlanarSubdivision(segments);
        DumpBitmap(result, "out-test-2.png");
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
                    X = 2, Y = 0
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
                    X = 1, Y = 3
                },
                EndPoint = new Point
                {
                    X = 2, Y = 0
                }
            },
            new()
            {
                StartPoint = new Point
                {
                    X = 0, Y = 0
                },
                EndPoint = new Point
                {
                    X = 1, Y = 3
                }
            }
        };

        var result = GetPlanarSubdivision(segments);
        DumpBitmap(result, "out-test-3.png");
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

    private void DumpBitmap(List<HalfEdge> result, string fileName)
    {
#pragma warning disable CA1416
        var bitmap = new Bitmap(1000, 1000);

        var colors = new[]
        {
            Color.DodgerBlue,
            Color.BlueViolet,
            Color.LightGreen,
            Color.Red,
            Color.Orange,
            Color.Tomato,
            Color.Fuchsia,
            Color.Olive,
            Color.OliveDrab,
            Color.SteelBlue,
            Color.Gold,
            Color.PaleVioletRed,
            Color.Teal,
            Color.SaddleBrown,
            Color.Goldenrod,
        };
        var colorIndex = 0;

        using (var g = Graphics.FromImage(bitmap))
        {
            var pen = new Pen(colors[0]);

            foreach (var faceBoundary in new FaceBoundaryIterator(result))
            {
                foreach (var segment in faceBoundary)
                {
                    var ox = segment.StartPoint.X;
                    var oy = segment.StartPoint.Y;

                    var dx = segment.EndPoint.X;
                    var dy = segment.EndPoint.Y;

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
                }
                
                colorIndex = (colorIndex + 1) % colors.Length;
                pen = new Pen(colors[colorIndex]);
            }
        }

        bitmap.Save(fileName);
#pragma warning restore CA1416
    }
}