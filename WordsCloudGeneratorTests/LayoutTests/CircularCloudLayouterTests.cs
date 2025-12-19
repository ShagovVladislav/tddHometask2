using System.Drawing;
using FluentAssertions;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;

namespace WordsCloudGeneratorTests.LayoutTests;

[TestFixture]
public class CircularCloudLayouterTests
{
    [Test]
    public void PutNextRectangle_FirstRectangle_ShouldBeCentered()
    {
        var center = new Point(100, 100);
        var layouter = new CircularCloudLayouter(center);
        var size = new Size(20, 20);

        var result = layouter.PutNextRectangle(size);

        result.IsSuccess.Should().BeTrue();
        result.Value.X.Should().Be(center.X - 10);
        result.Value.Y.Should().Be(center.Y - 10);
    }

    [Test]
    public void PutNextRectangle_ShouldNotIntersect_PreviouslyPlacedRectangles()
    {
        var layouter = new CircularCloudLayouter(new Point(200, 200));

        var r1 = layouter.PutNextRectangle(new Size(40, 40)).Value;
        var r2 = layouter.PutNextRectangle(new Size(40, 40)).Value;

        r1.IntersectsWith(r2).Should().BeFalse();
        layouter.PlacedRectangles.Should().HaveCount(2);
    }

    [Test]
    public void LayoutCloud_ShouldFail_ForEmptyElements()
    {
        var layouter = new CircularCloudLayouter(new Point(50, 50));
        var fontRange = new FontSizeRange(10, 20);

        var result = layouter.LayoutCloud([], fontRange);

        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void LayoutCloud_ShouldPlaceAllElements()
    {
        var layouter = new CircularCloudLayouter(new Point(300, 300));
        var elements = new[]
        {
            new CloudElement("one", 10),
            new CloudElement("two", 8),
            new CloudElement("three", 5)
        };
        var fontRange = new FontSizeRange(10, 30);

        var result = layouter.LayoutCloud(elements, fontRange);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value.All(e => e.Rectangle != Rectangle.Empty).Should().BeTrue();
    }
}
