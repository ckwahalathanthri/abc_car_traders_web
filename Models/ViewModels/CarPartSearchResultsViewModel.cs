using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for car parts search results page
    /// </summary>
    public class CarPartSearchResultsViewModel
    {
        // Search parameters
        [Display(Name = "Search")]
        public string SearchQuery { get; set; } = string.Empty;
        
        [Display(Name = "Original Query")]
        public string OriginalQuery { get; set; } = string.Empty;
        
        // Search results
        public List<CarPartViewModel> SearchResults { get; set; } = new List<CarPartViewModel>();
        
        // Pagination
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
        
        // Search filters
        public CarPartSearchFiltersViewModel Filters { get; set; } = new CarPartSearchFiltersViewModel();
        
        // Search statistics
        public int TotalResults { get; set; }
        public double SearchTime { get; set; } // in milliseconds
        public int CurrentPageResults { get; set; }
        public bool HasResults => SearchResults.Any();
        public bool NoResults => !HasResults && !string.IsNullOrEmpty(SearchQuery);
        
        // Filter options
        public List<SelectListItem> BrandOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PriceRangeOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CompatibilityOptions { get; set; } = new List<SelectListItem>();
        
        // Search suggestions
        public List<string> SearchSuggestions { get; set; } = new List<string>();
        public List<string> RelatedSearches { get; set; } = new List<string>();
        public List<string> PopularSearches { get; set; } = new List<string>();
        
        // Featured/promoted results
        public List<CarPartViewModel> FeaturedResults { get; set; } = new List<CarPartViewModel>();
        public List<CarPartViewModel> SponsoredResults { get; set; } = new List<CarPartViewModel>();
        
        // Search facets (for filtering)
        public List<SearchFacetViewModel> SearchFacets { get; set; } = new List<SearchFacetViewModel>();
        
        // Sort and view options
        public string SortBy { get; set; } = "relevance";
        public bool IsDescending { get; set; } = false;
        public string ViewMode { get; set; } = "grid"; // grid or list
        
        // Search context
        public string SearchType { get; set; } = "all"; // all, name, description, partnumber
        public bool SearchInDescription { get; set; } = true;
        public bool SearchInCompatibility { get; set; } = true;
        
        // Price analysis
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal AveragePrice { get; set; }
        
        // Availability summary
        public int InStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int LowStockCount { get; set; }
        
        // Applied filters summary
        public List<AppliedFilterViewModel> AppliedFilters { get; set; } = new List<AppliedFilterViewModel>();
        
        // SEO and meta information
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? CanonicalUrl { get; set; }
        
        // Breadcrumb navigation
        public List<BreadcrumbItem> Breadcrumbs { get; set; } = new List<BreadcrumbItem>();
        
        // Sort options for dropdown
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "relevance", Text = "Best Match" },
            new SelectListItem { Value = "name", Text = "Name (A-Z)" },
            new SelectListItem { Value = "name_desc", Text = "Name (Z-A)" },
            new SelectListItem { Value = "price", Text = "Price (Low to High)" },
            new SelectListItem { Value = "price_desc", Text = "Price (High to Low)" },
            new SelectListItem { Value = "newest", Text = "Newest First" },
            new SelectListItem { Value = "popularity", Text = "Most Popular" },
            new SelectListItem { Value = "rating", Text = "Highest Rated" },
            new SelectListItem { Value = "availability", Text = "In Stock First" }
        };
        
        // Computed properties
        public string SearchTimeDisplay => SearchTime < 1000 ? 
            $"{SearchTime:F0} ms" : 
            $"{SearchTime / 1000:F2} sec";
        
        public string ResultsCountDisplay => TotalResults switch
        {
            0 => "No results found",
            1 => "1 result found",
            _ => $"{TotalResults:N0} results found"
        };
        
        public string SearchSummary => string.IsNullOrEmpty(SearchQuery) ?
            "Browse all car parts" :
            $"Results for \"{SearchQuery}\" ({ResultsCountDisplay})";
        
        public bool HasAppliedFilters => AppliedFilters.Any();
        public int AppliedFilterCount => AppliedFilters.Count;
        
        public string PriceRangeDisplay => MinPrice > 0 && MaxPrice > 0 ? 
            $"${MinPrice:N2} - ${MaxPrice:N2}" : "Varies";
        
        public decimal InStockPercentage => TotalResults > 0 ? 
            (decimal)InStockCount / TotalResults * 100 : 0;
        
        // Initialize default values
        public CarPartSearchResultsViewModel()
        {
            SetDefaultBreadcrumbs();
            SetDefaultPriceRanges();
        }
        
        public CarPartSearchResultsViewModel(string searchQuery) : this()
        {
            SearchQuery = searchQuery;
            OriginalQuery = searchQuery;
        }
        
        private void SetDefaultBreadcrumbs()
        {
            Breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", Icon = "fas fa-home" },
                new BreadcrumbItem { Title = "Car Parts", Url = "/CarParts", Icon = "fas fa-cogs" },
                new BreadcrumbItem { Title = "Search Results", Url = null, Icon = "fas fa-search" }
            };
        }
        
        private void SetDefaultPriceRanges()
        {
            PriceRangeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Prices" },
                new SelectListItem { Value = "0-25", Text = "Under $25" },
                new SelectListItem { Value = "25-50", Text = "$25 - $50" },
                new SelectListItem { Value = "50-100", Text = "$50 - $100" },
                new SelectListItem { Value = "100-250", Text = "$100 - $250" },
                new SelectListItem { Value = "250-500", Text = "$250 - $500" },
                new SelectListItem { Value = "500-1000", Text = "$500 - $1,000" },
                new SelectListItem { Value = "1000+", Text = "$1,000+" }
            };
        }
        
        /// <summary>
        /// Gets the current sort option display text
        /// </summary>
        public string GetCurrentSortDisplay()
        {
            var currentSort = SortOptions.FirstOrDefault(s => s.Value == SortBy);
            return currentSort?.Text ?? "Best Match";
        }
        
        /// <summary>
        /// Gets search suggestions based on query
        /// </summary>
        public List<string> GetSearchSuggestions()
        {
            if (string.IsNullOrEmpty(SearchQuery) || SearchQuery.Length < 2)
                return new List<string>();
            
            // This would typically come from a search service
            return SearchSuggestions.Take(5).ToList();
        }
        
        /// <summary>
        /// Checks if a specific filter is applied
        /// </summary>
        public bool IsFilterApplied(string filterType, string filterValue)
        {
            return AppliedFilters.Any(f => f.FilterType == filterType && f.FilterValue == filterValue);
        }
        
        /// <summary>
        /// Gets URL for removing a specific filter
        /// </summary>
        public string GetRemoveFilterUrl(string filterType, string filterValue)
        {
            // This would build URL with filter removed
            return $"/CarParts/Search?q={SearchQuery}&remove={filterType}:{filterValue}";
        }
        
        /// <summary>
        /// Gets URL for clearing all filters
        /// </summary>
        public string GetClearAllFiltersUrl()
        {
            return $"/CarParts/Search?q={SearchQuery}";
        }
    }
    
    /// <summary>
    /// View model for search filters
    /// </summary>
    public class CarPartSearchFiltersViewModel
    {
        [Display(Name = "Brand")]
        public List<int> BrandIds { get; set; } = new List<int>();
        
        [Display(Name = "Category")]
        public List<int> CategoryIds { get; set; } = new List<int>();
        
        [Display(Name = "Min Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal? MinPrice { get; set; }
        
        [Display(Name = "Max Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal? MaxPrice { get; set; }
        
        [Display(Name = "Price Range")]
        public string? PriceRange { get; set; }
        
        [Display(Name = "Compatibility")]
        public string? Compatibility { get; set; }
        
        [Display(Name = "In Stock Only")]
        public bool InStockOnly { get; set; } = false;
        
        [Display(Name = "On Sale Only")]
        public bool OnSaleOnly { get; set; } = false;
        
        [Display(Name = "Free Shipping")]
        public bool FreeShippingOnly { get; set; } = false;
        
        [Display(Name = "Featured Only")]
        public bool FeaturedOnly { get; set; } = false;
        
        [Display(Name = "New Arrivals")]
        public bool NewArrivalsOnly { get; set; } = false;
        
        [Display(Name = "Minimum Rating")]
        public int? MinRating { get; set; }
        
        [Display(Name = "Warranty")]
        public string? Warranty { get; set; }
        
        [Display(Name = "Material")]
        public List<string> Materials { get; set; } = new List<string>();
        
        // Advanced filters
        [Display(Name = "Part Condition")]
        public string? PartCondition { get; set; } // New, Refurbished, Used
        
        [Display(Name = "OEM/Aftermarket")]
        public string? PartType { get; set; } // OEM, Aftermarket, Both
        
        [Display(Name = "Installation Difficulty")]
        public string? InstallationDifficulty { get; set; } // Easy, Moderate, Difficult
        
        public bool HasActiveFilters => 
            BrandIds.Any() ||
            CategoryIds.Any() ||
            MinPrice.HasValue ||
            MaxPrice.HasValue ||
            !string.IsNullOrEmpty(PriceRange) ||
            !string.IsNullOrEmpty(Compatibility) ||
            InStockOnly ||
            OnSaleOnly ||
            FreeShippingOnly ||
            FeaturedOnly ||
            NewArrivalsOnly ||
            MinRating.HasValue ||
            !string.IsNullOrEmpty(Warranty) ||
            Materials.Any() ||
            !string.IsNullOrEmpty(PartCondition) ||
            !string.IsNullOrEmpty(PartType) ||
            !string.IsNullOrEmpty(InstallationDifficulty);
        
        public void ClearAll()
        {
            BrandIds.Clear();
            CategoryIds.Clear();
            MinPrice = null;
            MaxPrice = null;
            PriceRange = null;
            Compatibility = null;
            InStockOnly = false;
            OnSaleOnly = false;
            FreeShippingOnly = false;
            FeaturedOnly = false;
            NewArrivalsOnly = false;
            MinRating = null;
            Warranty = null;
            Materials.Clear();
            PartCondition = null;
            PartType = null;
            InstallationDifficulty = null;
        }
    }
    
    /// <summary>
    /// View model for search facets (filter categories with counts)
    /// </summary>
    public class SearchFacetViewModel
    {
        public string FacetType { get; set; } = string.Empty; // brand, category, price, etc.
        public string FacetName { get; set; } = string.Empty;
        public List<SearchFacetValueViewModel> Values { get; set; } = new List<SearchFacetValueViewModel>();
        public bool IsExpanded { get; set; } = true;
        public int MaxDisplayCount { get; set; } = 10;
        
        public bool HasValues => Values.Any();
        public bool HasMoreValues => Values.Count > MaxDisplayCount;
        public List<SearchFacetValueViewModel> TopValues => Values.Take(MaxDisplayCount).ToList();
    }
    
    /// <summary>
    /// Individual facet value with count
    /// </summary>
    public class SearchFacetValueViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool IsSelected { get; set; } = false;
        public string FilterUrl { get; set; } = string.Empty;
        public string RemoveUrl { get; set; } = string.Empty;
        
        public string CountDisplay => $"({Count:N0})";
    }
    
    /// <summary>
    /// Applied filter information
    /// </summary>
    public class AppliedFilterViewModel
    {
        public string FilterType { get; set; } = string.Empty;
        public string FilterValue { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string RemoveUrl { get; set; } = string.Empty;
        public string IconClass { get; set; } = "fas fa-times";
        
        public string FilterTag => $"{FilterType}:{FilterValue}";
    }
    
    /// <summary>
    /// Search autocomplete suggestion
    /// </summary>
    public class SearchSuggestionViewModel
    {
        public string Suggestion { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // part, brand, category
        public int ResultCount { get; set; }
        public string IconClass { get; set; } = "fas fa-search";
        
        public string DisplayText => ResultCount > 0 ? 
            $"{Suggestion} ({ResultCount:N0})" : 
            Suggestion;
    }
    
    /// <summary>
    /// Search analytics and tracking
    /// </summary>
    public class SearchAnalyticsViewModel
    {
        public string SearchQuery { get; set; } = string.Empty;
        public int ResultCount { get; set; }
        public double SearchTime { get; set; }
        public DateTime SearchDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public string UserAgent { get; set; } = string.Empty;
        public string? RefererUrl { get; set; }
        public bool HasResults => ResultCount > 0;
        
        // Search performance metrics
        public List<string> FiltersApplied { get; set; } = new List<string>();
        public string SortOrder { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        
        // User interaction tracking
        public List<string> ClickedResults { get; set; } = new List<string>();
        public List<string> ViewedProducts { get; set; } = new List<string>();
        public TimeSpan TimeOnSearchPage { get; set; }
        public bool ConvertedToSale { get; set; } = false;
    }
}