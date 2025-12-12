using Autofac;
using System.Drawing;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;
using WordsCloudGenerator.visualization;
using WordsCloudGenerator.Words;

namespace WordsCloudGenerator;

public static class ContainerFactory
{
    public static Result<IContainer> CreateDefaultContainer()
    {
        var wordsSettings = new WordsSettings();
        
        var layoutSettings = new LayoutSettings();

        var visualizationSettings = new VisualizationSettings();
        
        return new ContainerConfig().Build(wordsSettings, layoutSettings, visualizationSettings);
    }
}