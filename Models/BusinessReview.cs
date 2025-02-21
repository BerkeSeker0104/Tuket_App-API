using System.ComponentModel.DataAnnotations;

namespace TuketAppAPI.Models
{
    public class BusinessReview
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BusinessId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}