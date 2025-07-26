using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for order items
    /// </summary>
    public class OrderItemViewModel
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int ItemId => ProductId; // Alias for backwards compatibility
        public string ProductType { get; set; } = "Car"; // Car or CarPart
        public ItemType ItemType => ProductType == "Car" ? Models.ItemType.Car : Models.ItemType.CarPart;
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Item details (populated by controller)
        public string ItemName { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string ItemImageUrl { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        
        public string FormattedUnitPrice => UnitPrice.ToString("C");
        public string FormattedTotalPrice => TotalPrice.ToString("C");
        public string ProductUrl => ProductType == "Car" ? $"/Cars/Details/{ProductId}" : $"/CarParts/Details/{ProductId}";
        public string ItemTypeName => ProductType == "Car" ? "Car" : "Car Part";
        
        // Convert to OrderItem entity
        public OrderItem ToOrderItem()
        {
            return new OrderItem
            {
                OrderItemId = OrderItemId,
                OrderId = OrderId,
                ItemType = ItemType,
                ItemId = ItemId,
                Quantity = Quantity,
                UnitPrice = UnitPrice,
                TotalPrice = TotalPrice,
                CreatedAt = CreatedAt
            };
        }
        
        // Static factory method for creating from OrderItem entity
        public static OrderItemViewModel FromOrderItem(OrderItem orderItem)
        {
            return new OrderItemViewModel
            {
                OrderItemId = orderItem.OrderItemId,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ItemId,
                ProductType = orderItem.ItemType == Models.ItemType.Car ? "Car" : "CarPart",
                UnitPrice = orderItem.UnitPrice,
                Quantity = orderItem.Quantity,
                CreatedAt = orderItem.CreatedAt
            };
        }
    }
}