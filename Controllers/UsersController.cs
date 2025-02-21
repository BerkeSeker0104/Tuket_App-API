using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TuketAppAPI.Models;
using TuketAppAPI.Models.Auth;
using Serilog; //  Serilog kütüphanesi eklendi

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
            try
            {
                Log.Information(" Yeni kullanıcı kaydı yapılıyor: {Email}", request.Email);

                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    Log.Warning("⚠️ Kayıt başarısız: {Email} zaten mevcut.", request.Email);
                    return BadRequest("Bu e-posta zaten kayıtlı.");
                }

                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = HashPassword(request.Password),
                    Role = request.Role ?? "consumer",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                Log.Information(" Kullanıcı başarıyla kaydedildi: {Email}", user.Email);
                return Ok("Kullanıcı başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, " Kullanıcı kaydı sırasında hata oluştu: {Email}", request.Email);
                return StatusCode(500, "Sunucu hatası. Lütfen daha sonra tekrar deneyiniz.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            try
            {
                Log.Information("🔑 Kullanıcı giriş yapıyor: {Email}", request.Email);

                var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (foundUser == null || foundUser.Password != HashPassword(request.Password))
                {
                    Log.Warning("⚠️ Başarısız giriş denemesi: {Email}", request.Email);
                    return Unauthorized("Geçersiz e-posta veya şifre.");
                }

                var token = GenerateJwtToken(foundUser);

                Log.Information(" Kullanıcı giriş yaptı: {Email}", request.Email);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                Log.Error(ex, " Kullanıcı girişi sırasında hata oluştu: {Email}", request.Email);
                return StatusCode(500, "Sunucu hatası. Lütfen daha sonra tekrar deneyiniz.");
            }
        }

        [HttpGet("me")]
        [Authorize]  
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Log.Warning("⚠️ Yetkilendirme başarısız. Kullanıcı ID bulunamadı.");
                    return Unauthorized(new { message = "Yetkilendirme başarısız." });
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    Log.Warning("⚠️ Geçersiz kullanıcı ID'si.");
                    return Unauthorized(new { message = "Geçersiz kullanıcı ID'si." });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    Log.Warning("⚠️ Kullanıcı bulunamadı. ID: {UserId}", userId);
                    return NotFound(new { message = "Kullanıcı bulunamadı." });
                }

                Log.Information(" Kullanıcı bilgileri getirildi: {Email}", user.Email);
                return Ok(new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, " Kullanıcı bilgileri getirilirken hata oluştu.");
                return StatusCode(500, "Sunucu hatası. Lütfen daha sonra tekrar deneyiniz.");
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKeyString = jwtSettings["Secret"] ?? throw new Exception(" Error: Secret Key is missing from configuration!");

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