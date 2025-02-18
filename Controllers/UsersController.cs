using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TuketAppAPI.Models;
using TuketAppAPI.Models.Auth; // ✅ DTO'ları ekledik

namespace TuketAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly TuketDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(TuketDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Bu e-posta zaten kayıtlı.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = HashPassword(request.Password),
                Role = "consumer",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (foundUser == null || foundUser.Password != HashPassword(request.Password))
                return Unauthorized("Geçersiz e-posta veya şifre.");

            var token = GenerateJwtToken(foundUser);
            return Ok(new { token });
        }

        // ✅ Kullanıcı Bilgilerini Getiren Endpoint (GET /api/Users/me)
        [HttpGet("me")]
        [Authorize]  // 🔐 Yetkilendirme Gerektirir
        public async Task<IActionResult> GetUserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Token'dan kullanıcı ID'sini al
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Yetkilendirme başarısız." });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı ID'si." });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

            return Ok(new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                role = user.Role
            });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKeyString = jwtSettings["Secret"] ?? throw new Exception("🚨 Error: Secret Key is missing from configuration!");

            var key = new SymmetricSecurityKey(Convert.FromBase64String(secretKeyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}