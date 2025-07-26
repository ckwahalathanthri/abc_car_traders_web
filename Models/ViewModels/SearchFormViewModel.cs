using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for search form component
    /// </summary>
    public class SearchFormViewModel
    {
        /// <summary>
        /// Unique identifier for the search form
        /// </summary>
        public string Id { get; set; } = string.Empty;
        
        /// <summary>
        /// Controller name for form submission
        /// </summary>
        public string? Controller { get; set; }
        
        /// <summary>
        /// Action name for form submission
        /// </summary>
        public string Action { get; set; } = "Search";
        
        /// <summary>
        /// HTTP method for form submission
        /// </summary>
        public string Method { get; set; } = "GET";
        
        /// <summary>
        /// Form layout style (horizontal, vertical, compact)
        /// </summary>
        public string Layout { get; set; } = "horizontal";
        
        /// <summary>
        /// Search input placeholder text
        /// </summary>
        public string Placeholder { get; set; } = "Search...";
        
        /// <summary>
        /// Form title
        /// </summary>
        public string Title { get; set; } = "Search";
        
        /// <summary>
        /// Form subtitle
        /// </summary>
        public string Subtitle { get; set; } = string.Empty;
        
        /// <summary>
        /// Icon for the form title
        /// </summary>
        public string Icon { get; set; } = "fas fa-search";
        
        /// <summary>
        /// Whether to show the form title
        /// </summary>
        public bool ShowTitle { get; set; } = false;
        
        /// <summary>
        /// Whether to show search icon in input
        /// </summary>
        public bool ShowSearchIcon { get; set; } = true;
        
        /// <summary>
        /// Whether to show clear button
        /// </summary>
        public bool ShowClearButton { get; set; } = true;
        
        /// <summary>
        /// Search button text
        /// </summary>
        public string ButtonText { get; set; } = "Search";
        
        /// <summary>
        /// Whether to show search button text
        /// </summary>
        public bool ShowButtonText { get; set; } = true;
        
        /// <summary>
        /// Search button color theme
        /// </summary>
        public string ButtonColor { get; set; } = "primary";
        
        /// <summary>
        /// Additional CSS classes for the search button
        /// </summary>
        public string ButtonClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Name of the search parameter
        /// </summary>
        public string SearchParameterName { get; set; } = "searchTerm";
        
        /// <summary>
        /// Whether the search input is required
        /// </summary>
        public bool Required { get; set; } = false;
        
        /// <summary>
        /// Minimum length for search input
        /// </summary>
        public int MinLength { get; set; } = 0;
        
        /// <summary>
        /// Maximum length for search input
        /// </summary>
        public int MaxLength { get; set; } = 0;
        
        /// <summary>
        /// Whether to auto-focus the search input
        /// </summary>
        public bool AutoFocus { get; set; } = false;
        
        /// <summary>
        /// Whether to auto-submit the form on input
        /// </summary>
        public bool AutoSubmit { get; set; } = false;
        
        /// <summary>
        /// Delay in milliseconds for auto-submit
        /// </summary>
        public int AutoSubmitDelay { get; set; } = 500;
        
        /// <summary>
        /// Whether to use AJAX for form submission
        /// </summary>
        public bool IsAjax { get; set; } = false;
        
        /// <summary>
        /// CSS selector for results container (for AJAX)
        /// </summary>
        public string ResultsContainerSelector { get; set; } = "#search-results";
        
        /// <summary>
        /// Whether to show search suggestions
        /// </summary>
        public bool ShowSuggestions { get; set; } = false;
        
        /// <summary>
        /// Type of suggestions to fetch
        /// </summary>
        public string SuggestionType { get; set; } = "general";
        
        /// <summary>
        /// Whether to show category filter
        /// </summary>
        public bool ShowCategoryFilter { get; set; } = false;
        
        /// <summary>
        /// Name of the category parameter
        /// </summary>
        public string CategoryParameterName { get; set; } = "category";
        
        /// <summary>
        /// Text for "All Categories" option
        /// </summary>
        public string CategoryAllText { get; set; } = "All Categories";
        
        /// <summary>
        /// Category options
        /// </summary>
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        
        /// <summary>
        /// Whether to show sort options
        /// </summary>
        public bool ShowSortOptions { get; set; } = false;
        
        /// <summary>
        /// Name of the sort parameter
        /// </summary>
        public string SortParameterName { get; set; } = "sort";
        
        /// <summary>
        /// Sort options
        /// </summary>
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>();
        
        /// <summary>
        /// Whether to show advanced search toggle
        /// </summary>
        public bool ShowAdvancedToggle { get; set; } = false;
        
        /// <summary>
        /// Advanced search fields
        /// </summary>
        public List<SearchField> AdvancedFields { get; set; } = new List<SearchField>();
        
        /// <summary>
        /// Whether to show filter actions
        /// </summary>
        public bool ShowFilterActions { get; set; } = false;
        
        /// <summary>
        /// Whether to show active filters
        /// </summary>
        public bool ShowActiveFilters { get; set; } = false;
        
        /// <summary>
        /// Whether to show results count
        /// </summary>
        public bool ShowResultsCount { get; set; } = false;
        
        /// <summary>
        /// Additional hidden form fields
        /// </summary>
        public Dictionary<string, string> HiddenFields { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Additional CSS classes for the form
        /// </summary>
        public string FormClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Additional CSS classes for the main row
        /// </summary>
        public string RowClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Additional CSS classes for the input group
        /// </summary>
        public string InputGroupClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Additional CSS classes for the search input
        /// </summary>
        public string InputClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Additional CSS classes for select elements
        /// </summary>
        public string SelectClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Column size for search input in horizontal layout
        /// </summary>
        public int SearchColumnSize { get; set; } = 6;
        
        /// <summary>
        /// Column size for category filter in horizontal layout
        /// </summary>
        public int CategoryColumnSize { get; set; } = 3;
        
        /// <summary>
        /// Column size for sort options in horizontal layout
        /// </summary>
        public int SortColumnSize { get; set; } = 3;
        
        /// <summary>
        /// Current search values (for preserving state)
        /// </summary>
        public Dictionary<string, string> CurrentValues { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public SearchFormViewModel()
        {
        }
        
        /// <summary>
        /// Constructor with controller and basic options
        /// </summary>
        /// <param name="controller">Controller name</param>
        /// <param name="placeholder">Search placeholder</param>
        public SearchFormViewModel(string controller, string placeholder = "Search...")
        {
            Controller = controller;
            Placeholder = placeholder;
        }
        
        /// <summary>
        /// Checks if there are active filters
        /// </summary>
        public bool HasActiveFilters => GetActiveFilters().Any();
        
        /// <summary>
        /// Gets list of active filters
        /// </summary>
        /// <returns>List of active filter information</returns>
        public List<ActiveFilter> GetActiveFilters()
        {
            var activeFilters = new List<ActiveFilter>();
            
            // Check current values for active filters
            foreach (var value in CurrentValues.Where(v => !string.IsNullOrEmpty(v.Value)))
            {
                if (value.Key == SearchParameterName && !string.IsNullOrEmpty(value.Value))
                {
                    activeFilters.Add(new ActiveFilter
                    {
                        Key = value.Key,
                        Label = "Search",
                        Value = value.Value,
                        RemoveUrl = GetRemoveFilterUrl(value.Key)
                    });
                }
                else if (value.Key == CategoryParameterName && !string.IsNullOrEmpty(value.Value))
                {
                    var category = Categories.FirstOrDefault(c => c.Value == value.Value);
                    activeFilters.Add(new ActiveFilter
                    {
                        Key = value.Key,
                        Label = "Category",
                        Value = category?.Text ?? value.Value,
                        RemoveUrl = GetRemoveFilterUrl(value.Key)
                    });
                }
                else if (value.Key == SortParameterName && !string.IsNullOrEmpty(value.Value))
                {
                    var sort = SortOptions.FirstOrDefault(s => s.Value == value.Value);
                    activeFilters.Add(new ActiveFilter
                    {
                        Key = value.Key,
                        Label = "Sort",
                        Value = sort?.Text ?? value.Value,
                        RemoveUrl = GetRemoveFilterUrl(value.Key)
                    });
                }
                else
                {
                    // Check advanced fields
                    var field = AdvancedFields.FirstOrDefault(f => f.Name == value.Key);
                    if (field != null)
                    {
                        activeFilters.Add(new ActiveFilter
                        {
                            Key = value.Key,
                            Label = field.Label,
                            Value = value.Value,
                            RemoveUrl = GetRemoveFilterUrl(value.Key)
                        });
                    }
                }
            }
            
            return activeFilters;
        }
        
        /// <summary>
        /// Gets URL for removing a specific filter
        /// </summary>
        /// <param name="filterKey">Filter key to remove</param>
        /// <returns>URL string</returns>
        private string GetRemoveFilterUrl(string filterKey)
        {
            var queryParams = CurrentValues
                .Where(v => v.Key != filterKey && !string.IsNullOrEmpty(v.Value))
                .Select(v => $"{v.Key}={Uri.EscapeDataString(v.Value)}")
                .ToList();
            
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return $"/{Controller}/{Action}{queryString}";
        }
        
        /// <summary>
        /// Adds a hidden field
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="value">Field value</param>
        /// <returns>This instance for chaining</returns>
        public SearchFormViewModel AddHiddenField(string name, string value)
        {
            HiddenFields[name] = value;
            return this;
        }
        
        /// <summary>
        /// Adds a category option
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="value">Option value</param>
        /// <returns>This instance for chaining</returns>
        public SearchFormViewModel AddCategory(string text, string value)
        {
            Categories.Add(new SelectListItem(text, value));
            return this;
        }
        
        /// <summary>
        /// Adds a sort option
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="value">Option value</param>
        /// <returns>This instance for chaining</returns>
        public SearchFormViewModel AddSortOption(string text, string value)
        {
            SortOptions.Add(new SelectListItem(text, value));
            return this;
        }
        
        /// <summary>
        /// Adds an advanced search field
        /// </summary>
        /// <param name="field">Search field</param>
        /// <returns>This instance for chaining</returns>
        public SearchFormViewModel AddAdvancedField(SearchField field)
        {
            AdvancedFields.Add(field);
            return this;
        }
        
        /// <summary>
        /// Sets the current search value
        /// </summary>
        /// <param name="key">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>This instance for chaining</returns>
        public SearchFormViewModel SetCurrentValue(string key, string value)
        {
            CurrentValues[key] = value;
            return this;
        }
        
        /// <summary>
        /// Creates a car search form
        /// </summary>
        /// <returns>SearchFormViewModel</returns>
        public static SearchFormViewModel ForCars()
        {
            return new SearchFormViewModel("Cars", "Search cars by make, model, year...")
            {
                ShowCategoryFilter = true,
                ShowSortOptions = true,
                ShowSuggestions = true,
                SuggestionType = "cars"
            }
            .AddSortOption("Newest First", "newest")
            .AddSortOption("Price: Low to High", "price_asc")
            .AddSortOption("Price: High to Low", "price_desc")
            .AddSortOption("Year: Newest", "year_desc")
            .AddSortOption("Mileage: Lowest", "mileage_asc");
        }
        
        /// <summary>
        /// Creates a car parts search form
        /// </summary>
        /// <returns>SearchFormViewModel</returns>
        public static SearchFormViewModel ForCarParts()
        {
            return new SearchFormViewModel("CarParts", "Search car parts by name, part number...")
            {
                ShowCategoryFilter = true,
                ShowSortOptions = true,
                ShowSuggestions = true,
                SuggestionType = "parts"
            }
            .AddSortOption("Relevance", "relevance")
            .AddSortOption("Price: Low to High", "price_asc")
            .AddSortOption("Price: High to Low", "price_desc")
            .AddSortOption("Newest First", "newest");
        }
        
        /// <summary>
        /// Creates a compact search form
        /// </summary>
        /// <param name="controller">Controller name</param>
        /// <param name="placeholder">Placeholder text</param>
        /// <returns>SearchFormViewModel</returns>
        public static SearchFormViewModel Compact(string controller, string placeholder = "Search...")
        {
            return new SearchFormViewModel(controller, placeholder)
            {
                Layout = "compact",
                ShowButtonText = false,
                FormClass = "compact",
                AutoSubmit = true
            };
        }
    }
    
    /// <summary>
    /// Represents an advanced search field
    /// </summary>
    public class SearchField
    {
        /// <summary>
        /// Field name (parameter name)
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Field type (text, number, select, range, checkbox)
        /// </summary>
        public string Type { get; set; } = "text";
        
        /// <summary>
        /// Field label
        /// </summary>
        public string Label { get; set; } = string.Empty;
        
        /// <summary>
        /// Field placeholder
        /// </summary>
        public string Placeholder { get; set; } = string.Empty;
        
        /// <summary>
        /// Column size (1-12)
        /// </summary>
        public int ColumnSize { get; set; } = 6;
        
        /// <summary>
        /// Options for select/checkbox fields
        /// </summary>
        public List<SelectListItem> Options { get; set; } = new List<SelectListItem>();
        
        /// <summary>
        /// Whether the field is required
        /// </summary>
        public bool Required { get; set; } = false;
        
        /// <summary>
        /// Additional CSS classes
        /// </summary>
        public string CssClass { get; set; } = string.Empty;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public SearchField()
        {
        }
        
        /// <summary>
        /// Constructor with basic properties
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="label">Field label</param>
        /// <param name="type">Field type</param>
        public SearchField(string name, string label, string type = "text")
        {
            Name = name;
            Label = label;
            Type = type;
        }
        
        /// <summary>
        /// Adds an option for select/checkbox fields
        /// </summary>
        /// <param name="text">Option text</param>
        /// <param name="value">Option value</param>
        /// <returns>This instance for chaining</returns>
        public SearchField AddOption(string text, string value)
        {
            Options.Add(new SelectListItem(text, value));
            return this;
        }
    }
    
    /// <summary>
    /// Represents an active filter
    /// </summary>
    public class ActiveFilter
    {
        /// <summary>
        /// Filter parameter key
        /// </summary>
        public string Key { get; set; } = string.Empty;
        
        /// <summary>
        /// Filter display label
        /// </summary>
        public string Label { get; set; } = string.Empty;
        
        /// <summary>
        /// Filter display value
        /// </summary>
        public string Value { get; set; } = string.Empty;
        
        /// <summary>
        /// URL to remove this filter
        /// </summary>
        public string RemoveUrl { get; set; } = string.Empty;
    }
}