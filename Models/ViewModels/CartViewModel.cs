using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for shopping cart
    /// </summary>
    public class CartViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal TaxAmount { get; set; } = 0; // Alias for Tax
        public decimal GrandTotal { get; set; }
        public decimal Total => GrandTotal; // Alias for GrandTotal
        public int TotalItems { get; set; }
        public bool IsValid { get; set; } = true;
        
        public string FormattedSubTotal => SubTotal.ToString("C");
        public string FormattedShippingCost => ShippingCost.ToString("C");
        public string FormattedTax => Tax.ToString("C");
        public string FormattedTaxAmount => TaxAmount.ToString("C");
        public string FormattedGrandTotal => GrandTotal.ToString("C");
        public string FormattedTotal => Total.ToString("C");
        
        public bool HasItems => CartItems.Any();
        public bool HasInvalidItems => CartItems.Any(ci => !ci.IsAvailable || !ci.HasSufficientStock);
        public bool QualifiesForFreeShipping => SubTotal >= 1000;
        public decimal AmountForFreeShipping => Math.Max(0, 1000 - SubTotal);
        public string FormattedAmountForFreeShipping => AmountForFreeShipping.ToString("C");
        
        // Cart summary statistics
        public int CarCount => CartItems.Count(ci => ci.ItemType == ItemType.Car);
        public int CarPartCount => CartItems.Count(ci => ci.ItemType == ItemType.CarPart);
        public decimal CarsSubTotal => CartItems.Where(ci => ci.ItemType == ItemType.Car).Sum(ci => ci.TotalPrice);
        public decimal CarPartsSubTotal => CartItems.Where(ci => ci.ItemType == ItemType.CarPart).Sum(ci => ci.TotalPrice);
        
        public string FormattedCarsSubTotal => CarsSubTotal.ToString("C");
        public string FormattedCarPartsSubTotal => CarPartsSubTotal.ToString("C");
        
        // Shipping calculation details
        public bool HasFreeShippingItems => CartItems.Any(ci => ci.ItemType == ItemType.Car); // Cars get free shipping
        public string ShippingMessage => QualifiesForFreeShipping 
            ? "FREE SHIPPING" 
            : $"Spend {FormattedAmountForFreeShipping} more for free shipping";
        
        // Tax calculation
        public decimal TaxRate { get; set; } = 0.10m; // 10% default tax rate
        public string FormattedTaxRate => (TaxRate * 100).ToString("F1") + "%";
        
        // Validation messages
        public List<string> ValidationMessages { get; set; } = new List<string>();
        
        public void AddValidationMessage(string message)
        {
            if (!ValidationMessages.Contains(message))
            {
                ValidationMessages.Add(message);
            }
        }
        
        public void ClearValidationMessages()
        {
            ValidationMessages.Clear();
        }
        
        // Helper methods
        public void RecalculateTotals()
        {
            SubTotal = CartItems.Sum(ci => ci.TotalPrice);
            ShippingCost = SubTotal > 1000 ? 0 : 50;
            Tax = SubTotal * TaxRate;
            TaxAmount = Tax; // Keep both properties in sync
            GrandTotal = SubTotal + ShippingCost + Tax;
            TotalItems = CartItems.Sum(ci => ci.Quantity);
        }
        
        public void ValidateCart()
        {
            ClearValidationMessages();
            IsValid = true;
            
            foreach (var item in CartItems)
            {
                if (!item.IsAvailable)
                {
                    AddValidationMessage($"{item.ItemName} is no longer available");
                    IsValid = false;
                }
                
                if (!item.HasSufficientStock)
                {
                    AddValidationMessage($"Only {item.StockQuantity} {item.ItemName} in stock (you have {item.Quantity} in cart)");
                    IsValid = false;
                }
                
                if (item.IsLowStock)
                {
                    AddValidationMessage($"{item.ItemName} has low stock ({item.StockQuantity} remaining)");
                }
            }
        }
    }
}