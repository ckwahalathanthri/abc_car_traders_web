using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for pagination controls
    /// </summary>
    public class PaginationViewModel
    {
        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int CurrentPage { get; set; } = 1;
        
        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }
        
        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalItems { get; set; }
        
        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 25;
        
        /// <summary>
        /// Controller name for generating pagination links
        /// </summary>
        public string? Controller { get; set; }
        
        /// <summary>
        /// Action name for generating pagination links
        /// </summary>
        public string? Action { get; set; }
        
        /// <summary>
        /// Additional route values to include in pagination links
        /// </summary>
        public Dictionary<string, object> RouteValues { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Whether to show page size selector dropdown
        /// </summary>
        public bool ShowPageSizeSelector { get; set; } = true;
        
        /// <summary>
        /// Whether to show First/Last page buttons
        /// </summary>
        public bool ShowFirstLast { get; set; } = true;
        
        /// <summary>
        /// Whether to show Previous/Next text labels
        /// </summary>
        public bool ShowPreviousNext { get; set; } = true;
        
        /// <summary>
        /// Whether to show jump to page input for large datasets
        /// </summary>
        public bool ShowJumpToPage { get; set; } = false;
        
        /// <summary>
        /// CSS class for the pagination container
        /// </summary>
        public string CssClass { get; set; } = "";
        
        /// <summary>
        /// Size of the pagination controls (sm, md, lg)
        /// </summary>
        public string Size { get; set; } = "md";
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public PaginationViewModel()
        {
        }
        
        /// <summary>
        /// Constructor with basic pagination info
        /// </summary>
        /// <param name="currentPage">Current page number</param>
        /// <param name="totalItems">Total number of items</param>
        /// <param name="pageSize">Items per page</param>
        public PaginationViewModel(int currentPage, int totalItems, int pageSize = 25)
        {
            CurrentPage = Math.Max(1, currentPage);
            TotalItems = Math.Max(0, totalItems);
            PageSize = Math.Max(1, pageSize);
            TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
            
            // Ensure current page is within valid range
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }
        }
        
        /// <summary>
        /// Constructor with full pagination info
        /// </summary>
        /// <param name="currentPage">Current page number</param>
        /// <param name="totalItems">Total number of items</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="controller">Controller name</param>
        /// <param name="action">Action name</param>
        /// <param name="routeValues">Additional route values</param>
        public PaginationViewModel(int currentPage, int totalItems, int pageSize, string controller, string action, Dictionary<string, object>? routeValues = null)
            : this(currentPage, totalItems, pageSize)
        {
            Controller = controller;
            Action = action;
            RouteValues = routeValues ?? new Dictionary<string, object>();
        }
        
        /// <summary>
        /// Checks if there is a previous page
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;
        
        /// <summary>
        /// Checks if there is a next page
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;
        
        /// <summary>
        /// Gets the previous page number (or current if no previous)
        /// </summary>
        public int PreviousPageNumber => HasPreviousPage ? CurrentPage - 1 : CurrentPage;
        
        /// <summary>
        /// Gets the next page number (or current if no next)
        /// </summary>
        public int NextPageNumber => HasNextPage ? CurrentPage + 1 : CurrentPage;
        
        /// <summary>
        /// Gets the starting item number for the current page
        /// </summary>
        public int StartItemNumber => (CurrentPage - 1) * PageSize + 1;
        
        /// <summary>
        /// Gets the ending item number for the current page
        /// </summary>
        public int EndItemNumber => Math.Min(CurrentPage * PageSize, TotalItems);
        
        /// <summary>
        /// Checks if pagination should be displayed (more than one page)
        /// </summary>
        public bool ShouldShowPagination => TotalPages > 1;
        
        /// <summary>
        /// Gets available page size options
        /// </summary>
        public List<int> PageSizeOptions { get; set; } = new List<int> { 10, 25, 50, 100 };
        
        /// <summary>
        /// Gets the range of pages to display in pagination
        /// </summary>
        /// <param name="maxPagesToShow">Maximum number of page links to show</param>
        /// <returns>Tuple with start and end page numbers</returns>
        public (int StartPage, int EndPage) GetPageRange(int maxPagesToShow = 5)
        {
            var startPage = Math.Max(1, CurrentPage - maxPagesToShow / 2);
            var endPage = Math.Min(TotalPages, startPage + maxPagesToShow - 1);
            
            // Adjust start page if we're near the end
            if (endPage - startPage < maxPagesToShow - 1)
            {
                startPage = Math.Max(1, endPage - maxPagesToShow + 1);
            }
            
            return (startPage, endPage);
        }
        
        /// <summary>
        /// Creates pagination info text
        /// </summary>
        /// <returns>Formatted pagination info string</returns>
        public string GetPaginationInfo()
        {
            if (TotalItems == 0)
                return "No results found";
                
            var startItem = StartItemNumber;
            var endItem = EndItemNumber;
            var itemText = TotalItems == 1 ? "result" : "results";
            
            return $"Showing {startItem} to {endItem} of {TotalItems} {itemText}";
        }
        
        /// <summary>
        /// Validates pagination parameters and adjusts if necessary
        /// </summary>
        public void ValidateAndAdjust()
        {
            // Ensure positive values
            CurrentPage = Math.Max(1, CurrentPage);
            TotalItems = Math.Max(0, TotalItems);
            PageSize = Math.Max(1, PageSize);
            
            // Recalculate total pages
            TotalPages = TotalItems > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;
            
            // Adjust current page if it exceeds total pages
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }
            
            // Set ShowJumpToPage automatically for large datasets
            if (TotalPages > 10)
            {
                ShowJumpToPage = true;
            }
        }
        
        /// <summary>
        /// Creates a new PaginationViewModel for a different page
        /// </summary>
        /// <param name="newPage">The new page number</param>
        /// <returns>New PaginationViewModel instance</returns>
        public PaginationViewModel ForPage(int newPage)
        {
            return new PaginationViewModel
            {
                CurrentPage = Math.Max(1, Math.Min(newPage, TotalPages)),
                TotalPages = TotalPages,
                TotalItems = TotalItems,
                PageSize = PageSize,
                Controller = Controller,
                Action = Action,
                RouteValues = new Dictionary<string, object>(RouteValues),
                ShowPageSizeSelector = ShowPageSizeSelector,
                ShowFirstLast = ShowFirstLast,
                ShowPreviousNext = ShowPreviousNext,
                ShowJumpToPage = ShowJumpToPage,
                CssClass = CssClass,
                Size = Size,
                PageSizeOptions = PageSizeOptions
            };
        }
        
        /// <summary>
        /// Creates a new PaginationViewModel with a different page size
        /// </summary>
        /// <param name="newPageSize">The new page size</param>
        /// <returns>New PaginationViewModel instance</returns>
        public PaginationViewModel WithPageSize(int newPageSize)
        {
            var newTotalPages = (int)Math.Ceiling((double)TotalItems / newPageSize);
            var newCurrentPage = Math.Min(CurrentPage, newTotalPages);
            
            return new PaginationViewModel
            {
                CurrentPage = Math.Max(1, newCurrentPage),
                TotalPages = newTotalPages,
                TotalItems = TotalItems,
                PageSize = newPageSize,
                Controller = Controller,
                Action = Action,
                RouteValues = new Dictionary<string, object>(RouteValues),
                ShowPageSizeSelector = ShowPageSizeSelector,
                ShowFirstLast = ShowFirstLast,
                ShowPreviousNext = ShowPreviousNext,
                ShowJumpToPage = ShowJumpToPage,
                CssClass = CssClass,
                Size = Size,
                PageSizeOptions = PageSizeOptions
            };
        }
        
        /// <summary>
        /// Adds route values to the pagination links
        /// </summary>
        /// <param name="key">Route parameter key</param>
        /// <param name="value">Route parameter value</param>
        public void AddRouteValue(string key, object value)
        {
            RouteValues[key] = value;
        }
        
        /// <summary>
        /// Removes a route value from pagination links
        /// </summary>
        /// <param name="key">Route parameter key to remove</param>
        public void RemoveRouteValue(string key)
        {
            RouteValues.Remove(key);
        }
        
        /// <summary>
        /// Gets CSS classes for pagination size
        /// </summary>
        /// <returns>CSS class string</returns>
        public string GetSizeCssClass()
        {
            return Size.ToLower() switch
            {
                "sm" => "pagination-sm",
                "lg" => "pagination-lg",
                _ => ""
            };
        }
    }
    
    /// <summary>
    /// Generic pagination result that includes data and pagination info
    /// </summary>
    /// <typeparam name="T">Type of items being paginated</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// The paginated data items
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();
        
        /// <summary>
        /// Pagination information
        /// </summary>
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public PagedResult()
        {
        }
        
        /// <summary>
        /// Constructor with items and pagination info
        /// </summary>
        /// <param name="items">The paginated items</param>
        /// <param name="currentPage">Current page number</param>
        /// <param name="totalItems">Total number of items</param>
        /// <param name="pageSize">Items per page</param>
        public PagedResult(List<T> items, int currentPage, int totalItems, int pageSize = 25)
        {
            Items = items ?? new List<T>();
            Pagination = new PaginationViewModel(currentPage, totalItems, pageSize);
        }
        
        /// <summary>
        /// Creates an empty paged result
        /// </summary>
        /// <returns>Empty PagedResult</returns>
        public static PagedResult<T> Empty()
        {
            return new PagedResult<T>(new List<T>(), 1, 0, 25);
        }
    }
}