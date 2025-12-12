using System.Text.RegularExpressions;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator.Words;

public partial class WordNormalizer : IWordNormalizer
{
    private static readonly Regex CleaningRegex = MyRegex();
    
    public string Normalize(string word)
    {
        return string.IsNullOrWhiteSpace(word) 
            ? string.Empty 
            : CleaningRegex.Replace(word.ToLowerInvariant(), "").Trim();
    }

    [GeneratedRegex(@"[^\p{L}\p{M}]", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}