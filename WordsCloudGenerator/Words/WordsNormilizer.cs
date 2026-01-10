using System.Text.RegularExpressions;
using WordsCloudGenerator.models;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator.Words;

public partial class WordNormalizer : IWordNormalizer
{
    private static readonly Regex CleaningRegex = MyRegex();

    public Result<string> Normalize(string word)
    {
        if (string.IsNullOrWhiteSpace(word) || word.Length < WordsSettings.MinWordLength)
            return Result<string>.Ok(string.Empty);

        var normalized = CleaningRegex
            .Replace(word.ToLowerInvariant(), string.Empty)
            .Trim();

        return Result<string>.Ok(normalized);
    }

    [GeneratedRegex(@"[^\p{L}\p{M}]", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
