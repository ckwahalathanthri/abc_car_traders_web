using System.Text.Json;
using ABCCarTraders.Models;

namespace ABCCarTraders.Helpers
{
    public static class SessionHelper
    {
        // Session keys
        public const string UserIdKey = "UserId";
        public const string UserNameKey = "UserName";
        public const string UserTypeKey = "UserType";
        public const string UserEmailKey = "UserEmail";
        public const string CartCountKey = "CartCount";
        public const string LastLoginKey = "LastLogin";
        public const string IsAuthenticatedKey = "IsAuthenticated";

        // User session management
        public static void SetUserSession(this ISession session, User user)
        {
            session.SetInt32(UserIdKey, user.UserId);
            session.SetString(UserNameKey, $"{user.FirstName} {user.LastName}");
            session.SetString(UserTypeKey, user.UserType.ToString());
            session.SetString(UserEmailKey, user.Email);
            session.SetString(LastLoginKey, DateTime.Now.ToString());
            session.SetString(IsAuthenticatedKey, "true");
        }

        public static void ClearUserSession(this ISession session)
        {
            session.Remove(UserIdKey);
            session.Remove(UserNameKey);
            session.Remove(UserTypeKey);
            session.Remove(UserEmailKey);
            session.Remove(CartCountKey);
            session.Remove(LastLoginKey);
            session.Remove(IsAuthenticatedKey);
        }

        public static bool IsAuthenticated(this ISession session)
        {
            return session.GetString(IsAuthenticatedKey) == "true";
        }

        public static int? GetUserId(this ISession session)
        {
            return session.GetInt32(UserIdKey);
        }

        public static string? GetUserName(this ISession session)
        {
            return session.GetString(UserNameKey);
        }

        public static string? GetUserEmail(this ISession session)
        {
            return session.GetString(UserEmailKey);
        }

        public static UserType? GetUserType(this ISession session)
        {
            var userTypeString = session.GetString(UserTypeKey);
            if (string.IsNullOrEmpty(userTypeString))
                return null;

            return Enum.TryParse<UserType>(userTypeString, out var userType) ? userType : null;
        }

        public static bool IsAdmin(this ISession session)
        {
            return GetUserType(session) == UserType.Admin;
        }

        public static bool IsCustomer(this ISession session)
        {
            return GetUserType(session) == UserType.Customer;
        }

        public static DateTime? GetLastLogin(this ISession session)
        {
            var lastLoginString = session.GetString(LastLoginKey);
            if (string.IsNullOrEmpty(lastLoginString))
                return null;

            return DateTime.TryParse(lastLoginString, out var lastLogin) ? lastLogin : null;
        }

        // Cart management
        public static void SetCartCount(this ISession session, int count)
        {
            session.SetInt32(CartCountKey, count);
        }

        public static int GetCartCount(this ISession session)
        {
            return session.GetInt32(CartCountKey) ?? 0;
        }

        public static void IncrementCartCount(this ISession session)
        {
            var currentCount = GetCartCount(session);
            SetCartCount(session, currentCount + 1);
        }

        public static void DecrementCartCount(this ISession session)
        {
            var currentCount = GetCartCount(session);
            SetCartCount(session, Math.Max(0, currentCount - 1));
        }

        // Generic session methods for complex objects
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        // Boolean helper methods
        public static void SetBoolean(this ISession session, string key, bool value)
        {
            session.SetString(key, value.ToString());
        }

        public static bool GetBoolean(this ISession session, string key)
        {
            var value = session.GetString(key);
            return bool.TryParse(value, out var result) && result;
        }

        // Temporary messages
        public static void SetTempMessage(this ISession session, string message, string type = "info")
        {
            session.SetString("TempMessage", message);
            session.SetString("TempMessageType", type);
        }

        public static string? GetTempMessage(this ISession session)
        {
            var message = session.GetString("TempMessage");
            session.Remove("TempMessage");
            return message;
        }

        public static string GetTempMessageType(this ISession session)
        {
            var type = session.GetString("TempMessageType");
            session.Remove("TempMessageType");
            return type ?? "info";
        }

        public static void SetSuccessMessage(this ISession session, string message)
        {
            SetTempMessage(session, message, "success");
        }

        public static void SetErrorMessage(this ISession session, string message)
        {
            SetTempMessage(session, message, "error");
        }

        public static void SetWarningMessage(this ISession session, string message)
        {
            SetTempMessage(session, message, "warning");
        }

        public static void SetInfoMessage(this ISession session, string message)
        {
            SetTempMessage(session, message, "info");
        }

        // Navigation history
        public static void SetReturnUrl(this ISession session, string url)
        {
            session.SetString("ReturnUrl", url);
        }

        public static string? GetReturnUrl(this ISession session)
        {
            var url = session.GetString("ReturnUrl");
            session.Remove("ReturnUrl");
            return url;
        }

        // Search history
        public static void AddToSearchHistory(this ISession session, string searchTerm)
        {
            var history = GetObject<List<string>>(session, "SearchHistory") ?? new List<string>();

            // Remove if already exists
            history.RemoveAll(h => h.Equals(searchTerm, StringComparison.OrdinalIgnoreCase));

            // Add to beginning
            history.Insert(0, searchTerm);

            // Keep only last 10 searches
            if (history.Count > 10)
                history = history.Take(10).ToList();

            SetObject(session, "SearchHistory", history);
        }

        public static List<string> GetSearchHistory(this ISession session)
        {
            return GetObject<List<string>>(session, "SearchHistory") ?? new List<string>();
        }

        // Recently viewed items
        public static void AddToRecentlyViewed(this ISession session, string itemType, int itemId, string itemName)
        {
            var recentItems = GetObject<List<RecentItem>>(session, "RecentlyViewed") ?? new List<RecentItem>();

            // Remove if already exists
            recentItems.RemoveAll(r => r.ItemType == itemType && r.ItemId == itemId);

            // Add to beginning
            recentItems.Insert(0, new RecentItem
            {
                ItemType = itemType,
                ItemId = itemId,
                ItemName = itemName,
                ViewedAt = DateTime.Now
            });

            // Keep only last 20 items
            if (recentItems.Count > 20)
                recentItems = recentItems.Take(20).ToList();

            SetObject(session, "RecentlyViewed", recentItems);
        }

        public static List<RecentItem> GetRecentlyViewed(this ISession session)
        {
            return GetObject<List<RecentItem>>(session, "RecentlyViewed") ?? new List<RecentItem>();
        }

        // Session timeout management
        public static void ExtendSession(this ISession session)
        {
            session.SetString("LastActivity", DateTime.Now.ToString());
        }

        public static bool IsSessionExpired(this ISession session, int timeoutMinutes = 30)
        {
            var lastActivityString = session.GetString("LastActivity");
            if (string.IsNullOrEmpty(lastActivityString))
                return true;

            if (DateTime.TryParse(lastActivityString, out var lastActivity))
            {
                return DateTime.Now.Subtract(lastActivity).TotalMinutes > timeoutMinutes;
            }

            return true;
        }

        // Preferences
        public static void SetUserPreference(this ISession session, string key, string value)
        {
            var preferences = GetObject<Dictionary<string, string>>(session, "UserPreferences") ?? new Dictionary<string, string>();
            preferences[key] = value;
            SetObject(session, "UserPreferences", preferences);
        }

        public static string? GetUserPreference(this ISession session, string key)
        {
            var preferences = GetObject<Dictionary<string, string>>(session, "UserPreferences");
            return preferences?.GetValueOrDefault(key);
        }

        // Session validation
        public static bool ValidateUserSession(this ISession session)
        {
            if (!IsAuthenticated(session))
                return false;

            if (IsSessionExpired(session))
            {
                ClearUserSession(session);
                return false;
            }

            ExtendSession(session);
            return true;
        }
    }

    // Helper classes
    public class RecentItem
    {
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; }
    }
}