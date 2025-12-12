using WordsCloudGenerator.models;

namespace WordsCloudGenerator.Words.WordsInterfaces;

public interface ILemmatizer : IDisposable
{
    Result<string> Lemmatize(string word);
}