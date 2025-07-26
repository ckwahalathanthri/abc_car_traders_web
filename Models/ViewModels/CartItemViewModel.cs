using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for cart items
    /// </summary>
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int UserId { get; set; }
        
        public int ProductId { get; set; }
        public int ItemId => ProductId; // Alias for backwards compatibility
        
        public string ProductType { get; set; } = "Car"; // Car or CarPart
        public ItemType ItemType => ProductType == "Car" ? Models.ItemType.Car : Models.ItemType.CarPart; // Enum version
        
        public string ProductName { get; set; } = string.Empty;
        public string ItemName => ProductName; // Alias for backwards compatibility
        
        public string ProductImage { get; set; } = string.Empty;
        public string ItemImageUrl => ProductImage; // Alias for backwards compatibility
        
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public bool IsAvailable { get; set; } = true;
        public int StockQuantity { get; set; }
        public bool HasSufficientStock => StockQuantity >= Quantity;
        public DateTime DateAdded { get; set; }
        public DateTime CreatedAt => DateAdded; // Alias for backwards compatibility
        
        // Additional properties for enhanced functionality
        public string ItemDescription { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public bool IsLowStock => StockQuantity > 0 && StockQuantity <= 5;
        
        public string FormattedUnitPrice => UnitPrice.ToString("C");
        public string FormattedTotalPrice => TotalPrice.ToString("C");
        public string ProductUrl => ProductType == "Car" ? $"/Cars/Details/{ProductId}" : $"/CarParts/Details/{ProductId}";
        public string ItemTypeName => ProductType == "Car" ? "Car" : "Car Part";
        public string StockStatus => HasSufficientStock ? "In Stock" : "Insufficient Stock";
        public string StockStatusColor => HasSufficientStock ? "success" : "danger";
        
        // Convert to Cart entity
        public Cart ToCart()
        {
            return new Cart
            {
                CartId = CartId,
                UserId = UserId,
                ItemType = ItemType,
                ItemId = ItemId,
                Quantity = Quantity,
                CreatedAt = CreatedAt
            };
        }
        
        // Static factory method for creating from Cart entity
        public static CartItemViewModel FromCart(Cart cart)
        {
            return new CartItemViewModel
            {
                CartId = cart.CartId,
                CartItemId = cart.CartId, // Use CartId as CartItemId for now
                UserId = cart.UserId,
                ProductType = cart.ItemType == Models.ItemType.Car ? "Car" : "CarPart",
                ProductId = cart.ItemId,
                Quantity = cart.Quantity,
                DateAdded = cart.CreatedAt
            };
        }
    }
}