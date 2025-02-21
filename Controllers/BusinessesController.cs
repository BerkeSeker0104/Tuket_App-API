using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using TuketAppAPI.Models;

namespace TuketAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessesController : ControllerBase
    {
        private readonly TuketDbContext _context;

        public BusinessesController(TuketDbContext context)
        {
            _context = context;
        }

        // İşletme Ekleme (Sadece admin)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateBusiness([FromBody] Business business)
        {
            if (business == null || string.IsNullOrEmpty(business.Name))
            {
                Log.Warning("Eksik işletme bilgisi ile kayıt yapılmaya çalışıldı.");
                return BadRequest("İşletme bilgileri eksik!");
            }

            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();
            Log.Information("Yeni işletme oluşturuldu: {@Business}", business);
            return CreatedAtAction(nameof(GetBusiness), new { id = business.Id }, business);
        }

        // Tüm İşletmeleri Listeleme
        [HttpGet]
        public async Task<IActionResult> GetBusinesses()
        {
            var businesses = await _context.Businesses.ToListAsync();
            Log.Information("Tüm işletmeler listelendi.");
            return Ok(businesses);
        }

        // Belirli Bir İşletmeyi Getirme
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusiness(int id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                Log.Warning("İşletme bulunamadı. ID: {BusinessId}", id);
                return NotFound("İşletme bulunamadı.");
            }

            Log.Information("İşletme getirildi. ID: {BusinessId}", id);
            return Ok(business);
        }

        // İşletme Güncelleme (Sadece admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBusiness(int id, [FromBody] Business updatedBusiness)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                Log.Warning("Güncellenecek işletme bulunamadı. ID: {BusinessId}", id);
                return NotFound("İşletme bulunamadı.");
            }

            business.Name = updatedBusiness.Name;
            business.Address = updatedBusiness.Address;
            business.Latitude = updatedBusiness.Latitude;
            business.Longitude = updatedBusiness.Longitude;
            await _context.SaveChangesAsync();

            Log.Information("İşletme güncellendi. ID: {BusinessId}", id);
            return Ok("İşletme güncellendi.");
        }

        // İşletme Silme (Sadece admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBusiness(int id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                Log.Warning("Silinecek işletme bulunamadı. ID: {BusinessId}", id);
                return NotFound("İşletme bulunamadı.");
            }

            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync();
            Log.Information("İşletme silindi. ID: {BusinessId}", id);
            return Ok("İşletme başarıyla silindi.");
        }
    }
}