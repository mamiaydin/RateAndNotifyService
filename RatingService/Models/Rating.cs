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
    
    public Service Service { get; set; }
}