using System.Drawing;
using FluentAssertions;
using WordsCloudGenerator.models;
using WordsCloudGenerator.visualization;

namespace WordsCloudGeneratorTests.VizualizationTests;

[TestFixture]
public class CloudVisualizerTests
{
    [Test]
    public void Visualize_ShouldFail_WhenElementsAreEmpty()
    {
        var visualizer = new CloudVisualizer();
        var elements = Array.Empty<CloudElement>();
        var settings = new VisualizationSettings();

        var result = visualizer.Visualize(elements, settings);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(VisualizationErrors.NoElements);
    }

    [Test]
    public void Visualize_ShouldFail_WhenFontSizeIsMissing()
    {
        var visualizer = new CloudVisualizer();
        var elements = new[]
        {
            new CloudElement("word", 1)
            {
                Rectangle = new Rectangle(0, 0, 50, 20)
            }
        };
        var settings = new VisualizationSettings();

        var result = visualizer.Visualize(elements, settings);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(VisualizationErrors.MissingFontSize);
    }
    
    [Test]
    public void Visualize_ShouldFail_WhenRectangleIsEmpty()
    {
        var visualizer = new CloudVisualizer();
        var elements = new[]
        {
            new CloudElement("word", 1)
            {
                FontSize = 20
            }
        };
        var settings = new VisualizationSettings();

        var result = visualizer.Visualize(elements, settings);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(VisualizationErrors.MissingRectangle);
    }
    
    [Test]
    public void Visualize_ShouldAutoCalculateImageSize_WhenImageSizeIsEmpty()
    {
        var visualizer = new CloudVisualizer();
        var elements = new[]
        {
            new CloudElement("hello", 5)
            {
                FontSize = 30,
                Rectangle = new Rectangle(0, 0, 100, 40)
            }
        };

        var settings = new VisualizationSettings
        {
            ImageSize = Size.Empty
        };

        var result = visualizer.Visualize(elements, settings);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Width.Should().BeGreaterThan(0);
        result.Value.Height.Should().BeGreaterThan(0);

        result.Value.Dispose();
    }
    
    [Test]
    public void SaveToFile_ShouldCreateDirectoryAndSaveFile()
    {
        var visualizer = new CloudVisualizer();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "cloud.png");

        var elements = new[]
        {
            new CloudElement("test", 1)
            {
                FontSize = 20,
                Rectangle = new Rectangle(0, 0, 80, 30)
            }
        };

        var settings = new VisualizationSettings
        {
            ImageSize = new Size(300, 200)
        };

        var result = visualizer.SaveToFile(elements, settings, filePath);

        result.IsSuccess.Should().BeTrue();
        Directory.Exists(tempDir).Should().BeTrue();
        File.Exists(filePath).Should().BeTrue();
    }

}