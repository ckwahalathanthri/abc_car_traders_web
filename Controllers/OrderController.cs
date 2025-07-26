using Microsoft.AspNetCore.Mvc;
using ABCCarTraders.Models;
using ABCCarTraders.Models.ViewModels;
using ABCCarTraders.Services;
using ABCCarTraders.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using ModelOrderStatus = ABCCarTraders.Models.OrderStatus;
using ModelPaymentStatus = ABCCarTraders.Models.PaymentStatus;
using ModelPaymentMethod = ABCCarTraders.Models.PaymentMethod;

namespace ABCCarTraders.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICarService _carService;
        private readonly ICarPartService _carPartService;
        private readonly IUserService _userService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            ICarService carService,
            ICarPartService carPartService,
            IUserService userService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _carService = carService;
            _carPartService = carPartService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Cart", "Order") });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var cartItems = await _orderService.GetUserCartAsync(userId.Value);
                var model = new CartViewModel
                {
                    CartItems = new List<CartItemViewModel>(),
                    SubTotal = 0,
                    TotalItems = 0,
                    ShippingCost = 0,
                    TaxAmount = 0
                };

                foreach (var cartItem in cartItems)
                {
                    var cartItemViewModel = CartItemViewModel.FromCart(cartItem);

                    // Get item details
                    if (cartItem.ItemType == ItemType.Car)
                    {
                        var car = await _carService.GetCarByIdAsync(cartItem.ItemId);
                        if (car != null)
                        {
                            // Get brand separately if navigation property is not loaded
                            var brand = car.Brand ?? (await _carService.GetAllBrandsAsync()).FirstOrDefault(b => b.BrandId == car.BrandId);

                            cartItemViewModel.ProductName = $"{brand?.BrandName ?? "Unknown"} {car.Model} ({car.Year})";
                            cartItemViewModel.ItemDescription = car.Description ?? "";
                            cartItemViewModel.ProductImage = car.ImageUrl ?? "";
                            cartItemViewModel.BrandName = brand?.BrandName ?? "";
                            cartItemViewModel.UnitPrice = car.Price;
                            cartItemViewModel.StockQuantity = car.StockQuantity;
                            cartItemViewModel.IsAvailable = car.IsAvailable;
                        }
                    }
                    else
                    {
                        var carPart = await _carPartService.GetCarPartByIdAsync(cartItem.ItemId);
                        if (carPart != null)
                        {
                            // Get brand separately if navigation property is not loaded
                            var brand = carPart.Brand ?? (await _carPartService.GetAllBrandsAsync()).FirstOrDefault(b => b.BrandId == carPart.BrandId);

                            cartItemViewModel.ProductName = $"{brand?.BrandName ?? "Unknown"} {carPart.PartName}";
                            cartItemViewModel.ItemDescription = carPart.Description ?? "";
                            cartItemViewModel.ProductImage = carPart.ImageUrl ?? "";
                            cartItemViewModel.BrandName = brand?.BrandName ?? "";
                            cartItemViewModel.UnitPrice = carPart.Price;
                            cartItemViewModel.StockQuantity = carPart.StockQuantity;
                            cartItemViewModel.IsAvailable = carPart.IsAvailable;
                            cartItemViewModel.PartNumber = carPart.PartNumber;
                        }
                    }

                    model.CartItems.Add(cartItemViewModel);
                    model.SubTotal += cartItemViewModel.TotalPrice;
                    model.TotalItems += cartItemViewModel.Quantity;
                }

                // Calculate shipping and tax
                model.ShippingCost = model.SubTotal > 1000 ? 0 : 50; // Free shipping over $1000
                model.TaxAmount = model.SubTotal * 0.10m; // 10% tax
                model.Tax = model.TaxAmount; // Keep both properties in sync
                model.GrandTotal = model.SubTotal + model.ShippingCost + model.TaxAmount;

                model.IsValid = await _orderService.ValidateCartAsync(userId.Value);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cart for user: {UserId}", userId);
                return View(new CartViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int itemId, string itemType, int quantity = 1)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return Json(new { success = false, message = "Please log in to add items to cart", requireLogin = true });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "User session expired", requireLogin = true });
            }

            try
            {
                ItemType type = itemType.ToLower() == "car" ? ItemType.Car : ItemType.CarPart;

                // Check if item is available
                bool isAvailable = type == ItemType.Car
                    ? await _carService.IsCarAvailableAsync(itemId)
                    : await _carPartService.IsCarPartAvailableAsync(itemId);

                if (!isAvailable)
                {
                    return Json(new { success = false, message = "Item is not available" });
                }

                // Check stock
                bool hasStock = type == ItemType.Car
                    ? await _carService.HasSufficientStockAsync(itemId, quantity)
                    : await _carPartService.HasSufficientStockAsync(itemId, quantity);

                if (!hasStock)
                {
                    return Json(new { success = false, message = "Insufficient stock" });
                }

                // Add to cart
                var result = await _orderService.AddToCartAsync(userId.Value, type, itemId, quantity);

                if (result)
                {
                    var cartCount = await _orderService.GetCartItemCountAsync(userId.Value);
                    var cartTotal = await _orderService.GetCartTotalAsync(userId.Value);
                    HttpContext.Session.SetCartCount(cartCount);

                    return Json(new
                    {
                        success = true,
                        message = "Item added to cart",
                        cartCount = cartCount,
                        cartTotal = cartTotal.ToString("C")
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to add item to cart" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCart(int cartId, int quantity)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Please log in again" });
            }

            try
            {
                var result = await _orderService.UpdateCartItemAsync(cartId, quantity);

                if (result)
                {
                    var cartCount = await _orderService.GetCartItemCountAsync(userId.Value);
                    var cartTotal = await _orderService.GetCartTotalAsync(userId.Value);

                    HttpContext.Session.SetCartCount(cartCount);

                    return Json(new
                    {
                        success = true,
                        cartCount = cartCount,
                        cartTotal = cartTotal.ToString("C"),
                        message = "Cart updated successfully"
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update cart item" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item: {CartId} for user: {UserId}", cartId, userId);
                return Json(new { success = false, message = "An error occurred while updating the cart item" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Please log in again" });
            }

            try
            {
                var result = await _orderService.RemoveFromCartAsync(cartId);

                if (result)
                {
                    var cartCount = await _orderService.GetCartItemCountAsync(userId.Value);
                    var cartTotal = await _orderService.GetCartTotalAsync(userId.Value);

                    HttpContext.Session.SetCartCount(cartCount);

                    return Json(new
                    {
                        success = true,
                        cartCount = cartCount,
                        cartTotal = cartTotal.ToString("C"),
                        message = "Item removed from cart"
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to remove item from cart" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item: {CartId} for user: {UserId}", cartId, userId);
                return Json(new { success = false, message = "An error occurred while removing the item from cart" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Please log in again" });
            }

            try
            {
                var result = await _orderService.ClearCartAsync(userId.Value);

                if (result)
                {
                    HttpContext.Session.SetCartCount(0);
                    return Json(new { success = true, message = "Cart cleared successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to clear cart" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user: {UserId}", userId);
                return Json(new { success = false, message = "An error occurred while clearing the cart" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Check if cart is not empty
                var cartCount = await _orderService.GetCartItemCountAsync(userId.Value);
                if (cartCount == 0)
                {
                    HttpContext.Session.SetWarningMessage("Your cart is empty. Please add items before checkout.");
                    return RedirectToAction("Cart");
                }

                // Validate cart items
                if (!await _orderService.ValidateCartAsync(userId.Value))
                {
                    HttpContext.Session.SetErrorMessage("Some items in your cart are no longer available. Please review your cart.");
                    return RedirectToAction("Cart");
                }

                var user = await _userService.GetUserByIdAsync(userId.Value);
                var cartItems = await _orderService.GetUserCartAsync(userId.Value);
                var cartTotal = await _orderService.GetCartTotalAsync(userId.Value);

                var model = new CheckoutViewModel
                {
                    // Customer information
                    FirstName = user?.FirstName ?? "",
                    LastName = user?.LastName ?? "",
                    Email = user?.Email ?? "",
                    PhoneNumber = user?.PhoneNumber ?? "",

                    // Shipping information
                    ShippingAddress = user?.Address ?? "",
                    City = user?.City ?? "",
                    Country = user?.Country ?? "",

                    // Order summary
                    SubTotal = cartTotal,
                    ShippingCost = cartTotal > 1000 ? 0 : 50,
                    Tax = cartTotal * 0.10m,
                    TotalItems = cartCount
                };

                model.GrandTotal = model.SubTotal + model.ShippingCost + model.Tax;

                // Load payment methods
                model.PaymentMethods = Enum.GetValues<Models.ViewModels.PaymentMethod>()
                    .Select(pm => new SelectListItem
                    {
                        Value = ((int)pm).ToString(),
                        Text = pm.ToString().Replace("Card", " Card")
                    }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading checkout page for user: {UserId}", userId);
                HttpContext.Session.SetErrorMessage("An error occurred while loading the checkout page.");
                return RedirectToAction("Cart");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
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
                    // Validate cart again
                    if (!await _orderService.ValidateCartAsync(userId.Value))
                    {
                        model.ErrorMessage = "Some items in your cart are no longer available. Please review your cart.";
                        await LoadCheckoutData(model, userId.Value);
                        return View(model);
                    }

                    var cartItems = await _orderService.GetUserCartAsync(userId.Value);
                    if (!cartItems.Any())
                    {
                        model.ErrorMessage = "Your cart is empty.";
                        return RedirectToAction("Cart");
                    }

                    // Prepare shipping address
                    var shippingAddress = $"{model.ShippingAddress}, {model.City}, {model.Country}";

                    // Convert ViewModels.PaymentMethod to Models.PaymentMethod
                    var modelsPaymentMethod = (ModelPaymentMethod)(int)model.PaymentMethod;

                    // Process order
                    var result = await _orderService.ProcessOrderAsync(
                        userId.Value,
                        cartItems.ToList(),
                        shippingAddress,
                        modelsPaymentMethod,
                        model.Notes
                    );

                    if (result)
                    {
                        HttpContext.Session.SetCartCount(0);
                        HttpContext.Session.SetSuccessMessage("Order placed successfully!");

                        // Get the latest order for confirmation
                        var latestOrder = await _orderService.GetLatestOrderAsync(userId.Value);
                        if (latestOrder != null)
                        {
                            return RedirectToAction("Confirmation", new { orderId = latestOrder.OrderId });
                        }
                        else
                        {
                            return RedirectToAction("Confirmation");
                        }
                    }
                    else
                    {
                        model.ErrorMessage = "Failed to process order. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing checkout for user: {UserId}", userId);
                    model.ErrorMessage = "An error occurred while processing your order. Please try again.";
                }
            }

            await LoadCheckoutData(model, userId.Value);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int? orderId = null)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                Order? order = null;

                if (orderId.HasValue)
                {
                    order = await _orderService.GetOrderByIdAsync(orderId.Value);

                    // Check if order belongs to current user
                    if (order == null || order.UserId != userId.Value)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Get latest order for user
                    order = await _orderService.GetLatestOrderAsync(userId.Value);
                }

                if (order == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var model = new OrderConfirmationViewModel
                {
                    Order = OrderViewModel.FromOrder(order),
                    EstimatedDeliveryDate = DateTime.Now.AddDays(7), // Example: 7 days delivery
                    TrackingNumber = $"ABC{order.OrderNumber.Replace("-", "")}",
                    SupportEmail = "support@abccartraders.com",
                    SupportPhone = "+94 11 234 5678"
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order confirmation for user: {UserId}, orderId: {OrderId}", userId, orderId);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    HttpContext.Session.SetErrorMessage("Order not found.");
                    return RedirectToAction("MyOrders", "Customer");
                }

                // Check if order belongs to current user or user is admin
                if (order.UserId != userId.Value && !HttpContext.Session.IsAdmin())
                {
                    HttpContext.Session.SetErrorMessage("You don't have permission to view this order.");
                    return RedirectToAction("MyOrders", "Customer");
                }

                var model = new OrderDetailsViewModel
                {
                    Order = OrderViewModel.FromOrder(order),
                    CanCancel = await _orderService.CanCancelOrderAsync(id),
                    OrderItems = new List<OrderItemDetailsViewModel>()
                };

                // Get detailed item information
                foreach (var orderItem in order.OrderItems)
                {
                    var itemDetails = new OrderItemDetailsViewModel
                    {
                        OrderItem = OrderItemViewModel.FromOrderItem(orderItem),
                        ItemType = orderItem.ItemType
                    };

                    if (orderItem.ItemType == ItemType.Car)
                    {
                        var car = await _carService.GetCarByIdAsync(orderItem.ItemId);
                        if (car != null)
                        {
                            // Get brand separately if navigation property is not loaded
                            var brand = car.Brand ?? (await _carService.GetAllBrandsAsync()).FirstOrDefault(b => b.BrandId == car.BrandId);

                            itemDetails.ItemName = $"{brand?.BrandName ?? "Unknown"} {car.Model} ({car.Year})";
                            itemDetails.ItemDescription = car.Description ?? "";
                            itemDetails.ItemImageUrl = car.ImageUrl ?? "";
                        }
                    }
                    else
                    {
                        var carPart = await _carPartService.GetCarPartByIdAsync(orderItem.ItemId);
                        if (carPart != null)
                        {
                            // Get brand separately if navigation property is not loaded
                            var brand = carPart.Brand ?? (await _carPartService.GetAllBrandsAsync()).FirstOrDefault(b => b.BrandId == carPart.BrandId);

                            itemDetails.ItemName = $"{brand?.BrandName ?? "Unknown"} {carPart.PartName}";
                            itemDetails.ItemDescription = carPart.Description ?? "";
                            itemDetails.ItemImageUrl = carPart.ImageUrl ?? "";
                            itemDetails.PartNumber = carPart.PartNumber;
                        }
                    }

                    model.OrderItems.Add(itemDetails);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order details: {OrderId} for user: {UserId}", id, userId);
                return RedirectToAction("MyOrders", "Customer");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Track(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
                if (order == null)
                {
                    ViewBag.ErrorMessage = "Order not found. Please check your order number.";
                    return View(new OrderTrackingViewModel());
                }

                var model = new OrderTrackingViewModel
                {
                    Order = OrderViewModel.FromOrder(order),
                    TrackingSteps = GetTrackingSteps(order.OrderStatus),
                    EstimatedDeliveryDate = order.OrderDate.AddDays(7),
                    TrackingNumber = $"ABC{order.OrderNumber.Replace("-", "")}"
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking order: {OrderNumber}", orderNumber);
                ViewBag.ErrorMessage = "An error occurred while tracking your order.";
                return View(new OrderTrackingViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Please log in again" });
            }

            try
            {
                // Check if order belongs to current user
                if (!await _orderService.IsOrderOwnedByUserAsync(orderId, userId.Value))
                {
                    return Json(new { success = false, message = "You don't have permission to cancel this order" });
                }

                // Check if order can be cancelled
                if (!await _orderService.CanCancelOrderAsync(orderId))
                {
                    return Json(new { success = false, message = "This order cannot be cancelled" });
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

        // Helper methods
        private async Task LoadCheckoutData(CheckoutViewModel model, int userId)
        {
            var cartTotal = await _orderService.GetCartTotalAsync(userId);
            var cartCount = await _orderService.GetCartItemCountAsync(userId);

            model.SubTotal = cartTotal;
            model.ShippingCost = cartTotal > 1000 ? 0 : 50;
            model.Tax = cartTotal * 0.10m;
            model.GrandTotal = model.SubTotal + model.ShippingCost + model.Tax;
            model.TotalItems = cartCount;

            model.PaymentMethods = Enum.GetValues<Models.ViewModels.PaymentMethod>()
                .Select(pm => new SelectListItem
                {
                    Value = ((int)pm).ToString(),
                    Text = pm.ToString().Replace("Card", " Card")
                }).ToList();
        }

        private List<TrackingStep> GetTrackingSteps(ModelOrderStatus currentStatus)
        {
            var steps = new List<TrackingStep>
            {
                new TrackingStep { Status = "Order Placed", IsCompleted = true },
                new TrackingStep { Status = "Order Confirmed", IsCompleted = currentStatus >= ModelOrderStatus.Confirmed },
                new TrackingStep { Status = "Processing", IsCompleted = currentStatus >= ModelOrderStatus.Processing },
                new TrackingStep { Status = "Shipped", IsCompleted = currentStatus >= ModelOrderStatus.Shipped },
                new TrackingStep { Status = "Delivered", IsCompleted = currentStatus >= ModelOrderStatus.Delivered }
            };

            if (currentStatus == ModelOrderStatus.Cancelled)
            {
                steps.Add(new TrackingStep { Status = "Cancelled", IsCompleted = true, IsCancelled = true });
            }

            return steps;
        }
    }
}