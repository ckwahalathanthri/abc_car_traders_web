using ABCCarTraders.Models;

namespace ABCCarTraders.Services
{
    public interface ICarPartService
    {
        // Car part management methods
        Task<List<CarPart>> GetAllCarPartsAsync();
        Task<List<CarPart>> GetAvailableCarPartsAsync();
        Task<CarPart?> GetCarPartByIdAsync(int carPartId);
        Task<CarPart?> GetCarPartByPartNumberAsync(string partNumber);
        Task<bool> AddCarPartAsync(CarPart carPart);
        Task<bool> UpdateCarPartAsync(CarPart carPart);
        Task<bool> DeleteCarPartAsync(int carPartId);
        Task<bool> ToggleAvailabilityAsync(int carPartId);
        
        // Search and filter methods
        Task<List<CarPart>> SearchCarPartsAsync(string searchTerm);
        Task<List<CarPart>> GetCarPartsByBrandAsync(int brandId);
        Task<List<CarPart>> GetCarPartsByCategoryAsync(int categoryId);
        Task<List<CarPart>> GetCarPartsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<List<CarPart>> GetCarPartsByCompatibilityAsync(string compatibility);
        
        // Advanced filtering
        Task<List<CarPart>> GetFilteredCarPartsAsync(CarPartFilterModel filter);
        Task<List<CarPart>> GetFeaturedCarPartsAsync(int count = 6);
        Task<List<CarPart>> GetLatestCarPartsAsync(int count = 10);
        Task<List<CarPart>> GetPopularCarPartsAsync(int count = 10);
        
        // Stock management
        Task<bool> UpdateStockAsync(int carPartId, int quantity);
        Task<bool> DecreaseStockAsync(int carPartId, int quantity);
        Task<bool> IncreaseStockAsync(int carPartId, int quantity);
        Task<List<CarPart>> GetLowStockCarPartsAsync(int threshold = 10);
        Task<List<CarPart>> GetOutOfStockCarPartsAsync();
        
        // Statistics and reporting
        Task<int> GetTotalCarPartsCountAsync();
        Task<int> GetAvailableCarPartsCountAsync();
        Task<decimal> GetAveragePriceAsync();
        Task<Dictionary<string, int>> GetCarPartStatisticsAsync();
        Task<List<CarPart>> GetTopSellingCarPartsAsync(int count = 5);
        
        // Brand and category helpers
        Task<List<Brand>> GetAllBrandsAsync();
        Task<List<Category>> GetCarPartCategoriesAsync();
        Task<List<string>> GetAvailableCompatibilitiesAsync();
        
        // Price calculations
        Task<decimal> GetMinPriceAsync();
        Task<decimal> GetMaxPriceAsync();
        
        // Validation
        Task<bool> IsCarPartAvailableAsync(int carPartId);
        Task<bool> HasSufficientStockAsync(int carPartId, int quantity);
        Task<bool> IsPartNumberUniqueAsync(string partNumber, int? excludeId = null);
        
        // Inventory management
        Task<Dictionary<string, object>> GetInventoryStatusAsync();
        Task<List<CarPart>> GetExpiringPartsAsync(); // If applicable
    }
    
    // Helper model for filtering
    public class CarPartFilterModel
    {
        public string? SearchTerm { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Compatibility { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? InStock { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; }
    }
}