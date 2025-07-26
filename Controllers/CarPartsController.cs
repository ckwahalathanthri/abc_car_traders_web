using Microsoft.AspNetCore.Mvc;
using ABCCarTraders.Models;
using ABCCarTraders.Models.ViewModels;
using ABCCarTraders.Services;
using ABCCarTraders.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Controllers
{
    public class CarPartsController : Controller
    {
        private readonly ICarPartService _carPartService;
        private readonly ILogger<CarPartsController> _logger;
        
        public CarPartsController(ICarPartService carPartService, ILogger<CarPartsController> logger)
        {
            _carPartService = carPartService;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(CarPartSearchViewModel? searchModel = null, int page = 1)
        {
            try
            {
                var filter = new CarPartFilterModel();
                var pageSize = 12;
                
                if (searchModel != null)
                {
                    filter.SearchTerm = searchModel.SearchTerm;
                    filter.BrandId = searchModel.BrandId;
                    filter.CategoryId = searchModel.CategoryId;
                    filter.MinPrice = searchModel.MinPrice;
                    filter.MaxPrice = searchModel.MaxPrice;
                    filter.Compatibility = searchModel.Compatibility;
                    filter.IsAvailable = true;
                    filter.InStock = searchModel.InStock;
                    filter.SortBy = searchModel.SortBy;
                    filter.IsDescending = searchModel.IsDescending;
                    pageSize = searchModel.PageSize;
                }
                else
                {
                    filter.IsAvailable = true;
                    searchModel = new CarPartSearchViewModel();
                }
                
                var carParts = await _carPartService.GetFilteredCarPartsAsync(filter);
                var totalParts = carParts.Count;
                var pagedParts = carParts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                
                var model = new CarPartListViewModel
                {
                    CarParts = pagedParts.Select(CarPartViewModel.FromCarPart).ToList(),
                    SearchModel = searchModel,
                    TotalCarParts = totalParts,
                    AvailableCarParts = carParts.Count(cp => cp.IsAvailable),
                    OutOfStockCarParts = carParts.Count(cp => cp.StockQuantity == 0),
                    AveragePrice = carParts.Any() ? carParts.Average(cp => cp.Price) : 0,
                    MinPrice = carParts.Any() ? carParts.Min(cp => cp.Price) : 0,
                    MaxPrice = carParts.Any() ? carParts.Max(cp => cp.Price) : 0,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling((double)totalParts / pageSize),
                        TotalItems = totalParts,
                        PageSize = pageSize
                    }
                };
                
                // Load filter options
                await LoadFilterOptionsAsync(model);
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car parts index page");
                return View(new CarPartListViewModel());
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var carPart = await _carPartService.GetCarPartByIdAsync(id);
                if (carPart == null)
                {
                    return NotFound();
                }
                
                var model = new CarPartDetailsViewModel
                {
                    CarPart = CarPartViewModel.FromCarPart(carPart),
                    SimilarCarParts = await _carPartService.GetCarPartsByBrandAsync(carPart.BrandId).ContinueWith(t => 
                        t.Result.Where(cp => cp.CarPartId != id).Take(4).Select(CarPartViewModel.FromCarPart).ToList()),
                    SameCategoryCarParts = await _carPartService.GetCarPartsByCategoryAsync(carPart.CategoryId).ContinueWith(t => 
                        t.Result.Where(cp => cp.CarPartId != id).Take(4).Select(CarPartViewModel.FromCarPart).ToList()),
                    CompatibleCarParts = !string.IsNullOrEmpty(carPart.Compatibility) 
                        ? await _carPartService.GetCarPartsByCompatibilityAsync(carPart.Compatibility).ContinueWith(t => 
                            t.Result.Where(cp => cp.CarPartId != id).Take(4).Select(CarPartViewModel.FromCarPart).ToList())
                        : new List<CarPartViewModel>(),
                    CanAddToCart = HttpContext.Session.IsCustomer() && carPart.IsAvailable && carPart.StockQuantity > 0,
                    IsUserLoggedIn = HttpContext.Session.IsAuthenticated()
                };
                
                // Add to recently viewed
                if (HttpContext.Session.IsAuthenticated())
                {
                    HttpContext.Session.AddToRecentlyViewed("CarPart", carPart.CarPartId, $"{carPart.Brand?.BrandName} {carPart.PartName}");
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car part details for ID: {CarPartId}", id);
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
                var carParts = await _carPartService.SearchCarPartsAsync(searchTerm);
                var pageSize = 12;
                var totalParts = carParts.Count;
                var pagedParts = carParts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                
                var model = new CarPartSearchResultsViewModel
                {
                    SearchTerm = searchTerm,
                    CarParts = pagedParts.Select(CarPartViewModel.FromCarPart).ToList(),
                    TotalResults = totalParts,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling((double)totalParts / pageSize),
                        TotalItems = totalParts,
                        PageSize = pageSize
                    }
                };
                
                // Add to search history
                if (HttpContext.Session.IsAuthenticated())
                {
                    HttpContext.Session.AddToSearchHistory(searchTerm);
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching car parts with term: {SearchTerm}", searchTerm);
                return View(new CarPartSearchResultsViewModel { SearchTerm = searchTerm });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            try
            {
                var carParts = await _carPartService.GetCarPartsByCategoryAsync(id);
                var pageSize = 12;
                var totalParts = carParts.Count;
                var pagedParts = carParts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                
                var model = new CarPartCategoryViewModel
                {
                    CategoryId = id,
                    CarParts = pagedParts.Select(CarPartViewModel.FromCarPart).ToList(),
                    TotalCarParts = totalParts,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling((double)totalParts / pageSize),
                        TotalItems = totalParts,
                        PageSize = pageSize
                    }
                };
                
                // Get category name
                var categories = await _carPartService.GetCarPartCategoriesAsync();
                var category = categories.FirstOrDefault(c => c.CategoryId == id);
                model.CategoryName = category?.CategoryName ?? "Unknown Category";
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car parts by category: {CategoryId}", id);
                return View(new CarPartCategoryViewModel());
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Brand(int id, int page = 1)
        {
            try
            {
                var carParts = await _carPartService.GetCarPartsByBrandAsync(id);
                var pageSize = 12;
                var totalParts = carParts.Count;
                var pagedParts = carParts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                
                var model = new CarPartBrandViewModel
                {
                    BrandId = id,
                    CarParts = pagedParts.Select(CarPartViewModel.FromCarPart).ToList(),
                    TotalCarParts = totalParts,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling((double)totalParts / pageSize),
                        TotalItems = totalParts,
                        PageSize = pageSize
                    }
                };
                
                // Get brand name
                var brands = await _carPartService.GetAllBrandsAsync();
                var brand = brands.FirstOrDefault(b => b.BrandId == id);
                model.BrandName = brand?.BrandName ?? "Unknown Brand";
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car parts by brand: {BrandId}", id);
                return View(new CarPartBrandViewModel());
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Compare(string ids)
        {
            try
            {
                var partIds = ids.Split(',').Select(int.Parse).ToList();
                if (partIds.Count < 2 || partIds.Count > 4)
                {
                    return BadRequest("Please select 2-4 car parts to compare");
                }
                
                var carParts = new List<CarPart>();
                foreach (var partId in partIds)
                {
                    var carPart = await _carPartService.GetCarPartByIdAsync(partId);
                    if (carPart != null)
                    {
                        carParts.Add(carPart);
                    }
                }
                
                var model = new CarPartComparisonViewModel
                {
                    CarParts = carParts.Select(CarPartViewModel.FromCarPart).ToList()
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car part comparison for IDs: {CarPartIds}", ids);
                return BadRequest("Invalid car part IDs");
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Compatibility(string compatibility, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(compatibility))
            {
                return RedirectToAction("Index");
            }
            
            try
            {
                var carParts = await _carPartService.GetCarPartsByCompatibilityAsync(compatibility);
                var pageSize = 12;
                var totalParts = carParts.Count;
                var pagedParts = carParts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                
                var model = new CarPartCompatibilityViewModel
                {
                    Compatibility = compatibility,
                    CarParts = pagedParts.Select(CarPartViewModel.FromCarPart).ToList(),
                    TotalCarParts = totalParts,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling((double)totalParts / pageSize),
                        TotalItems = totalParts,
                        PageSize = pageSize
                    }
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car parts by compatibility: {Compatibility}", compatibility);
                return View(new CarPartCompatibilityViewModel());
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCarPartsByBrand(int brandId)
        {
            try
            {
                var carParts = await _carPartService.GetCarPartsByBrandAsync(brandId);
                var result = carParts.Select(cp => new
                {
                    id = cp.CarPartId,
                    text = $"{cp.PartName} ({cp.PartNumber}) - {cp.Price:C}"
                }).ToList();
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting car parts by brand: {BrandId}", brandId);
                return Json(new List<object>());
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCarPartsByCategory(int categoryId)
        {
            try
            {
                var carParts = await _carPartService.GetCarPartsByCategoryAsync(categoryId);
                var result = carParts.Select(cp => new
                {
                    id = cp.CarPartId,
                    text = $"{cp.Brand?.BrandName} {cp.PartName} ({cp.PartNumber}) - {cp.Price:C}"
                }).ToList();
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting car parts by category: {CategoryId}", categoryId);
                return Json(new List<object>());
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPriceRange()
        {
            try
            {
                var minPrice = await _carPartService.GetMinPriceAsync();
                var maxPrice = await _carPartService.GetMaxPriceAsync();
                
                return Json(new { minPrice, maxPrice });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting car parts price range");
                return Json(new { minPrice = 0, maxPrice = 10000 });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCompatibilities()
        {
            try
            {
                var compatibilities = await _carPartService.GetAvailableCompatibilitiesAsync();
                var result = compatibilities.Select(c => new
                {
                    value = c,
                    text = c
                }).ToList();
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting compatibilities");
                return Json(new List<object>());
            }
        }
        
        // Helper methods
        private async Task LoadFilterOptionsAsync(CarPartListViewModel model)
        {
            try
            {
                var brands = await _carPartService.GetAllBrandsAsync();
                model.Brands = brands.Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.BrandName
                }).ToList();
                
                var categories = await _carPartService.GetCarPartCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
                
                var compatibilities = await _carPartService.GetAvailableCompatibilitiesAsync();
                model.Compatibilities = compatibilities.Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car part filter options");
            }
        }
    }
    
    // Supporting ViewModels for CarParts
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
        public List<SelectListItem> Compatibilities { get; set; } = new List<SelectListItem>();
        
        // Form state
        public bool IsEditMode { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        
        // File upload
        public IFormFile? ImageFile { get; set; }
        
        // Computed properties
        public string FormattedPrice => Price.ToString("C");
        public string StockStatus => StockQuantity > 0 ? "In Stock" : "Out of Stock";
        public string AvailabilityStatus => IsAvailable ? "Available" : "Unavailable";
        public bool InStock => StockQuantity > 0;
        public bool IsLowStock => StockQuantity > 0 && StockQuantity <= 10;
        
        // Convert to CarPart entity
        public CarPart ToCarPart()
        {
            return new CarPart
            {
                CarPartId = CarPartId,
                BrandId = BrandId,
                CategoryId = CategoryId,
                PartName = PartName,
                PartNumber = PartNumber,
                Price = Price,
                Description = Description,
                Compatibility = Compatibility,
                ImageUrl = ImageUrl,
                StockQuantity = StockQuantity,
                IsAvailable = IsAvailable
            };
        }
        
        // Create from CarPart entity
        public static CarPartViewModel FromCarPart(CarPart carPart)
        {
            return new CarPartViewModel
            {
                CarPartId = carPart.CarPartId,
                BrandId = carPart.BrandId,
                CategoryId = carPart.CategoryId,
                PartName = carPart.PartName,
                PartNumber = carPart.PartNumber,
                Price = carPart.Price,
                Description = carPart.Description,
                Compatibility = carPart.Compatibility,
                ImageUrl = carPart.ImageUrl,
                StockQuantity = carPart.StockQuantity,
                IsAvailable = carPart.IsAvailable,
                BrandName = carPart.Brand?.BrandName ?? "",
                CategoryName = carPart.Category?.CategoryName ?? "",
                IsEditMode = true
            };
        }
    }
    
    // Car part list view model
    public class CarPartListViewModel
    {
        public List<CarPartViewModel> CarParts { get; set; } = new List<CarPartViewModel>();
        public CarPartSearchViewModel SearchModel { get; set; } = new CarPartSearchViewModel();
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
        
        // Filter options
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Compatibilities { get; set; } = new List<SelectListItem>();
        
        // Statistics
        public int TotalCarParts { get; set; }
        public int AvailableCarParts { get; set; }
        public int OutOfStockCarParts { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
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
        
        [Display(Name = "Sort By")]
        public string? SortBy { get; set; }
        
        [Display(Name = "Order")]
        public bool IsDescending { get; set; } = false;
        
        // Results per page
        public int PageSize { get; set; } = 12;
        public int PageNumber { get; set; } = 1;
    }
    
    public class CarPartDetailsViewModel
    {
        public CarPartViewModel CarPart { get; set; } = new CarPartViewModel();
        public List<CarPartViewModel> SimilarCarParts { get; set; } = new List<CarPartViewModel>();
        public List<CarPartViewModel> SameCategoryCarParts { get; set; } = new List<CarPartViewModel>();
        public List<CarPartViewModel> CompatibleCarParts { get; set; } = new List<CarPartViewModel>();
        public bool CanAddToCart { get; set; } = false;
        public bool IsUserLoggedIn { get; set; } = false;
        public int Quantity { get; set; } = 1;
        public string? AddToCartMessage { get; set; }
        
        // Computed properties
        public bool IsLowStock => CarPart.StockQuantity <= 10 && CarPart.StockQuantity > 0;
        public bool IsOutOfStock => CarPart.StockQuantity == 0;
        public string StockMessage => IsOutOfStock ? "Out of Stock" : 
                                    IsLowStock ? $"Only {CarPart.StockQuantity} left in stock" : 
                                    "In Stock";
        public string StockStatusColor => IsOutOfStock ? "danger" : IsLowStock ? "warning" : "success";
        
        // Compatibility list
        public List<string> CompatibilityList => 
            !string.IsNullOrEmpty(CarPart.Compatibility) 
                ? CarPart.Compatibility.Split(',').Select(c => c.Trim()).ToList()
                : new List<string>();
    }
    
    public class CarPartSearchResultsViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public List<CarPartViewModel> CarParts { get; set; } = new List<CarPartViewModel>();
        public int TotalResults { get; set; }
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
    
    public class CarPartCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<CarPartViewModel> CarParts { get; set; } = new List<CarPartViewModel>();
        public int TotalCarParts { get; set; }
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
    
    public class CarPartBrandViewModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public List<CarPartViewModel> CarParts { get; set; } = new List<CarPartViewModel>();
        public int TotalCarParts { get; set; }
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
    
    public class CarPartCompatibilityViewModel
    {
        public string Compatibility { get; set; } = string.Empty;
        public List<CarPartViewModel> CarParts { get; set; } = new List<CarPartViewModel>();
        public int TotalCarParts { get; set; }
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
    
    public class CarPartComparisonViewModel
    {
        public List<CarPartViewModel> CarParts { get; set; } = new List<CarPartViewModel>();
        
        public bool HasCarParts => CarParts.Any();
        public int ComparisonCount => CarParts.Count;
        public List<string> ComparisonFeatures => new List<string>
        {
            "Price", "Part Number", "Brand", "Category", "Stock Quantity", 
            "Compatibility", "Description"
        };
    }
}