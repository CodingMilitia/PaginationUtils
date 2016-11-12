namespace CodingMilitia.Utils.Pagination
{
    public struct PaginationParameter
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }

        public PaginationParameter(int page, int itemsPerPage)
            : this()
        {
            PageNumber = page;
            ItemsPerPage = itemsPerPage;
        }
    }
}
