using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Models.ViewModels
{
    /// <summary>
    /// View model for error pages
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Unique request identifier for tracking errors
        /// </summary>
        public string? RequestId { get; set; }
        
        /// <summary>
        /// Determines whether to show the request ID on the error page
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        /// <summary>
        /// HTTP status code of the error
        /// </summary>
        public int StatusCode { get; set; }
        
        /// <summary>
        /// Error message to display to the user
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// Detailed error information (for development/debugging)
        /// </summary>
        public string? ErrorDetails { get; set; }
        
        /// <summary>
        /// The URL where the error occurred
        /// </summary>
        public string? RequestUrl { get; set; }
        
        /// <summary>
        /// Timestamp when the error occurred
        /// </summary>
        public DateTime ErrorTimestamp { get; set; } = DateTime.Now;
        
        /// <summary>
        /// User agent information
        /// </summary>
        public string? UserAgent { get; set; }
        
        /// <summary>
        /// Whether to show technical details (should be false in production)
        /// </summary>
        public bool ShowTechnicalDetails { get; set; } = false;
        
        /// <summary>
        /// Suggested actions for the user
        /// </summary>
        public List<string> SuggestedActions { get; set; } = new List<string>();
        
        /// <summary>
        /// Contact information for support
        /// </summary>
        public string? SupportContact { get; set; }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public ErrorViewModel()
        {
            // Set default suggested actions
            SuggestedActions = new List<string>
            {
                "Try refreshing the page",
                "Go back to the previous page",
                "Return to the homepage",
                "Contact support if the problem persists"
            };
            
            SupportContact = "support@abccartraders.com";
        }
        
        /// <summary>
        /// Constructor with request ID
        /// </summary>
        /// <param name="requestId">The request identifier</param>
        public ErrorViewModel(string requestId) : this()
        {
            RequestId = requestId;
        }
        
        /// <summary>
        /// Constructor with full error information
        /// </summary>
        /// <param name="requestId">The request identifier</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="requestUrl">URL where error occurred</param>
        public ErrorViewModel(string requestId, int statusCode, string errorMessage, string requestUrl) : this(requestId)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            RequestUrl = requestUrl;
        }
        
        /// <summary>
        /// Gets user-friendly error message based on status code
        /// </summary>
        /// <returns>User-friendly error message</returns>
        public string GetUserFriendlyMessage()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                return ErrorMessage;
                
            return StatusCode switch
            {
                400 => "Bad Request - The request could not be understood by the server.",
                401 => "Unauthorized - You are not authorized to access this resource.",
                403 => "Forbidden - Access to this resource is forbidden.",
                404 => "Page Not Found - The page you are looking for could not be found.",
                408 => "Request Timeout - The request timed out.",
                429 => "Too Many Requests - Please try again later.",
                500 => "Internal Server Error - An unexpected error occurred on the server.",
                502 => "Bad Gateway - The server received an invalid response.",
                503 => "Service Unavailable - The service is temporarily unavailable.",
                504 => "Gateway Timeout - The server timed out waiting for a response.",
                _ => "An unexpected error occurred. Please try again later."
            };
        }
        
        /// <summary>
        /// Gets appropriate icon for the error type
        /// </summary>
        /// <returns>Font Awesome icon class</returns>
        public string GetErrorIcon()
        {
            return StatusCode switch
            {
                401 or 403 => "fas fa-lock",
                404 => "fas fa-search",
                408 or 504 => "fas fa-clock",
                429 => "fas fa-ban",
                500 or 502 or 503 => "fas fa-server",
                _ => "fas fa-exclamation-triangle"
            };
        }
        
        /// <summary>
        /// Gets appropriate CSS class for the error type
        /// </summary>
        /// <returns>CSS class name</returns>
        public string GetErrorCssClass()
        {
            return StatusCode switch
            {
                401 or 403 => "text-warning",
                404 => "text-info",
                408 or 504 => "text-secondary",
                429 => "text-danger",
                500 or 502 or 503 => "text-danger",
                _ => "text-warning"
            };
        }
        
        /// <summary>
        /// Checks if the error is a client-side error (4xx)
        /// </summary>
        /// <returns>True if client-side error</returns>
        public bool IsClientError()
        {
            return StatusCode >= 400 && StatusCode < 500;
        }
        
        /// <summary>
        /// Checks if the error is a server-side error (5xx)
        /// </summary>
        /// <returns>True if server-side error</returns>
        public bool IsServerError()
        {
            return StatusCode >= 500 && StatusCode < 600;
        }
        
        /// <summary>
        /// Gets recommended actions based on error type
        /// </summary>
        /// <returns>List of recommended actions</returns>
        public List<string> GetRecommendedActions()
        {
            var actions = new List<string>();
            
            switch (StatusCode)
            {
                case 400:
                    actions.AddRange(new[] { "Check the URL for typos", "Try a different approach", "Contact support for assistance" });
                    break;
                case 401:
                    actions.AddRange(new[] { "Log in to your account", "Check your credentials", "Reset your password if needed" });
                    break;
                case 403:
                    actions.AddRange(new[] { "Check if you have permission", "Contact an administrator", "Try logging in with different credentials" });
                    break;
                case 404:
                    actions.AddRange(new[] { "Check the URL for typos", "Use the search feature", "Navigate from the homepage" });
                    break;
                case 408:
                case 504:
                    actions.AddRange(new[] { "Try again in a few moments", "Check your internet connection", "Refresh the page" });
                    break;
                case 429:
                    actions.AddRange(new[] { "Wait a few minutes before trying again", "Reduce the frequency of requests", "Contact support if needed" });
                    break;
                case 500:
                case 502:
                case 503:
                    actions.AddRange(new[] { "Try again in a few minutes", "Clear your browser cache", "Contact support if the problem persists" });
                    break;
                default:
                    actions.AddRange(SuggestedActions);
                    break;
            }
            
            return actions;
        }
    }
}