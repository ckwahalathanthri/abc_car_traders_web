using ABCCarTraders.Models;

namespace ABCCarTraders.Models.ViewModels
{
    public class DashboardViewModel
    {
        public UserType UserType { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime LastLoginDate { get; set; }

        // Common statistics
        public Dictionary<string, int> Statistics { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> FinancialData { get; set; } = new Dictionary<string, decimal>();

        // Admin specific data
        public AdminDashboardData? AdminData { get; set; }

        // Customer specific data
        public CustomerDashboardData? CustomerData { get; set; }

        // Recent activities
        public List<RecentActivity> RecentActivities { get; set; } = new List<RecentActivity>();

        // Notifications
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public class AdminDashboardData
    {
        public int TotalCars { get; set; }
        public int TotalCarParts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal DailyRevenue { get; set; }

        // Recent data
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<User> RecentCustomers { get; set; } = new List<User>();
        public List<Car> LowStockCars { get; set; } = new List<Car>();
        public List<CarPart> LowStockCarParts { get; set; } = new List<CarPart>();

        // Charts data
        public List<ChartData> SalesChartData { get; set; } = new List<ChartData>();
        public List<ChartData> OrderStatusChartData { get; set; } = new List<ChartData>();
        public List<ChartData> RevenueChartData { get; set; } = new List<ChartData>();

        // Quick stats
        public int NewCustomersThisMonth { get; set; }
        public int NewOrdersToday { get; set; }
        public int PendingPayments { get; set; }
        public int OutOfStockItems { get; set; }
    }

    public class RecentActivity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string? Link { get; set; }
    }

    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string Type { get; set; } = string.Empty; // success, warning, error, info
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
    }

    public class ChartData
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Color { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
    }
}