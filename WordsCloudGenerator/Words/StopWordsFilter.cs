using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator.Words;

public class SimpleStopWordsFilter : IStopWordsFilter
{
    private readonly HashSet<string> _stopWords;
    
    public SimpleStopWordsFilter(IEnumerable<string> stopWords)
    {
        _stopWords = new HashSet<string>(
            stopWords?.Select(w => w.Trim().ToLowerInvariant()) 
            ?? [],
            StringComparer.OrdinalIgnoreCase);
    }

    public bool IsStopWord(string word)
    {
        return string.IsNullOrWhiteSpace(word) || _stopWords.Contains(word.Trim());
    }
}