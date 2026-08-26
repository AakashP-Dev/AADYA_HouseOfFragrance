#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using AADYA_HouseOfFragrance.Services;

namespace AADYA_HouseOfFragrance.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public OrderController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

                HttpContext.Session.SetInt32("UserId", userId);
                await HttpContext.Session.CommitAsync();

                if (user == null)
                    return Unauthorized(new { message = "User not found." });

                var shippingAddress = string.IsNullOrEmpty(request.ShippingAddress) ? user.Address : request.ShippingAddress;
                if (string.IsNullOrWhiteSpace(shippingAddress))
                    return BadRequest(new { message = "Shipping address is required." });

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
                    OrderStatus = "Pending Payment",
                    ShippingAddress = shippingAddress,
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.Now,
                    PaymentMethod = null,
                    PaymentId = null,
                    OrderItems = orderItems
                };

                await _context.Orders.AddAsync(order);
                _context.ShoppingCarts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                // Don't send email here - will be sent after payment

                return Ok(new
                {
                    message = "Order placed! Please complete payment.",
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


        // POST: api/orders/process-payment
        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentDto request)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Ensure session has user ID
                HttpContext.Session.SetInt32("UserId", userId);
                await HttpContext.Session.CommitAsync();

                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                    .FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.UserId == userId);

                if (order == null)
                    return NotFound(new { message = "Order not found." });

                if (order.OrderStatus != "Pending Payment")
                    return BadRequest(new { message = "Payment already processed for this order." });

                // ---------- COD: Keep status as Pending Payment ----------
                if (request.PaymentMethod == "COD")
                {
                    order.PaymentMethod = "COD";
                    order.PaymentId = "COD-" + DateTime.Now.Ticks.ToString();
                    await _context.SaveChangesAsync();

                    // Reload order with full navigation properties for email
                    var savedOrder = await _context.Orders
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.ProductVariant)
                                .ThenInclude(v => v.Product)   // ✅ Important: Load Product
                        .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

                    // Send confirmation email
                    try
                    {
                        if (savedOrder != null && order.User != null)
                        {
                            await _emailService.SendOrderConfirmationAsync(
                                order.User.Email,
                                order.User.FullName,
                                savedOrder,
                                savedOrder.OrderItems.ToList()
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Email error: {ex.Message}");
                    }

                    return Ok(new
                    {
                        message = "Order placed successfully! Payment will be collected on delivery.",
                        orderId = order.OrderId,
                        paymentId = order.PaymentId,
                        orderStatus = "Pending Payment"
                    });
                }

                // ---------- Card / UPI: Process payment ----------
                var paymentSuccess = true; // Always succeeds in dummy mode

                if (paymentSuccess)
                {
                    order.OrderStatus = "Processing";
                    order.PaymentMethod = request.PaymentMethod;
                    order.PaymentId = "PAY-DUMMY-" + DateTime.Now.Ticks.ToString();
                    await _context.SaveChangesAsync();

                    // Reload order with full navigation properties for email
                    var savedOrder = await _context.Orders
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.ProductVariant)
                                .ThenInclude(v => v.Product)   // ✅ Important: Load Product
                        .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

                    // Send confirmation email
                    try
                    {
                        if (savedOrder != null && order.User != null)
                        {
                            await _emailService.SendOrderConfirmationAsync(
                                order.User.Email,
                                order.User.FullName,
                                savedOrder,
                                savedOrder.OrderItems.ToList()
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Email error: {ex.Message}");
                    }

                    return Ok(new
                    {
                        message = "Payment successful! Order confirmed.",
                        orderId = order.OrderId,
                        paymentId = order.PaymentId,
                        orderStatus = order.OrderStatus
                    });
                }
                else
                {
                    return BadRequest(new { message = "Payment failed. Please try again." });
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to process payment." });
            }
        }

        // POST: api/orders/cancel/{id}
        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

                if (order == null)
                    return NotFound(new { message = "Order not found." });

                // Check if order can be cancelled
                if (order.OrderStatus == "Dispatched" || order.OrderStatus == "Delivered")
                {
                    return BadRequest(new { message = "Order cannot be cancelled as it has already been shipped or delivered." });
                }

                if (order.OrderStatus == "Cancelled")
                {
                    return BadRequest(new { message = "Order is already cancelled." });
                }

                // Restore stock
                foreach (var item in order.OrderItems)
                {
                    if (item.ProductVariant != null)
                    {
                        item.ProductVariant.StockQuantity += item.Quantity;
                    }
                }

                order.OrderStatus = "Cancelled";
                await _context.SaveChangesAsync();

                // Send cancellation confirmation email
                try
                {
                    if (order.User != null)
                    {
                        await _emailService.SendOrderStatusUpdateAsync(
                            order.User.Email,
                            order.User.FullName,
                            order,
                            "Cancelled by customer"
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email error: {ex.Message}");
                }

                return Ok(new { message = "Order cancelled successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to cancel orders." });
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