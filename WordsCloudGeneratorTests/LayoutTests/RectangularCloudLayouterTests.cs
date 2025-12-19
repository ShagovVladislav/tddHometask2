using System.Drawing;
using FluentAssertions;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;


namespace WordsCloudGeneratorTests.LayoutTests;

[TestFixture]
public class RectangularCloudLayouterTests
{
    [Test]
    public void PutNextRectangle_ShouldPlaceRectangle_WhenEnoughSpace()
    {
        var canvas = new Size(100, 100);
        var layouter = new RectangularCloudLayouter(canvas, padding: 0);
        var rectSize = new Size(20, 20);

        var result = layouter.PutNextRectangle(rectSize);

        result.IsSuccess.Should().BeTrue();
        result.Value.Width.Should().Be(20);
        result.Value.Height.Should().Be(20);
        layouter.PlacedRectangles.Should().HaveCount(1);
    }

    [Test]
    public void PutNextRectangle_ShouldFail_WhenNoVerticalSpace()
    {
        var canvas = new Size(30, 20);
        var layouter = new RectangularCloudLayouter(canvas, padding: 0);
        layouter.PutNextRectangle(new Size(30, 20));

        var result = layouter.PutNextRectangle(new Size(30, 20));

        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void LayoutCloud_ShouldAutoCalculateCanvas_WhenCanvasIsEmpty()
    {
        var layouter = new RectangularCloudLayouter(Size.Empty);
        var elements = new[]
        {
            new CloudElement("hello", 5),
            new CloudElement("world", 3)
        };
        var fontRange = new FontSizeRange(10, 20);

        var result = layouter.LayoutCloud(elements, fontRange);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.All(e => e.Rectangle != Rectangle.Empty).Should().BeTrue();
    }
}

