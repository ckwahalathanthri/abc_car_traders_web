using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for product card component
    /// </summary>
    public class ProductCardViewModel
    {
        /// <summary>
        /// Unique identifier for the card
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Product ID
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Type of product (Car, CarPart)
        /// </summary>
        public string ProductType { get; set; } = "Car";

        /// <summary>
        /// Product title/name
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Product description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Product image URL
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Current selling price
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// Original price (before discount)
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// Product category
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// URL to category page
        /// </summary>
        public string? CategoryUrl { get; set; }

        /// <summary>
        /// Product brand
        /// </summary>
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// URL to brand page
        /// </summary>
        public string? BrandUrl { get; set; }

        /// <summary>
        /// URL to product details page
        /// </summary>
        public string? DetailsUrl { get; set; }

        /// <summary>
        /// Stock quantity available
        /// </summary>
        public int StockQuantity { get; set; } = 0;

        /// <summary>
        /// Whether the product is featured
        /// </summary>
        public bool IsFeatured { get; set; } = false;

        /// <summary>
        /// Whether the product is new
        /// </summary>
        public bool IsNew { get; set; } = false;

        /// <summary>
        /// Whether the product is on sale
        /// </summary>
        public bool IsOnSale { get; set; } = false;

        /// <summary>
        /// Whether the product is out of stock
        /// </summary>
        public bool IsOutOfStock => StockQuantity <= 0;

        /// <summary>
        /// Whether the product is in user's wishlist
        /// </summary>
        public bool IsInWishlist { get; set; } = false;

        /// <summary>
        /// Custom badge text
        /// </summary>
        public string? CustomBadge { get; set; }

        /// <summary>
        /// Discount percentage (calculated from prices)
        /// </summary>
        public int DiscountPercentage
        {
            get
            {
                if (OriginalPrice > CurrentPrice && OriginalPrice > 0)
                {
                    return (int)Math.Round((OriginalPrice - CurrentPrice) / OriginalPrice * 100);
                }
                return 0;
            }
        }

        /// <summary>
        /// Amount saved (calculated from prices)
        /// </summary>
        public decimal AmountSaved => OriginalPrice > CurrentPrice ? OriginalPrice - CurrentPrice : 0;

        /// <summary>
        /// Product rating (1-5 stars)
        /// </summary>
        public int Rating { get; set; } = 0;

        /// <summary>
        /// Number of reviews
        /// </summary>
        public int ReviewCount { get; set; } = 0;

        /// <summary>
        /// Product features to display
        /// </summary>
        public List<ProductFeature> Features { get; set; } = new List<ProductFeature>();

        /// <summary>
        /// Additional CSS classes for the card
        /// </summary>
        public string CardClass { get; set; } = string.Empty;

        /// <summary>
        /// Whether to show rating
        /// </summary>
        public bool ShowRating { get; set; } = true;

        /// <summary>
        /// Whether to show stock information
        /// </summary>
        public bool ShowStockInfo { get; set; } = false;

        /// <summary>
        /// Whether to show action buttons
        /// </summary>
        public bool ShowActionButtons { get; set; } = true;

        /// <summary>
        /// Whether to show wishlist button
        /// </summary>
        public bool ShowWishlistButton { get; set; } = true;

        /// <summary>
        /// Whether to show compare button
        /// </summary>
        public bool ShowCompareButton { get; set; } = true;

        /// <summary>
        /// Whether to show quick view button
        /// </summary>
        public bool ShowQuickViewButton { get; set; } = true;

        /// <summary>
        /// Whether to show contact button (for cars)
        /// </summary>
        public bool ShowContactButton { get; set; } = false;

        /// <summary>
        /// Whether to show price per unit
        /// </summary>
        public bool ShowPricePerUnit { get; set; } = false;

        /// <summary>
        /// Price unit (e.g., "piece", "set")
        /// </summary>
        public string PriceUnit { get; set; } = "piece";

        /// <summary>
        /// Whether to show card footer
        /// </summary>
        public bool ShowFooter { get; set; } = false;

        /// <summary>
        /// Footer text
        /// </summary>
        public string FooterText { get; set; } = string.Empty;

        /// <summary>
        /// Whether to show last updated in footer
        /// </summary>
        public bool ShowLastUpdated { get; set; } = false;

        /// <summary>
        /// Last updated timestamp
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductCardViewModel()
        {
        }

        /// <summary>
        /// Constructor with basic product info
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="title">Product title</param>
        /// <param name="price">Product price</param>
        /// <param name="productType">Product type</param>
        public ProductCardViewModel(int productId, string title, decimal price, string productType = "Car")
        {
            ProductId = productId;
            Title = title;
            CurrentPrice = price;
            OriginalPrice = price;
            ProductType = productType;
        }

        /// <summary>
        /// Creates a product card for a car
        /// </summary>
        /// <param name="car">Car entity</param>
        /// <returns>ProductCardViewModel</returns>
        public static ProductCardViewModel FromCar(Car car)
        {
            var viewModel = new ProductCardViewModel
            {
                ProductId = car.CarId,
                ProductType = "Car",
                Title = $"{car.Brand?.BrandName ?? "Unknown"} {car.Model} ({car.Year})",
                Description = car.Description ?? "",
                ImageUrl = car.ImageUrl,
                CurrentPrice = car.Price,
                OriginalPrice = car.Price,
                Category = car.Category?.CategoryName ?? "",
                Brand = car.Brand?.BrandName ?? "",
                StockQuantity = car.StockQuantity,
                IsNew = (DateTime.Now - car.CreatedAt).TotalDays <= 30,
                ShowContactButton = true,
                ShowStockInfo = false,
                ShowWishlistButton = true,
                ShowCompareButton = true,
                ShowQuickViewButton = true,
                LastUpdated = car.UpdatedAt
            };

            // Add car-specific features
            viewModel.Features.Add(new ProductFeature { Value = car.Year.ToString(), Icon = "fas fa-calendar" });
            viewModel.Features.Add(new ProductFeature { Value = car.FuelType.ToString(), Icon = "fas fa-gas-pump" });
            viewModel.Features.Add(new ProductFeature { Value = car.Transmission.ToString(), Icon = "fas fa-cogs" });

            if (car.Mileage.HasValue)
            {
                viewModel.Features.Add(new ProductFeature { Value = $"{car.Mileage:N0} km", Icon = "fas fa-tachometer-alt" });
            }

            return viewModel;
        }

        /// <summary>
        /// Creates a product card for a car part
        /// </summary>
        /// <param name="carPart">CarPart entity</param>
        /// <returns>ProductCardViewModel</returns>
        public static ProductCardViewModel FromCarPart(CarPart carPart)
        {
            var viewModel = new ProductCardViewModel
            {
                ProductId = carPart.CarPartId,
                ProductType = "CarPart",
                Title = carPart.PartName,
                Description = carPart.Description ?? "",
                ImageUrl = carPart.ImageUrl,
                CurrentPrice = carPart.Price,
                OriginalPrice = carPart.Price,
                Category = carPart.Category?.CategoryName ?? "",
                Brand = carPart.Brand?.BrandName ?? "",
                StockQuantity = carPart.StockQuantity,
                IsNew = (DateTime.Now - carPart.CreatedAt).TotalDays <= 30,
                IsOnSale = false, // CarPart does not support OriginalPrice
                ShowContactButton = false,
                ShowStockInfo = true,
                ShowWishlistButton = true,
                ShowCompareButton = true,
                ShowQuickViewButton = true,
                ShowPricePerUnit = true,
                PriceUnit = "piece",
                LastUpdated = carPart.UpdatedAt
            };

            // Add part-specific features
            if (!string.IsNullOrEmpty(carPart.PartNumber))
            {
                viewModel.Features.Add(new ProductFeature { Value = carPart.PartNumber, Icon = "fas fa-barcode" });
            }

            if (!string.IsNullOrEmpty(carPart.Compatibility))
            {
                viewModel.Features.Add(new ProductFeature { Value = carPart.Compatibility, Icon = "fas fa-car" });
            }

            return viewModel;
        }

        /// <summary>
        /// Adds a feature to the product
        /// </summary>
        /// <param name="value">Feature value</param>
        /// <param name="icon">Feature icon</param>
        /// <returns>This instance for chaining</returns>
        public ProductCardViewModel AddFeature(string value, string? icon = null)
        {
            Features.Add(new ProductFeature { Value = value, Icon = icon });
            return this;
        }

        /// <summary>
        /// Sets the product as featured
        /// </summary>
        /// <returns>This instance for chaining</returns>
        public ProductCardViewModel AsFeatured()
        {
            IsFeatured = true;
            return this;
        }

        /// <summary>
        /// Sets the product as new
        /// </summary>
        /// <returns>This instance for chaining</returns>
        public ProductCardViewModel AsNew()
        {
            IsNew = true;
            return this;
        }

        /// <summary>
        /// Sets the product as on sale
        /// </summary>
        /// <param name="originalPrice">Original price before sale</param>
        /// <returns>This instance for chaining</returns>
        public ProductCardViewModel AsOnSale(decimal originalPrice)
        {
            OriginalPrice = originalPrice;
            IsOnSale = originalPrice > CurrentPrice;
            return this;
        }

        /// <summary>
        /// Sets a custom badge
        /// </summary>
        /// <param name="badge">Badge text</param>
        /// <returns>This instance for chaining</returns>
        public ProductCardViewModel WithBadge(string badge)
        {
            CustomBadge = badge;
            return this;
        }

        /// <summary>
        /// Sets the rating
        /// </summary>
        /// <param name="rating">Rating (1-5)</param>
        /// <param name="reviewCount">Number of reviews</param>
        /// <returns>This instance for chaining</returns>
        public ProductCardViewModel WithRating(int rating, int reviewCount = 0)
        {
            Rating = Math.Clamp(rating, 0, 5);
            ReviewCount = Math.Max(0, reviewCount);
            return this;
        }

        /// <summary>
        /// Sets the URLs
        /// </summary>
        /// <param name="detailsUrl">Details page URL</param>
        /// <param name="categoryUrl">Category page URL</param>
        /// <param name="brandUrl">Brand page URL</param>
        /// <returns>This instance for chaining</returns>
        public ProductCardViewModel WithUrls(string? detailsUrl = null, string? categoryUrl = null, string? brandUrl = null)
        {
            DetailsUrl = detailsUrl;
            CategoryUrl = categoryUrl;
            BrandUrl = brandUrl;
            return this;
        }

        /// <summary>
        /// Configures display options
        /// </summary>
        /// <param name="showRating">Show rating</param>
        /// <param name="showStockInfo">Show stock info</param>
        /// <param name="showActionButtons">Show action buttons</param>
        /// <returns>This instance for chaining</returns>
        public ProductCardViewModel WithDisplayOptions(bool showRating = true, bool showStockInfo = false, bool showActionButtons = true)
        {
            ShowRating = showRating;
            ShowStockInfo = showStockInfo;
            ShowActionButtons = showActionButtons;
            return this;
        }

        /// <summary>
        /// Gets formatted price string
        /// </summary>
        /// <param name="price">Price to format</param>
        /// <returns>Formatted price string</returns>
        public string FormatPrice(decimal price)
        {
            return $"LKR {price:N2}";
        }

        /// <summary>
        /// Gets the current formatted price
        /// </summary>
        /// <returns>Formatted current price</returns>
        public string GetFormattedCurrentPrice()
        {
            return FormatPrice(CurrentPrice);
        }

        /// <summary>
        /// Gets the original formatted price
        /// </summary>
        /// <returns>Formatted original price</returns>
        public string GetFormattedOriginalPrice()
        {
            return FormatPrice(OriginalPrice);
        }

        /// <summary>
        /// Gets the formatted amount saved
        /// </summary>
        /// <returns>Formatted amount saved</returns>
        public string GetFormattedAmountSaved()
        {
            return FormatPrice(AmountSaved);
        }

        /// <summary>
        /// Gets availability status text
        /// </summary>
        /// <returns>Availability status</returns>
        public string GetAvailabilityStatus()
        {
            if (IsOutOfStock)
                return "Out of Stock";

            if (StockQuantity <= 5)
                return $"Only {StockQuantity} left";

            return "In Stock";
        }

        /// <summary>
        /// Gets availability CSS class
        /// </summary>
        /// <returns>CSS class for availability</returns>
        public string GetAvailabilityCssClass()
        {
            if (IsOutOfStock)
                return "text-danger";

            if (StockQuantity <= 5)
                return "text-warning";

            return "text-success";
        }

        /// <summary>
        /// Validates the product card data
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (ProductId <= 0)
                errors.Add("Product ID must be greater than 0");

            if (string.IsNullOrEmpty(Title))
                errors.Add("Title is required");

            if (CurrentPrice < 0)
                errors.Add("Current price cannot be negative");

            if (OriginalPrice < 0)
                errors.Add("Original price cannot be negative");

            if (StockQuantity < 0)
                errors.Add("Stock quantity cannot be negative");

            if (Rating < 0 || Rating > 5)
                errors.Add("Rating must be between 0 and 5");

            if (ReviewCount < 0)
                errors.Add("Review count cannot be negative");

            return errors;
        }

        /// <summary>
        /// Creates a clone of this product card
        /// </summary>
        /// <returns>Cloned ProductCardViewModel</returns>
        public ProductCardViewModel Clone()
        {
            return new ProductCardViewModel
            {
                Id = Id,
                ProductId = ProductId,
                ProductType = ProductType,
                Title = Title,
                Description = Description,
                ImageUrl = ImageUrl,
                CurrentPrice = CurrentPrice,
                OriginalPrice = OriginalPrice,
                Category = Category,
                CategoryUrl = CategoryUrl,
                Brand = Brand,
                BrandUrl = BrandUrl,
                DetailsUrl = DetailsUrl,
                StockQuantity = StockQuantity,
                IsFeatured = IsFeatured,
                IsNew = IsNew,
                IsOnSale = IsOnSale,
                IsInWishlist = IsInWishlist,
                CustomBadge = CustomBadge,
                Rating = Rating,
                ReviewCount = ReviewCount,
                Features = Features.Select(f => new ProductFeature { Value = f.Value, Icon = f.Icon }).ToList(),
                CardClass = CardClass,
                ShowRating = ShowRating,
                ShowStockInfo = ShowStockInfo,
                ShowActionButtons = ShowActionButtons,
                ShowWishlistButton = ShowWishlistButton,
                ShowCompareButton = ShowCompareButton,
                ShowQuickViewButton = ShowQuickViewButton,
                ShowContactButton = ShowContactButton,
                ShowPricePerUnit = ShowPricePerUnit,
                PriceUnit = PriceUnit,
                ShowFooter = ShowFooter,
                FooterText = FooterText,
                ShowLastUpdated = ShowLastUpdated,
                LastUpdated = LastUpdated
            };
        }
    }

    /// <summary>
    /// Represents a product feature
    /// </summary>
    public class ProductFeature
    {
        /// <summary>
        /// Feature value/text
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Feature icon CSS class
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Feature tooltip
        /// </summary>
        public string? Tooltip { get; set; }

        /// <summary>
        /// Feature URL (if clickable)
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Additional CSS classes
        /// </summary>
        public string CssClass { get; set; } = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductFeature()
        {
        }

        /// <summary>
        /// Constructor with value and icon
        /// </summary>
        /// <param name="value">Feature value</param>
        /// <param name="icon">Feature icon</param>
        public ProductFeature(string value, string? icon = null)
        {
            Value = value;
            Icon = icon;
        }
    }
}