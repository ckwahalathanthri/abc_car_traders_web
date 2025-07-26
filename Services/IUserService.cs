using ABCCarTraders.Models;

namespace ABCCarTraders.Services
{
    public interface IUserService
    {
        // Authentication methods
        Task<User?> AuthenticateAsync(string email, string password);
        Task<bool> RegisterAsync(User user);
        Task<bool> IsEmailExistsAsync(string email);
        
        // User management methods
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetUsersByTypeAsync(UserType userType);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> DeactivateUserAsync(int userId);
        Task<bool> ActivateUserAsync(int userId);
        
        // Admin specific methods
        Task<List<User>> GetAllCustomersAsync();
        Task<int> GetTotalCustomersCountAsync();
        Task<int> GetActiveCustomersCountAsync();
        Task<List<User>> GetRecentCustomersAsync(int count = 5);
        
        // Password management
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        
        // Profile management
        Task<bool> UpdateProfileAsync(int userId, string firstName, string lastName, string phoneNumber, string address, string city, string country);
        
        // User statistics
        Task<Dictionary<string, int>> GetUserStatisticsAsync();
    }
}