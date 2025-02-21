using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TuketAppAPI.Models;

namespace TuketAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly TuketDbContext _context;

        public ReviewsController(TuketDbContext context)
        {
            _context = context;
        }

        [HttpPost("business")]
        [Authorize]
        public async Task<IActionResult> AddBusinessReview([FromBody] BusinessReview review)
        {
            _context.BusinessReviews.Add(review);
            await _context.SaveChangesAsync();
            return Ok("İşletme yorumu başarıyla eklendi.");
        }

        [HttpGet("business/{businessId}")]
        public async Task<IActionResult> GetBusinessReviews(int businessId)
        {
            var reviews = await _context.BusinessReviews.Where(r => r.BusinessId == businessId).ToListAsync();
            return Ok(reviews);
        }

        [HttpPost("product")]
        [Authorize]
        public async Task<IActionResult> AddProductReview([FromBody] ProductReview review)
        {
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
            return Ok("Ürün yorumu başarıyla eklendi.");
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _context.ProductReviews.Where(r => r.ProductId == productId).ToListAsync();
            return Ok(reviews);
        }
    }
}