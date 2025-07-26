using Microsoft.AspNetCore.Mvc;
using ABCCarTraders.Models;
using ABCCarTraders.Models.ViewModels;
using ABCCarTraders.Services;
using ABCCarTraders.Helpers;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarService _carService;
        private readonly ICarPartService _carPartService;
        private readonly IOrderService _orderService;
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ICarService carService, ICarPartService carPartService, IOrderService orderService, ILogger<HomeController> logger)
        {
            _carService = carService;
            _carPartService = carPartService;
            _orderService = orderService;
            _logger = logger;
        }
        
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new HomeViewModel
                {
                    FeaturedCars = await _carService.GetFeaturedCarsAsync(6),
                    FeaturedCarParts = await _carPartService.GetFeaturedCarPartsAsync(6),
                    LatestCars = await _carService.GetLatestCarsAsync(4),
                    LatestCarParts = await _carPartService.GetLatestCarPartsAsync(4),
                    TotalCars = await _carService.GetAvailableCarsCountAsync(),
                    TotalCarParts = await _carPartService.GetAvailableCarPartsCountAsync(),
                    IsUserLoggedIn = HttpContext.Session.IsAuthenticated()
                };
                
                // Get cart count for logged-in users
                if (model.IsUserLoggedIn)
                {
                    var userId = HttpContext.Session.GetUserId();
                    if (userId.HasValue)
                    {
                        model.CartItemCount = await _orderService.GetCartItemCountAsync(userId.Value);
                    }
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                return View(new HomeViewModel());
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm, string category = "all")
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("Index");
            }
            
            try
            {
                var model = new SearchResultsViewModel
                {
                    SearchTerm = searchTerm,
                    Category = category
                };
                
                if (category == "all" || category == "cars")
                {
                    model.Cars = await _carService.SearchCarsAsync(searchTerm);
                }
                
                if (category == "all" || category == "parts")
                {
                    model.CarParts = await _carPartService.SearchCarPartsAsync(searchTerm);
                }
                
                model.TotalResults = model.Cars.Count + model.CarParts.Count;
                
                // Add to search history
                HttpContext.Session.AddToSearchHistory(searchTerm);
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing search for term: {SearchTerm}", searchTerm);
                return View(new SearchResultsViewModel { SearchTerm = searchTerm });
            }
        }
        
        [HttpGet]
        public IActionResult About()
        {
            var model = new AboutViewModel
            {
                CompanyName = "ABC Car Traders (Pvt) Ltd",
                EstablishedYear = 2020,
                Mission = "To provide quality vehicles and parts at competitive prices with exceptional customer service.",
                Vision = "To be the leading car trading company in the region, known for trust and reliability.",
                Services = new List<string>
                {
                    "New and Used Car Sales",
                    "Genuine Car Parts and Accessories",
                    "Vehicle Financing Options",
                    "After-Sales Service",
                    "Vehicle Maintenance",
                    "Customer Support"
                }
            };
            
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Contact()
        {
            var model = new ContactViewModel
            {
                CompanyInfo = new CompanyInfo
                {
                    Name = "ABC Car Traders (Pvt) Ltd",
                    Address = "123 Main Street, Colombo 01, Sri Lanka",
                    Phone = "+94 11 234 5678",
                    Email = "info@abccartraders.com",
                    WorkingHours = "Monday - Saturday: 8:00 AM - 6:00 PM",
                    EmergencyContact = "+94 77 123 4567"
                }
            };
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var contactMessage = new ContactMessage
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Subject = model.Subject,
                        Message = model.Message
                    };
                    
                    // Here you would save to database
                    // await _contactService.SaveContactMessageAsync(contactMessage);
                    
                    // Send email notification
                    await EmailHelper.SendContactFormResponseEmailAsync(contactMessage);
                    await EmailHelper.SendAdminNotificationEmailAsync(
                        "New Contact Form Submission",
                        $"New message from {contactMessage.Name} ({contactMessage.Email}): {contactMessage.Message}"
                    );
                    
                    HttpContext.Session.SetSuccessMessage("Thank you for your message! We'll get back to you soon.");
                    return RedirectToAction("Contact");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing contact form");
                    ModelState.AddModelError("", "An error occurred while sending your message. Please try again.");
                }
            }
            
            // Reload company info on error
            model.CompanyInfo = new CompanyInfo
            {
                Name = "ABC Car Traders (Pvt) Ltd",
                Address = "123 Main Street, Colombo 01, Sri Lanka",
                Phone = "+94 11 234 5678",
                Email = "info@abccartraders.com",
                WorkingHours = "Monday - Saturday: 8:00 AM - 6:00 PM",
                EmergencyContact = "+94 77 123 4567"
            };
            
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddToCart(int itemId, string itemType, int quantity = 1)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return Json(new { success = false, message = "Please log in to add items to cart" });
            }
            
            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "User session expired" });
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
                    HttpContext.Session.SetCartCount(cartCount);
                    
                    return Json(new { success = true, message = "Item added to cart", cartCount = cartCount });
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
        
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return Json(new { count = 0 });
            }
            
            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { count = 0 });
            }
            
            try
            {
                var count = await _orderService.GetCartItemCountAsync(userId.Value);
                return Json(new { count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count");
                return Json(new { count = 0 });
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    
    // Supporting ViewModels
    public class HomeViewModel
    {
        public List<Car> FeaturedCars { get; set; } = new List<Car>();
        public List<CarPart> FeaturedCarParts { get; set; } = new List<CarPart>();
        public List<Car> LatestCars { get; set; } = new List<Car>();
        public List<CarPart> LatestCarParts { get; set; } = new List<CarPart>();
        public int TotalCars { get; set; }
        public int TotalCarParts { get; set; }
        public bool IsUserLoggedIn { get; set; }
        public int CartItemCount { get; set; }
    }
    
    public class SearchResultsViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string Category { get; set; } = "all";
        public List<Car> Cars { get; set; } = new List<Car>();
        public List<CarPart> CarParts { get; set; } = new List<CarPart>();
        public int TotalResults { get; set; }
    }
    
    public class AboutViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public int EstablishedYear { get; set; }
        public string Mission { get; set; } = string.Empty;
        public string Vision { get; set; } = string.Empty;
        public List<string> Services { get; set; } = new List<string>();
    }
    
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string? Subject { get; set; }
        
        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;
        
        public CompanyInfo CompanyInfo { get; set; } = new CompanyInfo();
    }
    
    public class CompanyInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string WorkingHours { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
    }
    
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}