using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for user profile management
    /// </summary>
    public class ProfileViewModel
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
        
        [Display(Name = "User Type")]
        public UserType UserType { get; set; }
        
        [Display(Name = "Account Status")]
        public bool IsActive { get; set; }
        
        [Display(Name = "Member Since")]
        public DateTime CreatedAt { get; set; }
        
        [Display(Name = "Last Updated")]
        public DateTime UpdatedAt { get; set; }
        
        // Password change section
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }
        
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }
        
        // Profile image
        [Display(Name = "Profile Picture")]
        public string? ProfileImageUrl { get; set; }
        
        [Display(Name = "Upload New Picture")]
        public IFormFile? ProfileImageFile { get; set; }
        
        // Preferences
        [Display(Name = "Email Notifications")]
        public bool ReceiveEmailNotifications { get; set; } = true;
        
        [Display(Name = "SMS Notifications")]
        public bool ReceiveSmsNotifications { get; set; } = false;
        
        [Display(Name = "Marketing Emails")]
        public bool ReceiveMarketingEmails { get; set; } = true;
        
        // Customer-specific data
        public CustomerProfileData? CustomerData { get; set; }
        
        // Admin-specific data
        public AdminProfileData? AdminData { get; set; }
        
        // Form state
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsEditMode { get; set; } = false;
        public bool ShowPasswordSection { get; set; } = false;
        
        // Computed properties
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string UserTypeDisplay => UserType.ToString();
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string StatusBadgeClass => IsActive ? "success" : "secondary";
        public string MemberSince => CreatedAt.ToString("MMMM yyyy");
        
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
                UserType = UserType,
                IsActive = IsActive,
                CreatedAt = CreatedAt,
                UpdatedAt = DateTime.Now
            };
        }
        
        // Create from User entity
        public static ProfileViewModel FromUser(User user)
        {
            var model = new ProfileViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                UserType = user.UserType,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsEditMode = true
            };
            
            // Initialize user-type specific data
            if (user.UserType == UserType.Customer)
            {
                model.CustomerData = new CustomerProfileData();
            }
            else if (user.UserType == UserType.Admin)
            {
                model.AdminData = new AdminProfileData();
            }
            
            return model;
        }
        
        // Validation
        public bool IsPasswordChangeRequested => !string.IsNullOrEmpty(NewPassword);
        
        public List<string> Validate()
        {
            var errors = new List<string>();
            
            // Email validation
            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("Email is required");
            
            // Password change validation
            if (IsPasswordChangeRequested)
            {
                if (string.IsNullOrWhiteSpace(CurrentPassword))
                    errors.Add("Current password is required to change password");
                
                if (string.IsNullOrWhiteSpace(NewPassword))
                    errors.Add("New password is required");
                else if (NewPassword.Length < 6)
                    errors.Add("New password must be at least 6 characters long");
                
                if (NewPassword != ConfirmNewPassword)
                    errors.Add("New password and confirmation do not match");
            }
            
            return errors;
        }
    }
    
    /// <summary>
    /// Customer-specific profile data
    /// </summary>
    public class CustomerProfileData
    {
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int FavoriteItems { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public string? PreferredContactMethod { get; set; }
        public List<string> Interests { get; set; } = new List<string>();
        
        // Loyalty program
        public int LoyaltyPoints { get; set; }
        public string? LoyaltyTier { get; set; }
        
        // Preferences
        public string? PreferredBrand { get; set; }
        public string? PreferredCarType { get; set; }
        
        public string TotalSpentFormatted => TotalSpent.ToString("C");
        public string LastOrderFormatted => LastOrderDate?.ToString("MMM dd, yyyy") ?? "Never";
    }
    
    /// <summary>
    /// Admin-specific profile data
    /// </summary>
    public class AdminProfileData
    {
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int TotalLoginsThisMonth { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        
        // Admin statistics
        public int CarsManaged { get; set; }
        public int PartsManaged { get; set; }
        public int OrdersProcessed { get; set; }
        public int CustomersServed { get; set; }
        
        public string LastLoginFormatted => LastLoginDate?.ToString("MMM dd, yyyy HH:mm") ?? "Never";
    }
    
    /// <summary>
    /// Profile settings view model
    /// </summary>
    public class ProfileSettingsViewModel
    {
        public int UserId { get; set; }
        
        // Notification preferences
        [Display(Name = "Email Notifications")]
        public bool EmailNotifications { get; set; } = true;
        
        [Display(Name = "SMS Notifications")]
        public bool SmsNotifications { get; set; } = false;
        
        [Display(Name = "Push Notifications")]
        public bool PushNotifications { get; set; } = true;
        
        [Display(Name = "Marketing Emails")]
        public bool MarketingEmails { get; set; } = false;
        
        [Display(Name = "Weekly Newsletter")]
        public bool WeeklyNewsletter { get; set; } = false;
        
        // Privacy preferences
        [Display(Name = "Profile Visibility")]
        public string ProfileVisibility { get; set; } = "Private";
        
        [Display(Name = "Show Activity")]
        public bool ShowActivity { get; set; } = false;
        
        [Display(Name = "Allow Contact")]
        public bool AllowContact { get; set; } = true;
        
        // Display preferences
        [Display(Name = "Items Per Page")]
        public int ItemsPerPage { get; set; } = 25;
        
        [Display(Name = "Default Sort Order")]
        public string DefaultSortOrder { get; set; } = "Newest";
        
        [Display(Name = "Preferred Currency")]
        public string PreferredCurrency { get; set; } = "USD";
        
        [Display(Name = "Date Format")]
        public string DateFormat { get; set; } = "MM/dd/yyyy";
        
        // Two-factor authentication
        [Display(Name = "Enable Two-Factor Authentication")]
        public bool TwoFactorEnabled { get; set; } = false;
        
        [Display(Name = "Backup Email")]
        public string? BackupEmail { get; set; }
        
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
    
    /// <summary>
    /// Change password view model
    /// </summary>
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password confirmation is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
        
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}