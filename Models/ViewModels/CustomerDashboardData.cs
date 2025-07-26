using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for customer dashboard data
    /// </summary>
    public class CustomerDashboardData
    {
        // Order summary
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal MonthlySpent { get; set; }
        public decimal AverageOrderValue => TotalOrders > 0 ? TotalSpent / TotalOrders : 0;

        // Cart data
        public int CartItemsCount { get; set; }
        public decimal CartTotal { get; set; }

        // Recent & latest orders
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public Order? LatestOrder { get; set; }
        public List<OrderSummary> RecentOrderSummaries { get; set; } = new List<OrderSummary>();

        // Recommendations
        public List<Car> RecommendedCars { get; set; } = new List<Car>();
        public List<CarPart> RecommendedCarParts { get; set; } = new List<CarPart>();

        // Wishlist
        public List<Car> WishlistCars { get; set; } = new List<Car>();
        public List<CarPart> WishlistCarParts { get; set; } = new List<CarPart>();
        public int WishlistItemCount => WishlistCars.Count + WishlistCarParts.Count;

        // Membership & Loyalty
        public DateTime MemberSince { get; set; }
        public int LoyaltyPoints { get; set; }
        public CustomerTier MembershipTier { get; set; } = CustomerTier.Bronze;

        // Profile completion
        public bool IsProfileComplete { get; set; }
        public List<string> MissingProfileFields { get; set; } = new List<string>();

        // Monthly stats
        public int NewCustomersThisMonth { get; set; } // Primarily for admin, included for completeness

        // Formatted display properties
        public string FormattedTotalSpent => $"LKR {TotalSpent:N2}";
        public string FormattedMonthlySpent => $"LKR {MonthlySpent:N2}";
        public string FormattedMemberSince => MemberSince.ToString("MMMM yyyy");
        public string FormattedAverageOrderValue => $"LKR {AverageOrderValue:N2}";

    }

    /// <summary>
    /// View model for order summary
    /// </summary>
    public class OrderSummary
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }

        public string FormattedTotal => $"LKR {Total:N2}";
        public string FormattedOrderDate => OrderDate.ToString("MMM dd, yyyy");
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
    }

    /// <summary>
    /// View model for order confirmation page
    /// </summary>
    public class OrderConfirmationViewModel
    {
        public OrderViewModel Order { get; set; } = new OrderViewModel();
        public DateTime EstimatedDeliveryDate { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string SupportEmail { get; set; } = string.Empty;
        public string SupportPhone { get; set; } = string.Empty;
        public string ConfirmationMessage { get; set; } = string.Empty;
        public bool EmailSent { get; set; }

        // Next steps
        public List<string> NextSteps { get; set; } = new List<string>();

        // Related recommendations
        public List<CarViewModel> RecommendedCars { get; set; } = new List<CarViewModel>();
        public List<CarPartViewModel> RecommendedParts { get; set; } = new List<CarPartViewModel>();

        public string FormattedEstimatedDeliveryDate => EstimatedDeliveryDate.ToString("MMM dd, yyyy");
    }

    /// <summary>
    /// View model for order details page
    /// </summary>
    public class OrderDetailsViewModel
    {
        public OrderViewModel Order { get; set; } = new OrderViewModel();
        public List<OrderItemDetailsViewModel> OrderItems { get; set; } = new List<OrderItemDetailsViewModel>();
        public bool CanCancel { get; set; }
    }

    /// <summary>
    /// View model for detailed order item information
    /// </summary>
    public class OrderItemDetailsViewModel
    {
        public OrderItemViewModel OrderItem { get; set; } = new OrderItemViewModel();
        public ItemType ItemType { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string ItemImageUrl { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for order tracking page
    /// </summary>
    public class OrderTrackingViewModel
    {
        public OrderViewModel Order { get; set; } = new OrderViewModel();
        public List<TrackingStep> TrackingSteps { get; set; } = new List<TrackingStep>();
        public DateTime EstimatedDeliveryDate { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;

        public string FormattedEstimatedDeliveryDate => EstimatedDeliveryDate.ToString("MMM dd, yyyy");
    }

    /// <summary>
    /// View model for tracking step information
    /// </summary>
    public class TrackingStep
    {
        public string Status { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CompletedDate { get; set; }

        public string FormattedCompletedDate => CompletedDate?.ToString("MMM dd, yyyy HH:mm") ?? "";
    }

    public enum CustomerTier
    {
        Bronze,
        Silver,
        Gold,
        Platinum
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}