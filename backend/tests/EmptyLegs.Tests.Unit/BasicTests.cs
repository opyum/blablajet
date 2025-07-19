using FluentAssertions;
using Xunit;

namespace EmptyLegs.Tests.Unit;

public class BasicTests
{
    [Fact]
    public void Basic_Test_Should_Pass()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = true;

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(0, 0)]
    public void Basic_Theory_Test_Should_Pass(int input, int expected)
    {
        // Act & Assert
        input.Should().Be(expected);
    }
}