using Microsoft.AspNetCore.Mvc;
using ABCCarTraders.Models;
using ABCCarTraders.Models.ViewModels;
using ABCCarTraders.Services;
using ABCCarTraders.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ABCCarTraders.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;
        
        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // If user is already logged in, redirect to dashboard
            if (HttpContext.Session.IsAuthenticated())
            {
                var userType = HttpContext.Session.GetUserType();
                if (userType.HasValue)
                {
                    return RedirectToAction("Dashboard", userType.Value == UserType.Admin ? "Admin" : "Customer");
                }
            }
            
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var clientIp = AuthHelper.GetClientIpAddress(HttpContext);
                    
                    // Check if account is locked
                    if (AuthHelper.IsAccountLocked(model.Email))
                    {
                        var lockoutTime = AuthHelper.GetLockoutTimeRemaining(model.Email);
                        model.ErrorMessage = $"Account is temporarily locked. Please try again in {lockoutTime.Minutes} minutes.";
                        AuthHelper.LogLoginAttempt(model.Email, false, clientIp);
                        return View(model);
                    }
                    
                    // Check for rate limiting
                    if (AuthHelper.IsRateLimited(clientIp))
                    {
                        model.ErrorMessage = "Too many login attempts. Please try again later.";
                        return View(model);
                    }
                    
                    // Authenticate user
                    var user = await _userService.AuthenticateAsync(model.Email, model.Password);
                    
                    if (user != null)
                    {
                        // Clear any failed login attempts
                        AuthHelper.ClearFailedLogins(model.Email);
                        
                        // Set user session
                        HttpContext.Session.SetUserSession(user);
                        
                        // Log successful login
                        AuthHelper.LogLoginAttempt(model.Email, true, clientIp);
                        
                        // Redirect based on user type or return URL
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && AuthHelper.IsValidRedirectUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        
                        return RedirectToAction("Dashboard", user.UserType == UserType.Admin ? "Admin" : "Customer");
                    }
                    else
                    {
                        // Record failed login attempt
                        AuthHelper.RecordFailedLogin(model.Email);
                        AuthHelper.LogLoginAttempt(model.Email, false, clientIp);
                        
                        model.ErrorMessage = "Invalid email or password.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during login attempt for email: {Email}", model.Email);
                    model.ErrorMessage = "An error occurred during login. Please try again.";
                }
            }
            
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            // If user is already logged in, redirect to dashboard
            if (HttpContext.Session.IsAuthenticated())
            {
                var userType = HttpContext.Session.GetUserType();
                if (userType.HasValue)
                {
                    return RedirectToAction("Dashboard", userType.Value == UserType.Admin ? "Admin" : "Customer");
                }
            }
            
            return View(new RegisterViewModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var clientIp = AuthHelper.GetClientIpAddress(HttpContext);
                    
                    // Check for rate limiting
                    if (AuthHelper.IsRateLimited(clientIp))
                    {
                        model.ErrorMessage = "Too many registration attempts. Please try again later.";
                        return View(model);
                    }
                    
                    // Additional validation
                    if (!AuthHelper.IsValidEmail(model.Email))
                    {
                        ModelState.AddModelError("Email", "Please enter a valid email address.");
                        return View(model);
                    }
                    
                    if (!AuthHelper.IsValidPassword(model.Password))
                    {
                        ModelState.AddModelError("Password", "Password must be at least 6 characters long and contain both letters and numbers.");
                        return View(model);
                    }
                    
                    if (!AuthHelper.IsValidPhoneNumber(model.PhoneNumber))
                    {
                        ModelState.AddModelError("PhoneNumber", "Please enter a valid phone number.");
                        return View(model);
                    }
                    
                    // Check if email already exists
                    if (await _userService.IsEmailExistsAsync(model.Email))
                    {
                        model.ErrorMessage = "An account with this email address already exists.";
                        return View(model);
                    }
                    
                    // Create user
                    var user = new User
                    {
                        FirstName = AuthHelper.SanitizeInput(model.FirstName),
                        LastName = AuthHelper.SanitizeInput(model.LastName),
                        Email = model.Email.ToLower(),
                        Password = model.Password,
                        PhoneNumber = model.PhoneNumber,
                        Address = AuthHelper.SanitizeInput(model.Address),
                        City = AuthHelper.SanitizeInput(model.City),
                        Country = AuthHelper.SanitizeInput(model.Country),
                        UserType = UserType.Customer,
                        IsActive = true
                    };
                    
                    var result = await _userService.RegisterAsync(user);
                    
                    if (result)
                    {
                        // Send welcome email
                        await EmailHelper.SendWelcomeEmailAsync(user);
                        
                        // Log security event
                        AuthHelper.LogSecurityEvent(user.Email, "USER_REGISTERED", "New user registration", clientIp);
                        
                        model.SuccessMessage = "Registration successful! Please log in with your credentials.";
                        HttpContext.Session.SetSuccessMessage("Registration successful! Please log in with your credentials.");
                        
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        model.ErrorMessage = "Registration failed. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during registration for email: {Email}", model.Email);
                    model.ErrorMessage = "An error occurred during registration. Please try again.";
                }
            }
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            var userId = HttpContext.Session.GetUserId();
            var clientIp = AuthHelper.GetClientIpAddress(HttpContext);
            
            if (userId.HasValue)
            {
                AuthHelper.LogLogout(userId.Value.ToString(), clientIp);
            }
            
            HttpContext.Session.ClearUserSession();
            HttpContext.Session.SetSuccessMessage("You have been logged out successfully.");
            
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var clientIp = AuthHelper.GetClientIpAddress(HttpContext);
                    
                    // Check for rate limiting
                    if (AuthHelper.IsRateLimited(clientIp))
                    {
                        model.ErrorMessage = "Too many password reset attempts. Please try again later.";
                        return View(model);
                    }
                    
                    var user = await _userService.GetUserByEmailAsync(model.Email);
                    
                    if (user != null)
                    {
                        // Generate reset token
                        var resetToken = AuthHelper.GeneratePasswordResetToken();
                        
                        // Send reset email
                        await EmailHelper.SendPasswordResetEmailAsync(user, resetToken);
                        
                        // Log security event
                        AuthHelper.LogSecurityEvent(user.Email, "PASSWORD_RESET_REQUEST", "Password reset requested", clientIp);
                    }
                    
                    // Always show success message for security
                    model.SuccessMessage = "If an account with this email exists, a password reset link has been sent.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during password reset for email: {Email}", model.Email);
                    model.ErrorMessage = "An error occurred. Please try again.";
                }
            }
            
            return View(model);
        }
        
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            
            var model = new ResetPasswordViewModel
            {
                Token = token
            };
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!AuthHelper.IsValidPassword(model.NewPassword))
                    {
                        ModelState.AddModelError("NewPassword", "Password must be at least 6 characters long and contain both letters and numbers.");
                        return View(model);
                    }
                    
                    // In a real application, you would validate the token and get the user
                    // For now, we'll just show success
                    model.SuccessMessage = "Password has been reset successfully. Please log in with your new password.";
                    
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during password reset");
                    model.ErrorMessage = "An error occurred while resetting your password. Please try again.";
                }
            }
            
            return View(model);
        }
        
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Login");
            }
            
            return View(new ChangePasswordViewModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Login");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = HttpContext.Session.GetUserId();
                    if (!userId.HasValue)
                    {
                        return RedirectToAction("Login");
                    }
                    
                    if (!AuthHelper.IsValidPassword(model.NewPassword))
                    {
                        ModelState.AddModelError("NewPassword", "Password must be at least 6 characters long and contain both letters and numbers.");
                        return View(model);
                    }
                    
                    var result = await _userService.ChangePasswordAsync(userId.Value, model.CurrentPassword, model.NewPassword);
                    
                    if (result)
                    {
                        var clientIp = AuthHelper.GetClientIpAddress(HttpContext);
                        AuthHelper.LogPasswordChange(userId.Value.ToString(), clientIp);
                        
                        HttpContext.Session.SetSuccessMessage("Password changed successfully.");
                        
                        var userType = HttpContext.Session.GetUserType();
                        return RedirectToAction("Dashboard", userType == UserType.Admin ? "Admin" : "Customer");
                    }
                    else
                    {
                        model.ErrorMessage = "Current password is incorrect.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during password change");
                    model.ErrorMessage = "An error occurred while changing your password. Please try again.";
                }
            }
            
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Unauthorized()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Login");
            }
            
            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            try
            {
                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }
                
                var model = new ProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    City = user.City,
                    Country = user.Country,
                    UserType = user.UserType,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile for user: {UserId}", userId);
                return RedirectToAction("Index", "Home");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Login");
            }
            
            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    if (!AuthHelper.IsValidPhoneNumber(model.PhoneNumber))
                    {
                        ModelState.AddModelError("PhoneNumber", "Please enter a valid phone number.");
                        return View(model);
                    }
                    
                    var result = await _userService.UpdateProfileAsync(
                        userId.Value,
                        AuthHelper.SanitizeInput(model.FirstName),
                        AuthHelper.SanitizeInput(model.LastName),
                        model.PhoneNumber,
                        AuthHelper.SanitizeInput(model.Address),
                        AuthHelper.SanitizeInput(model.City),
                        AuthHelper.SanitizeInput(model.Country)
                    );
                    
                    if (result)
                    {
                        // Update session with new name
                        HttpContext.Session.SetString(SessionHelper.UserNameKey, $"{model.FirstName} {model.LastName}");
                        
                        model.SuccessMessage = "Profile updated successfully.";
                        HttpContext.Session.SetSuccessMessage("Profile updated successfully.");
                    }
                    else
                    {
                        model.ErrorMessage = "Failed to update profile. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating profile for user: {UserId}", userId);
                    model.ErrorMessage = "An error occurred while updating your profile. Please try again.";
                }
            }
            
            return View(model);
        }
    }
    
    // Supporting ViewModels
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;
        
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
    
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please confirm your new password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
    
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please confirm your new password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
    
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? PhoneNumber { get; set; }
        
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        
        public UserType UserType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}