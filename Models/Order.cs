using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCCarTraders.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        
        [Required]
        [Display(Name = "Customer")]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        [Display(Name = "Total Amount")]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [Display(Name = "Order Status")]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        
        [Required]
        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        
        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Required]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }
        
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
        
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        // Computed properties
        [NotMapped]
        public string FormattedTotalAmount => TotalAmount.ToString("C");
        
        [NotMapped]
        public string FormattedOrderDate => OrderDate.ToString("MMM dd, yyyy");
        
        [NotMapped]
        public int TotalItems => OrderItems.Sum(oi => oi.Quantity);
        
        [NotMapped]
        public bool CanBeCancelled => OrderStatus == OrderStatus.Pending || OrderStatus == OrderStatus.Confirmed;
        
        [NotMapped]
        public string StatusColor => OrderStatus switch
        {
            OrderStatus.Pending => "warning",
            OrderStatus.Confirmed => "info",
            OrderStatus.Processing => "primary",
            OrderStatus.Shipped => "secondary",
            OrderStatus.Delivered => "success",
            OrderStatus.Cancelled => "danger",
            _ => "secondary"
        };
        
        [NotMapped]
        public string PaymentStatusColor => PaymentStatus switch
        {
            PaymentStatus.Pending => "warning",
            PaymentStatus.Paid => "success",
            PaymentStatus.Failed => "danger",
            PaymentStatus.Refunded => "info",
            _ => "secondary"
        };
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
    
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }
    
    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        BankTransfer,
        Cash
    }
}