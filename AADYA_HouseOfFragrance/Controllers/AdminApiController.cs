#nullable disable

using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using AADYA_HouseOfFragrance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AADYA_HouseOfFragrance.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AdminApiController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalClients = await _context.Users.CountAsync(u => u.IsAdmin == false);
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

        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] StatusUpdateDto request)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound(new { message = "Order not found." });

            order.OrderStatus = request.Status;
            await _context.SaveChangesAsync();

            // --- SEND STATUS UPDATE EMAIL ---
            try
            {
                if (order.User != null && !string.IsNullOrEmpty(order.User.Email))
                {
                    await _emailService.SendOrderStatusUpdateAsync(
                        order.User.Email,
                        order.User.FullName ?? "Customer",
                        order,
                        request.Reason // Pass the reason (for cancellation etc.)
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send status update email: {ex.Message}");
            }

            return Ok(new { message = "Order status updated." });
        }

        [HttpPost("products")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto request)
        {
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
                RecommendedOccasion = "Evening",
                Intensity = "Bold",
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var variants = new List<ProductVariant>
            {
                new ProductVariant { ProductId = product.ProductId, Size = "6ml", Price = request.Price6ml, StockQuantity = 50 },
                new ProductVariant { ProductId = product.ProductId, Size = "12ml", Price = request.Price12ml, StockQuantity = 30 }
            };
            _context.ProductVariants.AddRange(variants);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product added successfully.", productId = product.ProductId });
        }

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
                    u.IsAdmin,
                    u.IsSuperAdmin,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

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

        // --- PROMOTE USER ---
        [HttpPut("users/{id}/promote")]
        public async Task<IActionResult> PromoteUser(int id)
        {
            var currentUserId = GetCurrentUserId();
            var currentUser = await _context.Users.FindAsync(currentUserId);
            if (currentUser == null || !currentUser.IsSuperAdmin)
                return Unauthorized(new { message = "Only the super admin can promote users." });

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            if (user.IsAdmin)
                return BadRequest(new { message = "User is already an admin." });

            user.IsAdmin = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User '{user.FullName}' promoted to admin successfully." });
        }

        // --- DEMOTE USER ---
        [HttpPut("users/{id}/demote")]
        public async Task<IActionResult> DemoteUser(int id)
        {
            var currentUserId = GetCurrentUserId();
            var currentUser = await _context.Users.FindAsync(currentUserId);
            if (currentUser == null || !currentUser.IsSuperAdmin)
                return Unauthorized(new { message = "Only the super admin can demote users." });

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            if (!user.IsAdmin)
                return BadRequest(new { message = "User is not an admin." });

            if (id == currentUserId)
                return BadRequest(new { message = "You cannot demote yourself." });

            user.IsAdmin = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User '{user.FullName}' has been demoted to regular user." });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim.Value);
        }
    }
}