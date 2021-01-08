using System;
using System.Linq;
using EfCoreExtensions.Utils.Specific;
using FluentAssertions;
using Xunit;

namespace EfCoreExtensionTests.Utils.Specific
{
    public class OrderingReflectionUtilsTests
    {
        [Fact]
        public void IsQueryOrdered_OrderedAtEnd_True()
        {
            // Arrange
            var query = Enumerable.Empty<int>().AsQueryable().OrderBy(x => x);

            // Action
            var actual = OrderingReflectionUtils.IsQueryOrdered(query);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void IsQueryOrdered_OrderedNotAtEnd_True()
        {
            // Arrange
            var query = Enumerable.Empty<int>().AsQueryable().OrderBy(x => x).Select(x => x + 1).Where(x => x > 0);

            // Action
            var actual = OrderingReflectionUtils.IsQueryOrdered(query);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void IsQueryOrdered_NotOrdered_False()
        {
            // Arrange
            var query = Enumerable.Empty<int>().AsQueryable().Select(x => x + 1);

            // Action
            var actual = OrderingReflectionUtils.IsQueryOrdered(query);

            // Assert
            actual.Should().BeFalse();
        }

        [Fact]
        public void OrderByAnyPropertyOrItself_NotOrderedQueryOfClasses_OrderedQuery()
        {
            // Arrange
            var query = Enumerable.Empty<Exception>().AsQueryable();

            // Action
            var before = OrderingReflectionUtils.IsQueryOrdered(query);
            query = OrderingReflectionUtils.OrderByAnyPropertyOrItself(query);
            var after = OrderingReflectionUtils.IsQueryOrdered(query);

            // Assert
            before.Should().BeFalse();
            after.Should().BeTrue();
        }

        [Fact]
        public void OrderByAnyPropertyOrItself_NotOrderedQueryOfPrimitives_OrderedQuery()
        {
            // Arrange
            var random = new Random();
            var query = Enumerable.Range(1, 10).Select(_ => random.Next()).AsQueryable();

            // Action
            var before = OrderingReflectionUtils.IsQueryOrdered(query);
            query = OrderingReflectionUtils.OrderByAnyPropertyOrItself(query);
            var after = OrderingReflectionUtils.IsQueryOrdered(query);
            var items = query.ToList();

            // Assert
            before.Should().BeFalse();
            after.Should().BeTrue();
            items.Should().BeInAscendingOrder();
        }
    }
}
