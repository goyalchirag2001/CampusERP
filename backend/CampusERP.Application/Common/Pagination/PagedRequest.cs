using CampusERP.Application.Common.Constants;

namespace CampusERP.Application.Common.Pagination;

public class PagedRequest
{
    private int _pageNumber = PaginationConstants.DefaultPageNumber;

    private int _pageSize = PaginationConstants.DefaultPageSize;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1
            ? PaginationConstants.DefaultPageNumber
            : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value <= 0)
            {
                _pageSize = PaginationConstants.DefaultPageSize;
            }
            else
            {
                _pageSize = Math.Min(value, PaginationConstants.MaxPageSize);
            }
        }
    }

    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; }
}