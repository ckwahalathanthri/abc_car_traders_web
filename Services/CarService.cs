using ABCCarTraders.Data;
using ABCCarTraders.Models;
using Microsoft.EntityFrameworkCore;

namespace ABCCarTraders.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Car management methods
        public async Task<List<Car>> GetAllCarsAsync()
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetAvailableCarsAsync()
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<Car?> GetCarByIdAsync(int carId)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .FirstOrDefaultAsync(c => c.CarId == carId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddCarAsync(Car car)
        {
            try
            {
                car.CreatedAt = DateTime.Now;
                car.UpdatedAt = DateTime.Now;

                _context.Cars.Add(car);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCarAsync(Car car)
        {
            try
            {
                var existingCar = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == car.CarId);
                if (existingCar == null)
                {
                    return false;
                }

                existingCar.BrandId = car.BrandId;
                existingCar.CategoryId = car.CategoryId;
                existingCar.Model = car.Model;
                existingCar.Year = car.Year;
                existingCar.Color = car.Color;
                existingCar.Price = car.Price;
                existingCar.Mileage = car.Mileage;
                existingCar.FuelType = car.FuelType;
                existingCar.Transmission = car.Transmission;
                existingCar.EngineCapacity = car.EngineCapacity;
                existingCar.Description = car.Description;
                existingCar.Features = car.Features;
                existingCar.ImageUrl = car.ImageUrl;
                existingCar.StockQuantity = car.StockQuantity;
                existingCar.IsAvailable = car.IsAvailable;
                existingCar.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCarAsync(int carId)
        {
            try
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
                if (car == null)
                {
                    return false;
                }

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ToggleAvailabilityAsync(int carId)
        {
            try
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
                if (car == null)
                {
                    return false;
                }

                car.IsAvailable = !car.IsAvailable;
                car.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Search and filter methods
        public async Task<List<Car>> SearchCarsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAvailableCarsAsync();
                }

                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.IsAvailable && c.StockQuantity > 0 &&
                               (c.Model.Contains(searchTerm) ||
                                c.Brand.BrandName.Contains(searchTerm) ||
                                c.Category.CategoryName.Contains(searchTerm) ||
                                c.Color.Contains(searchTerm) ||
                                c.Description.Contains(searchTerm)))
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetCarsByBrandAsync(int brandId)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.BrandId == brandId && c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetCarsByCategoryAsync(int categoryId)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == categoryId && c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetCarsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.Price >= minPrice && c.Price <= maxPrice && c.IsAvailable && c.StockQuantity > 0)
                    .OrderBy(c => c.Price)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetCarsByYearRangeAsync(int minYear, int maxYear)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.Year >= minYear && c.Year <= maxYear && c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.Year)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetCarsByFuelTypeAsync(FuelType fuelType)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.FuelType == fuelType && c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetCarsByTransmissionAsync(Transmission transmission)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.Transmission == transmission && c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetCarsByColorAsync(string color)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.Color == color && c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        // Advanced filtering
        public async Task<List<Car>> GetFilteredCarsAsync(CarFilterModel filter)
        {
            try
            {
                var query = _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(c => c.Model.Contains(filter.SearchTerm) ||
                                           c.Brand.BrandName.Contains(filter.SearchTerm) ||
                                           c.Category.CategoryName.Contains(filter.SearchTerm));
                }

                if (filter.BrandId.HasValue)
                {
                    query = query.Where(c => c.BrandId == filter.BrandId.Value);
                }

                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(c => c.CategoryId == filter.CategoryId.Value);
                }

                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(c => c.Price >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(c => c.Price <= filter.MaxPrice.Value);
                }

                if (filter.MinYear.HasValue)
                {
                    query = query.Where(c => c.Year >= filter.MinYear.Value);
                }

                if (filter.MaxYear.HasValue)
                {
                    query = query.Where(c => c.Year <= filter.MaxYear.Value);
                }

                if (filter.FuelType.HasValue)
                {
                    query = query.Where(c => c.FuelType == filter.FuelType.Value);
                }

                if (filter.Transmission.HasValue)
                {
                    query = query.Where(c => c.Transmission == filter.Transmission.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.Color))
                {
                    query = query.Where(c => c.Color == filter.Color);
                }

                if (filter.IsAvailable.HasValue)
                {
                    query = query.Where(c => c.IsAvailable == filter.IsAvailable.Value);
                }

                // Apply sorting
                query = filter.SortBy switch
                {
                    "price" => filter.IsDescending ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price),
                    "year" => filter.IsDescending ? query.OrderByDescending(c => c.Year) : query.OrderBy(c => c.Year),
                    "model" => filter.IsDescending ? query.OrderByDescending(c => c.Model) : query.OrderBy(c => c.Model),
                    "brand" => filter.IsDescending ? query.OrderByDescending(c => c.Brand.BrandName) : query.OrderBy(c => c.Brand.BrandName),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };

                return await query.ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetFeaturedCarsAsync(int count = 6)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.Price)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetLatestCarsAsync(int count = 10)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        public async Task<List<Car>> GetPopularCarsAsync(int count = 10)
        {
            try
            {
                // For now, we'll return cars with highest stock as popular
                // In a real app, this would be based on sales data
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.IsAvailable && c.StockQuantity > 0)
                    .OrderByDescending(c => c.StockQuantity)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        // Stock management
        public async Task<bool> UpdateStockAsync(int carId, int quantity)
        {
            try
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
                if (car == null)
                {
                    return false;
                }

                car.StockQuantity = quantity;
                car.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DecreaseStockAsync(int carId, int quantity)
        {
            try
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
                if (car == null || car.StockQuantity < quantity)
                {
                    return false;
                }

                car.StockQuantity -= quantity;
                car.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IncreaseStockAsync(int carId, int quantity)
        {
            try
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
                if (car == null)
                {
                    return false;
                }

                car.StockQuantity += quantity;
                car.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Car>> GetLowStockCarsAsync(int threshold = 3)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.StockQuantity <= threshold && c.IsAvailable)
                    .OrderBy(c => c.StockQuantity)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
            }
        }

        // Category management
        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                return await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.CategoryType == CategoryType.Car);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Statistics and reporting
        public async Task<int> GetTotalCarsCountAsync()
        {
            try
            {
                return await _context.Cars.CountAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> GetAvailableCarsCountAsync()
        {
            try
            {
                return await _context.Cars.CountAsync(c => c.IsAvailable && c.StockQuantity > 0);
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
                return await _context.Cars
                    .Where(c => c.IsAvailable)
                    .AverageAsync(c => c.Price);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<Dictionary<string, int>> GetCarStatisticsAsync()
        {
            try
            {
                var stats = new Dictionary<string, int>();

                stats["TotalCars"] = await GetTotalCarsCountAsync();
                stats["AvailableCars"] = await GetAvailableCarsCountAsync();
                stats["UnavailableCars"] = stats["TotalCars"] - stats["AvailableCars"];
                stats["LowStockCars"] = await _context.Cars.CountAsync(c => c.StockQuantity <= 3);
                stats["NewCarsThisMonth"] = await _context.Cars.CountAsync(c => c.CreatedAt.Month == DateTime.Now.Month);

                return stats;
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }

        public async Task<List<Car>> GetTopSellingCarsAsync(int count = 5)
        {
            try
            {
                // This would be based on order items in a real application
                return await _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Category)
                    .Where(c => c.IsAvailable)
                    .OrderByDescending(c => c.Price)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Car>();
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

        public async Task<List<Category>> GetCarCategoriesAsync()
        {
            try
            {
                return await _context.Categories
                    .Where(c => c.CategoryType == CategoryType.Car && c.IsActive)
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Category>();
            }
        }

        public async Task<List<string>> GetAvailableColorsAsync()
        {
            try
            {
                return await _context.Cars
                    .Where(c => c.IsAvailable && !string.IsNullOrEmpty(c.Color))
                    .Select(c => c.Color)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            try
            {
                return await _context.Cars
                    .Where(c => c.IsAvailable)
                    .Select(c => c.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<int>();
            }
        }

        // Price calculations
        public async Task<decimal> GetMinPriceAsync()
        {
            try
            {
                return await _context.Cars
                    .Where(c => c.IsAvailable)
                    .MinAsync(c => c.Price);
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
                return await _context.Cars
                    .Where(c => c.IsAvailable)
                    .MaxAsync(c => c.Price);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // Validation
        public async Task<bool> IsCarAvailableAsync(int carId)
        {
            try
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
                return car != null && car.IsAvailable && car.StockQuantity > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> HasSufficientStockAsync(int carId, int quantity)
        {
            try
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
                return car != null && car.StockQuantity >= quantity;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}