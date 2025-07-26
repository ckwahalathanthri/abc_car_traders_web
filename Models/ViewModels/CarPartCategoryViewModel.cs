using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for car parts category page
    /// </summary>
    public class CarPartCategoryViewModel
    {
        // Category information
        public int CategoryId { get; set; }
        
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Display(Name = "Category Type")]
        public CategoryType CategoryType { get; set; }
        
        // Parts in this category
        public List<CarPartViewModel> CarParts { get; set; } = new List<CarPartViewModel>();
        
        // Pagination
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
        
        // Search and filtering
        public CarPartCategorySearchViewModel SearchModel { get; set; } = new CarPartCategorySearchViewModel();
        
        // Filter options
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PriceRanges { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CompatibilityOptions { get; set; } = new List<SelectListItem>();
        
        // Category statistics
        public int TotalPartsInCategory { get; set; }
        public int AvailablePartsInCategory { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal AveragePrice { get; set; }
        
        // Related categories
        public List<CategorySummaryViewModel> RelatedCategories { get; set; } = new List<CategorySummaryViewModel>();
        
        // Popular brands in this category
        public List<BrandSummaryViewModel> PopularBrands { get; set; } = new List<BrandSummaryViewModel>();
        
        // Featured parts
        public List<CarPartViewModel> FeaturedParts { get; set; } = new List<CarPartViewModel>();
        
        // Breadcrumb navigation
        public List<BreadcrumbItem> Breadcrumbs { get; set; } = new List<BreadcrumbItem>();
        
        // SEO and meta information
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        
        // View preferences
        public string ViewMode { get; set; } = "grid"; // grid or list
        public string SortBy { get; set; } = "name";
        public bool IsDescending { get; set; } = false;
        
        // Computed properties
        public string CategoryTitle => !string.IsNullOrEmpty(CategoryName) ? $"{CategoryName} Car Parts" : "Car Parts";
        public string PriceRangeDisplay => MinPrice > 0 && MaxPrice > 0 ? $"${MinPrice:N0} - ${MaxPrice:N0}" : "Varies";
        public string AveragePriceDisplay => AveragePrice > 0 ? $"${AveragePrice:N2}" : "N/A";
        public bool HasParts => CarParts.Any();
        public int OutOfStockCount => TotalPartsInCategory - AvailablePartsInCategory;
        
        // Category image (if available)
        public string? CategoryImageUrl { get; set; }
        public string? CategoryBannerUrl { get; set; }
        
        // Category-specific messaging
        public string CategoryMessage { get; set; } = string.Empty;
        public bool ShowCategoryMessage => !string.IsNullOrEmpty(CategoryMessage);
        
        // Filters applied indicator
        public bool HasActiveFilters => SearchModel.HasActiveFilters;
        public int ActiveFilterCount => SearchModel.GetActiveFilterCount();
        
        // Sort options for dropdown
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "name", Text = "Name (A-Z)" },
            new SelectListItem { Value = "name_desc", Text = "Name (Z-A)" },
            new SelectListItem { Value = "price", Text = "Price (Low to High)" },
            new SelectListItem { Value = "price_desc", Text = "Price (High to Low)" },
            new SelectListItem { Value = "newest", Text = "Newest First" },
            new SelectListItem { Value = "popularity", Text = "Most Popular" },
            new SelectListItem { Value = "rating", Text = "Highest Rated" }
        };
        
        // Initialize default values
        public CarPartCategoryViewModel()
        {
            SetDefaultBreadcrumbs();
            SetDefaultPriceRanges();
        }
        
        public CarPartCategoryViewModel(int categoryId, string categoryName) : this()
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            SearchModel.CategoryId = categoryId;
        }
        
        private void SetDefaultBreadcrumbs()
        {
            Breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/", Icon = "fas fa-home" },
                new BreadcrumbItem { Title = "Car Parts", Url = "/CarParts", Icon = "fas fa-cogs" }
            };
        }
        
        private void SetDefaultPriceRanges()
        {
            PriceRanges = new List<SelectListItem>
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
            return currentSort?.Text ?? "Name (A-Z)";
        }
        
        /// <summary>
        /// Gets category-specific recommendations message
        /// </summary>
        public string GetCategoryRecommendation()
        {
            return CategoryName.ToLower() switch
            {
                "engine parts" => "Keep your engine running smoothly with genuine parts",
                "brake system" => "Ensure your safety with quality brake components",
                "electrical" => "Reliable electrical parts for all your automotive needs",
                "filters" => "Maintain your vehicle's performance with quality filters",
                "body parts" => "Restore your vehicle's appearance with OEM body parts",
                "suspension" => "Smooth ride guaranteed with premium suspension parts",
                _ => "Quality automotive parts for your vehicle"
            };
        }
    }
    
    /// <summary>
    /// Search and filter model for category page
    /// </summary>
    public class CarPartCategorySearchViewModel
    {
        public int CategoryId { get; set; }
        
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }
        
        [Display(Name = "Brand")]
        public int? BrandId { get; set; }
        
        [Display(Name = "Price Range")]
        public string? PriceRange { get; set; }
        
        [Display(Name = "Compatibility")]
        public string? Compatibility { get; set; }
        
        [Display(Name = "In Stock Only")]
        public bool InStockOnly { get; set; } = false;
        
        [Display(Name = "On Sale Only")]
        public bool OnSaleOnly { get; set; } = false;
        
        [Display(Name = "Featured Only")]
        public bool FeaturedOnly { get; set; } = false;
        
        [Display(Name = "Minimum Rating")]
        public int? MinRating { get; set; }
        
        [Display(Name = "Free Shipping")]
        public bool FreeShippingOnly { get; set; } = false;
        
        // Custom price range
        [Display(Name = "Min Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal? MinPrice { get; set; }
        
        [Display(Name = "Max Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal? MaxPrice { get; set; }
        
        // Sorting and pagination
        public string SortBy { get; set; } = "name";
        public bool IsDescending { get; set; } = false;
        public int PageSize { get; set; } = 12;
        public int PageNumber { get; set; } = 1;
        
        // View mode
        public string ViewMode { get; set; } = "grid";
        
        public bool HasActiveFilters => 
            !string.IsNullOrEmpty(SearchTerm) ||
            BrandId.HasValue ||
            !string.IsNullOrEmpty(PriceRange) ||
            !string.IsNullOrEmpty(Compatibility) ||
            InStockOnly ||
            OnSaleOnly ||
            FeaturedOnly ||
            MinRating.HasValue ||
            FreeShippingOnly ||
            MinPrice.HasValue ||
            MaxPrice.HasValue;
        
        public int GetActiveFilterCount()
        {
            int count = 0;
            if (!string.IsNullOrEmpty(SearchTerm)) count++;
            if (BrandId.HasValue) count++;
            if (!string.IsNullOrEmpty(PriceRange)) count++;
            if (!string.IsNullOrEmpty(Compatibility)) count++;
            if (InStockOnly) count++;
            if (OnSaleOnly) count++;
            if (FeaturedOnly) count++;
            if (MinRating.HasValue) count++;
            if (FreeShippingOnly) count++;
            if (MinPrice.HasValue || MaxPrice.HasValue) count++;
            return count;
        }
        
        public void ClearFilters()
        {
            SearchTerm = null;
            BrandId = null;
            PriceRange = null;
            Compatibility = null;
            InStockOnly = false;
            OnSaleOnly = false;
            FeaturedOnly = false;
            MinRating = null;
            FreeShippingOnly = false;
            MinPrice = null;
            MaxPrice = null;
        }
    }
    
    /// <summary>
    /// Summary information for a category
    /// </summary>
    public class CategorySummaryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PartsCount { get; set; }
        public string? ImageUrl { get; set; }
        public string? IconClass { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        
        public string PartsCountDisplay => $"{PartsCount} part{(PartsCount != 1 ? "s" : "")}";
        public string PriceRangeDisplay => MinPrice > 0 && MaxPrice > 0 ? 
            $"${MinPrice:N0} - ${MaxPrice:N0}" : "Various prices";
        
        public string GetCategoryUrl() => $"/CarParts/Category/{CategoryId}";
    }
    
    /// <summary>
    /// Summary information for a brand within a category
    /// </summary>
    public class BrandSummaryViewModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public int PartsCount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public bool IsFeatured { get; set; }
        
        public string PartsCountDisplay => $"{PartsCount} part{(PartsCount != 1 ? "s" : "")}";
        public string AveragePriceDisplay => AveragePrice > 0 ? $"Avg: ${AveragePrice:N2}" : "";
        public string PriceRangeDisplay => MinPrice > 0 && MaxPrice > 0 ? 
            $"${MinPrice:N0} - ${MaxPrice:N0}" : "";
    }
    
    // Note: BreadcrumbItem is defined in BreadcrumbViewModel.cs
    
    /// <summary>
    /// View model for category comparison
    /// </summary>
    public class CategoryComparisonViewModel
    {
        public List<CategorySummaryViewModel> Categories { get; set; } = new List<CategorySummaryViewModel>();
        public Dictionary<int, List<CarPartViewModel>> CategoryParts { get; set; } = new Dictionary<int, List<CarPartViewModel>>();
        public string ComparisonType { get; set; } = "price"; // price, features, popularity
        
        public CategorySummaryViewModel? GetMostAffordableCategory()
        {
            return Categories.Where(c => c.MinPrice > 0).OrderBy(c => c.MinPrice).FirstOrDefault();
        }
        
        public CategorySummaryViewModel? GetMostExpensiveCategory()
        {
            return Categories.Where(c => c.MaxPrice > 0).OrderByDescending(c => c.MaxPrice).FirstOrDefault();
        }
        
        public CategorySummaryViewModel? GetMostPopularCategory()
        {
            return Categories.OrderByDescending(c => c.PartsCount).FirstOrDefault();
        }
    }
    
    /// <summary>
    /// View model for category with filters applied
    /// </summary>
    public class FilteredCategoryViewModel
    {
        public CarPartCategoryViewModel Category { get; set; } = new CarPartCategoryViewModel();
        public List<FilterSummaryViewModel> AppliedFilters { get; set; } = new List<FilterSummaryViewModel>();
        public int OriginalCount { get; set; }
        public int FilteredCount { get; set; }
        
        public bool HasFiltersApplied => AppliedFilters.Any();
        public string FilterSummary => HasFiltersApplied ? 
            $"Showing {FilteredCount} of {OriginalCount} parts" : 
            $"Showing all {OriginalCount} parts";
    }
    
    /// <summary>
    /// Summary of an applied filter
    /// </summary>
    public class FilterSummaryViewModel
    {
        public string FilterType { get; set; } = string.Empty;
        public string FilterValue { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public string RemoveUrl { get; set; } = string.Empty;
    }
}