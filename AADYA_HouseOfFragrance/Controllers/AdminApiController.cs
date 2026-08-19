using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AADYA_HouseOfFragrance.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalClients = await _context.Users.CountAsync();

            return Ok(new { totalRevenue, totalProducts, totalOrders, totalClients });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(v => v.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var result = orders.Select(o => new {
                id = "#AAD-" + o.OrderId.ToString("D4"),
                customer = o.User?.FullName ?? "Unknown",
                items = string.Join(", ", o.OrderItems.Select(oi =>
                    $"{oi.ProductVariant?.Product?.Name} ({oi.ProductVariant?.Size})")),
                total = o.TotalAmount,
                status = o.OrderStatus
            });

            return Ok(result);
        }

        // PUT: api/admin/orders/{id}/status
        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] StatusUpdateDto request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found." });

            order.OrderStatus = request.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order status updated." });
        }

        // POST: api/admin/products
        [HttpPost("products")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto request)
        {
            // Find category by name
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToLower() == request.Category.ToLower());
            if (category == null)
                return BadRequest(new { message = "Category not found." });

            var product = new Product
            {
                CategoryId = category.CategoryId,
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl ?? "/images/products/default.jpg",
                ScentFamily = request.ScentFamily,
                RecommendedOccasion = "Evening", // default
                Intensity = "Bold",              // default
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Add variants
            var variants = new List<ProductVariant>
            {
                new ProductVariant { ProductId = product.ProductId, Size = "6ml", Price = request.Price6ml, StockQuantity = 50 },
                new ProductVariant { ProductId = product.ProductId, Size = "12ml", Price = request.Price12ml, StockQuantity = 30 }
            };
            _context.ProductVariants.AddRange(variants);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product added successfully.", productId = product.ProductId });
        }
    }
}