using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Models.ViewModels
{
    public class CarPartViewModel
    {
        public int CarPartId { get; set; }

        [Required(ErrorMessage = "Please select a brand")]
        [Display(Name = "Brand")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Part name is required")]
        [StringLength(200, ErrorMessage = "Part name cannot exceed 200 characters")]
        [Display(Name = "Part Name")]
        public string PartName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Part number is required")]
        [StringLength(100, ErrorMessage = "Part number cannot exceed 100 characters")]
        [Display(Name = "Part Number")]
        public string PartNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Display(Name = "Compatibility")]
        [StringLength(500, ErrorMessage = "Compatibility cannot exceed 500 characters")]
        public string? Compatibility { get; set; }

        [Display(Name = "Image URL")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; } = 0;

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        // Navigation properties for display
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        // Dropdown lists for forms
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        // Form state
        public bool IsEditMode { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        // Computed properties
        public string FormattedPrice => Price.ToString("C");
        public string StockStatus => StockQuantity > 0 ? "In Stock" : "Out of Stock";
        public string AvailabilityStatus => IsAvailable ? "Available" : "Unavailable";
        public string StockStatusColor => StockQuantity > 10 ? "success" : StockQuantity > 0 ? "warning" : "danger";

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Convert to CarPart entity
        public CarPart ToCarPart()
        {
            return new CarPart
            {
                CarPartId = CarPartId,
                BrandId = BrandId,
                CategoryId = CategoryId,
                PartName = PartName?.Trim() ?? throw new ArgumentException("Part name is required"),
                PartNumber = PartNumber?.Trim() ?? throw new ArgumentException("Part number is required"),
                Price = Price,
                Description = Description?.Trim(),
                Compatibility = Compatibility?.Trim(),
                ImageUrl = ImageUrl?.Trim(),
                StockQuantity = StockQuantity,
                IsAvailable = IsAvailable,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        // Create from CarPart entity
        public static CarPartViewModel FromCarPart(CarPart carPart)
        {
            if (carPart == null)
                throw new ArgumentNullException(nameof(carPart));

            return new CarPartViewModel
            {
                CarPartId = carPart.CarPartId,
                BrandId = carPart.BrandId,
                CategoryId = carPart.CategoryId,
                PartName = carPart.PartName ?? string.Empty,
                PartNumber = carPart.PartNumber ?? string.Empty,
                Price = carPart.Price,
                Description = carPart.Description,
                Compatibility = carPart.Compatibility,
                ImageUrl = carPart.ImageUrl,
                StockQuantity = carPart.StockQuantity,
                IsAvailable = carPart.IsAvailable,
                BrandName = carPart.Brand?.BrandName ?? string.Empty,
                CategoryName = carPart.Category?.CategoryName ?? string.Empty,
                IsEditMode = true,
                CreatedAt = carPart.CreatedAt,
                UpdatedAt = carPart.UpdatedAt
            };
        }
    }

    // Car part search view model
    public class CarPartSearchViewModel
    {
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Brand")]
        public int? BrandId { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Min Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Min price cannot be negative")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Max Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Max price cannot be negative")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Compatibility")]
        public string? Compatibility { get; set; }

        [Display(Name = "In Stock Only")]
        public bool? InStock { get; set; }

        [Display(Name = "Available Only")]
        public bool? IsAvailable { get; set; }

        [Display(Name = "Sort By")]
        public string? SortBy { get; set; }

        [Display(Name = "Order")]
        public bool IsDescending { get; set; } = false;

        // Results per page
        public int PageSize { get; set; } = 12;
        public int PageNumber { get; set; } = 1;
    }

    // Car part details view model
    public class _CarPartDetailsViewModel
    {
        public CarPartViewModel CarPart { get; set; } = new CarPartViewModel();
        public List<CarPartViewModel> RelatedParts { get; set; } = new List<CarPartViewModel>();
        public List<CarPartViewModel> SameBrandParts { get; set; } = new List<CarPartViewModel>();
        public List<CarPartViewModel> SameCategoryParts { get; set; } = new List<CarPartViewModel>();

        // Customer actions
        public int Quantity { get; set; } = 1;
        public bool CanAddToCart { get; set; } = true;
        public string? AddToCartMessage { get; set; }

        // Reviews and ratings (if implemented)
        public decimal AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;

        // Inventory information
        public bool IsLowStock => CarPart.StockQuantity <= 10;
        public bool IsOutOfStock => CarPart.StockQuantity == 0;
        public string StockMessage => IsOutOfStock ? "Out of Stock" :
                                    IsLowStock ? $"Only {CarPart.StockQuantity} left in stock" :
                                    "In Stock";

        // Compatibility information
        public List<string> CompatibilityList =>
            !string.IsNullOrEmpty(CarPart.Compatibility)
                ? CarPart.Compatibility.Split(',').Select(c => c.Trim()).ToList()
                : new List<string>();
    }

    // Car part bulk operations view model
    public class CarPartBulkOperationsViewModel
    {
        public List<int> SelectedCarPartIds { get; set; } = new List<int>();
        public string Operation { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        // Bulk update fields
        public int? NewStockQuantity { get; set; }
        public bool? NewAvailabilityStatus { get; set; }
        public decimal? NewPrice { get; set; }
        public decimal? PriceAdjustmentPercentage { get; set; }

        // Results
        public int AffectedItems { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string? SuccessMessage { get; set; }
    }

    // Car part inventory view model
    public class CarPartInventoryViewModel
    {
        // Lists of parts by stock status
        public List<CarPartViewModel> LowStockParts { get; set; } = new List<CarPartViewModel>();
        public List<CarPartViewModel> OutOfStockParts { get; set; } = new List<CarPartViewModel>();
        public List<CarPartViewModel> OverstockedParts { get; set; } = new List<CarPartViewModel>();

        // Inventory statistics (renamed to avoid conflicts)
        public int TotalParts { get; set; }
        public int InStockParts { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public decimal TotalInventoryValue { get; set; }

        // Thresholds
        public int LowStockThreshold { get; set; } = 10;
        public int OverstockThreshold { get; set; } = 100;

        // Filters
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        // Computed properties for convenience
        public decimal LowStockPercentage => TotalParts > 0 ? (decimal)LowStockCount / TotalParts * 100 : 0;
        public decimal OutOfStockPercentage => TotalParts > 0 ? (decimal)OutOfStockCount / TotalParts * 100 : 0;
    }
}