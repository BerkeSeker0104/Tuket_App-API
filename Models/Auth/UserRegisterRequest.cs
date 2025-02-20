using System.ComponentModel.DataAnnotations;

namespace TuketAppAPI.Models.Auth
{
    public class UserRegisterRequest
    {
        [Required] // Boş olamaz
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress] // Geçerli bir e-posta olmalı
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")] // Şifre minimum 6 karakter
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "consumer"; //  Varsayılan olarak "consumer"
    }
}