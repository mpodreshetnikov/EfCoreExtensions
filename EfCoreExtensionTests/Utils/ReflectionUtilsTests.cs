using EfCoreExtension.Utils;
using FluentAssertions;
using Xunit;

namespace EfCoreExtensionTests.Utils
{
    public class ReflectionUtilsTests
    {
        private int XPlusY(int x, int y)
        {
            return x + y;
        }

        private int XSubstractY(int x, int y)
        {
            return x - y;
        }

        [Fact]
        public void Replace_XPlusYWithXSubstractY_MethodIsReplaced()
        {
            // Arrange
            var x = 3;
            var y = 2;
            var xPlusYMethodInfo = ReflectionUtils.GetMethodInfo(() => XPlusY(x, y));
            var xSubstractYMethodInfo = ReflectionUtils.GetMethodInfo(() => XSubstractY(x, y));

            // Action
            using var state = ReflectionUtils.Replace(xPlusYMethodInfo, xSubstractYMethodInfo);

            // Assert
            var actual = XPlusY(x, y);
            var expected = XSubstractY(x, y);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Replace_XPlusYWithXSubstractYAndDisposeState_MethodIsNotReplaced()
        {
            // Arrange
            var x = 3;
            var y = 2;
            var xPlusYMethodInfo = ReflectionUtils.GetMethodInfo(() => XPlusY(x, y));
            var xSubstractYMethodInfo = ReflectionUtils.GetMethodInfo(() => XSubstractY(x, y));

            // Action
            using var state = ReflectionUtils.Replace(xPlusYMethodInfo, xSubstractYMethodInfo);
            state.Dispose();

            // Assert
            var actual = XPlusY(x, y);
            var expected = x + y;

            actual.Should().Be(expected);
        }
    }
}
