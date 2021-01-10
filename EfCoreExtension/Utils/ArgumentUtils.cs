using System;
using System.Collections;

namespace EfCoreExtensions.Utils
{
    internal static class ArgumentUtils
    {
        public static T DefaultIfNull<T>(T argument, T defaultValue = default)
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

        public static void AllMustBeNotNull<T>(T argument, string name)
            where T : IEnumerable
        {
            if (argument is null)
            {
                throw new ArgumentNullException(name);
            }
            foreach (var element in argument)
            {
                if (element is null)
                {
                    throw new ArgumentException($"All elements of {name} must be not null.", name);
                }
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
