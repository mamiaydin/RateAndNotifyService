using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models;

public class Notification
{
    public int Id { get; set; }
    public int Score { get; set; }
    public int ServiceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedIp { get; set; }
    public double AvgScore { get; set; }
}