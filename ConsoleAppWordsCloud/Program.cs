using WordsCloudGenerator;
using Autofac;
using ConsoleAppWordsCloud;

public class Program
{
    public static void Main()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<WordsCloudModule>();

        using var container = builder.Build();
        using var scope = container.BeginLifetimeScope();

        var generator = scope.Resolve<ICloudGenerator>();

        var client = new ConsoleCloudClient(generator);
        client.Run();
    }
}