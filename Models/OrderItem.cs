using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCCarTraders.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        
        [Required]
        [Display(Name = "Order")]
        public int OrderId { get; set; }
        
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
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        [Display(Name = "Unit Price")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0")]
        [Display(Name = "Total Price")]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalPrice { get; set; }
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual Order? Order { get; set; }
        
        // Computed properties
        [NotMapped]
        public string FormattedUnitPrice => UnitPrice.ToString("C");
        
        [NotMapped]
        public string FormattedTotalPrice => TotalPrice.ToString("C");
        
        [NotMapped]
        public string ItemTypeName => ItemType == ItemType.Car ? "Car" : "Car Part";
        
        // Methods to get item details
        public string GetItemName()
        {
            // This would be implemented in the service layer or through additional navigation
            return ItemType == ItemType.Car ? "Car Details" : "Car Part Details";
        }
        
        public void CalculateTotalPrice()
        {
            TotalPrice = UnitPrice * Quantity;
        }
    }
    
    public enum ItemType
    {
        Car,
        CarPart
    }
}