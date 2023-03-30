using System.ComponentModel.DataAnnotations;

namespace RatingService.Models;

public class RatingNotification
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public int RatingId { get; set; }
    public int Score { get; set; }
    public int ServiceId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string CreatedIp { get; set; }
}