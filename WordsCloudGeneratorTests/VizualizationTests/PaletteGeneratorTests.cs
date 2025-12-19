using System.Drawing;
using FluentAssertions;
using WordsCloudGenerator.visualization;

namespace WordsCloudGeneratorTests.VizualizationTests;

[TestFixture]
public class PaletteGeneratorTests
{
    [Test]
    public void GenerateColorPalette_ShouldReturnEmpty_WhenCountIsZero()
    {
        var baseColor = Color.Red;

        var result = PaletteGenerator.GenerateColorPalette(0, baseColor);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    
    [Test]
    public void GenerateColorPalette_ShouldReturnRequestedNumberOfColors()
    {
        var baseColor = Color.Blue;
        var count = 5;

        var result = PaletteGenerator.GenerateColorPalette(count, baseColor);

        result.Should().HaveCount(count);
    }
}