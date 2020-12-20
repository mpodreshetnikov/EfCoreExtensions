using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EfCoreExtensionTests")]

namespace EfCoreExtension.Utils
{
    internal static class AtomicUtils
    {
        public static string Join(string separator, params string[] strings)
            => string.Join(separator, strings?.Where(s => !string.IsNullOrEmpty(s)));
        public static string Join(string separator, IEnumerable<string> strings)
            => Join(separator, strings?.ToArray());
    }
}
