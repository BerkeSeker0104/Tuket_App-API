using System.ComponentModel.DataAnnotations;

namespace TuketAppAPI.Models.Auth
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress] // ✅ Geçerli bir e-posta olmalı
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;
    }
}