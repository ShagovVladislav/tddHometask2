using System.Drawing;
using WordsCloudGenerator;
using Autofac;
using ConsoleAppWordsCloud;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;
using WordsCloudGenerator.visualization;
using WordsCloudGenerator.Words;
using WordsCloudGenerator.Words.WordsInterfaces;

public class Program
{
    public static void Main()
    {
        
        var builder = new ContainerBuilder();
        builder.RegisterModule<WordsCloudModule>();
        using var container = builder.Build();


        using var scope = container.BeginLifetimeScope();

        var generator = scope.Resolve<ICloudGenerator>();

        generator
            .From("C:\\Users\\Владислав\\tddHometask2\\ConsoleAppWordsCloud\\input2.txt")
            .To("C:\\Users\\Владислав\\tddHometask2\\ConsoleAppWordsCloud\\output\\cloud.png")
            .WithFontSizeRange(10, 60)
            .WithBackground(Color.Black)
            .WithColorTheme(Color.LawnGreen)
            .WithFontFamily("Arial Black")
            .WithLayouter(LayoutType.Circular)
            .WithPadding(0)
            .Generate();
    }
}