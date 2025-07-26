using ABCCarTraders.Models;

namespace ABCCarTraders.Services
{
    public interface IOrderService
    {
        // Order management methods
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<bool> CreateOrderAsync(Order order);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId);

        // Order status management
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<bool> UpdatePaymentStatusAsync(int orderId, PaymentStatus status);
        Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<List<Order>> GetOrdersByPaymentStatusAsync(PaymentStatus status);

        // Order processing
        Task<string> GenerateOrderNumberAsync();
        Task<bool> ProcessOrderAsync(int userId, List<Cart> cartItems, string shippingAddress, PaymentMethod paymentMethod, string? notes = null);
        Task<bool> ConfirmOrderAsync(int orderId);
        Task<bool> ShipOrderAsync(int orderId);
        Task<bool> DeliverOrderAsync(int orderId);

        // Order item management
        Task<List<OrderItem>> GetOrderItemsAsync(int orderId);
        Task<bool> AddOrderItemAsync(OrderItem orderItem);
        Task<bool> UpdateOrderItemAsync(OrderItem orderItem);
        Task<bool> RemoveOrderItemAsync(int orderItemId);

        // Cart management
        Task<List<Cart>> GetUserCartAsync(int userId);
        Task<bool> AddToCartAsync(int userId, ItemType itemType, int itemId, int quantity);
        Task<bool> UpdateCartItemAsync(int cartId, int quantity);
        Task<bool> RemoveFromCartAsync(int cartId);
        Task<bool> ClearCartAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);

        // Order statistics and reporting
        Task<int> GetTotalOrdersCountAsync();
        Task<int> GetPendingOrdersCountAsync();
        Task<int> GetCompletedOrdersCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetMonthlyRevenueAsync();
        Task<Dictionary<string, int>> GetOrderStatisticsAsync();
        Task<List<Order>> GetRecentOrdersAsync(int count = 10);
        Task<List<Order>> GetTopOrdersAsync(int count = 10);

        // Customer order history
        Task<List<Order>> GetCustomerRecentOrdersAsync(int userId, int count);
        Task<List<Order>> GetCustomerOrdersAsync(int userId, int page, int pageSize);
        Task<List<Order>> GetCustomerOrderHistoryAsync(int userId);
        Task<Order?> GetLatestOrderAsync(int userId);
        Task<int> GetCustomerOrderCountAsync(int userId);
        Task<decimal> GetCustomerTotalSpentAsync(int userId);

        // Search and filtering
        Task<List<Order>> SearchOrdersAsync(string searchTerm);
        Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Order>> GetOrdersByAmountRangeAsync(decimal minAmount, decimal maxAmount);
        Task<List<Order>> GetFilteredOrdersAsync(OrderFilterModel filter);

        // Validation
        Task<bool> ValidateCartAsync(int userId);
        Task<bool> CanCancelOrderAsync(int orderId);
        Task<bool> IsOrderOwnedByUserAsync(int orderId, int userId);

        // Inventory management
        Task<bool> ReserveInventoryAsync(List<OrderItem> orderItems);
        Task<bool> ReleaseInventoryAsync(List<OrderItem> orderItems);
        Task<bool> UpdateInventoryAsync(List<OrderItem> orderItems);

        // Financial calculations
        Task<decimal> CalculateOrderTotalAsync(List<OrderItem> orderItems);
        Task<decimal> CalculateShippingCostAsync(List<OrderItem> orderItems);
        Task<decimal> CalculateTaxAsync(decimal subtotal);

        // Notifications and emails
        Task<bool> SendOrderConfirmationAsync(int orderId);
        Task<bool> SendOrderStatusUpdateAsync(int orderId);
        Task<bool> SendShippingNotificationAsync(int orderId);
    }

    // Helper model for filtering orders
    public class OrderFilterModel
    {
        public string? SearchTerm { get; set; }
        public int? UserId { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; }
    }
}