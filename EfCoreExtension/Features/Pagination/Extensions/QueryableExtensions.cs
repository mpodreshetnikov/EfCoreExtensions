using System.Linq;
using System.Threading.Tasks;
using EfCoreExtensions.Utils;
using EfCoreExtensions.Utils.Specific;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Take a few pages of entities.
        /// </summary>
        public static IQueryable<T> TakePages<T>(this IQueryable<T> query, IPagedQuery pagedQuery)
        {
            ArgumentUtils.MustBeNotNull(pagedQuery, nameof(pagedQuery));
            return TakePages(query, pagedQuery.FromPage, pagedQuery.PageElementsCount, pagedQuery.PagesCount);
        }

        /// <summary>
        /// Take a few pages of entities.
        /// </summary>
        /// <param name="fromPage">Page to take from (starting from 1).</param>
        /// <param name="pageElementsCount">Count of elements on a page.</param>
        /// <param name="pagesCount">Count of pages to take.</param>
        public static PagedResult<T> Paged<T>(this IQueryable<T> query, int fromPage, int pageElementsCount, int pagesCount = 1) =>
            new PagedResult<T>
            {
                FromPage = fromPage,
                PagesCount = pagesCount,
                ElementsCountOnPage = pageElementsCount,
                Elements = TakePages(query, fromPage, pageElementsCount, pagesCount).ToList()
            };

        /// <summary>
        /// Take a few pages of entities.
        /// </summary>
        /// <param name="fromPage">Page to take from (starting from 1).</param>
        /// <param name="pageElementsCount">Count of elements on a page.</param>
        /// <param name="pagesCount">Count of pages to take.</param>
        public static async Task<PagedResult<T>> PagedAsync<T>(this IQueryable<T> query, int fromPage, int pageElementsCount, int pagesCount = 1) =>
            new PagedResult<T>
            {
                FromPage = fromPage,
                PagesCount = pagesCount,
                ElementsCountOnPage = pageElementsCount,
                Elements = await TakePages(query, fromPage, pageElementsCount, pagesCount).ToListAsync()
            };

        /// <summary>
        /// Take a few pages of entities.
        /// </summary>
        public static async Task<PagedResult<T>> PagedAsync<T>(this IQueryable<T> query, IPagedQuery pagedQuery) =>
            new PagedResult<T>
            {
                FromPage = pagedQuery.FromPage,
                PagesCount = pagedQuery.PagesCount,
                ElementsCountOnPage = pagedQuery.PageElementsCount,
                Elements = await TakePages(query, pagedQuery.FromPage, pagedQuery.PageElementsCount, pagedQuery.PagesCount).ToListAsync()
            };
    }
}
