using System.Collections.Generic;
using System.Linq;

namespace EfCoreExtensions.Pagination
{
    public class PagedResult<T>
    {
        public int FromPage { get; internal set; }

        public int ElementsCountOnPage { get; internal set; }

        public int PagesCount { get; internal set; }

        public int TotalCount => PagesCount * ElementsCountOnPage;

        public int Offset => (FromPage - 1) * ElementsCountOnPage;

        private IEnumerable<IEnumerable<T>> pages;
        public IEnumerable<IEnumerable<T>> Pages => pages ??= Enumerable.Range(0, PagesCount)
            .Select(page => Elements.Skip(page * ElementsCountOnPage).Take(ElementsCountOnPage).ToList()).ToList();

        public IEnumerable<T> Elements { get; internal set; }
    }
}
