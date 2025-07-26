using ABCCarTraders.Helpers;
using ABCCarTraders.Models;
using ABCCarTraders.Models.ViewModels;
using ABCCarTraders.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using ModelOrderStatus = ABCCarTraders.Models.OrderStatus;
using ModelPaymentStatus = ABCCarTraders.Models.PaymentStatus;

namespace ABCCarTraders.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICarService _carService;
        private readonly ICarPartService _carPartService;
        private readonly IOrderService _orderService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IUserService userService,
            ICarService carService,
            ICarPartService carPartService,
            IOrderService orderService,
            ILogger<AdminController> logger)
        {
            _userService = userService;
            _carService = carService;
            _carPartService = carPartService;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            try
            {
                var model = new DashboardViewModel
                {
                    UserType = UserType.Admin,
                    UserName = HttpContext.Session.GetUserName() ?? "Admin",
                    LastLoginDate = HttpContext.Session.GetLastLogin() ?? DateTime.Now,
                    AdminData = new AdminDashboardData
                    {
                        TotalCars = await _carService.GetTotalCarsCountAsync(),
                        TotalCarParts = await _carPartService.GetTotalCarPartsCountAsync(),
                        TotalCustomers = await _userService.GetTotalCustomersCountAsync(),
                        TotalOrders = await _orderService.GetTotalOrdersCountAsync(),
                        PendingOrders = await _orderService.GetPendingOrdersCountAsync(),
                        CompletedOrders = await _orderService.GetCompletedOrdersCountAsync(),
                        TotalRevenue = await _orderService.GetTotalRevenueAsync(),
                        MonthlyRevenue = await _orderService.GetMonthlyRevenueAsync(),
                        RecentOrders = await _orderService.GetRecentOrdersAsync(5),
                        RecentCustomers = await _userService.GetRecentCustomersAsync(5),
                        LowStockCars = await _carService.GetLowStockCarsAsync(),
                        LowStockCarParts = await _carPartService.GetLowStockCarPartsAsync()
                    }
                };

                // Get statistics
                model.Statistics = await _orderService.GetOrderStatisticsAsync();
                model.AdminData.NewCustomersThisMonth = await _userService.GetUserStatisticsAsync().ContinueWith(t => t.Result.GetValueOrDefault("NewCustomersThisMonth", 0));

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                return View(new DashboardViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageCars(int page = 1, int pageSize = 10)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            try
            {
                var cars = await _carService.GetAllCarsAsync();
                var totalCars = cars.Count;
                var pagedCars = cars.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new CarListViewModel
                {
                    Cars = pagedCars.Select(CarViewModel.FromCar).ToList(),
                    TotalCars = totalCars,
                    AvailableCars = cars.Count(c => c.IsAvailable),
                    AveragePrice = cars.Any() ? cars.Average(c => c.Price) : 0,
                    Pagination = new PaginationViewModel(page, totalCars, pageSize, "Admin", "ManageCars")
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cars management");
                return View(new CarListViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddCar()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var model = new CarViewModel();
                await LoadCarDropdownsAsync(model);

                _logger.LogInformation("Brands loaded: {BrandCount}, Categories loaded: {CategoryCount}",
                                      model.Brands?.Count ?? 0, model.Categories?.Count ?? 0);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading AddCar page");
                TempData["ErrorMessage"] = "Error loading the form. Please try again.";
                return RedirectToAction("ManageCars");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCar([FromBody] CarViewModel model)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                // Log validation errors for debugging
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("AddCar ModelState invalid. Errors: {Errors}", string.Join(", ", errors));
                    return Json(new { success = false, message = "Validation failed", errors = errors });
                }

                var car = model.ToCar();

                // Set audit fields
                car.CreatedAt = DateTime.Now;
                car.UpdatedAt = DateTime.Now;

                var result = await _carService.AddCarAsync(car);

                if (result)
                {
                    return Json(new { success = true, message = "Car added successfully." });
                }
                else
                {
                    _logger.LogWarning("CarService.AddCarAsync returned false for car: {Model}", model.Model);
                    return Json(new { success = false, message = "Failed to add car. Please check the data and try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding car: {Model}", model?.Model ?? "Unknown");
                return Json(new { success = false, message = "An unexpected error occurred while adding the car." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCar(int id)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            try
            {
                var car = await _carService.GetCarByIdAsync(id);
                if (car == null)
                {
                    HttpContext.Session.SetErrorMessage("Car not found.");
                    return RedirectToAction("ManageCars");
                }

                var model = CarViewModel.FromCar(car);
                await LoadCarDropdownsAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car for editing: {CarId}", id);
                return RedirectToAction("ManageCars");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar([FromBody] CarViewModel model)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("EditCar ModelState invalid. Errors: {Errors}", string.Join(", ", errors));
                    return Json(new { success = false, message = "Validation failed", errors = errors });
                }

                var car = model.ToCar();
                car.UpdatedAt = DateTime.Now;

                var result = await _carService.UpdateCarAsync(car);

                if (result)
                {
                    return Json(new { success = true, message = "Car updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update car. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car: {CarId}", model.CarId);
                return Json(new { success = false, message = "An error occurred while updating the car." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCar(int id)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var result = await _carService.DeleteCarAsync(id);

                if (result)
                {
                    return Json(new { success = true, message = "Car deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete car." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting car: {CarId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the car." });
            }
        }

        // Helper method to get dropdown data for AJAX calls
        [HttpGet]
        public async Task<IActionResult> GetCarDropdownData()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var brands = await _carService.GetAllBrandsAsync();
                var categories = await _carService.GetCarCategoriesAsync();

                var brandOptions = brands.Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.BrandName
                }).ToList();

                var categoryOptions = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();

                return Json(new
                {
                    success = true,
                    brands = brandOptions,
                    categories = categoryOptions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dropdown data");
                return Json(new { success = false, message = "Error loading dropdown data" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageCarParts(int page = 1, int pageSize = 10)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            try
            {
                var carParts = await _carPartService.GetAllCarPartsAsync();
                var totalParts = carParts.Count;
                var pagedParts = carParts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new CarPartListViewModel
                {
                    CarParts = pagedParts.Select(CarPartViewModel.FromCarPart).ToList(),
                    TotalCarParts = totalParts,
                    AvailableCarParts = carParts.Count(cp => cp.IsAvailable),
                    OutOfStockCarParts = carParts.Count(cp => cp.StockQuantity == 0),
                    AveragePrice = carParts.Any() ? carParts.Average(cp => cp.Price) : 0,
                    Pagination = new PaginationViewModel(page, totalParts, pageSize, "Admin", "ManageCarParts")
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car parts management");
                return View(new CarPartListViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddCarPart()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            try
            {
                var model = new ABCCarTraders.Models.ViewModels.CarPartViewModel();
                await LoadCarPartDropdownsAsync(model);

                _logger.LogInformation("Brands loaded: {BrandCount}, Categories loaded: {CategoryCount}",
                                      model.Brands?.Count ?? 0, model.Categories?.Count ?? 0);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading AddCarPart page");
                TempData["ErrorMessage"] = "Error loading the form. Please try again.";
                return RedirectToAction("ManageCarParts");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCarPart([FromBody] ABCCarTraders.Models.ViewModels.CarPartViewModel model)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                // Log validation errors for debugging
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("AddCarPart ModelState invalid. Errors: {Errors}", string.Join(", ", errors));
                    return Json(new { success = false, message = "Validation failed", errors = errors });
                }

                var carPart = model.ToCarPart();

                // Set audit fields
                carPart.CreatedAt = DateTime.Now;
                carPart.UpdatedAt = DateTime.Now;

                var result = await _carPartService.AddCarPartAsync(carPart);

                if (result)
                {
                    return Json(new { success = true, message = "Car part added successfully." });
                }
                else
                {
                    _logger.LogWarning("CarPartService.AddCarPartAsync returned false for part: {PartName}", model.PartName);
                    return Json(new { success = false, message = "Failed to add car part. Part number may already exist." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding car part: {PartName}", model?.PartName ?? "Unknown");
                return Json(new { success = false, message = "An unexpected error occurred while adding the car part." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCarPart(int id)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            try
            {
                var carPart = await _carPartService.GetCarPartByIdAsync(id);
                if (carPart == null)
                {
                    HttpContext.Session.SetErrorMessage("Car part not found.");
                    return RedirectToAction("ManageCarParts");
                }

                var model = ABCCarTraders.Models.ViewModels.CarPartViewModel.FromCarPart(carPart);
                await LoadCarPartDropdownsAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car part for editing: {CarPartId}", id);
                return RedirectToAction("ManageCarParts");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCarPart([FromBody] ABCCarTraders.Models.ViewModels.CarPartViewModel model)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("EditCarPart ModelState invalid. Errors: {Errors}", string.Join(", ", errors));
                    return Json(new { success = false, message = "Validation failed", errors = errors });
                }

                var carPart = model.ToCarPart();
                carPart.UpdatedAt = DateTime.Now;

                var result = await _carPartService.UpdateCarPartAsync(carPart);

                if (result)
                {
                    return Json(new { success = true, message = "Car part updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update car part. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car part: {CarPartId}", model.CarPartId);
                return Json(new { success = false, message = "An error occurred while updating the car part." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCarPart(int id)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var result = await _carPartService.DeleteCarPartAsync(id);

                if (result)
                {
                    return Json(new { success = true, message = "Car part deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete car part." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting car part: {CarPartId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the car part." });
            }
        }

        // Helper method to get dropdown data for AJAX calls
        [HttpGet]
        public async Task<IActionResult> GetCarPartDropdownData()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var brands = await _carPartService.GetAllBrandsAsync();
                var categories = await _carPartService.GetCarPartCategoriesAsync();

                var brandOptions = brands.Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.BrandName
                }).ToList();

                var categoryOptions = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();

                return Json(new
                {
                    success = true,
                    brands = brandOptions,
                    categories = categoryOptions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car part dropdown data");
                return Json(new { success = false, message = "Error loading dropdown data" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageCustomers(int page = 1, int pageSize = 10)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            try
            {
                var customers = await _userService.GetAllCustomersAsync();
                var totalCustomers = customers.Count;
                var pagedCustomers = customers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new CustomerListViewModel
                {
                    Customers = pagedCustomers,
                    TotalCustomers = totalCustomers,
                    ActiveCustomers = customers.Count(c => c.IsActive),
                    Pagination = new PaginationViewModel(page, totalCustomers, pageSize, "Admin", "ManageCustomers")
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customers management");
                return View(new CustomerListViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCustomerStatus(int id)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Customer not found." });
                }

                var result = user.IsActive
                    ? await _userService.DeactivateUserAsync(id)
                    : await _userService.ActivateUserAsync(id);

                if (result)
                {
                    var status = user.IsActive ? "deactivated" : "activated";
                    return Json(new { success = true, message = $"Customer {status} successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update customer status." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling customer status: {CustomerId}", id);
                return Json(new { success = false, message = "An error occurred while updating customer status." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageOrders(int page = 1, int pageSize = 10)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                var totalOrders = orders.Count;
                var pagedOrders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new OrderListViewModel
                {
                    Orders = pagedOrders.Select(OrderViewModel.FromOrder).ToList(),
                    TotalOrders = totalOrders,
                    PendingOrders = orders.Count(o => o.OrderStatus == ModelOrderStatus.Pending),
                    CompletedOrders = orders.Count(o => o.OrderStatus == ModelOrderStatus.Delivered),
                    TotalRevenue = orders.Where(o => o.PaymentStatus == ModelPaymentStatus.Paid).Sum(o => o.TotalAmount),
                    Pagination = new PaginationViewModel(page, totalOrders, pageSize, "Admin", "ManageOrders")
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders management");
                return View(new OrderListViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, ModelOrderStatus status)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(orderId, status);

                if (result)
                {
                    // Send notification email to customer
                    var order = await _orderService.GetOrderByIdAsync(orderId);
                    if (order != null)
                    {
                        await EmailHelper.SendOrderStatusUpdateEmailAsync(order, order.User);
                    }

                    return Json(new { success = true, message = "Order status updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update order status." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status: {OrderId}", orderId);
                return Json(new { success = false, message = "An error occurred while updating order status." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    HttpContext.Session.SetErrorMessage("Order not found.");
                    return RedirectToAction("ManageOrders");
                }

                var model = OrderViewModel.FromOrder(order);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order details: {OrderId}", id);
                return RedirectToAction("ManageOrders");
            }
        }

        // Helper methods
        private async Task LoadCarDropdownsAsync(CarViewModel model)
        {
            try
            {
                var brands = await _carService.GetAllBrandsAsync();
                model.Brands = brands.Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.BrandName
                }).ToList();

                var categories = await _carService.GetCarCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car dropdowns");
            }
        }

        private async Task LoadCarPartDropdownsAsync(ABCCarTraders.Models.ViewModels.CarPartViewModel model)
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading car part dropdowns");
            }
        }
    }

    // Supporting ViewModels
    public class CustomerListViewModel
    {
        public List<User> Customers { get; set; } = new List<User>();
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}