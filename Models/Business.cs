using System.ComponentModel.DataAnnotations;

namespace TuketAppAPI.Models
{
    public class Business
    {
        public int Id { get; set; }

        [Required] // ✅ Name boş geçilemez!
        public string Name { get; set; } = string.Empty; // ✅ Varsayılan değer eklendi.

        [Required] // ✅ Address boş geçilemez!
        public string Address { get; set; } = string.Empty; // ✅ Varsayılan değer eklendi.

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}