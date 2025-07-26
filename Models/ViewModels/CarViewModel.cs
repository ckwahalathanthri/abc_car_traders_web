using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Models.ViewModels
{
    public class CarViewModel
    {
        public int CarId { get; set; }
        
        [Required(ErrorMessage = "Please select a brand")]
        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        
        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        
        [Required(ErrorMessage = "Model is required")]
        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
        [Display(Name = "Model")]
        public string Model { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
        [Display(Name = "Year")]
        public int Year { get; set; }
        
        [Display(Name = "Color")]
        public string? Color { get; set; }
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        
        [Display(Name = "Mileage")]
        [Range(0, int.MaxValue, ErrorMessage = "Mileage cannot be negative")]
        public int? Mileage { get; set; }
        
        [Required(ErrorMessage = "Please select fuel type")]
        [Display(Name = "Fuel Type")]
        public FuelType FuelType { get; set; }
        
        [Required(ErrorMessage = "Please select transmission type")]
        [Display(Name = "Transmission")]
        public Transmission Transmission { get; set; }
        
        [Display(Name = "Engine Capacity")]
        public string? EngineCapacity { get; set; }
        
        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
        
        [Display(Name = "Features")]
        [StringLength(500, ErrorMessage = "Features cannot exceed 500 characters")]
        public string? Features { get; set; }
        
        [Display(Name = "Image URL")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }
        
        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; } = 1;
        
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;
        
        // Navigation properties for display
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        
        // Dropdown lists for forms
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> FuelTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Transmissions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Colors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Years { get; set; } = new List<SelectListItem>();
        
        // Form state
        public bool IsEditMode { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        
        // Computed properties
        public string FormattedPrice => Price.ToString("C");
        public string FormattedMileage => Mileage?.ToString("N0") ?? "N/A";
        public string StockStatus => StockQuantity > 0 ? "In Stock" : "Out of Stock";
        public string AvailabilityStatus => IsAvailable ? "Available" : "Unavailable";
        
        // Constructor
        public CarViewModel()
        {
            LoadDropdownData();
        }
        
        // Methods
        private void LoadDropdownData()
        {
            // Load fuel types
            FuelTypes = Enum.GetValues<FuelType>()
                .Select(ft => new SelectListItem
                {
                    Value = ((int)ft).ToString(),
                    Text = ft.ToString()
                }).ToList();
            
            // Load transmissions
            Transmissions = Enum.GetValues<Transmission>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                }).ToList();
            
            // Load common colors
            var commonColors = new[] { "White", "Black", "Silver", "Gray", "Red", "Blue", "Green", "Yellow", "Brown", "Orange" };
            Colors = commonColors.Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();
            
            // Load years (current year + 1 down to 1980)
            var currentYear = DateTime.Now.Year;
            Years = Enumerable.Range(1980, currentYear - 1979 + 1)
                .Reverse()
                .Select(y => new SelectListItem
                {
                    Value = y.ToString(),
                    Text = y.ToString()
                }).ToList();
        }

        // Convert to Car entity
        public Car ToCar()
        {
            return new Car
            {
                CarId = CarId,
                BrandId = BrandId,
                CategoryId = CategoryId,
                Model = Model?.Trim() ?? throw new ArgumentException("Model is required"),
                Year = Year,
                Color = Color?.Trim(),
                Price = Price,
                Mileage = Mileage,
                FuelType = FuelType,
                Transmission = Transmission,
                EngineCapacity = EngineCapacity?.Trim(),
                Description = Description?.Trim(),
                Features = Features?.Trim(),
                ImageUrl = ImageUrl?.Trim(),
                StockQuantity = StockQuantity,
                IsAvailable = IsAvailable,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        // Enhanced FromCar method
        public static CarViewModel FromCar(Car car)
        {
            if (car == null)
                throw new ArgumentNullException(nameof(car));

            var viewModel = new CarViewModel
            {
                CarId = car.CarId,
                BrandId = car.BrandId,
                CategoryId = car.CategoryId,
                Model = car.Model ?? string.Empty,
                Year = car.Year,
                Color = car.Color,
                Price = car.Price,
                Mileage = car.Mileage,
                FuelType = car.FuelType,
                Transmission = car.Transmission,
                EngineCapacity = car.EngineCapacity,
                Description = car.Description,
                Features = car.Features,
                ImageUrl = car.ImageUrl,
                StockQuantity = car.StockQuantity,
                IsAvailable = car.IsAvailable,
                BrandName = car.Brand?.BrandName ?? string.Empty,
                CategoryName = car.Category?.CategoryName ?? string.Empty,
                IsEditMode = true
            };

            return viewModel;
        }
    }
    
    // Car list view model
    public class CarListViewModel
    {
        public List<CarViewModel> Cars { get; set; } = new List<CarViewModel>();
        public CarSearchViewModel SearchModel { get; set; } = new CarSearchViewModel();
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
        
        // Filter options
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> FuelTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Transmissions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Colors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Years { get; set; } = new List<SelectListItem>();
        
        // Statistics
        public int TotalCars { get; set; }
        public int AvailableCars { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
    
    // Car search view model
    public class CarSearchViewModel
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
        
        [Display(Name = "Min Year")]
        public int? MinYear { get; set; }
        
        [Display(Name = "Max Year")]
        public int? MaxYear { get; set; }
        
        [Display(Name = "Fuel Type")]
        public FuelType? FuelType { get; set; }
        
        [Display(Name = "Transmission")]
        public Transmission? Transmission { get; set; }
        
        [Display(Name = "Color")]
        public string? Color { get; set; }
        
        [Display(Name = "Sort By")]
        public string? SortBy { get; set; }
        
        [Display(Name = "Order")]
        public bool IsDescending { get; set; } = false;
        
        // Results per page
        public int PageSize { get; set; } = 12;
        public int PageNumber { get; set; } = 1;
    }
    
    // Pagination view model
    public class CustomPaginationViewModel
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalItems { get; set; } = 0;
        public int ItemsPerPage { get; set; } = 12;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartItem => (CurrentPage - 1) * ItemsPerPage + 1;
        public int EndItem => Math.Min(CurrentPage * ItemsPerPage, TotalItems);
        
        public List<int> GetPaginationNumbers()
        {
            var numbers = new List<int>();
            var start = Math.Max(1, CurrentPage - 2);
            var end = Math.Min(TotalPages, CurrentPage + 2);
            
            for (var i = start; i <= end; i++)
            {
                numbers.Add(i);
            }
            
            return numbers;
        }

        public static implicit operator CustomPaginationViewModel(PaginationViewModel v)
        {
            throw new NotImplementedException();
        }
    }
}