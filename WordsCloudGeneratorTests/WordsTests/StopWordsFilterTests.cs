using FluentAssertions;
using WordsCloudGenerator.Words;

namespace WordsCloudGeneratorTests;

[TestFixture]
public class StopWordsFilterTests
{
    [Test]
    public void IsStopWord_ShouldReturnTrue_ForConfiguredWord()
    {
        var filter = new StopWordsFilter(new[] { "and", "the" });

        var result = filter.IsStopWord("The");

        result.Should().BeTrue();
    }

    [Test]
    public void IsStopWord_ShouldReturnFalse_ForNonStopWord()
    {
        var filter = new StopWordsFilter(new[] { "and" });

        var result = filter.IsStopWord("cloud");

        result.Should().BeFalse();
    }
}