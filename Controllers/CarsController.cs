using Microsoft.AspNetCore.Mvc;
using ABCCarTraders.Models;
using ABCCarTraders.Models.ViewModels;
using ABCCarTraders.Services;
using ABCCarTraders.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarService _carService;
        private readonly ILogger<CarsController> _logger;

        public CarsController(ICarService carService, ILogger<CarsController> logger)
        {
            _carService = carService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string sortBy = "name", bool isDescending = false)
        {
            try
            {
                var pageSize = 12;
                var cars = await _carService.GetAllCarsAsync();

                // Apply sorting and convert to List<Car>
                cars = sortBy.ToLower() switch
                {
                    "price" => isDescending ? cars.OrderByDescending(c => c.Price).ToList() : cars.OrderBy(c => c.Price).ToList(),
                    "year" => isDescending ? cars.OrderByDescending(c => c.Year).ToList() : cars.OrderBy(c => c.Year).ToList(),
                    "mileage" => isDescending ? cars.OrderByDescending(c => c.Mileage).ToList() : cars.OrderBy(c => c.Mileage).ToList(),
                    "brand" => isDescending ? cars.OrderByDescending(c => c.Brand.BrandName).ToList() : cars.OrderBy(c => c.Brand.BrandName).ToList(),
                    _ => isDescending ? cars.OrderByDescending(c => c.Model).ToList() : cars.OrderBy(c => c.Model).ToList()
                };

                var totalCars = cars.Count;
                var pagedCars = cars.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new CarListViewModel
                {
                    Cars = pagedCars.Select(CarViewModel.FromCar).ToList(),
                    TotalCars = totalCars,
                    AvailableCars = cars.Count(c => c.IsAvailable),
                    AveragePrice = cars.Any() ? cars.Average(c => c.Price) : 0,
                    MinPrice = cars.Any() ? cars.Min(c => c.Price) : 0,
                    MaxPrice = cars.Any() ? cars.Max(c => c.Price) : 0,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling((double)totalCars / pageSize),
                        TotalItems = totalCars,
                        PageSize = pageSize
                    }
                };

                // Load filter options
                await LoadFilterOptionsAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cars index page");
                return View(new CarListViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var car = await _carService.GetCarByIdAsync(id);
                if (car == null)
                {
                    return NotFound();
                }

                var model = new CarDetailsViewModel
                {
                    Car = CarViewModel.FromCar(car),
                    SimilarCars = (await _carService.GetCarsByBrandAsync(car.BrandId))
                        .Where(c => c.CarId != id).Take(4).Select(CarViewModel.FromCar).ToList(),
                    SameCategoryCars = (await _carService.GetCarsByCategoryAsync(car.CategoryId))
                        .Where(c => c.CarId != id).Take(4).Select(CarViewModel.FromCar).ToList(),
                    CanAddToCart = HttpContext.Session.IsCustomer() && car.IsAvailable && car.StockQuantity > 0,
                    IsUserLoggedIn = HttpContext.Session.IsAuthenticated()
                };

                // Set additional properties
                model.Breadcrumbs = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem("Home", "/"),
                    new BreadcrumbItem("Cars", "/Cars"),
                    new BreadcrumbItem(car.Brand?.BrandName ?? "Unknown", $"/Cars?brand={car.BrandId}"),
                    new BreadcrumbItem($"{car.Model} ({car.Year})", null)
                };

                model.SharingUrl = Url.Action("Details", "Cars", new { id = car.CarId }, Request.Scheme);

                // Add to recently viewed
                if (HttpContext.Session.IsAuthenticated())
                {
                    HttpContext.Session.AddToRecentlyViewed("Car", car.CarId, $"{car.Brand?.BrandName} {car.Model} ({car.Year})");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car details for ID: {CarId}", id);
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("Index");
            }

            try
            {
                var cars = await _carService.SearchCarsAsync(searchTerm);
                var pageSize = 12;
                var totalCars = cars.Count;
                var pagedCars = cars.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new CarSearchResultsViewModel
                {
                    SearchTerm = searchTerm,
                    Cars = pagedCars.Select(CarViewModel.FromCar).ToList(),
                    TotalResults = totalCars,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling((double)totalCars / pageSize),
                        TotalItems = totalCars,
                        PageSize = pageSize
                    }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching cars with term: {SearchTerm}", searchTerm);
                return View(new CarSearchResultsViewModel { SearchTerm = searchTerm });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            try
            {
                var category = await _carService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                var cars = await _carService.GetCarsByCategoryAsync(id);
                var pageSize = 12;
                var totalCars = cars.Count;
                var pagedCars = cars.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new CarCategoryViewModel
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    Cars = pagedCars.Select(CarViewModel.FromCar).ToList(),
                    TotalCarsInCategory = totalCars,
                    AvailableCarsInCategory = cars.Count(c => c.IsAvailable),
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling((double)totalCars / pageSize),
                        TotalItems = totalCars,
                        PageSize = pageSize
                    }
                };

                if (cars.Any())
                {
                    model.MinPrice = cars.Min(c => c.Price);
                    model.MaxPrice = cars.Max(c => c.Price);
                    model.AveragePrice = cars.Average(c => c.Price);
                    model.MinYear = cars.Min(c => c.Year);
                    model.MaxYear = cars.Max(c => c.Year);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category with ID: {CategoryId}", id);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int carId, int quantity = 1)
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return Json(new { success = false, message = "Please login as a customer to add items to cart." });
            }

            try
            {
                var car = await _carService.GetCarByIdAsync(carId);
                if (car == null)
                {
                    return Json(new { success = false, message = "Car not found." });
                }

                if (!car.IsAvailable || car.StockQuantity < quantity)
                {
                    return Json(new { success = false, message = "Car is not available or insufficient stock." });
                }

                // Add to cart logic would go here
                var userId = HttpContext.Session.GetUserId();
                // await _cartService.AddToCartAsync(userId, carId, quantity);

                HttpContext.Session.IncrementCartCount();

                return Json(new { success = true, message = "Car added to cart successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding car to cart. CarId: {CarId}, Quantity: {Quantity}", carId, quantity);
                return Json(new { success = false, message = "An error occurred while adding the car to cart." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int carId)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return Json(new { success = false, message = "Please login to add items to wishlist." });
            }

            try
            {
                var car = await _carService.GetCarByIdAsync(carId);
                if (car == null)
                {
                    return Json(new { success = false, message = "Car not found." });
                }

                // Add to wishlist logic would go here
                var userId = HttpContext.Session.GetUserId();
                // await _wishlistService.AddToWishlistAsync(userId, carId);

                return Json(new { success = true, message = "Car added to wishlist successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding car to wishlist. CarId: {CarId}", carId);
                return Json(new { success = false, message = "An error occurred while adding the car to wishlist." });
            }
        }

        private async Task LoadFilterOptionsAsync(CarListViewModel model)
        {
            try
            {
                // Load brands
                var brands = await _carService.GetAllBrandsAsync();
                model.Brands = brands.Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.BrandName
                }).ToList();

                // Load categories
                var categories = await _carService.GetCarCategoriesAsync();
                model.Categories = categories.Where(c => c.CategoryType == CategoryType.Car)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();

                // Load available years
                var cars = await _carService.GetAllCarsAsync();
                var years = cars.Select(c => c.Year).Distinct().OrderByDescending(y => y);
                model.Years = years.Select(y => new SelectListItem
                {
                    Value = y.ToString(),
                    Text = y.ToString()
                }).ToList();

                // Load fuel types
                model.FuelTypes = Enum.GetValues<FuelType>()
                    .Select(ft => new SelectListItem
                    {
                        Value = ((int)ft).ToString(),
                        Text = ft.ToString()
                    }).ToList();

                // Load transmissions
                model.Transmissions = Enum.GetValues<Transmission>()
                    .Select(t => new SelectListItem
                    {
                        Value = ((int)t).ToString(),
                        Text = t.ToString()
                    }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading filter options");
            }
        }
    }
}