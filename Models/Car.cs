using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCCarTraders.Models
{
    public class Car
    {
        public int CarId { get; set; }
        
        [Required]
        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Model")]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
        [Display(Name = "Year")]
        public int Year { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Color")]
        public string? Color { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        [Column(TypeName = "decimal(15,2)")]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Mileage cannot be negative")]
        [Display(Name = "Mileage")]
        public int? Mileage { get; set; }
        
        [Required]
        [Display(Name = "Fuel Type")]
        public FuelType FuelType { get; set; }
        
        [Required]
        [Display(Name = "Transmission")]
        public Transmission Transmission { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Engine Capacity")]
        public string? EngineCapacity { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Display(Name = "Features")]
        public string? Features { get; set; }
        
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; } = 1;
        
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties - Note: These may not be configured in some contexts
        public virtual Brand? Brand { get; set; }
        public virtual Category? Category { get; set; }
        
        // Computed properties
        [NotMapped]
        public string FormattedPrice => Price.ToString("C");
        
        [NotMapped]
        public string FormattedMileage => Mileage?.ToString("N0") ?? "N/A";
        
        [NotMapped]
        public bool InStock => StockQuantity > 0;
        
        [NotMapped]
        public string StockStatus => InStock ? "In Stock" : "Out of Stock";
        
        [NotMapped]
        public string AvailabilityStatus => IsAvailable ? "Available" : "Unavailable";
        
        [NotMapped]
        public bool IsLowStock => StockQuantity > 0 && StockQuantity <= 3;
        
        [NotMapped]
        public string FormattedCreatedAt => CreatedAt.ToString("MMM dd, yyyy");
        
        [NotMapped]
        public string FormattedUpdatedAt => UpdatedAt.ToString("MMM dd, yyyy");
        
        [NotMapped]
        public string DisplayName => $"{Brand?.BrandName ?? "Unknown"} {Model} ({Year})";
        
        [NotMapped]
        public string ShortDescription => $"{Brand?.BrandName ?? "Unknown"} {Model}";
    }
    
    public enum FuelType
    {
        Petrol,
        Diesel,
        Electric,
        Hybrid
    }
    
    public enum Transmission
    {
        Manual,
        Automatic
    }
}