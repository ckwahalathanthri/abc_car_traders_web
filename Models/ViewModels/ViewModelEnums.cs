namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// Enum for order status (ViewModels version)
    /// </summary>
    public enum _OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Processing = 3,
        Shipped = 4,
        Delivered = 5,
        Cancelled = 6,
        Refunded = 7
    }

    /// <summary>
    /// Enum for payment status (ViewModels version)
    /// </summary>
    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Failed = 3,
        Refunded = 4
    }

    /// <summary>
    /// Enum for payment method (ViewModels version)
    /// </summary>
    public enum PaymentMethod
    {
        CreditCard = 1,
        DebitCard = 2,
        BankTransfer = 3,
        Cash = 4
    }

    /// <summary>
    /// Enum for customer tier
    /// </summary>
    public enum __CustomerTier
    {
        Bronze = 1,
        Silver = 2,
        Gold = 3,
        Platinum = 4
    }
}