using EfCoreExtension.Utils;
using FluentAssertions;
using Xunit;

namespace EfCoreExtensionTests.Utils
{
    public class AtomicUtilsTests
    {
        [Fact]
        public void Join_JoinWithEmptyOne_AllExludeEmptyOne()
        {
            // Arrange
            var strings = new[] { "A", "", "C" };
            const string separator = ":";

            const string expected = "A:C";

            // Action
            var actual = AtomicUtils.Join(separator, strings);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Join_JoinWithEmptyOne_InitialValuesAreNotCorrupted()
        {
            // Arrange
            var strings = new[] { "A", "", "C" };
            const string separator = ":";

            var expected = new[] { "A", "", "C" };

            // Action
            _ = AtomicUtils.Join(separator, strings);

            // Assert
            strings.Should().BeEquivalentTo(expected);
        }
    }
}
