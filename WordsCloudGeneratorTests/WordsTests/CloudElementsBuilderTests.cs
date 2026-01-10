using FluentAssertions;
using Moq;
using WordsCloudGenerator.models;
using WordsCloudGenerator.Words;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGeneratorTests.WordsTests;

[TestFixture]
public class CloudElementsBuilderTests
{
    [Test]
    public void ProcessWords_ShouldNormalizeFilterLemmatizeAndGroup()
    {
        var normalizer = new Mock<IWordNormalizer>();
        var stopWords = new Mock<IStopWordsFilter>();
        var lemmatizer = new Mock<ILemmatizer>();

        normalizer.Setup(n => n.Normalize(It.IsAny<string>()))
            .Returns<string>(w => Result<string>.Ok(w.ToLower()));

        stopWords.Setup(f => f.IsStopWord("and")).Returns(true);
        stopWords.Setup(f => f.IsStopWord("cloud")).Returns(false);

        lemmatizer.Setup(l => l.Lemmatize(It.IsAny<string>()))
            .Returns<string>(w => Result<string>.Ok(w));

        var processor = new CloudElementsBuilder(
            normalizer.Object,
            stopWords.Object,
            lemmatizer.Object);

        var words = new[] { "Cloud", "cloud", "and" };

        var result = processor.ProcessWords(words);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);

        var element = result.Value[0];
        element.Word.Should().Be("cloud");
        element.Frequency.Should().Be(2);
    }


    [Test]
    public void ProcessWords_ShouldFail_ForEmptyInput()
    {
        var processor = new CloudElementsBuilder(
            Mock.Of<IWordNormalizer>(),
            Mock.Of<IStopWordsFilter>(),
            Mock.Of<ILemmatizer>());

        var result = processor.ProcessWords([]);

        result.IsSuccess.Should().BeFalse();
    }
}