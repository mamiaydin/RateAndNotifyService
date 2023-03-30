using System.ComponentModel.DataAnnotations;

namespace RatingService.Models;

public class Notification
{
    [Required]
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedIp { get; set; }
}