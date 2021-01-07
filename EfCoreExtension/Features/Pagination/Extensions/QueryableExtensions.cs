using System;
using System.Linq;
using System.Linq.Expressions;
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
            return TakeSomePages(query, page, pageElementsCount);
        }

        /// <summary>
        /// Take page of entities.
        /// </summary>
        /// <param name="page">Page starting from 1.</param>
        /// <param name="pageElementsCount">Count of elements on a page.</param>
        /// <param name="pagesCount">Count of pages to take.</param>
        public static IQueryable<T> TakeSomePages<T>(this IQueryable<T> query, int page, int pageElementsCount, int pagesCount = 1)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));
            ArgumentUtils.MustBePositive(page, nameof(page));
            ArgumentUtils.MustBePositiveOrZero(pageElementsCount, nameof(pageElementsCount));
            ArgumentUtils.MustBePositiveOrZero(pagesCount, nameof(pagesCount));

            if (!OrderingTester.IsQueryOrdered(query))
            {
                // order query.
            }

            return query.Skip(--page * pageElementsCount).Take(pageElementsCount * pagesCount);
        }
    }
}
