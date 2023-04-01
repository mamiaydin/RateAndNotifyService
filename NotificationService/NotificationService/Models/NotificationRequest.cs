using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models;

public class NotificationRequest
{
    [Key]
    [Required]
    public Guid Guid { get; set; }
    public int NotificationCount { get; set; }
    [Required]
    public DateTime Timestamp { get; set; }
}