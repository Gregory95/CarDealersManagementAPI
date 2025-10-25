namespace CarsDealersManagement.Domain.Base
{
    public class PagingDto
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        //default page size
        private int _pageSize = 5;
        // if the user specifies a page size greater than my maxpagesize then i set it as the maxpagesize otherwise i set it as the value of the user
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? ReferenceNo { get; set; }
        public string? AddressTo { get; set; }
        public string? Status { get; set; }

        public string? SortBy { get; set; }
        public bool SortDesc { get; set; } = false;
    }
}