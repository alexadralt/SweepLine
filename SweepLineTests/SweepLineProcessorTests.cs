using SweepLine.Algorithm;
using SweepLine.DataStructuresLinkedListImpl;
using SweepLine.Primitives;

namespace SweepLineTests;

public class SweepLineProcessorTests
{
    private static Dictionary<Point, List<Segment>> SegmentStartEvents { get; set; } = [];
    
    private static Dictionary<Point, List<Segment>> SegmentSubsequenceEvents { get; set; } = [];
    
    private static Dictionary<Point, List<Segment>> SegmentEndEvents { get; set; } = [];
    
    private class SweepLineTestVisitor : ISweepLineVisitor<XStructureNode, YStructureNode>
    {
        public void VisitEndingSegments(XStructureNode eventPoint, IEnumerable<YStructureNode> segments)
        {
            var segmentList = segments.Select(segment => segment.Value).ToList();
            var list = SegmentEndEvents.GetValueOrDefault(eventPoint.Value);
            list?.AddRange(segmentList);
            SegmentEndEvents[eventPoint.Value] = list ?? segmentList;
        }

        public void VisitSubsequence(XStructureNode eventPoint, IEnumerable<YStructureNode> subsequence)
        {
            SegmentSubsequenceEvents[eventPoint.Value] = subsequence.Select(segment => segment.Value).ToList();
        }

        public void VisitStartingSegments(XStructureNode eventPoint, IEnumerable<YStructureNode> segments)
        {
            var segmentList = segments.Select(segment => segment.Value).ToList();
            var list = SegmentStartEvents.GetValueOrDefault(eventPoint.Value);
            list?.AddRange(segmentList);
            SegmentStartEvents[eventPoint.Value] = list ?? segmentList;
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
            }
        });
        
        processor.Process(new SweepLineTestVisitor());

        Assert.Pass();
    }
}