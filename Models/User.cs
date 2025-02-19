using System.ComponentModel.DataAnnotations;

namespace TuketAppAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required] //  Zorunlu alan
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress] //  Geçerli bir e-posta olmalı
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "consumer"; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}