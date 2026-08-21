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

        // GET: api/admin/products
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Description,
                    p.ImageUrl,
                    p.ScentFamily,
                    p.RecommendedOccasion,
                    p.Intensity,
                    p.CreatedAt,
                    CategoryName = p.Category.CategoryName,
                    Variants = p.Variants.Select(v => new { v.VariantId, v.Size, v.Price, v.StockQuantity })
                })
                .ToListAsync();

            return Ok(products);
        }

        // DELETE: api/admin/products/{id}
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound(new { message = "Product not found." });

            if (product.Variants.Any())
                _context.ProductVariants.RemoveRange(product.Variants);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully." });
        }

        // PUT: api/admin/products/{id}/stock
        [HttpPut("products/{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateDto request)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound(new { message = "Product not found." });

            foreach (var variant in product.Variants)
            {
                variant.StockQuantity = request.StockQuantity;
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Stock updated successfully." });
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.Address,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/admin/users/{id}/orders
        [HttpGet("users/{id}/orders")]
        public async Task<IActionResult> GetUserOrders(int id)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == id)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(v => v.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var result = orders.Select(o => new
            {
                o.OrderId,
                o.OrderStatus,
                o.TotalAmount,
                o.CreatedAt,
                o.ShippingAddress,
                Items = o.OrderItems.Select(oi => new
                {
                    ProductName = oi.ProductVariant?.Product?.Name ?? "Unknown",
                    Size = oi.ProductVariant?.Size ?? "N/A",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPriceAtSale,
                    Total = oi.UnitPriceAtSale * oi.Quantity
                })
            });

            return Ok(result);
        }
    }
}