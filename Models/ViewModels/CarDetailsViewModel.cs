using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for car details page
    /// </summary>
    public class CarDetailsViewModel
    {
        /// <summary>
        /// Main car information
        /// </summary>
        public CarViewModel Car { get; set; } = new CarViewModel();
        
        /// <summary>
        /// Similar cars from the same brand
        /// </summary>
        public List<CarViewModel> SimilarCars { get; set; } = new List<CarViewModel>();
        
        /// <summary>
        /// Cars from the same category
        /// </summary>
        public List<CarViewModel> SameCategoryCars { get; set; } = new List<CarViewModel>();
        
        /// <summary>
        /// Related car parts
        /// </summary>
        public List<CarPartViewModel> RelatedParts { get; set; } = new List<CarPartViewModel>();
        
        /// <summary>
        /// Whether the current user can add this car to cart
        /// </summary>
        public bool CanAddToCart { get; set; } = false;
        
        /// <summary>
        /// Whether the user is logged in
        /// </summary>
        public bool IsUserLoggedIn { get; set; } = false;
        
        /// <summary>
        /// Quantity to add to cart
        /// </summary>
        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
        public int Quantity { get; set; } = 1;
        
        /// <summary>
        /// Message to show when adding to cart
        /// </summary>
        public string? AddToCartMessage { get; set; }
        
        /// <summary>
        /// Whether the car is in user's wishlist
        /// </summary>
        public bool IsInWishlist { get; set; } = false;
        
        /// <summary>
        /// Whether the car is featured
        /// </summary>
        public bool IsFeatured { get; set; } = false;
        
        /// <summary>
        /// User's current rating for this car
        /// </summary>
        public int UserRating { get; set; } = 0;
        
        /// <summary>
        /// Average rating for this car
        /// </summary>
        public double AverageRating { get; set; } = 0.0;
        
        /// <summary>
        /// Total number of ratings
        /// </summary>
        public int TotalRatings { get; set; } = 0;
        
        /// <summary>
        /// List of car reviews
        /// </summary>
        public List<CarReviewViewModel> Reviews { get; set; } = new List<CarReviewViewModel>();
        
        /// <summary>
        /// Additional car images
        /// </summary>
        public List<string> AdditionalImages { get; set; } = new List<string>();
        
        /// <summary>
        /// Car specifications
        /// </summary>
        public Dictionary<string, string> Specifications { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Computed properties
        /// </summary>
        public bool IsLowStock => Car.StockQuantity <= 3;
        public bool IsOutOfStock => Car.StockQuantity == 0;
        
        public string StockMessage => IsOutOfStock ? "Out of Stock" : 
                                    IsLowStock ? $"Only {Car.StockQuantity} left in stock" : 
                                    "In Stock";
                                    
        public string StockStatusColor => IsOutOfStock ? "danger" : IsLowStock ? "warning" : "success";
        
        /// <summary>
        /// Features list parsed from Car.Features
        /// </summary>
        public List<string> FeaturesList => 
            !string.IsNullOrEmpty(Car.Features) 
                ? Car.Features.Split(',').Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).ToList()
                : new List<string>();
        
        /// <summary>
        /// Formatted price display
        /// </summary>
        public string FormattedPrice => $"LKR {Car.Price:N2}";
        
        /// <summary>
        /// Car age in years
        /// </summary>
        public int CarAge => DateTime.Now.Year - Car.Year;
        
        /// <summary>
        /// Whether the car is considered new (less than 2 years old)
        /// </summary>
        public bool IsNewCar => CarAge < 2;
        
        /// <summary>
        /// Whether the car is considered vintage (more than 25 years old)
        /// </summary>
        public bool IsVintageCar => CarAge > 25;
        
        /// <summary>
        /// Car condition based on year and mileage
        /// </summary>
        public string Condition
        {
            get
            {
                if (IsNewCar && Car.Mileage < 10000) return "Excellent";
                if (CarAge < 5 && Car.Mileage < 50000) return "Very Good";
                if (CarAge < 10 && Car.Mileage < 100000) return "Good";
                if (CarAge < 15 && Car.Mileage < 150000) return "Fair";
                return "Used";
            }
        }
        
        /// <summary>
        /// Safety rating (placeholder for future implementation)
        /// </summary>
        public int SafetyRating { get; set; } = 0;
        
        /// <summary>
        /// Fuel efficiency rating (placeholder for future implementation)
        /// </summary>
        public int FuelEfficiencyRating { get; set; } = 0;
        
        /// <summary>
        /// Breadcrumb navigation
        /// </summary>
        public List<BreadcrumbItem> Breadcrumbs { get; set; } = new List<BreadcrumbItem>();
        
        /// <summary>
        /// SEO meta information
        /// </summary>
        public string MetaTitle => $"{Car.BrandName} {Car.Model} {Car.Year} - ABC Car Traders";
        public string MetaDescription => $"Buy {Car.BrandName} {Car.Model} {Car.Year} at ABC Car Traders. {Car.FuelType} engine, {Car.Transmission} transmission. Price: LKR {Car.Price:N2}. {(IsOutOfStock ? "Contact us for availability" : "In stock now!")}";
        public string MetaKeywords => $"{Car.BrandName}, {Car.Model}, {Car.Year}, {Car.FuelType}, {Car.Transmission}, car for sale, Sri Lanka, ABC Car Traders";
        
        /// <summary>
        /// Gets the main car image URL or placeholder
        /// </summary>
        public string MainImageUrl => !string.IsNullOrEmpty(Car.ImageUrl) ? Car.ImageUrl : "/images/placeholder-car.jpg";
        
        /// <summary>
        /// Gets all available images (main + additional)
        /// </summary>
        public List<string> AllImages
        {
            get
            {
                var images = new List<string> { MainImageUrl };
                images.AddRange(AdditionalImages);
                return images;
            }
        }
        
        /// <summary>
        /// Generates sharing URL for social media
        /// </summary>
        public string SharingUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Social media sharing title
        /// </summary>
        public string SharingTitle => $"Check out this {Car.BrandName} {Car.Model} {Car.Year}!";
        
        /// <summary>
        /// Gets the number of days since the car was listed
        /// </summary>
        public int DaysSinceListed { get; set; } = 0;
        
        /// <summary>
        /// Whether the car is recently listed (within 7 days)
        /// </summary>
        public bool IsRecentlyListed => DaysSinceListed <= 7;
        
        /// <summary>
        /// Financing options (placeholder for future implementation)
        /// </summary>
        public List<FinancingOption> FinancingOptions { get; set; } = new List<FinancingOption>();
        
        /// <summary>
        /// Insurance options (placeholder for future implementation)
        /// </summary>
        public List<InsuranceOption> InsuranceOptions { get; set; } = new List<InsuranceOption>();
        
        /// <summary>
        /// Gets basic car specifications as a dictionary
        /// </summary>
        public Dictionary<string, string> BasicSpecifications
        {
            get
            {
                var specs = new Dictionary<string, string>
                {
                    { "Brand", Car.BrandName },
                    { "Model", Car.Model },
                    { "Year", Car.Year.ToString() },
                    { "Fuel Type", Car.FuelType.ToString() },
                    { "Transmission", Car.Transmission.ToString() },
                    { "Condition", Condition }
                };
                
                if (!string.IsNullOrEmpty(Car.Color))
                    specs.Add("Color", Car.Color);
                    
                if (Car.Mileage.HasValue)
                    specs.Add("Mileage", $"{Car.Mileage:N0} km");
                    
                if (!string.IsNullOrEmpty(Car.EngineCapacity))
                    specs.Add("Engine Capacity", Car.EngineCapacity);
                
                return specs;
            }
        }
        
        /// <summary>
        /// Validates the view model
        /// </summary>
        public List<string> Validate()
        {
            var errors = new List<string>();
            
            if (Car == null)
                errors.Add("Car information is required");
            
            if (Quantity < 1 || Quantity > 10)
                errors.Add("Quantity must be between 1 and 10");
                
            return errors;
        }
    }
    
    /// <summary>
    /// View model for car reviews
    /// </summary>
    public class CarReviewViewModel
    {
        public int ReviewId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        public bool IsVerifiedPurchase { get; set; } = false;
        public string FormattedDate => ReviewDate.ToString("MMM dd, yyyy");
        public string CustomerInitials => CustomerName.Split(' ').Take(2).Select(n => n[0]).Aggregate("", (acc, c) => acc + c);
    }
    
    /// <summary>
    /// Financing option for cars
    /// </summary>
    public class FinancingOption
    {
        public int FinancingId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public decimal InterestRate { get; set; }
        public int MaxTermMonths { get; set; }
        public decimal MinDownPayment { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public decimal MonthlyPayment { get; set; }
        public string FormattedInterestRate => $"{InterestRate:F2}%";
        public string FormattedMonthlyPayment => $"LKR {MonthlyPayment:N2}";
    }
    
    /// <summary>
    /// Insurance option for cars
    /// </summary>
    public class InsuranceOption
    {
        public int InsuranceId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public decimal AnnualPremium { get; set; }
        public decimal CoverageAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Benefits { get; set; } = new List<string>();
        public string FormattedPremium => $"LKR {AnnualPremium:N2}";
        public string FormattedCoverage => $"LKR {CoverageAmount:N2}";
    }
}