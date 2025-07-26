using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ABCCarTraders.Models;

namespace ABCCarTraders.Helpers
{
    public static class AuthHelper
    {
        // Password validation
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;
            
            // Password must be at least 6 characters long
            if (password.Length < 6)
                return false;
            
            // Password must contain at least one letter and one number
            bool hasLetter = password.Any(char.IsLetter);
            bool hasNumber = password.Any(char.IsDigit);
            
            return hasLetter && hasNumber;
        }
        
        public static string GetPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "Very Weak";
            
            int score = 0;
            
            // Length check
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            
            // Character variety
            if (password.Any(char.IsLower)) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(ch => !char.IsLetterOrDigit(ch))) score++;
            
            return score switch
            {
                <= 2 => "Very Weak",
                3 => "Weak",
                4 => "Fair",
                5 => "Good",
                >= 6 => "Strong"
            };
        }
        
        // Email validation
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            
            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
        
        // Phone number validation
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return true; // Optional field
            
            // Remove all non-digit characters
            var digitsOnly = Regex.Replace(phoneNumber, @"[^\d]", "");
            
            // Check if it's a valid length (10-15 digits)
            return digitsOnly.Length >= 10 && digitsOnly.Length <= 15;
        }
        
        // Session validation
        public static bool IsUserAuthenticated(ISession session)
        {
            return session.IsAuthenticated();
        }
        
        public static bool IsUserAdmin(ISession session)
        {
            return session.IsAdmin();
        }
        
        public static bool IsUserCustomer(ISession session)
        {
            return session.IsCustomer();
        }
        
        public static int? GetCurrentUserId(ISession session)
        {
            return session.GetUserId();
        }
        
        public static string? GetCurrentUserName(ISession session)
        {
            return session.GetUserName();
        }
        
        public static UserType? GetCurrentUserType(ISession session)
        {
            return session.GetUserType();
        }
        
        // Authorization helpers
        public static bool CanAccessAdminFeatures(ISession session)
        {
            return IsUserAuthenticated(session) && IsUserAdmin(session);
        }
        
        public static bool CanAccessCustomerFeatures(ISession session)
        {
            return IsUserAuthenticated(session) && IsUserCustomer(session);
        }
        
        public static bool CanAccessResource(ISession session, int resourceUserId)
        {
            var currentUserId = GetCurrentUserId(session);
            return currentUserId.HasValue && (currentUserId.Value == resourceUserId || IsUserAdmin(session));
        }
        
        // URL helpers
        public static string GetLoginUrl(string returnUrl = "")
        {
            return string.IsNullOrEmpty(returnUrl) ? "/Account/Login" : $"/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}";
        }
        
        public static string GetUnauthorizedUrl()
        {
            return "/Account/Unauthorized";
        }
        
        public static string GetDashboardUrl(UserType userType)
        {
            return userType switch
            {
                UserType.Admin => "/Admin/Dashboard",
                UserType.Customer => "/Customer/Dashboard",
                _ => "/Home/Index"
            };
        }
        
        // Security helpers
        public static string GenerateSecureToken(int length = 32)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static string GeneratePasswordResetToken()
        {
            return GenerateSecureToken(64);
        }
        
        public static bool IsPasswordResetTokenValid(string token, DateTime createdAt, int expiryHours = 24)
        {
            if (string.IsNullOrEmpty(token))
                return false;
            
            return DateTime.Now.Subtract(createdAt).TotalHours <= expiryHours;
        }
        
        // Input sanitization
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            
            // Remove potentially dangerous characters
            return input.Replace("<", "&lt;")
                       .Replace(">", "&gt;")
                       .Replace("\"", "&quot;")
                       .Replace("'", "&#x27;")
                       .Replace("&", "&amp;");
        }
        
        // Rate limiting helpers
        private static readonly Dictionary<string, List<DateTime>> _requestLog = new();
        
        public static bool IsRateLimited(string identifier, int maxRequests = 5, int timeWindowMinutes = 15)
        {
            lock (_requestLog)
            {
                var now = DateTime.Now;
                var windowStart = now.AddMinutes(-timeWindowMinutes);
                
                if (!_requestLog.ContainsKey(identifier))
                {
                    _requestLog[identifier] = new List<DateTime>();
                }
                
                var requests = _requestLog[identifier];
                
                // Remove old requests
                requests.RemoveAll(r => r < windowStart);
                
                // Check if limit exceeded
                if (requests.Count >= maxRequests)
                {
                    return true;
                }
                
                // Add current request
                requests.Add(now);
                return false;
            }
        }
        
        // Session security
        public static void LogSecurityEvent(string userId, string eventType, string details, string ipAddress)
        {
            // In a real application, you would log to a database or file
            // For now, we'll use console logging
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] SECURITY: User {userId} - {eventType} - {details} - IP: {ipAddress}");
        }
        
        public static void LogLoginAttempt(string email, bool success, string ipAddress)
        {
            var eventType = success ? "LOGIN_SUCCESS" : "LOGIN_FAILED";
            LogSecurityEvent(email, eventType, $"Login attempt from {ipAddress}", ipAddress);
        }
        
        public static void LogPasswordChange(string userId, string ipAddress)
        {
            LogSecurityEvent(userId, "PASSWORD_CHANGE", "User changed password", ipAddress);
        }
        
        public static void LogLogout(string userId, string ipAddress)
        {
            LogSecurityEvent(userId, "LOGOUT", "User logged out", ipAddress);
        }
        
        // IP address helpers
        public static string GetClientIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            
            // Check for X-Forwarded-For header (when behind proxy)
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    ipAddress = forwardedFor.Split(',')[0].Trim();
                }
            }
            
            return ipAddress ?? "Unknown";
        }
        
        // User agent helpers
        public static string GetUserAgent(HttpContext context)
        {
            return context.Request.Headers["User-Agent"].ToString();
        }
        
        public static bool IsSuspiciousUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return true;
            
            // Check for common bot patterns
            var suspiciousPatterns = new[]
            {
                "bot", "crawler", "spider", "scraper", "curl", "wget", "python", "java"
            };
            
            return suspiciousPatterns.Any(pattern => 
                userAgent.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }
        
        // Validation helpers
        public static bool IsValidRedirectUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            
            // Only allow relative URLs or URLs from the same domain
            if (url.StartsWith("/"))
                return true;
            
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                // Add your domain validation logic here
                return false; // For security, don't allow absolute URLs
            }
            
            return false;
        }
        
        // Account lockout helpers
        private static readonly Dictionary<string, AccountLockoutInfo> _lockoutInfo = new();
        
        public static bool IsAccountLocked(string email)
        {
            lock (_lockoutInfo)
            {
                if (_lockoutInfo.ContainsKey(email))
                {
                    var info = _lockoutInfo[email];
                    if (DateTime.Now < info.LockoutEnd)
                    {
                        return true;
                    }
                    else
                    {
                        _lockoutInfo.Remove(email);
                    }
                }
                return false;
            }
        }
        
        public static void RecordFailedLogin(string email)
        {
            lock (_lockoutInfo)
            {
                if (!_lockoutInfo.ContainsKey(email))
                {
                    _lockoutInfo[email] = new AccountLockoutInfo();
                }
                
                var info = _lockoutInfo[email];
                info.FailedAttempts++;
                info.LastFailedAttempt = DateTime.Now;
                
                // Lock account after 5 failed attempts
                if (info.FailedAttempts >= 5)
                {
                    info.LockoutEnd = DateTime.Now.AddMinutes(30);
                }
            }
        }
        
        public static void ClearFailedLogins(string email)
        {
            lock (_lockoutInfo)
            {
                if (_lockoutInfo.ContainsKey(email))
                {
                    _lockoutInfo.Remove(email);
                }
            }
        }
        
        public static TimeSpan GetLockoutTimeRemaining(string email)
        {
            lock (_lockoutInfo)
            {
                if (_lockoutInfo.ContainsKey(email))
                {
                    var info = _lockoutInfo[email];
                    if (DateTime.Now < info.LockoutEnd)
                    {
                        return info.LockoutEnd - DateTime.Now;
                    }
                }
                return TimeSpan.Zero;
            }
        }
    }
    
    // Helper classes
    public class AccountLockoutInfo
    {
        public int FailedAttempts { get; set; }
        public DateTime LastFailedAttempt { get; set; }
        public DateTime LockoutEnd { get; set; }
    }
    
    // Authorization attributes
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public UserType[]? AllowedUserTypes { get; set; }
        public bool RequireAuthentication { get; set; } = true;
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            
            if (RequireAuthentication && !AuthHelper.IsUserAuthenticated(session))
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectResult(AuthHelper.GetLoginUrl(returnUrl));
                return;
            }
            
            if (AllowedUserTypes != null && AllowedUserTypes.Any())
            {
                var userType = AuthHelper.GetCurrentUserType(session);
                if (userType == null || !AllowedUserTypes.Contains(userType.Value))
                {
                    context.Result = new RedirectResult(AuthHelper.GetUnauthorizedUrl());
                    return;
                }
            }
            
            base.OnActionExecuting(context);
        }
    }
    
    public class AdminOnlyAttribute : AuthorizeAttribute
    {
        public AdminOnlyAttribute()
        {
            AllowedUserTypes = new[] { UserType.Admin };
        }
    }
    
    public class CustomerOnlyAttribute : AuthorizeAttribute
    {
        public CustomerOnlyAttribute()
        {
            AllowedUserTypes = new[] { UserType.Customer };
        }
    }
}