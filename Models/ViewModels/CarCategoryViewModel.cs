using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for cars category page
    /// </summary>
    public class CarCategoryViewModel
    {
        // Category information
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Category Type")]
        public CategoryType CategoryType { get; set; } = CategoryType.Car;

        // Cars in this category
        public List<CarViewModel> Cars { get; set; } = new List<CarViewModel>();

        // Pagination
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();

        // Search and filtering
        public CarCategorySearchViewModel SearchModel { get; set; } = new CarCategorySearchViewModel();

        // Filter options
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> YearRanges { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PriceRanges { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> FuelTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Transmissions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Colors { get; set; } = new List<SelectListItem>();

        // Category statistics
        public int TotalCarsInCategory { get; set; }
        public int AvailableCarsInCategory { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal AveragePrice { get; set; }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }

        // Related categories
        public List<CarCategorySummaryViewModel> RelatedCategories { get; set; } = new List<CarCategorySummaryViewModel>();

        // Popular brands in this category
        public List<CarBrandSummaryViewModel> PopularBrands { get; set; } = new List<CarBrandSummaryViewModel>();

        // Featured cars
        public List<CarViewModel> FeaturedCars { get; set; } = new List<CarViewModel>();

        // Recently added cars
        public List<CarViewModel> RecentlyAddedCars { get; set; } = new List<CarViewModel>();

        // Breadcrumb navigation
        public List<BreadcrumbItem> Breadcrumbs { get; set; } = new List<BreadcrumbItem>();

        // SEO and meta information
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        // View preferences
        public string ViewMode { get; set; } = "grid"; // grid or list
        public string SortBy { get; set; } = "price";
        public bool IsDescending { get; set; } = false;

        // Computed properties
        public string CategoryTitle => !string.IsNullOrEmpty(CategoryName) ? $"{CategoryName} Cars" : "Cars";
        public string PriceRangeDisplay => MinPrice > 0 && MaxPrice > 0 ? $"${MinPrice:N0} - ${MaxPrice:N0}" : "Varies";
        public string AveragePriceDisplay => AveragePrice > 0 ? $"${AveragePrice:N0}" : "N/A";
        public string YearRangeDisplay => MinYear > 0 && MaxYear > 0 ? $"{MinYear} - {MaxYear}" : "Various";
        public bool HasCars => Cars.Any();
        public int SoldOutCount => TotalCarsInCategory - AvailableCarsInCategory;

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
            new SelectListItem { Value = "price", Text = "Price (Low to High)" },
            new SelectListItem { Value = "price_desc", Text = "Price (High to Low)" },
            new SelectListItem { Value = "year", Text = "Year (Oldest First)" },
            new SelectListItem { Value = "year_desc", Text = "Year (Newest First)" },
            new SelectListItem { Value = "mileage", Text = "Mileage (Low to High)" },
            new SelectListItem { Value = "mileage_desc", Text = "Mileage (High to Low)" },
            new SelectListItem { Value = "brand", Text = "Brand (A-Z)" },
            new SelectListItem { Value = "model", Text = "Model (A-Z)" },
            new SelectListItem { Value = "newest", Text = "Recently Added" },
            new SelectListItem { Value = "featured", Text = "Featured First" }
        };

        // Initialize default values
        public CarCategoryViewModel()
        {
            SetDefaultBreadcrumbs();
            SetDefaultPriceRanges();
            SetDefaultYearRanges();
        }

        public CarCategoryViewModel(int categoryId, string categoryName) : this()
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
                new BreadcrumbItem { Title = "Cars", Url = "/Cars", Icon = "fas fa-car" }
            };
        }

        private void SetDefaultPriceRanges()
        {
            PriceRanges = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Prices" },
                new SelectListItem { Value = "0-10000", Text = "Under $10,000" },
                new SelectListItem { Value = "10000-20000", Text = "$10,000 - $20,000" },
                new SelectListItem { Value = "20000-30000", Text = "$20,000 - $30,000" },
                new SelectListItem { Value = "30000-50000", Text = "$30,000 - $50,000" },
                new SelectListItem { Value = "50000-75000", Text = "$50,000 - $75,000" },
                new SelectListItem { Value = "75000-100000", Text = "$75,000 - $100,000" },
                new SelectListItem { Value = "100000+", Text = "$100,000+" }
            };
        }

        private void SetDefaultYearRanges()
        {
            var currentYear = DateTime.Now.Year;
            YearRanges = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Years" },
                new SelectListItem { Value = $"{currentYear}-{currentYear}", Text = $"{currentYear}" },
                new SelectListItem { Value = $"{currentYear-1}-{currentYear}", Text = $"{currentYear-1} - {currentYear}" },
                new SelectListItem { Value = $"{currentYear-2}-{currentYear-1}", Text = $"{currentYear-2} - {currentYear-1}" },
                new SelectListItem { Value = $"{currentYear-5}-{currentYear-3}", Text = $"{currentYear-5} - {currentYear-3}" },
                new SelectListItem { Value = $"{currentYear-10}-{currentYear-6}", Text = $"{currentYear-10} - {currentYear-6}" },
                new SelectListItem { Value = $"2000-{currentYear-11}", Text = $"2000 - {currentYear-11}" },
                new SelectListItem { Value = "1990-1999", Text = "1990 - 1999" }
            };
        }

        /// <summary>
        /// Gets the current sort option display text
        /// </summary>
        public string GetCurrentSortDisplay()
        {
            var currentSort = SortOptions.FirstOrDefault(s => s.Value == SortBy);
            return currentSort?.Text ?? "Price (Low to High)";
        }

        /// <summary>
        /// Gets category-specific recommendations message
        /// </summary>
        public string GetCategoryRecommendation()
        {
            return CategoryName.ToLower() switch
            {
                "sedan" => "Perfect for city driving and family comfort",
                "suv" => "Spacious and versatile for all your adventures",
                "hatchback" => "Compact and fuel-efficient urban vehicles",
                "coupe" => "Stylish performance cars for driving enthusiasts",
                "convertible" => "Open-air driving experiences",
                "pickup truck" => "Powerful trucks for work and recreation",
                "wagon" => "Family-friendly vehicles with extra cargo space",
                _ => "Quality vehicles for every lifestyle"
            };
        }

        /// <summary>
        /// Gets average fuel efficiency for category
        /// </summary>
        public string GetCategoryFuelEfficiency()
        {
            return CategoryName.ToLower() switch
            {
                "sedan" => "25-35 MPG",
                "suv" => "20-28 MPG",
                "hatchback" => "28-40 MPG",
                "coupe" => "22-30 MPG",
                "convertible" => "20-28 MPG",
                "pickup truck" => "15-25 MPG",
                "wagon" => "22-32 MPG",
                _ => "Varies"
            };
        }
    }

    /// <summary>
    /// Search and filter model for car category page
    /// </summary>
    public class CarCategorySearchViewModel
    {
        public int CategoryId { get; set; }

        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Brand")]
        public int? BrandId { get; set; }

        [Display(Name = "Price Range")]
        public string? PriceRange { get; set; }

        [Display(Name = "Year Range")]
        public string? YearRange { get; set; }

        [Display(Name = "Fuel Type")]
        public string? FuelType { get; set; }

        [Display(Name = "Transmission")]
        public string? Transmission { get; set; }

        [Display(Name = "Color")]
        public string? Color { get; set; }

        [Display(Name = "Available Only")]
        public bool AvailableOnly { get; set; } = false;

        [Display(Name = "Low Mileage")]
        public bool LowMileageOnly { get; set; } = false;

        [Display(Name = "Single Owner")]
        public bool SingleOwnerOnly { get; set; } = false;

        [Display(Name = "Recently Added")]
        public bool RecentlyAddedOnly { get; set; } = false;

        [Display(Name = "Featured Only")]
        public bool FeaturedOnly { get; set; } = false;

        // Custom price range
        [Display(Name = "Min Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Max Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal? MaxPrice { get; set; }

        // Custom year range
        [Display(Name = "Min Year")]
        [Range(1990, 2030, ErrorMessage = "Year must be between 1990 and 2030")]
        public int? MinYear { get; set; }

        [Display(Name = "Max Year")]
        [Range(1990, 2030, ErrorMessage = "Year must be between 1990 and 2030")]
        public int? MaxYear { get; set; }

        // Custom mileage range
        [Display(Name = "Max Mileage")]
        [Range(0, int.MaxValue, ErrorMessage = "Mileage cannot be negative")]
        public int? MaxMileage { get; set; }

        // Sorting and pagination
        public string SortBy { get; set; } = "price";
        public bool IsDescending { get; set; } = false;
        public int PageSize { get; set; } = 12;
        public int PageNumber { get; set; } = 1;

        // View mode
        public string ViewMode { get; set; } = "grid";

        public bool HasActiveFilters =>
            !string.IsNullOrEmpty(SearchTerm) ||
            BrandId.HasValue ||
            !string.IsNullOrEmpty(PriceRange) ||
            !string.IsNullOrEmpty(YearRange) ||
            !string.IsNullOrEmpty(FuelType) ||
            !string.IsNullOrEmpty(Transmission) ||
            !string.IsNullOrEmpty(Color) ||
            AvailableOnly ||
            LowMileageOnly ||
            SingleOwnerOnly ||
            RecentlyAddedOnly ||
            FeaturedOnly ||
            MinPrice.HasValue ||
            MaxPrice.HasValue ||
            MinYear.HasValue ||
            MaxYear.HasValue ||
            MaxMileage.HasValue;

        public int GetActiveFilterCount()
        {
            int count = 0;
            if (!string.IsNullOrEmpty(SearchTerm)) count++;
            if (BrandId.HasValue) count++;
            if (!string.IsNullOrEmpty(PriceRange)) count++;
            if (!string.IsNullOrEmpty(YearRange)) count++;
            if (!string.IsNullOrEmpty(FuelType)) count++;
            if (!string.IsNullOrEmpty(Transmission)) count++;
            if (!string.IsNullOrEmpty(Color)) count++;
            if (AvailableOnly) count++;
            if (LowMileageOnly) count++;
            if (SingleOwnerOnly) count++;
            if (RecentlyAddedOnly) count++;
            if (FeaturedOnly) count++;
            if (MinPrice.HasValue || MaxPrice.HasValue) count++;
            if (MinYear.HasValue || MaxYear.HasValue) count++;
            if (MaxMileage.HasValue) count++;
            return count;
        }

        public void ClearFilters()
        {
            SearchTerm = null;
            BrandId = null;
            PriceRange = null;
            YearRange = null;
            FuelType = null;
            Transmission = null;
            Color = null;
            AvailableOnly = false;
            LowMileageOnly = false;
            SingleOwnerOnly = false;
            RecentlyAddedOnly = false;
            FeaturedOnly = false;
            MinPrice = null;
            MaxPrice = null;
            MinYear = null;
            MaxYear = null;
            MaxMileage = null;
        }
    }

    /// <summary>
    /// Summary information for a car category
    /// </summary>
    public class CarCategorySummaryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CarsCount { get; set; }
        public string? ImageUrl { get; set; }
        public string? IconClass { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal AveragePrice { get; set; }

        public string CarsCountDisplay => $"{CarsCount} car{(CarsCount != 1 ? "s" : "")}";
        public string PriceRangeDisplay => MinPrice > 0 && MaxPrice > 0 ?
            $"${MinPrice:N0} - ${MaxPrice:N0}" : "Various prices";
        public string AveragePriceDisplay => AveragePrice > 0 ? $"Avg: ${AveragePrice:N0}" : "";

        public string GetCategoryUrl() => $"/Cars/Category/{CategoryId}";

        public string GetCategoryIcon()
        {
            return CategoryName.ToLower() switch
            {
                "sedan" => "fas fa-car",
                "suv" => "fas fa-truck",
                "hatchback" => "fas fa-car-side",
                "coupe" => "fas fa-car-alt",
                "convertible" => "fas fa-car-crash",
                "pickup truck" => "fas fa-truck-pickup",
                "wagon" => "fas fa-shuttle-van",
                _ => "fas fa-car"
            };
        }
    }

    /// <summary>
    /// Summary information for a brand within a car category
    /// </summary>
    public class CarBrandSummaryViewModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public int CarsCount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public bool IsFeatured { get; set; }
        public int AverageYear { get; set; }

        public string CarsCountDisplay => $"{CarsCount} car{(CarsCount != 1 ? "s" : "")}";
        public string AveragePriceDisplay => AveragePrice > 0 ? $"Avg: ${AveragePrice:N0}" : "";
        public string PriceRangeDisplay => MinPrice > 0 && MaxPrice > 0 ?
            $"${MinPrice:N0} - ${MaxPrice:N0}" : "";
        public string AverageYearDisplay => AverageYear > 0 ? $"Avg: {AverageYear}" : "";
    }

    /// <summary>
    /// Car financing information
    /// </summary>
    public class CarFinancingViewModel
    {
        public decimal CarPrice { get; set; }
        public decimal DownPayment { get; set; }
        public decimal InterestRate { get; set; }
        public int LoanTermMonths { get; set; }

        public decimal LoanAmount => CarPrice - DownPayment;
        public decimal MonthlyPayment
        {
            get
            {
                if (InterestRate == 0) return LoanAmount / LoanTermMonths;

                var monthlyRate = InterestRate / 100 / 12;
                var monthlyRateDouble = (double)monthlyRate;
                var payment = LoanAmount * (decimal)((monthlyRateDouble * Math.Pow(1 + monthlyRateDouble, LoanTermMonths)) /
                             (Math.Pow(1 + monthlyRateDouble, LoanTermMonths) - 1));
                return payment;
            }
        }

        public decimal TotalInterest => (MonthlyPayment * LoanTermMonths) - LoanAmount;
        public decimal TotalCost => CarPrice + TotalInterest;

        public string MonthlyPaymentFormatted => MonthlyPayment.ToString("C");
        public string TotalInterestFormatted => TotalInterest.ToString("C");
        public string TotalCostFormatted => TotalCost.ToString("C");
    }

    /// <summary>
    /// Car comparison view model
    /// </summary>
    public class CarComparisonViewModel
    {
        public List<CarViewModel> CarsToCompare { get; set; } = new List<CarViewModel>();
        public string ComparisonType { get; set; } = "features"; // features, specs, price

        public bool CanCompare => CarsToCompare.Count >= 2 && CarsToCompare.Count <= 4;
        public int MaxComparisons => 4;
        public int RemainingSlots => MaxComparisons - CarsToCompare.Count;

        public CarViewModel? GetMostAffordable()
        {
            return CarsToCompare.Where(c => c.Price > 0).OrderBy(c => c.Price).FirstOrDefault();
        }

        public CarViewModel? GetMostExpensive()
        {
            return CarsToCompare.Where(c => c.Price > 0).OrderByDescending(c => c.Price).FirstOrDefault();
        }

        public CarViewModel? GetNewest()
        {
            return CarsToCompare.OrderByDescending(c => c.Year).FirstOrDefault();
        }

        public CarViewModel? GetLowestMileage()
        {
            return CarsToCompare.Where(c => c.Mileage.HasValue).OrderBy(c => c.Mileage).FirstOrDefault();
        }
    }
}