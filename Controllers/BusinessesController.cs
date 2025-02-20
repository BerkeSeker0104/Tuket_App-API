using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
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

        // 1. İşletme Oluşturma (POST /api/Businesses)
        [HttpPost]
        [Authorize(Roles = "admin")] // Sadece adminler işletme ekleyebilir
        public async Task<IActionResult> CreateBusiness([FromBody] Business business)
        {
            if (business == null || string.IsNullOrEmpty(business.Name))
            {
                return BadRequest("İşletme bilgileri eksik!");
            }

            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBusiness), new { id = business.Id }, business);
        }

        // 2. Tüm İşletmeleri Listeleme (GET /api/Businesses)
        [HttpGet]
        public async Task<IActionResult> GetBusinesses()
        {
            var businesses = await _context.Businesses.ToListAsync();
            return Ok(businesses);
        }

        // 3. Belirli Bir İşletmeyi Getirme (GET /api/Businesses/{id})
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusiness(int id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound("İşletme bulunamadı.");
            }
            return Ok(business);
        }

        // 4. İşletme Güncelleme (PUT /api/Businesses/{id})
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")] // Sadece adminler güncelleyebilir
        public async Task<IActionResult> UpdateBusiness(int id, [FromBody] Business updatedBusiness)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound("İşletme bulunamadı.");
            }

            business.Name = updatedBusiness.Name;
            business.Address = updatedBusiness.Address;
            business.Latitude = updatedBusiness.Latitude;
            business.Longitude = updatedBusiness.Longitude;
            await _context.SaveChangesAsync();

            return Ok("İşletme güncellendi.");
        }

        // 5. İşletme Silme (DELETE /api/Businesses/{id})
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // Sadece adminler silebilir
        public async Task<IActionResult> DeleteBusiness(int id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound("İşletme bulunamadı.");
            }

            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync();
            return Ok("İşletme başarıyla silindi.");
        }
    }
}
