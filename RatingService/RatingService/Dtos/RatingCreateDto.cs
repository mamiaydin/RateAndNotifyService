using System.ComponentModel.DataAnnotations;

namespace RatingService.Dtos;

public class RatingCreateDto
{
    [Range(minimum: 0 ,maximum: 10, ErrorMessage = "The field {0} must be greater than {1} and less then {2}.")]
    public int Score { get; set; }
    public int ServiceId { get; set; }
}