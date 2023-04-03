using System.ComponentModel.DataAnnotations;
using RatingService.Models;

namespace RatingService.Dtos;

public class RatingReadDto
{
    public int Id { get; set; }
    public int Score { get; set; }
    public int ServiceId { get; set; }
    public double AvgScore { get; set; }
}