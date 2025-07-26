using ABCCarTraders.Data;
using ABCCarTraders.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace ABCCarTraders.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // Authentication methods
        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
                
                if (user != null && VerifyPassword(password, user.Password))
                {
                    return user;
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                if (await IsEmailExistsAsync(user.Email))
                {
                    return false;
                }
                
                user.Password = HashPassword(user.Password);
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        // User management methods
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }
        
        public async Task<List<User>> GetUsersByTypeAsync(UserType userType)
        {
            try
            {
                return await _context.Users
                    .Where(u => u.UserType == userType)
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }
        
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
                if (existingUser == null)
                {
                    return false;
                }
                
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;
                existingUser.City = user.City;
                existingUser.Country = user.Country;
                existingUser.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return false;
                }
                
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> DeactivateUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return false;
                }
                
                user.IsActive = false;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> ActivateUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return false;
                }
                
                user.IsActive = true;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        // Admin specific methods
        public async Task<List<User>> GetAllCustomersAsync()
        {
            try
            {
                return await _context.Users
                    .Where(u => u.UserType == UserType.Customer)
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }
        
        public async Task<int> GetTotalCustomersCountAsync()
        {
            try
            {
                return await _context.Users
                    .CountAsync(u => u.UserType == UserType.Customer);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        
        public async Task<int> GetActiveCustomersCountAsync()
        {
            try
            {
                return await _context.Users
                    .CountAsync(u => u.UserType == UserType.Customer && u.IsActive);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        
        public async Task<List<User>> GetRecentCustomersAsync(int count = 5)
        {
            try
            {
                return await _context.Users
                    .Where(u => u.UserType == UserType.Customer)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }
        
        // Password management
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return false;
                }
                
                if (!VerifyPassword(currentPassword, user.Password))
                {
                    return false;
                }
                
                user.Password = HashPassword(newPassword);
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return false;
                }
                
                // Generate a temporary password
                var tempPassword = GenerateRandomPassword();
                user.Password = HashPassword(tempPassword);
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                
                // In a real application, you would send this password via email
                // For now, we'll just return true
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        
        // Profile management
        public async Task<bool> UpdateProfileAsync(int userId, string firstName, string lastName, string phoneNumber, string address, string city, string country)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return false;
                }
                
                user.FirstName = firstName;
                user.LastName = lastName;
                user.PhoneNumber = phoneNumber;
                user.Address = address;
                user.City = city;
                user.Country = country;
                user.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        // User statistics
        public async Task<Dictionary<string, int>> GetUserStatisticsAsync()
        {
            try
            {
                var stats = new Dictionary<string, int>();
                
                stats["TotalUsers"] = await _context.Users.CountAsync();
                stats["TotalCustomers"] = await _context.Users.CountAsync(u => u.UserType == UserType.Customer);
                stats["ActiveCustomers"] = await _context.Users.CountAsync(u => u.UserType == UserType.Customer && u.IsActive);
                stats["InactiveCustomers"] = await _context.Users.CountAsync(u => u.UserType == UserType.Customer && !u.IsActive);
                stats["NewCustomersThisMonth"] = await _context.Users.CountAsync(u => u.UserType == UserType.Customer && u.CreatedAt.Month == DateTime.Now.Month);
                
                return stats;
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }
        
        // Helper methods
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}