namespace EfCoreExtensions.Pagination
{
    public class PagedQuery : IPagedQuery
    {
        public int FromPage { get; set; }

        public int PagesCount { get; set; } = 1;

        public int PageElementsCount { get; set; }
    }
}
