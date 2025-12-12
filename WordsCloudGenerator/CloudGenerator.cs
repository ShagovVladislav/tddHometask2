using Autofac;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;
using WordsCloudGenerator.visualization;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator;

public static class SimpleCloudGenerator
{
    public static void GenerateWordsCloud(string inputFile, string outputFile = "cloud.png")
    {
        Console.WriteLine($"Создаем облако слов из файла: {inputFile}");
        
        var containerResult = ContainerFactory.CreateDefaultContainer();
        using var container = containerResult.Value;
        
        var fileReader = container.Resolve<IFileReader>();
        var wordProcessor = container.Resolve<IWordProcessor>();
        var layouter = container.Resolve<ICloudLayouter>();
        var visualizer = container.Resolve<ICloudVisualizer>();
        var visSettings = container.Resolve<VisualizationSettings>();
        var layoutSettings = container.Resolve<LayoutSettings>();
        
        var words = fileReader.ReadWords(inputFile).Value;
        var elements = wordProcessor.ProcessWords(words).Value;
        
        var fontSizeRange = new FontSizeRange(
            layoutSettings.MinFontSize, 
            layoutSettings.MaxFontSize
        );
        
        var cloud = layouter.LayoutCloud(elements, fontSizeRange).Value;
        
        visualizer.SaveToFile(cloud, visSettings, outputFile);
        
        Console.WriteLine($"Облако слов сохранено в: {outputFile}");
    }
}