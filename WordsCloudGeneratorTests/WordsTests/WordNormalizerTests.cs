using FluentAssertions;
using WordsCloudGenerator.Words;

namespace WordsCloudGeneratorTests.WordsTests;

[TestFixture]
public class WordNormalizerTests
{
    [Test]
    public void Normalize_ShouldLowercaseAndRemoveNonLetters()
    {
        var normalizer = new WordNormalizer();

        var result = normalizer.Normalize("HeLLo!!!");

        result.Value.Should().Be("hello");
    }

    [Test]
    public void Normalize_ShouldReturnEmpty_ForShortWord()
    {
        var normalizer = new WordNormalizer();
        WordsSettings.MinWordLength = 2;
        
        var result = normalizer.Normalize("a");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Test]
    public void Normalize_ShouldReturnEmpty_ForNullOrWhitespace()
    {
        var normalizer = new WordNormalizer();

        var result = normalizer.Normalize("   ");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}