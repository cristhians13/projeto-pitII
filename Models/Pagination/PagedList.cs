namespace Catalogo.Models.Pagination
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        public IEnumerable<T> Items => this;

        public PagedList(IEnumerable<T> source, int pageNumber, int pageSize, int totalCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;

            // Aplicar lógica de paginação para garantir que apenas os itens da página atual sejam incluídos
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            AddRange(items);
        }
    }

}
