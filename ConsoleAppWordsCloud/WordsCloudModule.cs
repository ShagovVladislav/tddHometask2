using System.Drawing;
using Autofac;
using WordsCloudGenerator;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;
using WordsCloudGenerator.visualization;
using WordsCloudGenerator.Words;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace ConsoleAppWordsCloud;

public class WordsCloudModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(new WordsSettings())
            .SingleInstance();

        builder.RegisterInstance(new LayoutSettings())
            .SingleInstance();

        builder.RegisterInstance(new VisualizationSettings())
            .SingleInstance();

        builder.RegisterType<CloudGenerator>()
            .As<ICloudGenerator>();

        
        builder.RegisterType<TextFileReader>()
            .As<IFileReader>();

        builder.Register(c =>
            {
                var settings = c.Resolve<WordsSettings>();
                if (File.Exists(settings.StopWordsFilePath))
                    return new StopWordsFilter(File.ReadAllLines(settings.StopWordsFilePath));

                return new StopWordsFilter(GetDefaultEnglishStopWords());
            })
            .As<IStopWordsFilter>()
            .SingleInstance();

        builder.RegisterType<WordNormalizer>()
            .As<IWordNormalizer>()
            .SingleInstance();

        builder.Register(c =>
            {
                var settings = c.Resolve<WordsSettings>();
                var result = HunspellLemmatizer.Create(
                    settings.HunspellAffPath,
                    settings.HunspellDicPath);

                if (!result.IsSuccess)
                    throw new InvalidOperationException(result.Error);

                return result.Value;
            })
            .As<ILemmatizer>()
            .SingleInstance();

        builder.RegisterType<WordProcessor>()
            .As<IWordProcessor>();

        builder.RegisterType<CloudLayouterFactory>()
            .As<ICloudLayouterFactory>()
            .SingleInstance();

        builder.RegisterType<CloudVisualizer>()
            .As<ICloudVisualizer>();

        builder.RegisterType<CloudGenerator>()
            .As<ICloudGenerator>();
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
