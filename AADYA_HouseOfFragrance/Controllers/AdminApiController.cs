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
    /// <summary>
    /// Provides administrative API endpoints for managing orders, products, and users.
    /// All endpoints require the caller to have the "Admin" role.
    /// </summary>
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

        // ===================== DASHBOARD STATISTICS =====================

        /// <summary>
        /// GET: api/admin/stats
        /// Returns dashboard statistics including revenue, product count, order count, and client count.
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalClients = await _context.Users.CountAsync(u => !u.IsAdmin);

            return Ok(new { totalRevenue, totalProducts, totalOrders, totalClients });
        }

        // ===================== ORDER MANAGEMENT =====================

        /// <summary>
        /// GET: api/admin/orders
        /// Returns a list of all orders with customer details and line items.
        /// </summary>
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

            var result = orders.Select(o => new
            {
                id = $"#AAD-{o.OrderId:D4}",
                customer = o.User?.FullName ?? "Unknown",
                customerPhone = o.User?.PhoneNumber ?? string.Empty,
                customerEmail = o.User?.Email ?? string.Empty,
                items = string.Join(", ", o.OrderItems.Select(oi =>
                    $"{oi.ProductVariant?.Product?.Name} ({oi.ProductVariant?.Size})")),
                total = o.TotalAmount,
                status = o.OrderStatus
            });

            return Ok(result);
        }

        /// <summary>
        /// PUT: api/admin/orders/{id}/status
        /// Updates the status of a specific order and sends a notification email to the customer.
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="request">New status and optional reason</param>
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

            // Validate status transition
            var validStatuses = new[] { "Processing", "Dispatched", "Delivered", "Cancelled", "Pending Payment" };
            if (!validStatuses.Contains(request.Status))
                return BadRequest(new { message = "Invalid status." });

            order.OrderStatus = request.Status;
            await _context.SaveChangesAsync();

            // Send status update email (non-blocking)
            try
            {
                if (order.User != null && !string.IsNullOrEmpty(order.User.Email))
                {
                    await _emailService.SendOrderStatusUpdateAsync(
                        order.User.Email,
                        order.User.FullName ?? "Customer",
                        order,
                        request.Reason
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] Failed to send status update email: {ex.Message}");
            }

            return Ok(new { message = "Order status updated." });
        }

        // ===================== PRODUCT MANAGEMENT =====================

        /// <summary>
        /// GET: api/admin/products
        /// Returns all products with their variants and category information.
        /// </summary>
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

        /// <summary>
        /// POST: api/admin/products
        /// Adds a new product to the catalog with 6ml and 12ml variants.
        /// </summary>
        [HttpPost("products")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto request)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == request.Category.ToLower());

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
                CreatedAt = DateTime.UtcNow
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

        /// <summary>
        /// PUT: api/admin/products/{id}/stock
        /// Updates stock quantities for both 6ml and 12ml variants of a product.
        /// </summary>
        [HttpPut("products/{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateDto request)
        {
            if (request.Stock6ml < 0 || request.Stock12ml < 0)
                return BadRequest(new { message = "Stock cannot be negative." });

            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound(new { message = "Product not found." });

            foreach (var variant in product.Variants)
            {
                if (variant.Size == "6ml")
                    variant.StockQuantity = request.Stock6ml;
                else if (variant.Size == "12ml")
                    variant.StockQuantity = request.Stock12ml;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Stock updated successfully." });
        }

        /// <summary>
        /// DELETE: api/admin/products/{id}
        /// Deletes a product only if it has not been ordered.
        /// </summary>
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Prevent deletion if product has been ordered
            var hasOrders = await _context.OrderItems
                .AnyAsync(oi => oi.ProductVariant.ProductId == id);

            if (hasOrders)
                return BadRequest(new { message = "Cannot delete product that has been ordered." });

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

        // ===================== USER MANAGEMENT =====================

        /// <summary>
        /// GET: api/admin/users
        /// Returns all registered users (excluding admin role check is done on frontend).
        /// </summary>
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

        /// <summary>
        /// GET: api/admin/users/{id}/orders
        /// Returns all orders for a specific user.
        /// </summary>
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

        /// <summary>
        /// PUT: api/admin/users/{id}/promote
        /// Promotes a regular user to admin (Super Admin only).
        /// </summary>
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

        /// <summary>
        /// PUT: api/admin/users/{id}/demote
        /// Demotes an admin to regular user (Super Admin only).
        /// The Super Admin cannot demote themselves.
        /// </summary>
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

        // ===================== CUSTOMER SEGMENTS (Data Mining) =====================

        /// <summary>
        /// GET: api/admin/customer-segments
        /// Returns customer segmentation data based on spending and order frequency.
        /// Categories: High Spenders (>₹5000), Regular (₹1000-₹5000),
        /// Occasional (≤₹1000 with orders), Inactive (no orders).
        /// </summary>
        [HttpGet("customer-segments")]
        public async Task<IActionResult> GetCustomerSegments()
        {
            var users = await _context.Users
                .Where(u => !u.IsAdmin)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    TotalSpent = _context.Orders.Where(o => o.UserId == u.UserId).Sum(o => o.TotalAmount),
                    OrderCount = _context.Orders.Count(o => o.UserId == u.UserId)
                })
                .ToListAsync();

            var segments = new
            {
                HighSpenders = users.Count(u => u.TotalSpent > 5000),
                Regular = users.Count(u => u.TotalSpent <= 5000 && u.TotalSpent > 1000),
                Occasional = users.Count(u => u.TotalSpent <= 1000 && u.OrderCount >= 1),
                Inactive = users.Count(u => u.OrderCount == 0)
            };

            return Ok(segments);
        }

        // ===================== HELPERS =====================

        /// <summary>
        /// Gets the current user's ID from the JWT token claims.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim.Value);
        }
    }
}