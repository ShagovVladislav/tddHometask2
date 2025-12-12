using System.Drawing;
using System.Runtime.Versioning;
using Autofac;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;
using WordsCloudGenerator.visualization;
using WordsCloudGenerator.Words;
using WordsCloudGenerator.Words.WordsInterfaces;
using VisualizationSettings = WordsCloudGenerator.visualization.VisualizationSettings;

namespace WordsCloudGenerator;
[SupportedOSPlatform("windows")]

public class ContainerConfig
{
    public Result<IContainer> Build()
    {
        return Build(
            new WordsSettings(),
            new LayoutSettings(),
            new VisualizationSettings());
    }
    public Result<IContainer> Build(
        WordsSettings wordsSettings,
        LayoutSettings layoutSettings,
        VisualizationSettings visualizationSettings)
    {
        try
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterInstance(wordsSettings).SingleInstance();
            builder.RegisterInstance(layoutSettings).SingleInstance();
            builder.RegisterInstance(visualizationSettings).SingleInstance();
            
            RegisterWordsComponents(builder, wordsSettings);
            RegisterLayoutComponents(builder, layoutSettings);
            RegisterVisualizationComponents(builder, visualizationSettings);
            
            var container = builder.Build();
            return Result<IContainer>.Ok(container);
        }
        catch (Exception ex)
        {
            return Result<IContainer>.Fail($"Failed to build DI container: {ex.Message}");
        }
    }

    
    private Result RegisterWordsComponents(ContainerBuilder builder, WordsSettings settings)
    {
        try
        {
            builder.RegisterType<TextFileReader>()
                   .As<IFileReader>()
                   .InstancePerDependency();
            
            builder.Register(c =>
                {
                    if (File.Exists(settings.StopWordsFilePath))
                    {
                        var stopWords = File.ReadAllLines(settings.StopWordsFilePath);
                        return new StopWordsFilter(stopWords);
                    }
                    else
                    {
                        var defaultStopWords = GetDefaultEnglishStopWords();
                        return new StopWordsFilter(defaultStopWords);
                    }
                })
                .As<IStopWordsFilter>()
                .SingleInstance();
            
            builder.RegisterType<WordNormalizer>()
                   .As<IWordNormalizer>()
                   .SingleInstance();
            
            // ▼▼▼ ОБЕРНУЛ В RESULT ▼▼▼
            builder.Register(c =>
                {
                    var wordsSettings = c.Resolve<WordsSettings>();
                    var lemmatizerResult = HunspellLemmatizer.Create(
                        wordsSettings.HunspellAffPath, 
                        wordsSettings.HunspellDicPath);
                    
                    return lemmatizerResult.IsSuccess 
                        ? lemmatizerResult.Value 
                        : throw new Exception($"Failed to create lemmatizer: {lemmatizerResult.Error}");
                })
                .As<ILemmatizer>()
                .SingleInstance();
            // ▲▲▲ КОНЕЦ ИСПРАВЛЕНИЯ ▲▲▲
            
            builder.RegisterType<WordProcessor>()
                   .As<IWordProcessor>()
                   .InstancePerDependency();
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to register words components: {ex.Message}");
        }
    }
    
    private Result RegisterLayoutComponents(ContainerBuilder builder, LayoutSettings settings)
    {
        try
        {
            builder.Register(c => new FontSizeRange(
                    settings.MinFontSize, 
                    settings.MaxFontSize))
                .As<FontSizeRange>()
                .SingleInstance();
            
            builder.Register(c =>
                {
                    var layoutSettings = c.Resolve<LayoutSettings>();
                    var center = new Point(
                        layoutSettings.CanvasSize.Width / 2,
                        layoutSettings.CanvasSize.Height / 2);
                    
                    return new CircularCloudLayouter(center);
                })
                .As<ICloudLayouter>()
                .InstancePerDependency();
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to register layout components: {ex.Message}");
        }
    }
    
    private Result RegisterVisualizationComponents(ContainerBuilder builder, VisualizationSettings settings)
    {
        try
        {
            builder.RegisterType<CloudVisualizer>()
                .As<ICloudVisualizer>()
                .InstancePerDependency();
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to register visualization components: {ex.Message}");
        }
    }
    private static string[] GetDefaultEnglishStopWords()
    {
        return
        [
            "a", "an", "the", "and", "or", "but", "in", "on", "at", "to",
            "for", "of", "with", "by", "as", "is", "are", "was", "were",
            "be", "been", "am", "i", "you", "he", "she", "it", "we", "they",
            "my", "your", "his", "her", "its", "our", "their", "me", "him",
            "us", "them", "this", "that", "these", "those", "what", "which",
            "who", "whom", "whose", "where", "when", "why", "how", "all",
            "any", "both", "each", "few", "more", "most", "other", "some",
            "such", "no", "nor", "not", "only", "own", "same", "so", "than",
            "too", "very", "can", "will", "just", "should", "now", "then"
        ];
    }
}