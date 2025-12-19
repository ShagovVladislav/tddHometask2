using System.Drawing;
using FluentAssertions;
using Moq;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;
using WordsCloudGenerator.visualization;
using WordsCloudGenerator.Words;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGeneratorTests;

[TestFixture]
public class PipelineTests
{

    [Test]
    public void FullPipeline_ShouldGenerateWordCloud()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var inputFile = Path.Combine(tempDir, "input.txt");
        var outputFile = Path.Combine(tempDir, "cloud.png");
        File.WriteAllText(inputFile, "hello world hello");

        var reader = new TextFileReader();
        var normalizer = new WordNormalizer();
        var stopWords = new StopWordsFilter(Array.Empty<string>());

        var lemmatizerMock = new Mock<ILemmatizer>();
        lemmatizerMock
            .Setup(l => l.Lemmatize(It.IsAny<string>()))
            .Returns<string>(w => Result<string>.Ok(w));

        var processor = new WordProcessor(normalizer, stopWords, lemmatizerMock.Object);
        var layouter = new RectangularCloudLayouter(new Size(600, 400), padding: 5);
        var visualizer = new CloudVisualizer();
        var fontRange = new FontSizeRange(20, 50);
        var visualizationSettings = new VisualizationSettings
        {
            ImageSize = new Size(600, 400),
            BackgroundColor = Color.Black,
            ColorTheme = [Color.White]
        };

        var readResult = reader.ReadWords(inputFile);
        var processResult = processor.ProcessWords(readResult.Value);
        var layoutResult = layouter.LayoutCloud(processResult.Value, fontRange);
        var saveResult = visualizer.SaveToFile(layoutResult.Value, visualizationSettings, outputFile);

        readResult.IsSuccess.Should().BeTrue();
        processResult.IsSuccess.Should().BeTrue();
        layoutResult.IsSuccess.Should().BeTrue();
        saveResult.IsSuccess.Should().BeTrue();
        File.Exists(outputFile).Should().BeTrue();
    }
}