using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for customer order history
    /// </summary>
    public class CustomerOrderHistoryViewModel
    {
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
        
        public string FormattedTotalSpent => TotalSpent.ToString("C");
        public string FormattedAverageOrderValue => AverageOrderValue.ToString("C");
    }
}