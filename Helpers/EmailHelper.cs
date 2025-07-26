using System.Net;
using System.Net.Mail;
using System.Text;
using ABCCarTraders.Models;

namespace ABCCarTraders.Helpers
{
    public static class EmailHelper
    {
        // Email configuration (these would typically come from appsettings.json)
        private static string SmtpServer = "smtp.gmail.com";
        private static int SmtpPort = 587;
        private static string SmtpUsername = "your-email@gmail.com";
        private static string SmtpPassword = "your-app-password";
        private static string FromEmail = "noreply@abccartraders.com";
        private static string FromName = "ABC Car Traders";
        
        // Initialize email configuration
        public static void ConfigureEmail(string smtpServer, int smtpPort, string username, string password, string fromEmail, string fromName)
        {
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
            SmtpUsername = username;
            SmtpPassword = password;
            FromEmail = fromEmail;
            FromName = fromName;
        }
        
        // Send email
        public static async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var client = new SmtpClient(SmtpServer, SmtpPort)
                {
                    Credentials = new NetworkCredential(SmtpUsername, SmtpPassword),
                    EnableSsl = true
                };
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(FromEmail, FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                
                mailMessage.To.Add(toEmail);
                
                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Log error (in a real application, use proper logging)
                Console.WriteLine($"Email send failed: {ex.Message}");
                return false;
            }
        }
        
        // Send email with attachments
        public static async Task<bool> SendEmailWithAttachmentAsync(string toEmail, string subject, string body, List<string> attachmentPaths, bool isHtml = true)
        {
            try
            {
                using var client = new SmtpClient(SmtpServer, SmtpPort)
                {
                    Credentials = new NetworkCredential(SmtpUsername, SmtpPassword),
                    EnableSsl = true
                };
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(FromEmail, FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                
                mailMessage.To.Add(toEmail);
                
                // Add attachments
                foreach (var attachmentPath in attachmentPaths)
                {
                    if (File.Exists(attachmentPath))
                    {
                        mailMessage.Attachments.Add(new Attachment(attachmentPath));
                    }
                }
                
                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send with attachment failed: {ex.Message}");
                return false;
            }
        }
        
        // Welcome email for new users
        public static async Task<bool> SendWelcomeEmailAsync(User user)
        {
            var subject = "Welcome to ABC Car Traders!";
            var body = GenerateWelcomeEmailBody(user);
            
            return await SendEmailAsync(user.Email, subject, body);
        }
        
        // Order confirmation email
        public static async Task<bool> SendOrderConfirmationEmailAsync(Order order, User user)
        {
            var subject = $"Order Confirmation - {order.OrderNumber}";
            var body = GenerateOrderConfirmationEmailBody(order, user);
            
            return await SendEmailAsync(user.Email, subject, body);
        }
        
        // Order status update email
        public static async Task<bool> SendOrderStatusUpdateEmailAsync(Order order, User user)
        {
            var subject = $"Order Status Update - {order.OrderNumber}";
            var body = GenerateOrderStatusUpdateEmailBody(order, user);
            
            return await SendEmailAsync(user.Email, subject, body);
        }
        
        // Password reset email
        public static async Task<bool> SendPasswordResetEmailAsync(User user, string resetToken)
        {
            var subject = "Password Reset Request";
            var body = GeneratePasswordResetEmailBody(user, resetToken);
            
            return await SendEmailAsync(user.Email, subject, body);
        }
        
        // Contact form response email
        public static async Task<bool> SendContactFormResponseEmailAsync(ContactMessage message)
        {
            var subject = "Thank you for contacting ABC Car Traders";
            var body = GenerateContactFormResponseEmailBody(message);
            
            return await SendEmailAsync(message.Email, subject, body);
        }
        
        // Admin notification email
        public static async Task<bool> SendAdminNotificationEmailAsync(string subject, string message)
        {
            var adminEmail = "admin@abccartraders.com"; // This would come from configuration
            var body = GenerateAdminNotificationEmailBody(subject, message);
            
            return await SendEmailAsync(adminEmail, subject, body);
        }
        
        // Low stock alert email
        public static async Task<bool> SendLowStockAlertEmailAsync(List<Car> lowStockCars, List<CarPart> lowStockParts)
        {
            var subject = "Low Stock Alert";
            var body = GenerateLowStockAlertEmailBody(lowStockCars, lowStockParts);
            
            return await SendAdminNotificationEmailAsync(subject, body);
        }
        
        // HTML email template generators
        private static string GenerateWelcomeEmailBody(User user)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }");
            sb.AppendLine("        .header { background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }");
            sb.AppendLine("        .content { padding: 20px; }");
            sb.AppendLine("        .footer { background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 0 0 10px 10px; }");
            sb.AppendLine("        .btn { display: inline-block; background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>Welcome to ABC Car Traders!</h1>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine($"            <h2>Hello {user.FirstName} {user.LastName},</h2>");
            sb.AppendLine("            <p>Thank you for joining ABC Car Traders! We're excited to have you as part of our community.</p>");
            sb.AppendLine("            <p>Here's what you can do with your new account:</p>");
            sb.AppendLine("            <ul>");
            sb.AppendLine("                <li>Browse our extensive collection of cars and car parts</li>");
            sb.AppendLine("                <li>Add items to your cart and place orders</li>");
            sb.AppendLine("                <li>Track your order history and status</li>");
            sb.AppendLine("                <li>Manage your profile and preferences</li>");
            sb.AppendLine("            </ul>");
            sb.AppendLine("            <p>If you have any questions, feel free to contact our support team.</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p>Best regards,<br>The ABC Car Traders Team</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }
        
        private static string GenerateOrderConfirmationEmailBody(Order order, User user)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }");
            sb.AppendLine("        .header { background-color: #28a745; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }");
            sb.AppendLine("        .content { padding: 20px; }");
            sb.AppendLine("        .order-details { background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0; }");
            sb.AppendLine("        .footer { background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 0 0 10px 10px; }");
            sb.AppendLine("        table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
            sb.AppendLine("        th, td { padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("        th { background-color: #f8f9fa; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>Order Confirmation</h1>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine($"            <h2>Hello {user.FirstName} {user.LastName},</h2>");
            sb.AppendLine("            <p>Thank you for your order! We've received your order and it's being processed.</p>");
            sb.AppendLine("            <div class='order-details'>");
            sb.AppendLine($"                <h3>Order Details</h3>");
            sb.AppendLine($"                <p><strong>Order Number:</strong> {order.OrderNumber}</p>");
            sb.AppendLine($"                <p><strong>Order Date:</strong> {order.OrderDate:MMM dd, yyyy}</p>");
            sb.AppendLine($"                <p><strong>Total Amount:</strong> {order.TotalAmount:C}</p>");
            sb.AppendLine($"                <p><strong>Payment Method:</strong> {order.PaymentMethod}</p>");
            sb.AppendLine($"                <p><strong>Status:</strong> {order.OrderStatus}</p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <h3>Order Items</h3>");
            sb.AppendLine("            <table>");
            sb.AppendLine("                <thead>");
            sb.AppendLine("                    <tr>");
            sb.AppendLine("                        <th>Item</th>");
            sb.AppendLine("                        <th>Quantity</th>");
            sb.AppendLine("                        <th>Price</th>");
            sb.AppendLine("                        <th>Total</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </thead>");
            sb.AppendLine("                <tbody>");
            
            foreach (var item in order.OrderItems)
            {
                sb.AppendLine("                    <tr>");
                sb.AppendLine($"                        <td>{item.ItemType} (ID: {item.ItemId})</td>");
                sb.AppendLine($"                        <td>{item.Quantity}</td>");
                sb.AppendLine($"                        <td>{item.UnitPrice:C}</td>");
                sb.AppendLine($"                        <td>{item.TotalPrice:C}</td>");
                sb.AppendLine("                    </tr>");
            }
            
            sb.AppendLine("                </tbody>");
            sb.AppendLine("            </table>");
            sb.AppendLine("            <p>We'll send you another email when your order ships.</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p>Best regards,<br>The ABC Car Traders Team</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }
        
        private static string GenerateOrderStatusUpdateEmailBody(Order order, User user)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }");
            sb.AppendLine("        .header { background-color: #17a2b8; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }");
            sb.AppendLine("        .content { padding: 20px; }");
            sb.AppendLine("        .status-update { background-color: #e7f3ff; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #17a2b8; }");
            sb.AppendLine("        .footer { background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 0 0 10px 10px; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>Order Status Update</h1>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine($"            <h2>Hello {user.FirstName} {user.LastName},</h2>");
            sb.AppendLine($"            <p>Your order #{order.OrderNumber} has been updated.</p>");
            sb.AppendLine("            <div class='status-update'>");
            sb.AppendLine($"                <h3>Current Status: {order.OrderStatus}</h3>");
            sb.AppendLine($"                <p><strong>Order Number:</strong> {order.OrderNumber}</p>");
            sb.AppendLine($"                <p><strong>Updated:</strong> {order.UpdatedAt:MMM dd, yyyy HH:mm}</p>");
            sb.AppendLine($"                <p><strong>Payment Status:</strong> {order.PaymentStatus}</p>");
            sb.AppendLine("            </div>");
            
            var statusMessage = order.OrderStatus switch
            {
                OrderStatus.Confirmed => "Your order has been confirmed and is being prepared.",
                OrderStatus.Processing => "Your order is currently being processed.",
                OrderStatus.Shipped => "Your order has been shipped and is on its way!",
                OrderStatus.Delivered => "Your order has been delivered successfully.",
                OrderStatus.Cancelled => "Your order has been cancelled.",
                _ => "Your order status has been updated."
            };
            
            sb.AppendLine($"            <p>{statusMessage}</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p>Best regards,<br>The ABC Car Traders Team</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }
        
        private static string GeneratePasswordResetEmailBody(User user, string resetToken)
        {
            var resetLink = $"https://yourdomain.com/Account/ResetPassword?token={resetToken}";
            
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }");
            sb.AppendLine("        .header { background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }");
            sb.AppendLine("        .content { padding: 20px; }");
            sb.AppendLine("        .footer { background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 0 0 10px 10px; }");
            sb.AppendLine("        .btn { display: inline-block; background-color: #dc3545; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }");
            sb.AppendLine("        .warning { background-color: #fff3cd; color: #856404; padding: 15px; border-radius: 5px; border: 1px solid #ffeaa7; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>Password Reset Request</h1>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine($"            <h2>Hello {user.FirstName} {user.LastName},</h2>");
            sb.AppendLine("            <p>You've requested to reset your password for your ABC Car Traders account.</p>");
            sb.AppendLine("            <p>Click the button below to reset your password:</p>");
            sb.AppendLine($"            <a href='{resetLink}' class='btn'>Reset Password</a>");
            sb.AppendLine("            <div class='warning'>");
            sb.AppendLine("                <p><strong>Important:</strong> This link will expire in 24 hours for security reasons.</p>");
            sb.AppendLine("                <p>If you didn't request this password reset, please ignore this email.</p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p>Best regards,<br>The ABC Car Traders Team</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }
        
        private static string GenerateContactFormResponseEmailBody(ContactMessage message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }");
            sb.AppendLine("        .header { background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }");
            sb.AppendLine("        .content { padding: 20px; }");
            sb.AppendLine("        .footer { background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 0 0 10px 10px; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>Thank You for Contacting Us</h1>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine($"            <h2>Hello {message.Name},</h2>");
            sb.AppendLine("            <p>Thank you for contacting ABC Car Traders. We've received your message and will respond within 24 hours.</p>");
            sb.AppendLine("            <p><strong>Your Message:</strong></p>");
            sb.AppendLine($"            <p><em>{message.Message}</em></p>");
            sb.AppendLine("            <p>We appreciate your interest in our services.</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p>Best regards,<br>The ABC Car Traders Team</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }
        
        private static string GenerateAdminNotificationEmailBody(string subject, string message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }");
            sb.AppendLine("        .header { background-color: #6c757d; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }");
            sb.AppendLine("        .content { padding: 20px; }");
            sb.AppendLine("        .footer { background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 0 0 10px 10px; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine($"            <h1>{subject}</h1>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine("            <h2>Admin Notification</h2>");
            sb.AppendLine($"            <p>{message}</p>");
            sb.AppendLine($"            <p><strong>Time:</strong> {DateTime.Now:MMM dd, yyyy HH:mm}</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p>ABC Car Traders System</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }
        
        private static string GenerateLowStockAlertEmailBody(List<Car> lowStockCars, List<CarPart> lowStockParts)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }");
            sb.AppendLine("        .header { background-color: #ffc107; color: #212529; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }");
            sb.AppendLine("        .content { padding: 20px; }");
            sb.AppendLine("        .footer { background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 0 0 10px 10px; }");
            sb.AppendLine("        table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
            sb.AppendLine("        th, td { padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("        th { background-color: #f8f9fa; }");
            sb.AppendLine("        .low-stock { color: #dc3545; font-weight: bold; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>⚠️ Low Stock Alert</h1>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine("            <h2>Low Stock Items Detected</h2>");
            sb.AppendLine("            <p>The following items are running low in stock and may need restocking:</p>");
            
            if (lowStockCars.Any())
            {
                sb.AppendLine("            <h3>Cars</h3>");
                sb.AppendLine("            <table>");
                sb.AppendLine("                <thead>");
                sb.AppendLine("                    <tr>");
                sb.AppendLine("                        <th>Model</th>");
                sb.AppendLine("                        <th>Year</th>");
                sb.AppendLine("                        <th>Stock</th>");
                sb.AppendLine("                    </tr>");
                sb.AppendLine("                </thead>");
                sb.AppendLine("                <tbody>");
                
                foreach (var car in lowStockCars)
                {
                    sb.AppendLine("                    <tr>");
                    sb.AppendLine($"                        <td>{car.Model}</td>");
                    sb.AppendLine($"                        <td>{car.Year}</td>");
                    sb.AppendLine($"                        <td class='low-stock'>{car.StockQuantity}</td>");
                    sb.AppendLine("                    </tr>");
                }
                
                sb.AppendLine("                </tbody>");
                sb.AppendLine("            </table>");
            }
            
            if (lowStockParts.Any())
            {
                sb.AppendLine("            <h3>Car Parts</h3>");
                sb.AppendLine("            <table>");
                sb.AppendLine("                <thead>");
                sb.AppendLine("                    <tr>");
                sb.AppendLine("                        <th>Part Name</th>");
                sb.AppendLine("                        <th>Part Number</th>");
                sb.AppendLine("                        <th>Stock</th>");
                sb.AppendLine("                    </tr>");
                sb.AppendLine("                </thead>");
                sb.AppendLine("                <tbody>");
                
                foreach (var part in lowStockParts)
                {
                    sb.AppendLine("                    <tr>");
                    sb.AppendLine($"                        <td>{part.PartName}</td>");
                    sb.AppendLine($"                        <td>{part.PartNumber}</td>");
                    sb.AppendLine($"                        <td class='low-stock'>{part.StockQuantity}</td>");
                    sb.AppendLine("                    </tr>");
                }
                
                sb.AppendLine("                </tbody>");
                sb.AppendLine("            </table>");
            }
            
            sb.AppendLine("            <p>Please review and restock these items as needed.</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p>ABC Car Traders Inventory System</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }
        
        // Email validation
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        // Email queue management (for high-volume scenarios)
        private static readonly Queue<EmailQueueItem> EmailQueue = new();
        
        public static void QueueEmail(string toEmail, string subject, string body, bool isHtml = true)
        {
            lock (EmailQueue)
            {
                EmailQueue.Enqueue(new EmailQueueItem
                {
                    ToEmail = toEmail,
                    Subject = subject,
                    Body = body,
                    IsHtml = isHtml,
                    QueuedAt = DateTime.Now
                });
            }
        }
        
        public static async Task ProcessEmailQueueAsync()
        {
            while (EmailQueue.Count > 0)
            {
                EmailQueueItem? item = null;
                
                lock (EmailQueue)
                {
                    if (EmailQueue.Count > 0)
                    {
                        item = EmailQueue.Dequeue();
                    }
                }
                
                if (item != null)
                {
                    await SendEmailAsync(item.ToEmail, item.Subject, item.Body, item.IsHtml);
                    
                    // Small delay to prevent overwhelming the SMTP server
                    await Task.Delay(100);
                }
            }
        }
    }
    
    // Email queue item class
    public class EmailQueueItem
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
        public DateTime QueuedAt { get; set; }
    }
}