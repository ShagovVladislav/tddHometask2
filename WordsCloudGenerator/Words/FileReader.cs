using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator.Words;

public class FileReader : IFileReader
{
    public string[] Read(string path)
    {
        try
        {
            return File.ReadAllLines(path);
        }
        catch (Exception e)
        {
            throw new FileNotFoundException("File not found", path);
        }
    }
}