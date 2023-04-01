using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models;

public class Rating
{
    public int Id { get; set; }
    public int Score { get; set; }
    public int ServiceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedIp { get; set; }
}