using ABCCarTraders.Data;
using ABCCarTraders.Models;
using Microsoft.EntityFrameworkCore;

namespace ABCCarTraders.Services
{
    public class CarPartService : ICarPartService
    {
        private readonly ApplicationDbContext _context;
        
        public CarPartService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // Car part management methods
        public async Task<List<CarPart>> GetAllCarPartsAsync()
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .OrderByDescending(cp => cp.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<List<CarPart>> GetAvailableCarPartsAsync()
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.IsAvailable && cp.StockQuantity > 0)
                    .OrderByDescending(cp => cp.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<CarPart?> GetCarPartByIdAsync(int carPartId)
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .FirstOrDefaultAsync(cp => cp.CarPartId == carPartId);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public async Task<CarPart?> GetCarPartByPartNumberAsync(string partNumber)
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .FirstOrDefaultAsync(cp => cp.PartNumber == partNumber);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public async Task<bool> AddCarPartAsync(CarPart carPart)
        {
            try
            {
                // Check if part number is unique
                if (await IsPartNumberUniqueAsync(carPart.PartNumber))
                {
                    carPart.CreatedAt = DateTime.Now;
                    carPart.UpdatedAt = DateTime.Now;
                    
                    _context.CarParts.Add(carPart);
                    await _context.SaveChangesAsync();
                    return true;
                }
                
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> UpdateCarPartAsync(CarPart carPart)
        {
            try
            {
                var existingCarPart = await _context.CarParts.FirstOrDefaultAsync(cp => cp.CarPartId == carPart.CarPartId);
                if (existingCarPart == null)
                {
                    return false;
                }
                
                // Check if part number is unique (excluding current record)
                if (!await IsPartNumberUniqueAsync(carPart.PartNumber, carPart.CarPartId))
                {
                    return false;
                }
                
                existingCarPart.BrandId = carPart.BrandId;
                existingCarPart.CategoryId = carPart.CategoryId;
                existingCarPart.PartName = carPart.PartName;
                existingCarPart.PartNumber = carPart.PartNumber;
                existingCarPart.Price = carPart.Price;
                existingCarPart.Description = carPart.Description;
                existingCarPart.Compatibility = carPart.Compatibility;
                existingCarPart.ImageUrl = carPart.ImageUrl;
                existingCarPart.StockQuantity = carPart.StockQuantity;
                existingCarPart.IsAvailable = carPart.IsAvailable;
                existingCarPart.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> DeleteCarPartAsync(int carPartId)
        {
            try
            {
                var carPart = await _context.CarParts.FirstOrDefaultAsync(cp => cp.CarPartId == carPartId);
                if (carPart == null)
                {
                    return false;
                }
                
                _context.CarParts.Remove(carPart);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> ToggleAvailabilityAsync(int carPartId)
        {
            try
            {
                var carPart = await _context.CarParts.FirstOrDefaultAsync(cp => cp.CarPartId == carPartId);
                if (carPart == null)
                {
                    return false;
                }
                
                carPart.IsAvailable = !carPart.IsAvailable;
                carPart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        // Search and filter methods
        public async Task<List<CarPart>> SearchCarPartsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAvailableCarPartsAsync();
                }
                
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.IsAvailable && cp.StockQuantity > 0 &&
                                (cp.PartName.Contains(searchTerm) ||
                                 cp.PartNumber.Contains(searchTerm) ||
                                 cp.Brand.BrandName.Contains(searchTerm) ||
                                 cp.Category.CategoryName.Contains(searchTerm) ||
                                 cp.Description.Contains(searchTerm) ||
                                 cp.Compatibility.Contains(searchTerm)))
                    .OrderByDescending(cp => cp.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<List<CarPart>> GetCarPartsByBrandAsync(int brandId)
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.BrandId == brandId && cp.IsAvailable && cp.StockQuantity > 0)
                    .OrderByDescending(cp => cp.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<List<CarPart>> GetCarPartsByCategoryAsync(int categoryId)
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.CategoryId == categoryId && cp.IsAvailable && cp.StockQuantity > 0)
                    .OrderByDescending(cp => cp.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<List<CarPart>> GetCarPartsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.Price >= minPrice && cp.Price <= maxPrice && cp.IsAvailable && cp.StockQuantity > 0)
                    .OrderBy(cp => cp.Price)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<List<CarPart>> GetCarPartsByCompatibilityAsync(string compatibility)
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.Compatibility.Contains(compatibility) && cp.IsAvailable && cp.StockQuantity > 0)
                    .OrderByDescending(cp => cp.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        // Advanced filtering
        public async Task<List<CarPart>> GetFilteredCarPartsAsync(CarPartFilterModel filter)
        {
            try
            {
                var query = _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(cp => cp.PartName.Contains(filter.SearchTerm) ||
                                             cp.PartNumber.Contains(filter.SearchTerm) ||
                                             cp.Brand.BrandName.Contains(filter.SearchTerm) ||
                                             cp.Category.CategoryName.Contains(filter.SearchTerm));
                }
                
                if (filter.BrandId.HasValue)
                {
                    query = query.Where(cp => cp.BrandId == filter.BrandId.Value);
                }
                
                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(cp => cp.CategoryId == filter.CategoryId.Value);
                }
                
                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(cp => cp.Price >= filter.MinPrice.Value);
                }
                
                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(cp => cp.Price <= filter.MaxPrice.Value);
                }
                
                if (!string.IsNullOrWhiteSpace(filter.Compatibility))
                {
                    query = query.Where(cp => cp.Compatibility.Contains(filter.Compatibility));
                }
                
                if (filter.IsAvailable.HasValue)
                {
                    query = query.Where(cp => cp.IsAvailable == filter.IsAvailable.Value);
                }
                
                if (filter.InStock.HasValue)
                {
                    query = filter.InStock.Value
                        ? query.Where(cp => cp.StockQuantity > 0)
                        : query.Where(cp => cp.StockQuantity == 0);
                }
                
                // Apply sorting
                query = filter.SortBy switch
                {
                    "price" => filter.IsDescending ? query.OrderByDescending(cp => cp.Price) : query.OrderBy(cp => cp.Price),
                    "name" => filter.IsDescending ? query.OrderByDescending(cp => cp.PartName) : query.OrderBy(cp => cp.PartName),
                    "partNumber" => filter.IsDescending ? query.OrderByDescending(cp => cp.PartNumber) : query.OrderBy(cp => cp.PartNumber),
                    "brand" => filter.IsDescending ? query.OrderByDescending(cp => cp.Brand.BrandName) : query.OrderBy(cp => cp.Brand.BrandName),
                    "stock" => filter.IsDescending ? query.OrderByDescending(cp => cp.StockQuantity) : query.OrderBy(cp => cp.StockQuantity),
                    _ => query.OrderByDescending(cp => cp.CreatedAt)
                };
                
                return await query.ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<List<CarPart>> GetFeaturedCarPartsAsync(int count = 6)
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.IsAvailable && cp.StockQuantity > 0)
                    .OrderByDescending(cp => cp.Price)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<List<CarPart>> GetLatestCarPartsAsync(int count = 10)
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.IsAvailable && cp.StockQuantity > 0)
                    .OrderByDescending(cp => cp.CreatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<List<CarPart>> GetPopularCarPartsAsync(int count = 10)
        {
            try
            {
                // For now, we'll return parts with highest stock as popular
                // In a real app, this would be based on sales data
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.IsAvailable && cp.StockQuantity > 0)
                    .OrderByDescending(cp => cp.StockQuantity)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        // Stock management
        public async Task<bool> UpdateStockAsync(int carPartId, int quantity)
        {
            try
            {
                var carPart = await _context.CarParts.FirstOrDefaultAsync(cp => cp.CarPartId == carPartId);
                if (carPart == null)
                {
                    return false;
                }
                
                carPart.StockQuantity = quantity;
                carPart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> DecreaseStockAsync(int carPartId, int quantity)
        {
            try
            {
                var carPart = await _context.CarParts.FirstOrDefaultAsync(cp => cp.CarPartId == carPartId);
                if (carPart == null || carPart.StockQuantity < quantity)
                {
                    return false;
                }
                
                carPart.StockQuantity -= quantity;
                carPart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> IncreaseStockAsync(int carPartId, int quantity)
        {
            try
            {
                var carPart = await _context.CarParts.FirstOrDefaultAsync(cp => cp.CarPartId == carPartId);
                if (carPart == null)
                {
                    return false;
                }
                
                carPart.StockQuantity += quantity;
                carPart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<List<CarPart>> GetLowStockCarPartsAsync(int threshold = 10)
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.StockQuantity <= threshold && cp.IsAvailable)
                    .OrderBy(cp => cp.StockQuantity)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        public async Task<List<CarPart>> GetOutOfStockCarPartsAsync()
        {
            try
            {
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.StockQuantity == 0 && cp.IsAvailable)
                    .OrderBy(cp => cp.PartName)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        // Statistics and reporting
        public async Task<int> GetTotalCarPartsCountAsync()
        {
            try
            {
                return await _context.CarParts.CountAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        
        public async Task<int> GetAvailableCarPartsCountAsync()
        {
            try
            {
                return await _context.CarParts.CountAsync(cp => cp.IsAvailable && cp.StockQuantity > 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        
        public async Task<decimal> GetAveragePriceAsync()
        {
            try
            {
                return await _context.CarParts
                    .Where(cp => cp.IsAvailable)
                    .AverageAsync(cp => cp.Price);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        
        public async Task<Dictionary<string, int>> GetCarPartStatisticsAsync()
        {
            try
            {
                var stats = new Dictionary<string, int>();
                
                stats["TotalCarParts"] = await GetTotalCarPartsCountAsync();
                stats["AvailableCarParts"] = await GetAvailableCarPartsCountAsync();
                stats["UnavailableCarParts"] = stats["TotalCarParts"] - stats["AvailableCarParts"];
                stats["LowStockCarParts"] = await _context.CarParts.CountAsync(cp => cp.StockQuantity <= 10);
                stats["OutOfStockCarParts"] = await _context.CarParts.CountAsync(cp => cp.StockQuantity == 0);
                stats["NewCarPartsThisMonth"] = await _context.CarParts.CountAsync(cp => cp.CreatedAt.Month == DateTime.Now.Month);
                
                return stats;
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }
        
        public async Task<List<CarPart>> GetTopSellingCarPartsAsync(int count = 5)
        {
            try
            {
                // This would be based on order items in a real application
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.IsAvailable)
                    .OrderByDescending(cp => cp.Price)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
        
        // Brand and category helpers
        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            try
            {
                return await _context.Brands
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.BrandName)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Brand>();
            }
        }
        
        public async Task<List<Category>> GetCarPartCategoriesAsync()
        {
            try
            {
                return await _context.Categories
                    .Where(c => c.CategoryType == CategoryType.CarPart && c.IsActive)
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Category>();
            }
        }
        
        public async Task<List<string>> GetAvailableCompatibilitiesAsync()
        {
            try
            {
                return await _context.CarParts
                    .Where(cp => cp.IsAvailable && !string.IsNullOrEmpty(cp.Compatibility))
                    .Select(cp => cp.Compatibility)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        
        // Price calculations
        public async Task<decimal> GetMinPriceAsync()
        {
            try
            {
                return await _context.CarParts
                    .Where(cp => cp.IsAvailable)
                    .MinAsync(cp => cp.Price);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        
        public async Task<decimal> GetMaxPriceAsync()
        {
            try
            {
                return await _context.CarParts
                    .Where(cp => cp.IsAvailable)
                    .MaxAsync(cp => cp.Price);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        
        // Validation
        public async Task<bool> IsCarPartAvailableAsync(int carPartId)
        {
            try
            {
                var carPart = await _context.CarParts.FirstOrDefaultAsync(cp => cp.CarPartId == carPartId);
                return carPart != null && carPart.IsAvailable && carPart.StockQuantity > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> HasSufficientStockAsync(int carPartId, int quantity)
        {
            try
            {
                var carPart = await _context.CarParts.FirstOrDefaultAsync(cp => cp.CarPartId == carPartId);
                return carPart != null && carPart.StockQuantity >= quantity;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> IsPartNumberUniqueAsync(string partNumber, int? excludeId = null)
        {
            try
            {
                var query = _context.CarParts.Where(cp => cp.PartNumber == partNumber);
                
                if (excludeId.HasValue)
                {
                    query = query.Where(cp => cp.CarPartId != excludeId.Value);
                }
                
                return !await query.AnyAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        // Inventory management
        public async Task<Dictionary<string, object>> GetInventoryStatusAsync()
        {
            try
            {
                var status = new Dictionary<string, object>();
                
                status["TotalParts"] = await GetTotalCarPartsCountAsync();
                status["InStock"] = await GetAvailableCarPartsCountAsync();
                status["OutOfStock"] = await _context.CarParts.CountAsync(cp => cp.StockQuantity == 0);
                status["LowStock"] = await _context.CarParts.CountAsync(cp => cp.StockQuantity > 0 && cp.StockQuantity <= 10);
                status["TotalInventoryValue"] = await _context.CarParts.SumAsync(cp => cp.Price * cp.StockQuantity);
                
                return status;
            }
            catch (Exception)
            {
                return new Dictionary<string, object>();
            }
        }
        
        public async Task<List<CarPart>> GetExpiringPartsAsync()
        {
            try
            {
                // This is a placeholder - in a real system, you might have expiration dates
                // For now, we'll return parts that haven't been updated in a long time
                var cutoffDate = DateTime.Now.AddMonths(-6);
                
                return await _context.CarParts
                    .Include(cp => cp.Brand)
                    .Include(cp => cp.Category)
                    .Where(cp => cp.UpdatedAt < cutoffDate && cp.IsAvailable)
                    .OrderBy(cp => cp.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CarPart>();
            }
        }
    }
}