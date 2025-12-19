using System.Diagnostics;
using WordsCloudGenerator;
using Autofac;
using ConsoleAppWordsCloud;

public class Program
{
    static void Main()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<WordsCloudModule>();
        using var container = builder.Build();

        var ui = new ConsoleSettingsUI();
        var settings = ui.Run();

        if (settings == null)
            return;

        var generator = container.Resolve<ICloudGenerator>();
        var client = new CloudGenerationClient(generator);

        var result = client.Generate(settings);

        if (!result.IsSuccess)
        {
            Console.WriteLine($"Ошибка: {result.Error}");
            Console.ReadKey();
            return;
        }

        OpenFile(settings.OutputFile);
    }
    private static void OpenDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory))
            directory = Directory.GetCurrentDirectory();

        Process.Start(new ProcessStartInfo
        {
            FileName = directory,
            UseShellExecute = true
        });
    }

    private static void OpenFile(string path)
    {

        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }
}