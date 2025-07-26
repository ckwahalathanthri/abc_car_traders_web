using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for checkout process
    /// </summary>
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Shipping address is required")]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Country is required")]
        [Display(Name = "Country")]
        public string Country { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please select a payment method")]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Display(Name = "Order Notes")]
        public string? Notes { get; set; }
        
        // Order summary
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Total => GrandTotal; // Alias for compatibility
        public int TotalItems { get; set; }
        
        // Cart items for display
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        
        // Form data
        public List<SelectListItem> PaymentMethods { get; set; } = new List<SelectListItem>();
        public string? ErrorMessage { get; set; }
        public bool IsProcessing { get; set; } = false;
        
        // Customer information (for existing customers)
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        
        // Formatted properties
        public string FormattedSubTotal => SubTotal.ToString("C");
        public string FormattedShippingCost => ShippingCost.ToString("C");
        public string FormattedTax => Tax.ToString("C");
        public string FormattedGrandTotal => GrandTotal.ToString("C");
        public string FormattedTotal => Total.ToString("C");
        public string FullShippingAddress => $"{ShippingAddress}, {City}, {Country}";
        
        // Constructor
        public CheckoutViewModel()
        {
            LoadPaymentMethods();
        }
        
        private void LoadPaymentMethods()
        {
            PaymentMethods = Enum.GetValues<PaymentMethod>()
                .Select(pm => new SelectListItem
                {
                    Value = ((int)pm).ToString(),
                    Text = pm.ToString().Replace("Card", " Card")
                }).ToList();
        }
    }
}