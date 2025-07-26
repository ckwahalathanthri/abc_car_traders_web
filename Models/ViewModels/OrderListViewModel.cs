using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for order list page
    /// </summary>
    public class OrderListViewModel
    {
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public OrderSearchViewModel SearchModel { get; set; } = new OrderSearchViewModel();
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
        
        // Filter options
        public List<SelectListItem> OrderStatuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PaymentStatuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PaymentMethods { get; set; } = new List<SelectListItem>();
        
        // Statistics
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        
        public string FormattedTotalRevenue => TotalRevenue.ToString("C");
        public string FormattedAverageOrderValue => AverageOrderValue.ToString("C");
        
        // Constructor
        public OrderListViewModel()
        {
            LoadDropdownData();
        }
        
        private void LoadDropdownData()
        {
            // Load order statuses
            OrderStatuses = Enum.GetValues<OrderStatus>()
                .Select(os => new SelectListItem
                {
                    Value = ((int)os).ToString(),
                    Text = os.ToString()
                }).ToList();
            
            // Load payment statuses
            PaymentStatuses = Enum.GetValues<PaymentStatus>()
                .Select(ps => new SelectListItem
                {
                    Value = ((int)ps).ToString(),
                    Text = ps.ToString()
                }).ToList();
            
            // Load payment methods
            PaymentMethods = Enum.GetValues<PaymentMethod>()
                .Select(pm => new SelectListItem
                {
                    Value = ((int)pm).ToString(),
                    Text = pm.ToString().Replace("Card", " Card")
                }).ToList();
        }
    }
    
    /// <summary>
    /// View model for order search filters
    /// </summary>
    public class OrderSearchViewModel
    {
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }
        
        [Display(Name = "Order Status")]
        public OrderStatus? OrderStatus { get; set; }
        
        [Display(Name = "Payment Status")]
        public PaymentStatus? PaymentStatus { get; set; }
        
        [Display(Name = "Payment Method")]
        public PaymentMethod? PaymentMethod { get; set; }
        
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        
        [Display(Name = "Min Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Min amount cannot be negative")]
        public decimal? MinAmount { get; set; }
        
        [Display(Name = "Max Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Max amount cannot be negative")]
        public decimal? MaxAmount { get; set; }
        
        [Display(Name = "Sort By")]
        public string? SortBy { get; set; }
        
        [Display(Name = "Order")]
        public bool IsDescending { get; set; } = true;
        
        // Results per page
        public int PageSize { get; set; } = 15;
        public int PageNumber { get; set; } = 1;
    }
}