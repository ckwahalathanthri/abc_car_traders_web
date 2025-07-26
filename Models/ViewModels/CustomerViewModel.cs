using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for customer information
    /// </summary>
    public class CustomerViewModel
    {
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        public string? Address { get; set; }
        
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        [Display(Name = "City")]
        public string? City { get; set; }
        
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        [Display(Name = "Country")]
        public string? Country { get; set; }
        
        [Display(Name = "Account Status")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Registration Date")]
        public DateTime CreatedAt { get; set; }
        
        [Display(Name = "Last Updated")]
        public DateTime UpdatedAt { get; set; }
        
        // Customer statistics
        [Display(Name = "Total Orders")]
        public int TotalOrders { get; set; }
        
        [Display(Name = "Total Spent")]
        public decimal TotalSpent { get; set; }
        
        [Display(Name = "Last Order Date")]
        public DateTime? LastOrderDate { get; set; }
        
        [Display(Name = "Average Order Value")]
        public decimal AverageOrderValue { get; set; }
        
        // Computed properties
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string StatusBadgeClass => IsActive ? "success" : "secondary";
        public string TotalSpentFormatted => TotalSpent.ToString("C");
        public string AverageOrderValueFormatted => AverageOrderValue.ToString("C");
        public string LastOrderFormatted => LastOrderDate?.ToString("MMM dd, yyyy") ?? "No orders";
        public string RegistrationDateFormatted => CreatedAt.ToString("MMM dd, yyyy");
        public string CustomerSince => CreatedAt.ToString("MMMM yyyy");
        
        // Customer tier based on spending
        public string CustomerTier
        {
            get
            {
                return TotalSpent switch
                {
                    >= 10000 => "Platinum",
                    >= 5000 => "Gold",
                    >= 2000 => "Silver",
                    >= 500 => "Bronze",
                    _ => "Standard"
                };
            }
        }
        
        public string CustomerTierBadgeClass
        {
            get
            {
                return CustomerTier switch
                {
                    "Platinum" => "dark",
                    "Gold" => "warning",
                    "Silver" => "secondary",
                    "Bronze" => "info",
                    _ => "light"
                };
            }
        }
        
        // Contact information availability
        public bool HasPhoneNumber => !string.IsNullOrWhiteSpace(PhoneNumber);
        public bool HasAddress => !string.IsNullOrWhiteSpace(Address);
        public bool HasCompleteProfile => HasPhoneNumber && HasAddress && !string.IsNullOrWhiteSpace(City);
        
        // Convert to User entity
        public User ToUser()
        {
            return new User
            {
                UserId = UserId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Address = Address,
                City = City,
                Country = Country,
                UserType = UserType.Customer,
                IsActive = IsActive,
                CreatedAt = CreatedAt,
                UpdatedAt = DateTime.Now
            };
        }
        
        // Create from User entity
        public static CustomerViewModel FromUser(User user)
        {
            return new CustomerViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
                // Note: Statistics will need to be populated separately from order data
            };
        }
    }
    
    /// <summary>
    /// View model for customer list with pagination and filtering
    /// </summary>
    public class CustomerListViewModel
    {
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
        public CustomerSearchViewModel SearchModel { get; set; } = new CustomerSearchViewModel();
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
        
        // Filter options
        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Countries { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CustomerTiers { get; set; } = new List<SelectListItem>();
        
        // Statistics
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int InactiveCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public decimal AverageCustomerValue { get; set; }
        public decimal TotalCustomerValue { get; set; }
        
        // Quick stats
        public int CustomersWithOrders { get; set; }
        public int CustomersWithoutOrders { get; set; }
        public DateTime? LastRegistration { get; set; }
        
        // Computed properties
        public decimal ActiveCustomerPercentage => TotalCustomers > 0 ? (decimal)ActiveCustomers / TotalCustomers * 100 : 0;
        public string AverageCustomerValueFormatted => AverageCustomerValue.ToString("C");
        public string TotalCustomerValueFormatted => TotalCustomerValue.ToString("C");
        public string LastRegistrationFormatted => LastRegistration?.ToString("MMM dd, yyyy") ?? "Never";
    }
    
    /// <summary>
    /// View model for customer search and filtering
    /// </summary>
    public class CustomerSearchViewModel
    {
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }
        
        [Display(Name = "Email")]
        public string? Email { get; set; }
        
        [Display(Name = "Phone")]
        public string? PhoneNumber { get; set; }
        
        [Display(Name = "City")]
        public string? City { get; set; }
        
        [Display(Name = "Country")]
        public string? Country { get; set; }
        
        [Display(Name = "Status")]
        public bool? IsActive { get; set; }
        
        [Display(Name = "Customer Tier")]
        public string? CustomerTier { get; set; }
        
        [Display(Name = "Registration From")]
        [DataType(DataType.Date)]
        public DateTime? RegistrationFrom { get; set; }
        
        [Display(Name = "Registration To")]
        [DataType(DataType.Date)]
        public DateTime? RegistrationTo { get; set; }
        
        [Display(Name = "Min Total Spent")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount cannot be negative")]
        public decimal? MinTotalSpent { get; set; }
        
        [Display(Name = "Max Total Spent")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount cannot be negative")]
        public decimal? MaxTotalSpent { get; set; }
        
        [Display(Name = "Has Orders")]
        public bool? HasOrders { get; set; }
        
        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "name";
        
        [Display(Name = "Order")]
        public bool IsDescending { get; set; } = false;
        
        // Results per page
        public int PageSize { get; set; } = 25;
        public int PageNumber { get; set; } = 1;
        
        // Available sort options
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "name", Text = "Name" },
            new SelectListItem { Value = "email", Text = "Email" },
            new SelectListItem { Value = "registration", Text = "Registration Date" },
            new SelectListItem { Value = "totalspent", Text = "Total Spent" },
            new SelectListItem { Value = "orders", Text = "Total Orders" },
            new SelectListItem { Value = "lastorder", Text = "Last Order Date" }
        };
    }
    
    /// <summary>
    /// View model for customer details page
    /// </summary>
    public class CustomerDetailsViewModel
    {
        public CustomerViewModel Customer { get; set; } = new CustomerViewModel();
        public List<OrderViewModel> RecentOrders { get; set; } = new List<OrderViewModel>();
        public List<OrderViewModel> OrderHistory { get; set; } = new List<OrderViewModel>();
        
        // Customer activity
        public DateTime? LastLoginDate { get; set; }
        public int LoginCount { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        
        // Preferences and behavior
        public List<string> PreferredBrands { get; set; } = new List<string>();
        public List<string> PreferredCategories { get; set; } = new List<string>();
        public string? PreferredContactMethod { get; set; }
        
        // Customer metrics
        public decimal LifetimeValue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int DaysSinceRegistration { get; set; }
        public int DaysSinceLastOrder { get; set; }
        
        // Communication history
        public int EmailsSent { get; set; }
        public int SmsMessagesSent { get; set; }
        public DateTime? LastContactDate { get; set; }
        
        // Computed properties
        public string LastLoginFormatted => LastLoginDate?.ToString("MMM dd, yyyy HH:mm") ?? "Never";
        public string LastActivityFormatted => LastActivityDate?.ToString("MMM dd, yyyy HH:mm") ?? "Never";
        public string LifetimeValueFormatted => LifetimeValue.ToString("C");
        public bool IsNewCustomer => DaysSinceRegistration <= 30;
        public bool IsActiveCustomer => DaysSinceLastOrder <= 90;
        public bool IsAtRiskCustomer => DaysSinceLastOrder > 180 && Customer.TotalOrders > 0;
        
        public string CustomerStatus
        {
            get
            {
                if (IsNewCustomer) return "New";
                if (IsActiveCustomer) return "Active";
                if (IsAtRiskCustomer) return "At Risk";
                return "Inactive";
            }
        }
        
        public string CustomerStatusBadgeClass
        {
            get
            {
                return CustomerStatus switch
                {
                    "New" => "primary",
                    "Active" => "success",
                    "At Risk" => "warning",
                    "Inactive" => "secondary",
                    _ => "light"
                };
            }
        }
    }
    
    /// <summary>
    /// View model for customer bulk operations
    /// </summary>
    public class CustomerBulkOperationsViewModel
    {
        public List<int> SelectedCustomerIds { get; set; } = new List<int>();
        public string Operation { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        
        // Bulk update fields
        public bool? NewActiveStatus { get; set; }
        public string? NewCity { get; set; }
        public string? NewCountry { get; set; }
        
        // Communication
        public string? EmailSubject { get; set; }
        public string? EmailMessage { get; set; }
        public string? SmsMessage { get; set; }
        
        // Results
        public int AffectedCustomers { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string? SuccessMessage { get; set; }
        
        // Available operations
        public List<SelectListItem> Operations { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "activate", Text = "Activate Accounts" },
            new SelectListItem { Value = "deactivate", Text = "Deactivate Accounts" },
            new SelectListItem { Value = "send_email", Text = "Send Email" },
            new SelectListItem { Value = "send_sms", Text = "Send SMS" },
            new SelectListItem { Value = "export", Text = "Export Data" },
            new SelectListItem { Value = "delete", Text = "Delete Accounts" }
        };
    }
    
    /// <summary>
    /// View model for customer analytics and reports
    /// </summary>
    public class CustomerAnalyticsViewModel
    {
        // Registration trends
        public Dictionary<string, int> RegistrationsByMonth { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> RegistrationsByCity { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> SpendingByTier { get; set; } = new Dictionary<string, decimal>();
        
        // Activity metrics
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int RetentionRate { get; set; }
        public decimal AverageCustomerLifetimeValue { get; set; }
        
        // Geographic distribution
        public Dictionary<string, int> CustomersByCountry { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CustomersByCity { get; set; } = new Dictionary<string, int>();
        
        // Behavior analysis
        public Dictionary<string, int> PreferredContactMethods { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> PurchasePatterns { get; set; } = new Dictionary<string, decimal>();
        
        // Top customers
        public List<CustomerViewModel> TopCustomersBySpending { get; set; } = new List<CustomerViewModel>();
        public List<CustomerViewModel> TopCustomersByOrders { get; set; } = new List<CustomerViewModel>();
        public List<CustomerViewModel> MostRecentCustomers { get; set; } = new List<CustomerViewModel>();
        
        // Date range for analytics
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; } = DateTime.Now.AddMonths(-12);
        
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; } = DateTime.Now;
        
        // Computed properties
        public string AverageCustomerLifetimeValueFormatted => AverageCustomerLifetimeValue.ToString("C");
        public decimal CustomerGrowthRate => RegistrationsByMonth.Count > 1 ? 
            (decimal)(RegistrationsByMonth.Values.TakeLast(1).First() - RegistrationsByMonth.Values.TakeLast(2).First()) / 
            RegistrationsByMonth.Values.TakeLast(2).First() * 100 : 0;
    }
}