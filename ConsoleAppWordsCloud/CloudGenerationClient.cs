using WordsCloudGenerator;
using WordsCloudGenerator.models;

namespace ConsoleAppWordsCloud;

public sealed class CloudGenerationClient
{
    private readonly ICloudGenerator generator;

    public CloudGenerationClient(ICloudGenerator generator)
    {
        this.generator = generator;
    }

    public Result Generate(ConsoleCloudSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.InputFile))
            return Result.Fail("Input file is empty");

        var g = generator
            .From(settings.InputFile)
            .To(settings.OutputFile)
            .WithLayouter(settings.Layout)
            .WithFontSizeRange(settings.MinFontSize, settings.MaxFontSize)
            .WithBackground(settings.Background)
            .WithColorTheme(settings.Colors.ToArray())
            .WithFontFamily(settings.FontFamily)
            .WithPadding(settings.Padding);

        if (settings.ExcludedWordsFile != null)
            g.WithExcludedWordsFile(settings.ExcludedWordsFile);

        if (settings.CanvasSize.HasValue)
            g.WithCanvasSize(
                settings.CanvasSize.Value.Width,
                settings.CanvasSize.Value.Height);

        g.Generate();
        return Result.Ok();
    }
}
