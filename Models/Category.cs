using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCCarTraders.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Required]
        [Display(Name = "Category Type")]
        public CategoryType CategoryType { get; set; }
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
        public virtual ICollection<CarPart> CarParts { get; set; } = new List<CarPart>();
        
        // Computed properties
        [NotMapped]
        public string CategoryTypeName => CategoryType == CategoryType.Car ? "Car" : "Car Part";
        
        [NotMapped]
        public int TotalItems => CategoryType == CategoryType.Car ? Cars.Count : CarParts.Count;
        
        [NotMapped]
        public int ActiveItems => CategoryType == CategoryType.Car 
            ? Cars.Count(c => c.IsAvailable) 
            : CarParts.Count(cp => cp.IsAvailable);
        
        [NotMapped]
        public string StatusBadge => IsActive ? "Active" : "Inactive";
        
        [NotMapped]
        public string StatusColor => IsActive ? "success" : "danger";
        
        [NotMapped]
        public string FormattedCreatedAt => CreatedAt.ToString("MMM dd, yyyy");
    }
    
    public enum CategoryType
    {
        Car,
        CarPart
    }
}