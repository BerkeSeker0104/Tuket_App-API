using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.Security.Claims;
using TuketAppAPI.Models;

namespace TuketAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly TuketDbContext _context;

        public ProductsController(TuketDbContext context)
        {
            _context = context;
        }

        // Tüm Ürünleri Listeleme
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.Include(p => p.Business).ToListAsync();
            Log.Information("Tüm ürünler listelendi.");
            return Ok(products);
        }

        // Belirli Bir İşletmenin Ürünlerini Listeleme
        [HttpGet("business/{businessId}")]
        public async Task<IActionResult> GetProductsByBusiness(int businessId)
        {
            var products = await _context.Products.Where(p => p.BusinessId == businessId).ToListAsync();
            if (products.Count == 0)
            {
                Log.Warning("Bu işletmeye ait ürün bulunamadı. Business ID: {BusinessId}", businessId);
                return NotFound("Bu işletmeye ait ürün bulunamadı.");
            }

            Log.Information("{BusinessId} ID'li işletmenin ürünleri listelendi.", businessId);
            return Ok(products);
        }

        // Yeni Ürün Ekleme (Sadece admin veya işletme sahibi ekleyebilir)
        [HttpPost]
        [Authorize(Roles = "admin,business_owner")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (!_context.Businesses.Any(b => b.Id == product.BusinessId))
            {
                Log.Warning("Ürün eklenirken geçersiz işletme ID kullanıldı. Business ID: {BusinessId}", product.BusinessId);
                return BadRequest("İlgili işletme bulunamadı.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            Log.Information("Yeni ürün eklendi: {@Product}", product);
            return CreatedAtAction(nameof(GetAllProducts), new { id = product.Id }, product);
        }

        // Ürün Güncelleme
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,business_owner")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                Log.Warning("Güncellenecek ürün bulunamadı. ID: {ProductId}", id);
                return NotFound("Ürün bulunamadı.");
            }

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.IsAvailable = updatedProduct.IsAvailable;
            await _context.SaveChangesAsync();

            Log.Information("Ürün güncellendi. ID: {ProductId}", id);
            return Ok(product);
        }

        // Ürün Silme
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,business_owner")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                Log.Warning("Silinecek ürün bulunamadı. ID: {ProductId}", id);
                return NotFound("Ürün bulunamadı.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            Log.Information("Ürün silindi. ID: {ProductId}", id);
            return NoContent();
        }
    }
}