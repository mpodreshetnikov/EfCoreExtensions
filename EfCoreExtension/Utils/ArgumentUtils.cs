using System;

namespace EfCoreExtension.Utils
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

        public static void ThrowIfNull<T>(T argument, string name)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
