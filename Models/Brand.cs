using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCCarTraders.Models
{
    public class Brand
    {
        public int BrandId { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Display(Name = "Logo URL")]
        public string? LogoUrl { get; set; }
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
        public virtual ICollection<CarPart> CarParts { get; set; } = new List<CarPart>();
        
        // Computed properties
        [NotMapped]
        public int TotalCars => Cars.Count;
        
        [NotMapped]
        public int TotalCarParts => CarParts.Count;
        
        [NotMapped]
        public int TotalProducts => TotalCars + TotalCarParts;
        
        [NotMapped]
        public int AvailableCars => Cars.Count(c => c.IsAvailable);
        
        [NotMapped]
        public int AvailableCarParts => CarParts.Count(cp => cp.IsAvailable);
        
        [NotMapped]
        public int TotalAvailableProducts => AvailableCars + AvailableCarParts;
        
        [NotMapped]
        public string StatusBadge => IsActive ? "Active" : "Inactive";
        
        [NotMapped]
        public string StatusColor => IsActive ? "success" : "danger";
        
        [NotMapped]
        public string FormattedCreatedAt => CreatedAt.ToString("MMM dd, yyyy");
        
        [NotMapped]
        public bool HasLogo => !string.IsNullOrEmpty(LogoUrl);
        
        [NotMapped]
        public string DisplayName => BrandName;
        
        // Methods
        public decimal GetAverageCarPrice()
        {
            return Cars.Any() ? Cars.Where(c => c.IsAvailable).Average(c => c.Price) : 0;
        }
        
        public decimal GetAverageCarPartPrice()
        {
            return CarParts.Any() ? CarParts.Where(cp => cp.IsAvailable).Average(cp => cp.Price) : 0;
        }
        
        public Car? GetMostExpensiveCar()
        {
            return Cars.Where(c => c.IsAvailable).OrderByDescending(c => c.Price).FirstOrDefault();
        }
        
        public CarPart? GetMostExpensiveCarPart()
        {
            return CarParts.Where(cp => cp.IsAvailable).OrderByDescending(cp => cp.Price).FirstOrDefault();
        }
    }
}