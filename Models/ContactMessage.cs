using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCCarTraders.Models
{
    public class ContactMessage
    {
        public int MessageId { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        
        [StringLength(200)]
        [Display(Name = "Subject")]
        public string? Subject { get; set; }
        
        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }
        
        [Display(Name = "Read")]
        public bool IsRead { get; set; } = false;
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Computed properties
        [NotMapped]
        public string FormattedCreatedAt => CreatedAt.ToString("MMM dd, yyyy HH:mm");
        
        [NotMapped]
        public string StatusBadge => IsRead ? "Read" : "Unread";
        
        [NotMapped]
        public string StatusColor => IsRead ? "success" : "warning";
        
        [NotMapped]
        public string ShortMessage => Message.Length > 100 ? Message.Substring(0, 100) + "..." : Message;
        
        [NotMapped]
        public string DisplaySubject => string.IsNullOrEmpty(Subject) ? "No Subject" : Subject;
        
        [NotMapped]
        public int DaysAgo => (DateTime.Now - CreatedAt).Days;
        
        [NotMapped]
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedAt;
                if (timeSpan.Days > 0)
                    return $"{timeSpan.Days} day{(timeSpan.Days > 1 ? "s" : "")} ago";
                if (timeSpan.Hours > 0)
                    return $"{timeSpan.Hours} hour{(timeSpan.Hours > 1 ? "s" : "")} ago";
                if (timeSpan.Minutes > 0)
                    return $"{timeSpan.Minutes} minute{(timeSpan.Minutes > 1 ? "s" : "")} ago";
                return "Just now";
            }
        }
        
        [NotMapped]
        public string Priority => DaysAgo > 7 ? "Low" : DaysAgo > 3 ? "Medium" : "High";
        
        [NotMapped]
        public string PriorityColor => Priority switch
        {
            "High" => "danger",
            "Medium" => "warning",
            "Low" => "info",
            _ => "secondary"
        };
    }
}