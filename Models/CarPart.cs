using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCCarTraders.Models
{
    public class CarPart
    {
        public int CarPartId { get; set; }
        
        [Required]
        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        
        [Required]
        [StringLength(200)]
        [Display(Name = "Part Name")]
        public string PartName { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Display(Name = "Compatibility")]
        public string? Compatibility { get; set; }
        
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; } = 0;
        
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual Brand? Brand { get; set; }
        public virtual Category? Category { get; set; }
        
        // Computed properties
        [NotMapped]
        public string FormattedPrice => Price.ToString("C");
        
        [NotMapped]
        public bool InStock => StockQuantity > 0;
        
        [NotMapped]
        public string StockStatus => InStock ? "In Stock" : "Out of Stock";
    }
}