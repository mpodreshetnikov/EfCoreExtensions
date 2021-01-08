using System.Linq;
using EfCoreExtensions.Utils;
using EfCoreExtensions.Utils.Specific;

namespace EfCoreExtensions.Pagination
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Take page of entities.
        /// </summary>
        /// <param name="page">Page starting from 1.</param>
        /// <param name="pageElementsCount">Count of elements on a page.</param>
        public static IQueryable<T> TakePage<T>(this IQueryable<T> query, int page, int pageElementsCount)
        {
            return TakePages(query, page, pageElementsCount);
        }

        /// <summary>
        /// Take a few pages of entities.
        /// </summary>
        /// <param name="fromPage">Page to take from (starting from 1).</param>
        /// <param name="pageElementsCount">Count of elements on a page.</param>
        /// <param name="pagesCount">Count of pages to take.</param>
        public static IQueryable<T> TakePages<T>(this IQueryable<T> query, int fromPage, int pageElementsCount, int pagesCount = 1)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));
            ArgumentUtils.MustBePositive(fromPage, nameof(fromPage));
            ArgumentUtils.MustBePositiveOrZero(pageElementsCount, nameof(pageElementsCount));
            ArgumentUtils.MustBePositiveOrZero(pagesCount, nameof(pagesCount));

            if (!OrderingReflectionUtils.IsQueryOrdered(query))
            {
                query = OrderingReflectionUtils.OrderByAnyPropertyOrItself(query);
            }

            return query.Skip(--fromPage * pageElementsCount).Take(pageElementsCount * pagesCount);
        }
    }
}
