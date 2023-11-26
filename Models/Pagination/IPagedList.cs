namespace Catalogo.Models.Pagination
{
    public interface IPagedList<T> : IEnumerable<T>
    {
        int PageNumber { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPrevious { get; }
        bool HasNext { get; }
        IEnumerable<T> Items { get; }
    }
}
