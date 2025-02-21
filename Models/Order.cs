using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TuketAppAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BusinessId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // "Pending", "Preparing", "Completed", "Cancelled"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}