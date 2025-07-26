using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for car search results page
    /// </summary>
    public class CarSearchResultsViewModel
    {
        // Search parameters
        [Display(Name = "Search")]
        public string SearchTerm { get; set; } = string.Empty;
        
        [Display(Name = "Original Query")]
        public string OriginalQuery { get; set; } = string.Empty;
        
        // Search results
        public List<CarViewModel> Cars { get; set; } = new List<CarViewModel>();
        
        // Pagination
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
        
        // Search filters
        public CarSearchFiltersViewModel Filters { get; set; } = new CarSearchFiltersViewModel();
        
        // Search statistics
        public int TotalResults { get; set; }
        public double SearchTime { get; set; } // in milliseconds
        public int CurrentPageResults { get; set; }
        public bool HasResults => Cars.Any();
        public bool NoResults => !HasResults && !string.IsNullOrEmpty(SearchTerm);
        
        // Filter options
        public List<SelectListItem> BrandOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PriceRangeOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> YearRangeOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> FuelTypeOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TransmissionOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ColorOptions { get; set; } = new List<SelectListItem>();
        
        // Search suggestions
        public List<string> SearchSuggestions { get; set; } = new List<string>();
        public List<string> RelatedSearches { get; set; } = new List<string>();
        public List<string> PopularSearches { get; set; } = new List<string>();
        
        // Featured/promoted results
        public List<CarViewModel> FeaturedResults { get; set; } = new List<CarViewModel>();
        public List<CarViewModel> SponsoredResults { get; set; } = new List<CarViewModel>();
        
        // Search facets (for filtering)
        public List<SearchFacet> SearchFacets { get; set; } = new List<SearchFacet>();
        
        // Applied filters
        public List<AppliedFilter> AppliedFilters { get; set; } = new List<AppliedFilter>();
        
        // Sort options
        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "relevance";
        
        [Display(Name = "Sort Order")]
        public string SortOrder { get; set; } = "asc";
        
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "relevance", Text = "Best Match" },
            new SelectListItem { Value = "price_asc", Text = "Price: Low to High" },
            new SelectListItem { Value = "price_desc", Text = "Price: High to Low" },
            new SelectListItem { Value = "year_desc", Text = "Year: Newest First" },
            new SelectListItem { Value = "year_asc", Text = "Year: Oldest First" },
            new SelectListItem { Value = "mileage_asc", Text = "Mileage: Low to High" },
            new SelectListItem { Value = "mileage_desc", Text = "Mileage: High to Low" },
            new SelectListItem { Value = "brand_asc", Text = "Brand: A to Z" },
            new SelectListItem { Value = "model_asc", Text = "Model: A to Z" }
        };
        
        // View options
        public string ViewMode { get; set; } = "grid"; // grid or list
        
        // Price statistics for the search results
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal AveragePrice { get; set; }
        
        // Year statistics
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
        
        // Brand distribution
        public Dictionary<string, int> BrandDistribution { get; set; } = new Dictionary<string, int>();
        
        // Category distribution
        public Dictionary<string, int> CategoryDistribution { get; set; } = new Dictionary<string, int>();
        
        // Recently viewed cars (for authenticated users)
        public List<CarViewModel> RecentlyViewed { get; set; } = new List<CarViewModel>();
        
        // Breadcrumb navigation - Uses the main BreadcrumbItem from BreadcrumbViewModel.cs
        public List<BreadcrumbItem> Breadcrumbs { get; set; } = new List<BreadcrumbItem>();
        
        // SEO and meta information
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        
        // Constructor
        public CarSearchResultsViewModel()
        {
            CurrentPageResults = Cars.Count;
        }
        
        /// <summary>
        /// Gets the display text for current sort option
        /// </summary>
        public string GetSortDisplayText()
        {
            var option = SortOptions.FirstOrDefault(x => x.Value == SortBy);
            return option?.Text ?? "Best Match";
        }
        
        /// <summary>
        /// Gets search suggestions based on query
        /// </summary>
        public List<string> GetSearchSuggestions()
        {
            if (string.IsNullOrEmpty(SearchTerm) || SearchTerm.Length < 2)
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
            return $"/Cars/Search?searchTerm={SearchTerm}&remove={filterType}:{filterValue}";
        }
        
        /// <summary>
        /// Gets URL for clearing all filters
        /// </summary>
        public string GetClearAllFiltersUrl()
        {
            return $"/Cars/Search?searchTerm={SearchTerm}";
        }
        
        /// <summary>
        /// Gets the search summary text
        /// </summary>
        public string GetSearchSummary()
        {
            if (TotalResults == 0)
            {
                return string.IsNullOrEmpty(SearchTerm) 
                    ? "No cars found" 
                    : $"No cars found for '{SearchTerm}'";
            }
            
            var summary = $"{TotalResults:N0} car{(TotalResults != 1 ? "s" : "")} found";
            
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                summary += $" for '{SearchTerm}'";
            }
            
            return summary;
        }
    }
    
    /// <summary>
    /// View model for car search filters
    /// </summary>
    public class CarSearchFiltersViewModel
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
        
        [Display(Name = "Min Year")]
        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
        public int? MinYear { get; set; }
        
        [Display(Name = "Max Year")]
        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
        public int? MaxYear { get; set; }
        
        [Display(Name = "Year Range")]
        public string? YearRange { get; set; }
        
        [Display(Name = "Fuel Type")]
        public List<FuelType> FuelTypes { get; set; } = new List<FuelType>();
        
        [Display(Name = "Transmission")]
        public List<Transmission> Transmissions { get; set; } = new List<Transmission>();
        
        [Display(Name = "Color")]
        public List<string> Colors { get; set; } = new List<string>();
        
        [Display(Name = "Min Mileage")]
        [Range(0, int.MaxValue, ErrorMessage = "Mileage cannot be negative")]
        public int? MinMileage { get; set; }
        
        [Display(Name = "Max Mileage")]
        [Range(0, int.MaxValue, ErrorMessage = "Mileage cannot be negative")]
        public int? MaxMileage { get; set; }
        
        [Display(Name = "Available Only")]
        public bool AvailableOnly { get; set; } = true;
        
        [Display(Name = "Featured Only")]
        public bool FeaturedOnly { get; set; } = false;
        
        [Display(Name = "Recently Added")]
        public bool RecentlyAddedOnly { get; set; } = false;
        
        [Display(Name = "Engine Capacity")]
        public string? EngineCapacity { get; set; }
        
        /// <summary>
        /// Checks if any filters are applied
        /// </summary>
        public bool HasFilters()
        {
            return BrandIds.Any() || 
                   CategoryIds.Any() || 
                   MinPrice.HasValue || 
                   MaxPrice.HasValue || 
                   MinYear.HasValue || 
                   MaxYear.HasValue || 
                   FuelTypes.Any() || 
                   Transmissions.Any() || 
                   Colors.Any() || 
                   MinMileage.HasValue || 
                   MaxMileage.HasValue || 
                   FeaturedOnly || 
                   RecentlyAddedOnly ||
                   !string.IsNullOrEmpty(EngineCapacity);
        }
        
        /// <summary>
        /// Clears all filters
        /// </summary>
        public void ClearFilters()
        {
            BrandIds.Clear();
            CategoryIds.Clear();
            MinPrice = null;
            MaxPrice = null;
            MinYear = null;
            MaxYear = null;
            FuelTypes.Clear();
            Transmissions.Clear();
            Colors.Clear();
            MinMileage = null;
            MaxMileage = null;
            AvailableOnly = true;
            FeaturedOnly = false;
            RecentlyAddedOnly = false;
            EngineCapacity = null;
        }
    }
    
    /// <summary>
    /// Represents a search facet for filtering
    /// </summary>
    public class SearchFacet
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<SearchFacetValue> Values { get; set; } = new List<SearchFacetValue>();
    }
    
    /// <summary>
    /// Represents a search facet value
    /// </summary>
    public class SearchFacetValue
    {
        public string Value { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool IsSelected { get; set; }
    }
    
    /// <summary>
    /// Represents an applied filter
    /// </summary>
    public class AppliedFilter
    {
        public string FilterType { get; set; } = string.Empty;
        public string FilterValue { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public string RemoveUrl { get; set; } = string.Empty;
    }
    
    // REMOVED: Duplicate BreadcrumbItem class definition
    // Use the main BreadcrumbItem class from BreadcrumbViewModel.cs instead
}