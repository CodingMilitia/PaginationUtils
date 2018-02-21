namespace CodingMilitia.Utils.Pagination
{
    public struct PaginationParameter
    {
        public int PageNumber { get; private set; }
        public int ItemsPerPage { get; private set; }

        public PaginationParameter(int page, int itemsPerPage)
            : this()
        {
            PageNumber = page;
            ItemsPerPage = itemsPerPage;
        }
    }
}
