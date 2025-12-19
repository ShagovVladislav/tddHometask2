using FluentAssertions;
using WordsCloudGenerator.Words;

namespace WordsCloudGeneratorTests;

[TestFixture]
public class WordNormalizerTests
{
    [Test]
    public void Normalize_ShouldLowercaseAndRemoveNonLetters()
    {
        var normalizer = new WordNormalizer();

        var result = normalizer.Normalize("HeLLo!!!");

        result.Should().Be("hello");
    }

    [Test]
    public void Normalize_ShouldReturnEmpty_ForShortWord()
    {
        var normalizer = new WordNormalizer();
        WordsSettings.MinWordLength = 2;
        
        var result = normalizer.Normalize("a");

        result.Should().BeEmpty();
    }

    [Test]
    public void Normalize_ShouldReturnEmpty_ForNullOrWhitespace()
    {
        var normalizer = new WordNormalizer();

        var result = normalizer.Normalize("   ");

        result.Should().BeEmpty();
    }
}