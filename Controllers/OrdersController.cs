using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TuketAppAPI.Models;

namespace TuketAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly TuketDbContext _context;

        public OrdersController(TuketDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound("Sipariş bulunamadı.");
            return Ok(order);
        }
    }
}