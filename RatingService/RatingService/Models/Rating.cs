using System.ComponentModel.DataAnnotations;

namespace RatingService.Models;

public class Rating
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public int Score { get; set; }
    [Required]
    public int ServiceId { get; set; }
    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required]
    public string? CreatedIp { get; set; }
    
    public Service Service { get; set; }
}