using WordsCloudGenerator;

public class Program
{
    public static void Main()
    {
        var inputFile = "C:\\Users\\Владислав\\tddHometask2\\ConsoleAppWordsCloud\\input.txt";
        var outputDir = "C:\\Users\\Владислав\\tddHometask2\\ConsoleAppWordsCloud\\output\\cloud.png";
        SimpleCloudGenerator.GenerateWordsCloud(inputFile, outputDir
            );
    }
}