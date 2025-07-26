using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for breadcrumb navigation
    /// </summary>
    public class BreadcrumbViewModel
    {
        /// <summary>
        /// List of breadcrumb items
        /// </summary>
        public List<BreadcrumbItem> Items { get; set; } = new List<BreadcrumbItem>();
        
        /// <summary>
        /// CSS class for the breadcrumb container
        /// </summary>
        public string CssClass { get; set; } = string.Empty;
        
        /// <summary>
        /// CSS class for the breadcrumb list
        /// </summary>
        public string BreadcrumbClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Custom separator HTML (overrides default)
        /// </summary>
        public string? CustomSeparator { get; set; }
        
        /// <summary>
        /// Whether to show back button
        /// </summary>
        public bool ShowBackButton { get; set; } = false;
        
        /// <summary>
        /// List of action buttons to display
        /// </summary>
        public List<BreadcrumbAction> Actions { get; set; } = new List<BreadcrumbAction>();
        
        /// <summary>
        /// Whether to generate structured data for SEO
        /// </summary>
        public bool GenerateStructuredData { get; set; } = true;
        
        /// <summary>
        /// Maximum number of visible items (others will be collapsed)
        /// </summary>
        public int MaxVisibleItems { get; set; } = 5;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public BreadcrumbViewModel()
        {
        }
        
        /// <summary>
        /// Constructor with items
        /// </summary>
        /// <param name="items">Breadcrumb items</param>
        public BreadcrumbViewModel(List<BreadcrumbItem> items)
        {
            Items = items ?? new List<BreadcrumbItem>();
        }
        
        /// <summary>
        /// Adds a breadcrumb item
        /// </summary>
        /// <param name="title">Item title</param>
        /// <param name="url">Item URL (null for current page)</param>
        /// <param name="icon">Icon CSS class</param>
        /// <returns>This instance for chaining</returns>
        public BreadcrumbViewModel AddItem(string title, string? url = null, string? icon = null)
        {
            Items.Add(new BreadcrumbItem
            {
                Title = title,
                Url = url,
                Icon = icon
            });
            return this;
        }
        
        /// <summary>
        /// Adds a breadcrumb action
        /// </summary>
        /// <param name="title">Action title</param>
        /// <param name="url">Action URL</param>
        /// <param name="icon">Icon CSS class</param>
        /// <param name="cssClass">Button CSS class</param>
        /// <returns>This instance for chaining</returns>
        public BreadcrumbViewModel AddAction(string title, string? url = null, string? icon = null, string cssClass = "btn-primary btn-sm")
        {
            Actions.Add(new BreadcrumbAction
            {
                Title = title,
                Url = url,
                Icon = icon,
                CssClass = cssClass
            });
            return this;
        }
        
        /// <summary>
        /// Creates a simple breadcrumb with home and current page
        /// </summary>
        /// <param name="currentPageTitle">Title of current page</param>
        /// <param name="homeUrl">URL to home page</param>
        /// <returns>BreadcrumbViewModel</returns>
        public static BreadcrumbViewModel Simple(string currentPageTitle, string homeUrl = "/")
        {
            return new BreadcrumbViewModel()
                .AddItem("Home", homeUrl, "fas fa-home")
                .AddItem(currentPageTitle);
        }
        
        /// <summary>
        /// Creates breadcrumb for car-related pages
        /// </summary>
        /// <param name="currentPageTitle">Title of current page</param>
        /// <param name="carsUrl">URL to cars index</param>
        /// <param name="homeUrl">URL to home page</param>
        /// <returns>BreadcrumbViewModel</returns>
        public static BreadcrumbViewModel ForCars(string currentPageTitle, string carsUrl = "/Cars", string homeUrl = "/")
        {
            return new BreadcrumbViewModel()
                .AddItem("Home", homeUrl, "fas fa-home")
                .AddItem("Cars", carsUrl, "fas fa-car")
                .AddItem(currentPageTitle);
        }
        
        /// <summary>
        /// Creates breadcrumb for car parts pages
        /// </summary>
        /// <param name="currentPageTitle">Title of current page</param>
        /// <param name="partsUrl">URL to parts index</param>
        /// <param name="homeUrl">URL to home page</param>
        /// <returns>BreadcrumbViewModel</returns>
        public static BreadcrumbViewModel ForCarParts(string currentPageTitle, string partsUrl = "/CarParts", string homeUrl = "/")
        {
            return new BreadcrumbViewModel()
                .AddItem("Home", homeUrl, "fas fa-home")
                .AddItem("Car Parts", partsUrl, "fas fa-cogs")
                .AddItem(currentPageTitle);
        }
        
        /// <summary>
        /// Creates breadcrumb for admin pages
        /// </summary>
        /// <param name="currentPageTitle">Title of current page</param>
        /// <param name="adminUrl">URL to admin dashboard</param>
        /// <param name="homeUrl">URL to home page</param>
        /// <returns>BreadcrumbViewModel</returns>
        public static BreadcrumbViewModel ForAdmin(string currentPageTitle, string adminUrl = "/Admin/Dashboard", string homeUrl = "/")
        {
            return new BreadcrumbViewModel()
                .AddItem("Home", homeUrl, "fas fa-home")
                .AddItem("Admin Panel", adminUrl, "fas fa-tools")
                .AddItem(currentPageTitle);
        }
        
        /// <summary>
        /// Creates breadcrumb for customer pages
        /// </summary>
        /// <param name="currentPageTitle">Title of current page</param>
        /// <param name="customerUrl">URL to customer dashboard</param>
        /// <param name="homeUrl">URL to home page</param>
        /// <returns>BreadcrumbViewModel</returns>
        public static BreadcrumbViewModel ForCustomer(string currentPageTitle, string customerUrl = "/Customer/Dashboard", string homeUrl = "/")
        {
            return new BreadcrumbViewModel()
                .AddItem("Home", homeUrl, "fas fa-home")
                .AddItem("My Account", customerUrl, "fas fa-user")
                .AddItem(currentPageTitle);
        }
    }
    
    /// <summary>
    /// Represents a single breadcrumb item
    /// </summary>
    public class BreadcrumbItem
    {
        /// <summary>
        /// Display title of the breadcrumb item
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// URL for the breadcrumb item (null for current page)
        /// </summary>
        public string? Url { get; set; }
        
        /// <summary>
        /// Icon CSS class (e.g., "fas fa-home")
        /// </summary>
        public string? Icon { get; set; }
        
        /// <summary>
        /// Additional CSS classes for the item
        /// </summary>
        public string CssClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Tooltip text for the item
        /// </summary>
        public string? Tooltip { get; set; }
        
        /// <summary>
        /// Whether this item is clickable
        /// </summary>
        public bool IsClickable => !string.IsNullOrEmpty(Url);
        
        /// <summary>
        /// Whether this is the current/active item
        /// </summary>
        public bool IsActive => string.IsNullOrEmpty(Url);
        
        /// <summary>
        /// Custom data attributes
        /// </summary>
        public Dictionary<string, string> DataAttributes { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public BreadcrumbItem()
        {
        }
        
        /// <summary>
        /// Constructor with title and URL
        /// </summary>
        /// <param name="title">Item title</param>
        /// <param name="url">Item URL</param>
        public BreadcrumbItem(string title, string? url = null)
        {
            Title = title;
            Url = url;
        }
        
        /// <summary>
        /// Constructor with full parameters
        /// </summary>
        /// <param name="title">Item title</param>
        /// <param name="url">Item URL</param>
        /// <param name="icon">Icon CSS class</param>
        /// <param name="cssClass">Additional CSS classes</param>
        public BreadcrumbItem(string title, string? url, string? icon, string cssClass = "")
        {
            Title = title;
            Url = url;
            Icon = icon;
            CssClass = cssClass;
        }
        
        /// <summary>
        /// Adds a data attribute
        /// </summary>
        /// <param name="key">Attribute key</param>
        /// <param name="value">Attribute value</param>
        /// <returns>This instance for chaining</returns>
        public BreadcrumbItem AddDataAttribute(string key, string value)
        {
            DataAttributes[key] = value;
            return this;
        }
        
        /// <summary>
        /// Sets the tooltip
        /// </summary>
        /// <param name="tooltip">Tooltip text</param>
        /// <returns>This instance for chaining</returns>
        public BreadcrumbItem WithTooltip(string tooltip)
        {
            Tooltip = tooltip;
            return this;
        }
        
        /// <summary>
        /// Adds CSS class
        /// </summary>
        /// <param name="cssClass">CSS class to add</param>
        /// <returns>This instance for chaining</returns>
        public BreadcrumbItem WithCssClass(string cssClass)
        {
            CssClass = string.IsNullOrEmpty(CssClass) ? cssClass : $"{CssClass} {cssClass}";
            return this;
        }
    }
    
    /// <summary>
    /// Represents an action button in breadcrumb navigation
    /// </summary>
    public class BreadcrumbAction
    {
        /// <summary>
        /// Display title of the action
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// URL for the action
        /// </summary>
        public string? Url { get; set; }
        
        /// <summary>
        /// JavaScript function to execute on click
        /// </summary>
        public string? OnClick { get; set; }
        
        /// <summary>
        /// Icon CSS class
        /// </summary>
        public string? Icon { get; set; }
        
        /// <summary>
        /// Button CSS classes
        /// </summary>
        public string CssClass { get; set; } = "btn-primary btn-sm";
        
        /// <summary>
        /// Tooltip text
        /// </summary>
        public string? Tooltip { get; set; }
        
        /// <summary>
        /// Whether the action requires confirmation
        /// </summary>
        public bool RequiresConfirmation { get; set; } = false;
        
        /// <summary>
        /// Confirmation message
        /// </summary>
        public string? ConfirmationMessage { get; set; }
        
        /// <summary>
        /// Custom data attributes
        /// </summary>
        public Dictionary<string, string> DataAttributes { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public BreadcrumbAction()
        {
        }
        
        /// <summary>
        /// Constructor with title and URL
        /// </summary>
        /// <param name="title">Action title</param>
        /// <param name="url">Action URL</param>
        public BreadcrumbAction(string title, string? url = null)
        {
            Title = title;
            Url = url;
        }
        
        /// <summary>
        /// Constructor with full parameters
        /// </summary>
        /// <param name="title">Action title</param>
        /// <param name="url">Action URL</param>
        /// <param name="icon">Icon CSS class</param>
        /// <param name="cssClass">Button CSS classes</param>
        public BreadcrumbAction(string title, string? url, string? icon, string cssClass = "btn-primary btn-sm")
        {
            Title = title;
            Url = url;
            Icon = icon;
            CssClass = cssClass;
        }
        
        /// <summary>
        /// Creates an edit action
        /// </summary>
        /// <param name="url">Edit URL</param>
        /// <returns>BreadcrumbAction</returns>
        public static BreadcrumbAction Edit(string url)
        {
            return new BreadcrumbAction("Edit", url, "fas fa-edit", "btn-primary btn-sm");
        }
        
        /// <summary>
        /// Creates a delete action
        /// </summary>
        /// <param name="onClick">Delete function call</param>
        /// <param name="confirmationMessage">Confirmation message</param>
        /// <returns>BreadcrumbAction</returns>
        public static BreadcrumbAction Delete(string onClick, string confirmationMessage = "Are you sure you want to delete this item?")
        {
            return new BreadcrumbAction("Delete", null, "fas fa-trash", "btn-danger btn-sm")
            {
                OnClick = onClick,
                RequiresConfirmation = true,
                ConfirmationMessage = confirmationMessage
            };
        }
        
        /// <summary>
        /// Creates a back action
        /// </summary>
        /// <param name="url">Back URL (optional)</param>
        /// <returns>BreadcrumbAction</returns>
        public static BreadcrumbAction Back(string? url = null)
        {
            return new BreadcrumbAction("Back", url, "fas fa-arrow-left", "btn-secondary btn-sm")
            {
                OnClick = url == null ? "history.back()" : null
            };
        }
        
        /// <summary>
        /// Creates a print action
        /// </summary>
        /// <returns>BreadcrumbAction</returns>
        public static BreadcrumbAction Print()
        {
            return new BreadcrumbAction("Print", null, "fas fa-print", "btn-outline-secondary btn-sm")
            {
                OnClick = "window.print()"
            };
        }
        
        /// <summary>
        /// Creates a share action
        /// </summary>
        /// <returns>BreadcrumbAction</returns>
        public static BreadcrumbAction Share()
        {
            return new BreadcrumbAction("Share", null, "fas fa-share", "btn-outline-primary btn-sm")
            {
                OnClick = "shareCurrentPage()"
            };
        }
        
        /// <summary>
        /// Adds a data attribute
        /// </summary>
        /// <param name="key">Attribute key</param>
        /// <param name="value">Attribute value</param>
        /// <returns>This instance for chaining</returns>
        public BreadcrumbAction AddDataAttribute(string key, string value)
        {
            DataAttributes[key] = value;
            return this;
        }
        
        /// <summary>
        /// Sets the tooltip
        /// </summary>
        /// <param name="tooltip">Tooltip text</param>
        /// <returns>This instance for chaining</returns>
        public BreadcrumbAction WithTooltip(string tooltip)
        {
            Tooltip = tooltip;
            return this;
        }
        
        /// <summary>
        /// Sets confirmation requirement
        /// </summary>
        /// <param name="message">Confirmation message</param>
        /// <returns>This instance for chaining</returns>
        public BreadcrumbAction WithConfirmation(string message)
        {
            RequiresConfirmation = true;
            ConfirmationMessage = message;
            return this;
        }
    }
    
    /// <summary>
    /// Builder class for creating complex breadcrumb navigation
    /// </summary>
    public class BreadcrumbBuilder
    {
        private readonly BreadcrumbViewModel _breadcrumb;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public BreadcrumbBuilder()
        {
            _breadcrumb = new BreadcrumbViewModel();
        }
        
        /// <summary>
        /// Adds a breadcrumb item
        /// </summary>
        /// <param name="title">Item title</param>
        /// <param name="url">Item URL</param>
        /// <param name="icon">Icon CSS class</param>
        /// <returns>This builder for chaining</returns>
        public BreadcrumbBuilder AddItem(string title, string? url = null, string? icon = null)
        {
            _breadcrumb.AddItem(title, url, icon);
            return this;
        }
        
        /// <summary>
        /// Adds a breadcrumb action
        /// </summary>
        /// <param name="action">Breadcrumb action</param>
        /// <returns>This builder for chaining</returns>
        public BreadcrumbBuilder AddAction(BreadcrumbAction action)
        {
            _breadcrumb.Actions.Add(action);
            return this;
        }
        
        /// <summary>
        /// Sets custom separator
        /// </summary>
        /// <param name="separator">Custom separator HTML</param>
        /// <returns>This builder for chaining</returns>
        public BreadcrumbBuilder WithSeparator(string separator)
        {
            _breadcrumb.CustomSeparator = separator;
            return this;
        }
        
        /// <summary>
        /// Shows back button
        /// </summary>
        /// <returns>This builder for chaining</returns>
        public BreadcrumbBuilder WithBackButton()
        {
            _breadcrumb.ShowBackButton = true;
            return this;
        }
        
        /// <summary>
        /// Sets CSS class
        /// </summary>
        /// <param name="cssClass">CSS class</param>
        /// <returns>This builder for chaining</returns>
        public BreadcrumbBuilder WithCssClass(string cssClass)
        {
            _breadcrumb.CssClass = cssClass;
            return this;
        }
        
        /// <summary>
        /// Builds the breadcrumb view model
        /// </summary>
        /// <returns>BreadcrumbViewModel</returns>
        public BreadcrumbViewModel Build()
        {
            return _breadcrumb;
        }
        
        /// <summary>
        /// Creates a new builder instance
        /// </summary>
        /// <returns>BreadcrumbBuilder</returns>
        public static BreadcrumbBuilder Create()
        {
            return new BreadcrumbBuilder();
        }
    }
}