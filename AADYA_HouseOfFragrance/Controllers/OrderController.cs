using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;

namespace AADYA_HouseOfFragrance.Controllers
{
    [Route("api/orders")]   // ← Changed from "api/[controller]" to "api/orders"
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim.Value);
        }

        // POST: api/orders/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return Unauthorized(new { message = "User not found." });

                var cartItems = await _context.ShoppingCarts
                    .Include(c => c.ProductVariant)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                    return BadRequest(new { message = "Your cart is empty." });

                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                foreach (var cartItem in cartItems)
                {
                    var variant = cartItem.ProductVariant;
                    if (variant == null)
                        return BadRequest(new { message = "Invalid product variant in cart." });

                    if (variant.StockQuantity < cartItem.Quantity)
                        return BadRequest(new { message = $"Not enough stock for {variant.Product?.Name} ({variant.Size}). Available: {variant.StockQuantity}" });

                    variant.StockQuantity -= cartItem.Quantity;
                    _context.ProductVariants.Update(variant);

                    var subtotal = variant.Price * cartItem.Quantity;
                    totalAmount += subtotal;

                    orderItems.Add(new OrderItem
                    {
                        VariantId = variant.VariantId,
                        Quantity = cartItem.Quantity,
                        UnitPriceAtSale = variant.Price
                    });
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderStatus = "Processing",
                    ShippingAddress = string.IsNullOrEmpty(request.ShippingAddress) ? user.Address : request.ShippingAddress,
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.Now,
                    OrderItems = orderItems
                };

                await _context.Orders.AddAsync(order);
                _context.ShoppingCarts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Order placed successfully!",
                    orderId = order.OrderId,
                    totalAmount = order.TotalAmount,
                    orderStatus = order.OrderStatus,
                    shippingAddress = order.ShippingAddress
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to place an order." });
            }
        }

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var userId = GetCurrentUserId();

                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                            .ThenInclude(v => v.Product)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                var result = orders.Select(order => new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    OrderStatus = order.OrderStatus,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    ShippingAddress = order.ShippingAddress,
                    Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductName = oi.ProductVariant?.Product?.Name ?? "Unknown Product",
                        Size = oi.ProductVariant?.Size ?? "N/A",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPriceAtSale,
                        Total = oi.UnitPriceAtSale * oi.Quantity
                    }).ToList()
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to view your orders." });
            }
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                            .ThenInclude(v => v.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

                if (order == null)
                    return NotFound(new { message = "Order not found." });

                var result = new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    OrderStatus = order.OrderStatus,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    ShippingAddress = order.ShippingAddress,
                    Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductName = oi.ProductVariant?.Product?.Name ?? "Unknown Product",
                        Size = oi.ProductVariant?.Size ?? "N/A",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPriceAtSale,
                        Total = oi.UnitPriceAtSale * oi.Quantity
                    }).ToList()
                };

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to view your orders." });
            }
        }
    }
}