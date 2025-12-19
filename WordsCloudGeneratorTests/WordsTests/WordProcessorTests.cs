using FluentAssertions;
using Moq;
using WordsCloudGenerator.models;
using WordsCloudGenerator.Words;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGeneratorTests;

[TestFixture]
public class WordProcessorTests
{
    [Test]
    public void ProcessWords_ShouldNormalizeFilterLemmatizeAndGroup()
    {
        var normalizer = new Mock<IWordNormalizer>();
        var stopWords = new Mock<IStopWordsFilter>();
        var lemmatizer = new Mock<ILemmatizer>();

        normalizer.Setup(n => n.Normalize(It.IsAny<string>()))
            .Returns<string>(w => w.ToLower());

        stopWords.Setup(f => f.IsStopWord("and")).Returns(true);
        stopWords.Setup(f => f.IsStopWord(It.Is<string>(w => w != "and"))).Returns(false);

        lemmatizer.Setup(l => l.Lemmatize(It.IsAny<string>()))
            .Returns<string>(w => Result<string>.Ok(w));

        var processor = new WordProcessor(
            normalizer.Object,
            stopWords.Object,
            lemmatizer.Object);

        var words = new[] { "Cloud", "cloud", "and" };

        var result = processor.ProcessWords(words);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Word.Should().Be("cloud");
        result.Value[0].Frequency.Should().Be(2);
    }

    [Test]
    public void ProcessWords_ShouldFail_ForEmptyInput()
    {
        var processor = new WordProcessor(
            Mock.Of<IWordNormalizer>(),
            Mock.Of<IStopWordsFilter>(),
            Mock.Of<ILemmatizer>());

        var result = processor.ProcessWords([]);

        result.IsSuccess.Should().BeFalse();
    }
}