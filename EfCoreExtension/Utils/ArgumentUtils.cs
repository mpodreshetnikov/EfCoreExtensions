using System;

namespace EfCoreExtensions.Utils
{
    internal static class ArgumentUtils
    {
        public static T DefaultIfNull<T>(ref T argument, T defaultValue = default)
        {
            if (argument is null)
            {
                argument = defaultValue;
            }
            return argument;
        }

        public static void MustBeNotNull<T>(T argument, string name)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void MustBePositive(double argument, string name, string message = "Must be positive.")
        {
            if (argument <= 0)
            {
                throw new ArgumentNullException(name, message);
            }
        }

        public static void MustBePositiveOrZero(double argument, string name, string message = "Must be positive or zero.")
        {
            if (argument < 0)
            {
                throw new ArgumentNullException(name, message);
            }
        }
    }
}
