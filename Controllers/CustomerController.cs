using Microsoft.AspNetCore.Mvc;
using ABCCarTraders.Models;
using ABCCarTraders.Models.ViewModels;
using ABCCarTraders.Services;
using ABCCarTraders.Helpers;
using ABCCarTraders.Models; // Added for explicit OrderStatus reference

namespace ABCCarTraders.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IUserService userService, IOrderService orderService, ILogger<CustomerController> logger)
        {
            _userService = userService;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var recentOrders = await _orderService.GetCustomerRecentOrdersAsync(userId.Value, 5);
                var totalOrders = await _orderService.GetCustomerOrderCountAsync(userId.Value);
                var totalSpent = await _orderService.GetCustomerTotalSpentAsync(userId.Value);

                var model = new DashboardViewModel
                {
                    UserType = UserType.Customer,
                    UserName = $"{user.FirstName} {user.LastName}",
                    LastLoginDate = HttpContext.Session.GetLastLogin() ?? DateTime.Now,
                    CustomerData = new CustomerDashboardData
                    {
                        TotalOrders = totalOrders,
                        TotalSpent = totalSpent,
                        RecentOrderSummaries = recentOrders?.Select(o => new OrderSummary
                        {
                            OrderId = o.OrderId,
                            OrderNumber = o.OrderNumber,
                            OrderDate = o.OrderDate,
                            Status = (Models.ViewModels.OrderStatus)o.OrderStatus, // Updated to OrderStatus
                            Total = o.TotalAmount // Updated to TotalAmount
                        }).ToList() ?? new List<OrderSummary>(),
                        MemberSince = user.CreatedAt
                    }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer dashboard for user: {UserId}", userId);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory(int page = 1)
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var pageSize = 10;
                var orders = await _orderService.GetCustomerOrdersAsync(userId.Value, page, pageSize);
                var totalOrders = await _orderService.GetCustomerOrderCountAsync(userId.Value);
                var totalSpent = await _orderService.GetCustomerTotalSpentAsync(userId.Value);

                var model = new CustomerOrderHistoryViewModel
                {
                    Orders = orders?.Select(o => new OrderViewModel
                    {
                        OrderId = o.OrderId,
                        OrderNumber = o.OrderNumber,
                        OrderDate = o.OrderDate,
                        Status = (Models.ViewModels.OrderStatus)o.OrderStatus, // Updated to OrderStatus
                        Total = o.TotalAmount, // Updated to TotalAmount
                        ShippingAddress = o.ShippingAddress,
                        ItemCount = o.OrderItems?.Count ?? 0
                    }).ToList() ?? new List<OrderViewModel>(),
                    TotalOrders = totalOrders,
                    TotalSpent = totalSpent,
                    AverageOrderValue = totalOrders > 0 ? totalSpent / totalOrders : 0,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize),
                        TotalItems = totalOrders,
                        PageSize = pageSize,
                        Controller = "Customer",
                        Action = "OrderHistory"
                    }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order history for user: {UserId}", userId);
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return Json(new { success = false, message = "Access denied" });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Please login" });
            }

            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null || order.UserId != userId.Value) // Updated CustomerId to UserId
                {
                    return Json(new { success = false, message = "Order not found" });
                }

                if (order.OrderStatus != ABCCarTraders.Models.OrderStatus.Pending) // Qualified OrderStatus
                {
                    return Json(new { success = false, message = "Order cannot be cancelled" });
                }

                var result = await _orderService.CancelOrderAsync(orderId);

                if (result)
                {
                    return Json(new { success = true, message = "Order cancelled successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to cancel order" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order: {OrderId} for user: {UserId}", orderId, userId);
                return Json(new { success = false, message = "An error occurred while cancelling the order" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var model = new CustomerProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    City = user.City,
                    Country = user.Country,
                    CreatedAt = user.CreatedAt,
                    TotalOrders = await _orderService.GetCustomerOrderCountAsync(userId.Value),
                    TotalSpent = await _orderService.GetCustomerTotalSpentAsync(userId.Value),
                    MemberSince = user.CreatedAt,
                    LastLoginDate = HttpContext.Session.GetLastLogin()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer profile for user: {UserId}", userId);
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(CustomerProfileViewModel model)
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.PhoneNumber) && !AuthHelper.IsValidPhoneNumber(model.PhoneNumber))
                    {
                        ModelState.AddModelError("PhoneNumber", "Please enter a valid phone number.");
                        return View(model);
                    }

                    var result = await _userService.UpdateProfileAsync(
                        userId.Value,
                        AuthHelper.SanitizeInput(model.FirstName),
                        AuthHelper.SanitizeInput(model.LastName),
                        model.PhoneNumber,
                        AuthHelper.SanitizeInput(model.Address),
                        AuthHelper.SanitizeInput(model.City),
                        AuthHelper.SanitizeInput(model.Country)
                    );

                    if (result)
                    {
                        // Update session with new name
                        HttpContext.Session.SetString(SessionHelper.UserNameKey, $"{model.FirstName} {model.LastName}");

                        HttpContext.Session.SetSuccessMessage("Profile updated successfully.");
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        model.ErrorMessage = "Failed to update profile. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating profile for user: {UserId}", userId);
                    model.ErrorMessage = "An error occurred while updating your profile. Please try again.";
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Get cart items for the customer
                // This would typically come from a cart service
                var model = new CartViewModel
                {
                    CartItems = new List<CartItemViewModel>(),
                    SubTotal = 0,
                    TotalItems = 0
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cart for user: {UserId}", userId);
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, int quantity)
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return Json(new { success = false, message = "Access denied" });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Please login" });
            }

            try
            {
                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Quantity must be greater than 0" });
                }

                // Update cart item logic would go here
                // var result = await _cartService.UpdateCartItemAsync(cartItemId, userId.Value, quantity);

                return Json(new { success = true, message = "Cart updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item: {CartItemId} for user: {UserId}", cartItemId, userId);
                return Json(new { success = false, message = "An error occurred while updating the cart" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return Json(new { success = false, message = "Access denied" });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Please login" });
            }

            try
            {
                // Remove from cart logic would go here
                // var result = await _cartService.RemoveFromCartAsync(cartItemId, userId.Value);

                HttpContext.Session.DecrementCartCount();

                return Json(new { success = true, message = "Item removed from cart" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart: {CartItemId} for user: {UserId}", cartItemId, userId);
                return Json(new { success = false, message = "An error occurred while removing the item" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            if (!HttpContext.Session.IsCustomer())
            {
                return Json(new { success = false, message = "Access denied" });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Please login" });
            }

            try
            {
                // Clear cart logic would go here
                // var result = await _cartService.ClearCartAsync(userId.Value);

                HttpContext.Session.SetCartCount(0);

                return Json(new { success = true, message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user: {UserId}", userId);
                return Json(new { success = false, message = "An error occurred while clearing the cart" });
            }
        }
    }
}