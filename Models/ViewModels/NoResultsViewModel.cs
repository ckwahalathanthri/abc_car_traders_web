using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for no results component
    /// </summary>
    public class NoResultsViewModel
    {
        /// <summary>
        /// Main title to display
        /// </summary>
        public string Title { get; set; } = "No Results Found";
        
        /// <summary>
        /// Main message to display
        /// </summary>
        public string Message { get; set; } = "We couldn't find what you're looking for.";
        
        /// <summary>
        /// Secondary message
        /// </summary>
        public string SubMessage { get; set; } = string.Empty;
        
        /// <summary>
        /// Type of items being searched (Cars, Parts, etc.)
        /// </summary>
        public string ItemType { get; set; } = "Items";
        
        /// <summary>
        /// Icon to display
        /// </summary>
        public string Icon { get; set; } = "fas fa-search";
        
        /// <summary>
        /// Custom image URL (overrides icon)
        /// </summary>
        public string? CustomImageUrl { get; set; }
        
        /// <summary>
        /// Additional CSS classes for container
        /// </summary>
        public string ContainerClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether to show suggestions
        /// </summary>
        public bool ShowSuggestions { get; set; } = true;
        
        /// <summary>
        /// Custom suggestions to display
        /// </summary>
        public List<string> CustomSuggestions { get; set; } = new List<string>();
        
        /// <summary>
        /// Whether to show clear filters button
        /// </summary>
        public bool ShowClearFiltersButton { get; set; } = true;
        
        /// <summary>
        /// Whether to show browse all button
        /// </summary>
        public bool ShowBrowseAllButton { get; set; } = true;
        
        /// <summary>
        /// URL for browse all button
        /// </summary>
        public string BrowseAllUrl { get; set; } = "/";
        
        /// <summary>
        /// Whether to show contact button
        /// </summary>
        public bool ShowContactButton { get; set; } = true;
        
        /// <summary>
        /// Whether to show home button
        /// </summary>
        public bool ShowHomeButton { get; set; } = true;
        
        /// <summary>
        /// Whether to show popular items
        /// </summary>
        public bool ShowPopularItems { get; set; } = false;
        
        /// <summary>
        /// Popular items to display
        /// </summary>
        public List<PopularItem> PopularItems { get; set; } = new List<PopularItem>();
        
        /// <summary>
        /// Whether to show search again form
        /// </summary>
        public bool ShowSearchAgain { get; set; } = true;
        
        /// <summary>
        /// Whether to show help text
        /// </summary>
        public bool ShowHelpText { get; set; } = false;
        
        /// <summary>
        /// Help text to display
        /// </summary>
        public string HelpText { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether to show statistics
        /// </summary>
        public bool ShowStatistics { get; set; } = true;
        
        /// <summary>
        /// Custom actions to display
        /// </summary>
        public List<NoResultsAction> CustomActions { get; set; } = new List<NoResultsAction>();
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public NoResultsViewModel()
        {
        }
        
        /// <summary>
        /// Constructor with basic properties
        /// </summary>
        /// <param name="title">Main title</param>
        /// <param name="message">Main message</param>
        /// <param name="itemType">Type of items</param>
        public NoResultsViewModel(string title, string message, string itemType)
        {
            Title = title;
            Message = message;
            ItemType = itemType;
        }
        
        /// <summary>
        /// Creates a no results view model for cars
        /// </summary>
        /// <returns>NoResultsViewModel</returns>
        public static NoResultsViewModel ForCars()
        {
            return new NoResultsViewModel
            {
                Title = "No Cars Found",
                Message = "We couldn't find any cars matching your search criteria.",
                ItemType = "Cars",
                Icon = "fas fa-car",
                BrowseAllUrl = "/Cars",
                CustomSuggestions = new List<string>
                {
                    "Try searching by brand (Toyota, Honda, etc.)",
                    "Search by year range",
                    "Browse by fuel type",
                    "Check different price ranges"
                }
            };
        }
        
        /// <summary>
        /// Creates a no results view model for car parts
        /// </summary>
        /// <returns>NoResultsViewModel</returns>
        public static NoResultsViewModel ForCarParts()
        {
            return new NoResultsViewModel
            {
                Title = "No Car Parts Found",
                Message = "We couldn't find any car parts matching your search criteria.",
                ItemType = "Car Parts",
                Icon = "fas fa-cogs",
                BrowseAllUrl = "/CarParts",
                CustomSuggestions = new List<string>
                {
                    "Try searching by part number",
                    "Search by car make and model",
                    "Browse by part category",
                    "Check compatible vehicles"
                }
            };
        }
        
        /// <summary>
        /// Creates a no results view model for general search
        /// </summary>
        /// <returns>NoResultsViewModel</returns>
        public static NoResultsViewModel ForGeneral()
        {
            return new NoResultsViewModel
            {
                Title = "No Results Found",
                Message = "We couldn't find anything matching your search.",
                ItemType = "Results",
                Icon = "fas fa-search",
                BrowseAllUrl = "/",
                ShowPopularItems = true
            };
        }
        
        /// <summary>
        /// Creates a no results view model for categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <returns>NoResultsViewModel</returns>
        public static NoResultsViewModel ForCategory(string categoryName)
        {
            return new NoResultsViewModel
            {
                Title = $"No Items in {categoryName}",
                Message = $"There are currently no items available in the {categoryName} category.",
                ItemType = "Items",
                Icon = "fas fa-folder-open",
                ShowClearFiltersButton = false,
                CustomSuggestions = new List<string>
                {
                    "Browse other categories",
                    "Check back later for new arrivals",
                    "Contact us for special requests"
                }
            };
        }
        
        /// <summary>
        /// Sets search information
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="category">Category</param>
        /// <returns>This instance for chaining</returns>
        public NoResultsViewModel WithSearchInfo(string? searchTerm, string? category = null)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Message = $"We couldn't find any {ItemType.ToLower()} matching \"{searchTerm}\"";
                if (!string.IsNullOrEmpty(category))
                {
                    Message += $" in the {category} category";
                }
                Message += ".";
            }
            
            return this;
        }
        
        /// <summary>
        /// Sets popular items
        /// </summary>
        /// <param name="items">Popular items</param>
        /// <returns>This instance for chaining</returns>
        public NoResultsViewModel WithPopularItems(List<PopularItem> items)
        {
            PopularItems = items ?? new List<PopularItem>();
            ShowPopularItems = PopularItems.Any();
            return this;
        }
        
        /// <summary>
        /// Adds a custom suggestion
        /// </summary>
        /// <param name="suggestion">Suggestion text</param>
        /// <returns>This instance for chaining</returns>
        public NoResultsViewModel AddSuggestion(string suggestion)
        {
            CustomSuggestions.Add(suggestion);
            return this;
        }
        
        /// <summary>
        /// Adds a custom action
        /// </summary>
        /// <param name="text">Action text</param>
        /// <param name="url">Action URL</param>
        /// <param name="cssClass">Button CSS class</param>
        /// <param name="icon">Icon CSS class</param>
        /// <returns>This instance for chaining</returns>
        public NoResultsViewModel WithCustomAction(string text, string url, string cssClass = "btn-primary", string? icon = null)
        {
            CustomActions.Add(new NoResultsAction
            {
                Text = text,
                Url = url,
                CssClass = cssClass,
                Icon = icon
            });
            return this;
        }
        
        /// <summary>
        /// Sets help text
        /// </summary>
        /// <param name="helpText">Help text</param>
        /// <returns>This instance for chaining</returns>
        public NoResultsViewModel WithHelpText(string helpText)
        {
            HelpText = helpText;
            ShowHelpText = !string.IsNullOrEmpty(helpText);
            return this;
        }
        
        /// <summary>
        /// Sets custom image
        /// </summary>
        /// <param name="imageUrl">Image URL</param>
        /// <returns>This instance for chaining</returns>
        public NoResultsViewModel WithCustomImage(string imageUrl)
        {
            CustomImageUrl = imageUrl;
            return this;
        }
        
        /// <summary>
        /// Configures display options
        /// </summary>
        /// <param name="showSuggestions">Show suggestions</param>
        /// <param name="showSearchAgain">Show search again form</param>
        /// <param name="showStatistics">Show statistics</param>
        /// <returns>This instance for chaining</returns>
        public NoResultsViewModel WithDisplayOptions(bool showSuggestions = true, bool showSearchAgain = true, bool showStatistics = true)
        {
            ShowSuggestions = showSuggestions;
            ShowSearchAgain = showSearchAgain;
            ShowStatistics = showStatistics;
            return this;
        }
        
        /// <summary>
        /// Sets the browse all URL
        /// </summary>
        /// <param name="url">Browse all URL</param>
        /// <returns>This instance for chaining</returns>
        public NoResultsViewModel WithBrowseAllUrl(string url)
        {
            BrowseAllUrl = url;
            return this;
        }
        
        /// <summary>
        /// Disables certain buttons
        /// </summary>
        /// <param name="clearFilters">Disable clear filters</param>
        /// <param name="browseAll">Disable browse all</param>
        /// <param name="contact">Disable contact</param>
        /// <param name="home">Disable home</param>
        /// <returns>This instance for chaining</returns>
        public NoResultsViewModel DisableButtons(bool clearFilters = false, bool browseAll = false, bool contact = false, bool home = false)
        {
            if (clearFilters) ShowClearFiltersButton = false;
            if (browseAll) ShowBrowseAllButton = false;
            if (contact) ShowContactButton = false;
            if (home) ShowHomeButton = false;
            return this;
        }
    }
    
    /// <summary>
    /// Represents a popular item to show in no results
    /// </summary>
    public class PopularItem
    {
        /// <summary>
        /// Item name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Item description
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Item image URL
        /// </summary>
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// Item URL
        /// </summary>
        public string Url { get; set; } = "#";
        
        /// <summary>
        /// Item count or additional info
        /// </summary>
        public string? Count { get; set; }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public PopularItem()
        {
        }
        
        /// <summary>
        /// Constructor with basic properties
        /// </summary>
        /// <param name="name">Item name</param>
        /// <param name="url">Item URL</param>
        /// <param name="description">Item description</param>
        public PopularItem(string name, string url, string description = "")
        {
            Name = name;
            Url = url;
            Description = description;
        }
        
        /// <summary>
        /// Creates a popular item from a car category
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="count">Number of cars</param>
        /// <returns>PopularItem</returns>
        public static PopularItem FromCarCategory(Category category, int count)
        {
            return new PopularItem
            {
                Name = category.CategoryName,
                Description = $"{count} cars available",
                Url = $"/Cars/Category/{category.CategoryId}",
                Count = count.ToString()
            };
        }
        
        /// <summary>
        /// Creates a popular item from a brand
        /// </summary>
        /// <param name="brand">Brand</param>
        /// <param name="count">Number of items</param>
        /// <returns>PopularItem</returns>
        public static PopularItem FromBrand(Brand brand, int count)
        {
            return new PopularItem
            {
                Name = brand.BrandName,
                Description = $"{count} vehicles",
                Url = $"/Cars/Brand/{brand.BrandId}",
                ImageUrl = brand.LogoUrl,
                Count = count.ToString()
            };
        }
    }
    
    /// <summary>
    /// Represents a custom action in no results
    /// </summary>
    public class NoResultsAction
    {
        /// <summary>
        /// Action text
        /// </summary>
        public string Text { get; set; } = string.Empty;
        
        /// <summary>
        /// Action URL
        /// </summary>
        public string Url { get; set; } = "#";
        
        /// <summary>
        /// Button CSS classes
        /// </summary>
        public string CssClass { get; set; } = "btn-primary";
        
        /// <summary>
        /// Icon CSS class
        /// </summary>
        public string? Icon { get; set; }
        
        /// <summary>
        /// Whether the action opens in new tab
        /// </summary>
        public bool OpenInNewTab { get; set; } = false;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public NoResultsAction()
        {
        }
        
        /// <summary>
        /// Constructor with basic properties
        /// </summary>
        /// <param name="text">Action text</param>
        /// <param name="url">Action URL</param>
        /// <param name="cssClass">CSS class</param>
        public NoResultsAction(string text, string url, string cssClass = "btn-primary")
        {
            Text = text;
            Url = url;
            CssClass = cssClass;
        }
    }
}