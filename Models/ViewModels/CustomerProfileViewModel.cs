using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for customer profile management
    /// </summary>
    public class CustomerProfileViewModel
    {
        /// <summary>
        /// Customer's first name
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        
        /// <summary>
        /// Customer's last name
        /// </summary>
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        
        /// <summary>
        /// Customer's email address (readonly after registration)
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Customer's phone number
        /// </summary>
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        
        /// <summary>
        /// Customer's address
        /// </summary>
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        public string? Address { get; set; }
        
        /// <summary>
        /// Customer's city
        /// </summary>
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        [Display(Name = "City")]
        public string? City { get; set; }
        
        /// <summary>
        /// Customer's country
        /// </summary>
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        [Display(Name = "Country")]
        public string? Country { get; set; }
        
        /// <summary>
        /// When the customer account was created
        /// </summary>
        [Display(Name = "Account Created")]
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Total number of orders placed by customer
        /// </summary>
        [Display(Name = "Total Orders")]
        public int TotalOrders { get; set; }
        
        /// <summary>
        /// Total amount spent by customer
        /// </summary>
        [Display(Name = "Total Spent")]
        public decimal TotalSpent { get; set; }
        
        /// <summary>
        /// When the customer became a member
        /// </summary>
        [Display(Name = "Member Since")]
        public DateTime MemberSince { get; set; }
        
        /// <summary>
        /// Last login date
        /// </summary>
        [Display(Name = "Last Login")]
        public DateTime? LastLoginDate { get; set; }
        
        /// <summary>
        /// Customer's preferred contact method
        /// </summary>
        [Display(Name = "Preferred Contact Method")]
        public ContactMethod PreferredContactMethod { get; set; } = ContactMethod.Email;
        
        /// <summary>
        /// Whether customer wants to receive promotional emails
        /// </summary>
        [Display(Name = "Receive Promotional Emails")]
        public bool ReceivePromotionalEmails { get; set; } = true;
        
        /// <summary>
        /// Whether customer wants to receive SMS notifications
        /// </summary>
        [Display(Name = "Receive SMS Notifications")]
        public bool ReceiveSMSNotifications { get; set; } = false;
        
        /// <summary>
        /// Customer's preferred language
        /// </summary>
        [Display(Name = "Preferred Language")]
        public string PreferredLanguage { get; set; } = "English";
        
        /// <summary>
        /// Customer's timezone
        /// </summary>
        [Display(Name = "Timezone")]
        public string Timezone { get; set; } = "Asia/Colombo";
        
        /// <summary>
        /// Whether to show customer's profile publicly
        /// </summary>
        [Display(Name = "Public Profile")]
        public bool IsPublicProfile { get; set; } = false;
        
        /// <summary>
        /// Customer's bio or about section
        /// </summary>
        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }
        
        /// <summary>
        /// Customer's profile picture URL
        /// </summary>
        [Display(Name = "Profile Picture")]
        public string? ProfilePictureUrl { get; set; }
        
        /// <summary>
        /// Customer's date of birth
        /// </summary>
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        
        /// <summary>
        /// Customer's gender
        /// </summary>
        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }
        
        /// <summary>
        /// Error message to display
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// Success message to display
        /// </summary>
        public string? SuccessMessage { get; set; }
        
        /// <summary>
        /// Whether the profile is being edited
        /// </summary>
        public bool IsEditMode { get; set; } = true;
        
        /// <summary>
        /// Customer's loyalty points
        /// </summary>
        [Display(Name = "Loyalty Points")]
        public int LoyaltyPoints { get; set; } = 0;
        
        /// <summary>
        /// Customer's membership tier
        /// </summary>
        [Display(Name = "Membership Tier")]
        public CustomerTier MembershipTier { get; set; } = CustomerTier.Bronze;
        
        /// <summary>
        /// Customer's average order value
        /// </summary>
        [Display(Name = "Average Order Value")]
        public decimal AverageOrderValue { get; set; } = 0;
        
        /// <summary>
        /// Number of items in customer's wishlist
        /// </summary>
        [Display(Name = "Wishlist Items")]
        public int WishlistItemCount { get; set; } = 0;
        
        /// <summary>
        /// Last order date
        /// </summary>
        [Display(Name = "Last Order")]
        public DateTime? LastOrderDate { get; set; }
        
        // Computed Properties
        
        /// <summary>
        /// Customer's full name
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();
        
        /// <summary>
        /// Formatted display of total spent
        /// </summary>
        public string FormattedTotalSpent => TotalSpent.ToString("C");
        
        /// <summary>
        /// Formatted display of member since date
        /// </summary>
        public string FormattedMemberSince => MemberSince.ToString("MMMM yyyy");
        
        /// <summary>
        /// Formatted display of average order value
        /// </summary>
        public string FormattedAverageOrderValue => AverageOrderValue.ToString("C");
        
        /// <summary>
        /// Customer's age based on date of birth
        /// </summary>
        public int? Age => DateOfBirth.HasValue ? DateTime.Now.Year - DateOfBirth.Value.Year : null;
        
        /// <summary>
        /// Number of days since customer joined
        /// </summary>
        public int DaysSinceJoined => (DateTime.Now - MemberSince).Days;
        
        /// <summary>
        /// Whether customer is a new member (less than 30 days)
        /// </summary>
        public bool IsNewMember => DaysSinceJoined <= 30;
        
        /// <summary>
        /// Whether customer is a loyal member (more than 1 year)
        /// </summary>
        public bool IsLoyalMember => DaysSinceJoined > 365;
        
        /// <summary>
        /// Customer's initials for avatar display
        /// </summary>
        public string Initials => $"{FirstName?.FirstOrDefault()}{LastName?.FirstOrDefault()}".ToUpper();
        
        /// <summary>
        /// CSS class for membership tier badge
        /// </summary>
        public string MembershipTierBadgeClass => MembershipTier switch
        {
            CustomerTier.Bronze => "badge-bronze",
            CustomerTier.Silver => "badge-silver",
            CustomerTier.Gold => "badge-gold",
            CustomerTier.Platinum => "badge-platinum",
            _ => "badge-secondary"
        };
        
        /// <summary>
        /// Display text for membership tier
        /// </summary>
        public string MembershipTierDisplay => MembershipTier switch
        {
            CustomerTier.Bronze => "Bronze Member",
            CustomerTier.Silver => "Silver Member",
            CustomerTier.Gold => "Gold Member",
            CustomerTier.Platinum => "Platinum Member",
            _ => "Member"
        };
        
        /// <summary>
        /// Customer activity status
        /// </summary>
        public string ActivityStatus
        {
            get
            {
                if (LastOrderDate.HasValue)
                {
                    var daysSinceLastOrder = (DateTime.Now - LastOrderDate.Value).Days;
                    return daysSinceLastOrder switch
                    {
                        <= 30 => "Active",
                        <= 90 => "Moderate",
                        <= 180 => "Inactive",
                        _ => "Dormant"
                    };
                }
                return "New";
            }
        }
        
        /// <summary>
        /// CSS class for activity status badge
        /// </summary>
        public string ActivityStatusBadgeClass => ActivityStatus switch
        {
            "Active" => "bg-success",
            "Moderate" => "bg-warning",
            "Inactive" => "bg-secondary",
            "Dormant" => "bg-danger",
            "New" => "bg-primary",
            _ => "bg-secondary"
        };
        
        /// <summary>
        /// Gets the profile completion percentage
        /// </summary>
        public int ProfileCompletionPercentage
        {
            get
            {
                int totalFields = 12;
                int completedFields = 0;
                
                if (!string.IsNullOrEmpty(FirstName)) completedFields++;
                if (!string.IsNullOrEmpty(LastName)) completedFields++;
                if (!string.IsNullOrEmpty(Email)) completedFields++;
                if (!string.IsNullOrEmpty(PhoneNumber)) completedFields++;
                if (!string.IsNullOrEmpty(Address)) completedFields++;
                if (!string.IsNullOrEmpty(City)) completedFields++;
                if (!string.IsNullOrEmpty(Country)) completedFields++;
                if (DateOfBirth.HasValue) completedFields++;
                if (Gender.HasValue) completedFields++;
                if (!string.IsNullOrEmpty(Bio)) completedFields++;
                if (!string.IsNullOrEmpty(ProfilePictureUrl)) completedFields++;
                if (!string.IsNullOrEmpty(PreferredLanguage)) completedFields++;
                
                return (int)((double)completedFields / totalFields * 100);
            }
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomerProfileViewModel()
        {
            CreatedAt = DateTime.Now;
            MemberSince = DateTime.Now;
        }
        
        /// <summary>
        /// Constructor with user data
        /// </summary>
        /// <param name="user">User entity</param>
        public CustomerProfileViewModel(User user)
        {
            if (user != null)
            {
                FirstName = user.FirstName;
                LastName = user.LastName;
                Email = user.Email;
                PhoneNumber = user.PhoneNumber;
                Address = user.Address;
                City = user.City;
                Country = user.Country;
                CreatedAt = user.CreatedAt;
                MemberSince = user.CreatedAt;
            }
        }
        
        /// <summary>
        /// Validates the profile information
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateProfile()
        {
            var errors = new List<string>();
            
            if (string.IsNullOrWhiteSpace(FirstName))
                errors.Add("First name is required");
            
            if (string.IsNullOrWhiteSpace(LastName))
                errors.Add("Last name is required");
            
            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("Email is required");
            
            if (!string.IsNullOrEmpty(PhoneNumber) && PhoneNumber.Length < 10)
                errors.Add("Phone number must be at least 10 digits");
            
            if (DateOfBirth.HasValue && DateOfBirth.Value > DateTime.Now.AddYears(-13))
                errors.Add("Must be at least 13 years old");
            
            return errors;
        }
        
        /// <summary>
        /// Gets suggested improvements for profile completion
        /// </summary>
        /// <returns>List of profile improvement suggestions</returns>
        public List<string> GetProfileImprovementSuggestions()
        {
            var suggestions = new List<string>();
            
            if (string.IsNullOrEmpty(PhoneNumber))
                suggestions.Add("Add your phone number for better account security");
            
            if (string.IsNullOrEmpty(Address))
                suggestions.Add("Add your address for faster checkout");
            
            if (string.IsNullOrEmpty(ProfilePictureUrl))
                suggestions.Add("Upload a profile picture to personalize your account");
            
            if (!DateOfBirth.HasValue)
                suggestions.Add("Add your date of birth for special birthday offers");
            
            if (string.IsNullOrEmpty(Bio))
                suggestions.Add("Write a short bio to tell others about yourself");
            
            return suggestions;
        }
    }
    
    /// <summary>
    /// Enum for customer contact method preferences
    /// </summary>
    public enum ContactMethod
    {
        Email = 1,
        Phone = 2,
        SMS = 3,
        WhatsApp = 4
    }
    
    /// <summary>
    /// Enum for customer gender
    /// </summary>
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3,
        PreferNotToSay = 4
    }
    
    /// <summary>
    /// Enum for customer membership tiers
    /// </summary>
    public enum _CustomerTier
    {
        Bronze = 1,
        Silver = 2,
        Gold = 3,
        Platinum = 4
    }
}