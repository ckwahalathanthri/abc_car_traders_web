using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for customer orders
    /// </summary>
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public OrderStatus OrderStatus => Status; // Alias for backwards compatibility
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Total { get; set; }
        public decimal TotalAmount => Total; // Alias for backwards compatibility
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int ItemCount { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string? TrackingNumber { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
        
        // Navigation properties for admin view
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        
        public string FormattedTotal => Total.ToString("C");
        public string FormattedTotalAmount => TotalAmount.ToString("C");
        public string FormattedOrderDate => OrderDate.ToString("MMM dd, yyyy");
        public string FormattedUpdatedAt => UpdatedAt.ToString("MMM dd, yyyy HH:mm");
        public string FormattedDeliveryDate => DeliveryDate?.ToString("MMM dd, yyyy") ?? "TBD";
        public string StatusDisplay => Status.ToString();
        public string StatusBadgeClass => Status switch
        {
            OrderStatus.Pending => "bg-warning",
            OrderStatus.Confirmed => "bg-info",
            OrderStatus.Processing => "bg-primary",
            OrderStatus.Shipped => "bg-secondary",
            OrderStatus.Delivered => "bg-success",
            OrderStatus.Cancelled => "bg-danger",
            _ => "bg-secondary"
        };
        
        public int TotalItems => OrderItems.Sum(oi => oi.Quantity);
        public bool CanCancel => Status == OrderStatus.Pending;
        public bool CanBeCancelled => Status == OrderStatus.Pending || Status == OrderStatus.Confirmed;
        public bool CanBeShipped => Status == OrderStatus.Confirmed || Status == OrderStatus.Processing;
        public bool CanBeDelivered => Status == OrderStatus.Shipped;
        public bool CanTrack => !string.IsNullOrEmpty(TrackingNumber) && (Status == OrderStatus.Shipped || Status == OrderStatus.Processing);
        
        public string StatusColor => Status switch
        {
            OrderStatus.Pending => "warning",
            OrderStatus.Confirmed => "info",
            OrderStatus.Processing => "primary",
            OrderStatus.Shipped => "secondary",
            OrderStatus.Delivered => "success",
            OrderStatus.Cancelled => "danger",
            _ => "secondary"
        };
        
        public string PaymentStatusColor => PaymentStatus switch
        {
            PaymentStatus.Pending => "warning",
            PaymentStatus.Paid => "success",
            PaymentStatus.Failed => "danger",
            PaymentStatus.Refunded => "info",
            _ => "secondary"
        };
        
        // Convert to Order entity
        public Order ToOrder()
        {
            return new Order
            {
                OrderId = OrderId,
                UserId = UserId,
                OrderNumber = OrderNumber,
                TotalAmount = TotalAmount,
                OrderStatus = (Models.OrderStatus)(int)Status, // Convert to Models enum
                PaymentStatus = (Models.PaymentStatus)(int)PaymentStatus, // Convert to Models enum
                PaymentMethod = (Models.PaymentMethod)(int)PaymentMethod, // Convert to Models enum
                ShippingAddress = ShippingAddress,
                Notes = Notes,
                OrderDate = OrderDate,
                UpdatedAt = UpdatedAt
            };
        }
        
        // Static factory method for creating from Order entity
        public static OrderViewModel FromOrder(Order order)
        {
            return new OrderViewModel
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = (OrderStatus)(int)order.OrderStatus, // Convert from Models.OrderStatus to ViewModels.OrderStatus
                PaymentStatus = (PaymentStatus)(int)order.PaymentStatus, // Convert from Models.PaymentStatus
                PaymentMethod = (PaymentMethod)(int)order.PaymentMethod, // Convert from Models.PaymentMethod
                Total = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                UpdatedAt = order.UpdatedAt,
                CustomerName = order.User != null ? $"{order.User.FirstName} {order.User.LastName}" : "",
                CustomerEmail = order.User?.Email ?? "",
                CustomerPhone = order.User?.PhoneNumber ?? "",
                OrderItems = order.OrderItems?.Select(OrderItemViewModel.FromOrderItem).ToList() ?? new List<OrderItemViewModel>()
            };
        }
    }
}