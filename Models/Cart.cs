using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCCarTraders.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        
        [Required]
        [Display(Name = "User")]
        public int UserId { get; set; }
        
        [Required]
        [Display(Name = "Item Type")]
        public ItemType ItemType { get; set; }
        
        [Required]
        [Display(Name = "Item ID")]
        public int ItemId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        
        [Display(Name = "Added At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual User? User { get; set; }
        
        // Computed properties
        [NotMapped]
        public string ItemTypeName => ItemType == ItemType.Car ? "Car" : "Car Part";
        
        [NotMapped]
        public string FormattedCreatedAt => CreatedAt.ToString("MMM dd, yyyy HH:mm");
        
        // Methods to get item details (these would be populated by the service layer)
        [NotMapped]
        public string ItemName { get; set; } = string.Empty;
        
        [NotMapped]
        public decimal UnitPrice { get; set; }
        
        [NotMapped]
        public decimal TotalPrice => UnitPrice * Quantity;
        
        [NotMapped]
        public string FormattedUnitPrice => UnitPrice.ToString("C");
        
        [NotMapped]
        public string FormattedTotalPrice => TotalPrice.ToString("C");
        
        [NotMapped]
        public string? ImageUrl { get; set; }
        
        [NotMapped]
        public bool IsAvailable { get; set; } = true;
        
        [NotMapped]
        public int StockQuantity { get; set; }
        
        [NotMapped]
        public bool HasSufficientStock => StockQuantity >= Quantity;
        
        [NotMapped]
        public string StockStatus => HasSufficientStock ? "In Stock" : "Insufficient Stock";
        
        [NotMapped]
        public string StockStatusColor => HasSufficientStock ? "success" : "danger";
    }
}