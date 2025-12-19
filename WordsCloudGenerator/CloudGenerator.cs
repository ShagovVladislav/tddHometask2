using System.Drawing;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;
using WordsCloudGenerator.visualization;
using WordsCloudGenerator.Words;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator;

public class CloudGenerator : ICloudGenerator
{
    private readonly IFileReader fileReader;
    private readonly IWordProcessor wordProcessor;
    private ICloudLayouterFactory layouterFactory;
    private readonly ICloudVisualizer visualizer;

    private readonly WordsSettings wordsSettings;
    private readonly LayoutSettings layoutSettings;
    private readonly VisualizationSettings visualizationSettings;

    private string inputFile;
    private string outputFile = "cloud.png";

    public CloudGenerator(
        IFileReader fileReader,
        IWordProcessor wordProcessor,
        ICloudLayouterFactory layouterFactory,
        ICloudVisualizer visualizer,
        WordsSettings wordsSettings,
        LayoutSettings layoutSettings,
        VisualizationSettings visualizationSettings)
    {
        this.fileReader = fileReader;
        this.wordProcessor = wordProcessor;
        this.layouterFactory = layouterFactory;
        this.visualizer = visualizer;
        this.wordsSettings = wordsSettings;
        this.layoutSettings = layoutSettings;
        this.visualizationSettings = visualizationSettings;
    }

    public ICloudGenerator From(string filePath)
    {
        inputFile = filePath;
        return this;
    }

    public ICloudGenerator To(string filePath)
    {
        outputFile = filePath;
        return this;
    }

    public ICloudGenerator WithColorTheme(params Color[] colors)
    {
        if (colors.Length == 1)
        {
            colors = PaletteGenerator.GenerateColorPalette(5, colors[0]);
        }
        visualizationSettings.ColorTheme = colors;
        return this;
    }

    public ICloudGenerator WithBackground(Color color)
    {
        visualizationSettings.BackgroundColor = color;
        return this;
    }

    public ICloudGenerator WithCanvasSize(int width, int height)
    {
        layoutSettings.CanvasSize = new Size(width, height);
        return this;
    }

    public ICloudGenerator WithFontSizeRange(int min, int max)
    {
        layoutSettings.MinFontSize = min;
        layoutSettings.MaxFontSize = max;
        return this;
    }

    public ICloudGenerator WithExcludedWordsFile(string path)
    {
        wordsSettings.StopWordsFilePath = path;
        return this;
    }

    public ICloudGenerator WithFontFamily(string fontFamily)
    {
        visualizationSettings.FontName = fontFamily;
        return this;
    }

    public ICloudGenerator WithPadding(int padding)
    {
        visualizationSettings.Padding = padding;
        return this;
    }
    
    public ICloudGenerator WithLayouter(LayoutType layoutType)
    {
        layoutSettings.LayoutType = layoutType;
        return this;
    }

    public void Generate()
    {
        if (string.IsNullOrWhiteSpace(inputFile))
            throw new InvalidOperationException("Input file is not specified");

        var words = fileReader.ReadWords(inputFile).Value;
        var elements = wordProcessor.ProcessWords(words).Value;

        var fontSizeRange = new FontSizeRange(
            layoutSettings.MinFontSize,
            layoutSettings.MaxFontSize);

        var layouter = layouterFactory.Create();

        var cloud = layouter.LayoutCloud(elements, fontSizeRange).Value;

        visualizer.SaveToFile(cloud, visualizationSettings, outputFile);
    }

}
