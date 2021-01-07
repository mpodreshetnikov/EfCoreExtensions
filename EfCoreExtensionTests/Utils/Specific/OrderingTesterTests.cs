using System.Linq;
using EfCoreExtensions.Utils.Specific;
using FluentAssertions;
using Xunit;

namespace EfCoreExtensionTests.Utils.Specific
{
    public class OrderingTesterTests
    {
        [Fact]
        public void IsQueryOrdered_OrderedAtEnd_True()
        {
            // Arrange
            var query = Enumerable.Empty<int>().AsQueryable().OrderBy(x => x);

            // Action
            var actual = OrderingTester.IsQueryOrdered(query);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void IsQueryOrdered_OrderedNotAtEnd_True()
        {
            // Arrange
            var query = Enumerable.Empty<int>().AsQueryable().OrderBy(x => x).Select(x => x + 1).Where(x => x > 0);

            // Action
            var actual = OrderingTester.IsQueryOrdered(query);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void IsQueryOrdered_NotOrdered_False()
        {
            // Arrange
            var query = Enumerable.Empty<int>().AsQueryable().Select(x => x + 1);

            // Action
            var actual = OrderingTester.IsQueryOrdered(query);

            // Assert
            actual.Should().BeFalse();
        }
    }
}
