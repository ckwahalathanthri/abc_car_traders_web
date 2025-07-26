using ABCCarTraders.Models;

namespace ABCCarTraders.Services
{
    public interface ICarService
    {
        // Car management methods
        Task<List<Car>> GetAllCarsAsync();
        Task<List<Car>> GetAvailableCarsAsync();
        Task<Car?> GetCarByIdAsync(int carId);
        Task<bool> AddCarAsync(Car car);
        Task<bool> UpdateCarAsync(Car car);
        Task<bool> DeleteCarAsync(int carId);
        Task<bool> ToggleAvailabilityAsync(int carId);

        // Search and filter methods
        Task<List<Car>> SearchCarsAsync(string searchTerm);
        Task<List<Car>> GetCarsByBrandAsync(int brandId);
        Task<List<Car>> GetCarsByCategoryAsync(int categoryId);
        Task<List<Car>> GetCarsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<List<Car>> GetCarsByYearRangeAsync(int minYear, int maxYear);
        Task<List<Car>> GetCarsByFuelTypeAsync(FuelType fuelType);
        Task<List<Car>> GetCarsByTransmissionAsync(Transmission transmission);
        Task<List<Car>> GetCarsByColorAsync(string color);

        // Advanced filtering
        Task<List<Car>> GetFilteredCarsAsync(CarFilterModel filter);
        Task<List<Car>> GetFeaturedCarsAsync(int count = 6);
        Task<List<Car>> GetLatestCarsAsync(int count = 10);
        Task<List<Car>> GetPopularCarsAsync(int count = 10);

        // Stock management
        Task<bool> UpdateStockAsync(int carId, int quantity);
        Task<bool> DecreaseStockAsync(int carId, int quantity);
        Task<bool> IncreaseStockAsync(int carId, int quantity);
        Task<List<Car>> GetLowStockCarsAsync(int threshold = 3);

        // Statistics and reporting
        Task<int> GetTotalCarsCountAsync();
        Task<int> GetAvailableCarsCountAsync();
        Task<decimal> GetAveragePriceAsync();
        Task<Dictionary<string, int>> GetCarStatisticsAsync();
        Task<List<Car>> GetTopSellingCarsAsync(int count = 5);

        // Brand and category helpers
        Task<List<Brand>> GetAllBrandsAsync();
        Task<List<Category>> GetCarCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<List<string>> GetAvailableColorsAsync();
        Task<List<int>> GetAvailableYearsAsync();

        // Price calculations
        Task<decimal> GetMinPriceAsync();
        Task<decimal> GetMaxPriceAsync();

        // Validation
        Task<bool> IsCarAvailableAsync(int carId);
        Task<bool> HasSufficientStockAsync(int carId, int quantity);
    }

    // Helper model for filtering
    public class CarFilterModel
    {
        public string? SearchTerm { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public FuelType? FuelType { get; set; }
        public Transmission? Transmission { get; set; }
        public string? Color { get; set; }
        public bool? IsAvailable { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; }
    }
}