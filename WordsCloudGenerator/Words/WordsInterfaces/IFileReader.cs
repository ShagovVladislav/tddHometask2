using WordsCloudGenerator.models;

namespace WordsCloudGenerator.Words.WordsInterfaces;

public interface IFileReader
{
    Result<string[]> ReadWords(string path);
}