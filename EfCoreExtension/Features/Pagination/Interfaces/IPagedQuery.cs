namespace EfCoreExtensions.Pagination
{
    public interface IPagedQuery
    {
        int FromPage { get; }

        int PagesCount { get; }

        int PageElementsCount { get; }
    }
}
