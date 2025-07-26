using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for loading spinner component
    /// </summary>
    public class LoadingSpinnerViewModel
    {
        /// <summary>
        /// Unique identifier for the spinner
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Type of spinner animation
        /// </summary>
        public string Type { get; set; } = "border";

        /// <summary>
        /// Size of the spinner (sm, md, lg, xl)
        /// </summary>
        public string Size { get; set; } = "md";

        /// <summary>
        /// Color theme for the spinner
        /// </summary>
        public string Color { get; set; } = "primary";

        /// <summary>
        /// Main loading message
        /// </summary>
        public string Message { get; set; } = "Loading...";

        /// <summary>
        /// Secondary/sub message
        /// </summary>
        public string SubMessage { get; set; } = string.Empty;

        /// <summary>
        /// Whether the spinner is currently visible
        /// </summary>
        public bool IsVisible { get; set; } = false;

        /// <summary>
        /// Whether to show full screen overlay
        /// </summary>
        public bool ShowOverlay { get; set; } = false;

        /// <summary>
        /// Whether to show local backdrop
        /// </summary>
        public bool ShowBackdrop { get; set; } = false;

        /// <summary>
        /// Whether to show progress bar
        /// </summary>
        public bool ShowProgress { get; set; } = false;

        /// <summary>
        /// Current progress value (0-100)
        /// </summary>
        public int Progress { get; set; } = 0;

        /// <summary>
        /// Whether to show progress percentage text
        /// </summary>
        public bool ShowProgressText { get; set; } = false;

        /// <summary>
        /// Whether to show cancel button
        /// </summary>
        public bool ShowCancelButton { get; set; } = false;

        /// <summary>
        /// Additional CSS classes for container
        /// </summary>
        public string ContainerClass { get; set; } = string.Empty;

        /// <summary>
        /// Additional CSS classes for spinner
        /// </summary>
        public string SpinnerClass { get; set; } = string.Empty;

        /// <summary>
        /// Auto-hide after specified duration (milliseconds)
        /// </summary>
        public int AutoHideDuration { get; set; } = 0;

        /// <summary>
        /// Whether to auto-hide the spinner
        /// </summary>
        public bool AutoHide => AutoHideDuration > 0;

        /// <summary>
        /// Custom inline styles
        /// </summary>
        public string Style { get; set; } = string.Empty;

        /// <summary>
        /// Accessibility label
        /// </summary>
        public string AriaLabel { get; set; } = "Loading content";

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoadingSpinnerViewModel()
        {
        }

        /// <summary>
        /// Constructor with message
        /// </summary>
        /// <param name="message">Loading message</param>
        public LoadingSpinnerViewModel(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Constructor with full options
        /// </summary>
        /// <param name="message">Loading message</param>
        /// <param name="type">Spinner type</param>
        /// <param name="showOverlay">Whether to show overlay</param>
        /// <param name="isVisible">Whether spinner is visible</param>
        public LoadingSpinnerViewModel(string message, string type = "border", bool showOverlay = false, bool isVisible = false)
        {
            Message = message;
            Type = type;
            ShowOverlay = showOverlay;
            IsVisible = isVisible;
        }

        /// <summary>
        /// Available spinner types
        /// </summary>
        public static class SpinnerTypes
        {
            public const string Border = "border";
            public const string Dots = "dots";
            public const string Pulse = "pulse";
            public const string Bars = "bars";
            public const string Ring = "ring";
            public const string Car = "car";
        }

        /// <summary>
        /// Available spinner sizes
        /// </summary>
        public static class SpinnerSizes
        {
            public const string Small = "sm";
            public const string Medium = "md";
            public const string Large = "lg";
            public const string ExtraLarge = "xl";
        }

        /// <summary>
        /// Available color themes
        /// </summary>
        public static class SpinnerColors
        {
            public const string Primary = "primary";
            public const string Secondary = "secondary";
            public const string Success = "success";
            public const string Danger = "danger";
            public const string Warning = "warning";
            public const string Info = "info";
            public const string Light = "light";
            public const string Dark = "dark";
        }

        /// <summary>
        /// Creates a basic loading spinner
        /// </summary>
        /// <param name="message">Loading message</param>
        /// <returns>LoadingSpinnerViewModel</returns>
        public static LoadingSpinnerViewModel Basic(string message = "Loading...")
        {
            return new LoadingSpinnerViewModel
            {
                Message = message,
                Type = SpinnerTypes.Border,
                Size = SpinnerSizes.Medium,
                Color = SpinnerColors.Primary,
                IsVisible = true
            };
        }

        /// <summary>
        /// Creates an overlay loading spinner
        /// </summary>
        /// <param name="message">Loading message</param>
        /// <returns>LoadingSpinnerViewModel</returns>
        public static LoadingSpinnerViewModel Overlay(string message = "Loading...")
        {
            return new LoadingSpinnerViewModel
            {
                Message = message,
                Type = SpinnerTypes.Ring,
                Size = SpinnerSizes.Large,
                Color = SpinnerColors.Primary,
                ShowOverlay = true,
                IsVisible = true
            };
        }

        /// <summary>
        /// Creates a progress loading spinner
        /// </summary>
        /// <param name="message">Loading message</param>
        /// <param name="progress">Current progress (0-100)</param>
        /// <returns>LoadingSpinnerViewModel</returns>
        public static LoadingSpinnerViewModel WithProgress(string message = "Processing...", int progress = 0)
        {
            return new LoadingSpinnerViewModel
            {
                Message = message,
                Type = SpinnerTypes.Border,
                Size = SpinnerSizes.Medium,
                Color = SpinnerColors.Primary,
                ShowProgress = true,
                Progress = Math.Clamp(progress, 0, 100),
                ShowProgressText = true,
                ShowBackdrop = true,
                IsVisible = true
            };
        }

        /// <summary>
        /// Creates a car-themed loading spinner
        /// </summary>
        /// <param name="message">Loading message</param>
        /// <returns>LoadingSpinnerViewModel</returns>
        public static LoadingSpinnerViewModel CarThemed(string message = "Loading vehicles...")
        {
            return new LoadingSpinnerViewModel
            {
                Message = message,
                Type = SpinnerTypes.Car,
                Size = SpinnerSizes.Large,
                Color = SpinnerColors.Primary,
                ShowBackdrop = true,
                IsVisible = true
            };
        }

        /// <summary>
        /// Creates a small inline spinner
        /// </summary>
        /// <param name="message">Loading message</param>
        /// <returns>LoadingSpinnerViewModel</returns>
        public static LoadingSpinnerViewModel Small(string message = "")
        {
            return new LoadingSpinnerViewModel
            {
                Message = message,
                Type = SpinnerTypes.Border,
                Size = SpinnerSizes.Small,
                Color = SpinnerColors.Primary,
                IsVisible = true
            };
        }

        /// <summary>
        /// Creates a dots spinner
        /// </summary>
        /// <param name="message">Loading message</param>
        /// <returns>LoadingSpinnerViewModel</returns>
        public static LoadingSpinnerViewModel Dots(string message = "Loading...")
        {
            return new LoadingSpinnerViewModel
            {
                Message = message,
                Type = SpinnerTypes.Dots,
                Size = SpinnerSizes.Medium,
                Color = SpinnerColors.Primary,
                IsVisible = true
            };
        }

        /// <summary>
        /// Creates a pulse spinner
        /// </summary>
        /// <param name="message">Loading message</param>
        /// <returns>LoadingSpinnerViewModel</returns>
        public static LoadingSpinnerViewModel Pulse(string message = "Loading...")
        {
            return new LoadingSpinnerViewModel
            {
                Message = message,
                Type = SpinnerTypes.Pulse,
                Size = SpinnerSizes.Medium,
                Color = SpinnerColors.Primary,
                IsVisible = true
            };
        }

        /// <summary>
        /// Validates the spinner configuration
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            // Validate type
            var validTypes = new[] { SpinnerTypes.Border, SpinnerTypes.Dots, SpinnerTypes.Pulse, SpinnerTypes.Bars, SpinnerTypes.Ring, SpinnerTypes.Car };
            if (!validTypes.Contains(Type))
            {
                errors.Add($"Invalid spinner type: {Type}");
            }

            // Validate size
            var validSizes = new[] { SpinnerSizes.Small, SpinnerSizes.Medium, SpinnerSizes.Large, SpinnerSizes.ExtraLarge };
            if (!validSizes.Contains(Size))
            {
                errors.Add($"Invalid spinner size: {Size}");
            }

            // Validate color
            var validColors = new[] { SpinnerColors.Primary, SpinnerColors.Secondary, SpinnerColors.Success, SpinnerColors.Danger, SpinnerColors.Warning, SpinnerColors.Info, SpinnerColors.Light, SpinnerColors.Dark };
            if (!validColors.Contains(Color))
            {
                errors.Add($"Invalid spinner color: {Color}");
            }

            // Validate progress
            if (ShowProgress && (Progress < 0 || Progress > 100))
            {
                errors.Add("Progress must be between 0 and 100");
            }

            // Validate auto-hide duration
            if (AutoHideDuration < 0)
            {
                errors.Add("Auto-hide duration cannot be negative");
            }

            return errors;
        }

        /// <summary>
        /// Gets the CSS class for the spinner type
        /// </summary>
        /// <returns>CSS class string</returns>
        public string GetSpinnerCssClass()
        {
            var baseClass = Type switch
            {
                SpinnerTypes.Border => "spinner-border",
                SpinnerTypes.Dots => "spinner-dots",
                SpinnerTypes.Pulse => "spinner-pulse",
                SpinnerTypes.Bars => "spinner-bars",
                SpinnerTypes.Ring => "spinner-ring",
                SpinnerTypes.Car => "spinner-car",
                _ => "spinner-border"
            };

            var sizeClass = Size switch
            {
                SpinnerSizes.Small => "spinner-sm",
                SpinnerSizes.Large => "spinner-lg",
                SpinnerSizes.ExtraLarge => "spinner-xl",
                _ => ""
            };

            return string.Join(" ", new[] { baseClass, sizeClass, SpinnerClass }.Where(s => !string.IsNullOrEmpty(s)));
        }

        /// <summary>
        /// Gets the complete container CSS class
        /// </summary>
        /// <returns>CSS class string</returns>
        public string GetContainerCssClass()
        {
            var classes = new List<string> { "loading-container" };

            if (!string.IsNullOrEmpty(ContainerClass))
            {
                classes.Add(ContainerClass);
            }

            return string.Join(" ", classes);
        }

        /// <summary>
        /// Clones the spinner view model
        /// </summary>
        /// <returns>New instance with same properties</returns>
        public LoadingSpinnerViewModel Clone()
        {
            return new LoadingSpinnerViewModel
            {
                Id = Id,
                Type = Type,
                Size = Size,
                Color = Color,
                Message = Message,
                SubMessage = SubMessage,
                IsVisible = IsVisible,
                ShowOverlay = ShowOverlay,
                ShowBackdrop = ShowBackdrop,
                ShowProgress = ShowProgress,
                Progress = Progress,
                ShowProgressText = ShowProgressText,
                ShowCancelButton = ShowCancelButton,
                ContainerClass = ContainerClass,
                SpinnerClass = SpinnerClass,
                AutoHideDuration = AutoHideDuration,
                Style = Style,
                AriaLabel = AriaLabel
            };
        }
    }
}