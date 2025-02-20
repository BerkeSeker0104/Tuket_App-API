using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
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

        //  Tüm Ürünleri Listele
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.Include(p => p.Business).ToListAsync();
            return Ok(products);
        }

        //  Belirli Bir İşletmenin Ürünlerini Listele
        [HttpGet("business/{businessId}")]
        public async Task<IActionResult> GetProductsByBusiness(int businessId)
        {
            var products = await _context.Products.Where(p => p.BusinessId == businessId).ToListAsync();
            if (products.Count == 0)
                return NotFound("Bu işletmeye ait ürün bulunamadı.");
            return Ok(products);
        }

        //  Yeni Ürün Ekle (Sadece admin veya işletme sahibi ekleyebilir)
        [HttpPost]
        [Authorize(Roles = "admin,business_owner")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (!_context.Businesses.Any(b => b.Id == product.BusinessId))
                return BadRequest("İlgili işletme bulunamadı.");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllProducts), new { id = product.Id }, product);
        }

        //  Ürün Güncelleme
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,business_owner")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Ürün bulunamadı.");

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.IsAvailable = updatedProduct.IsAvailable;

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        //  Ürün Silme
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,business_owner")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Ürün bulunamadı.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}