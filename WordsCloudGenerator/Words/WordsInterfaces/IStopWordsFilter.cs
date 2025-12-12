namespace WordsCloudGenerator.Words.WordsInterfaces;

public interface IStopWordsFilter
{
    bool IsStopWord(string word);
}