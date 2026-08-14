using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;

namespace AADYA_HouseOfFragrance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper method to get current user ID from JWT
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim.Value);
        }

        // GET: api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItems = await _context.ShoppingCarts
                    .Include(c => c.ProductVariant)
                        .ThenInclude(v => v.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var result = cartItems.Select(item => new CartItemResponseDto
                {
                    CartItemId = item.CartItemId,
                    VariantId = item.VariantId,
                    ProductName = item.ProductVariant?.Product?.Name ?? "Unknown Product",
                    ProductImage = item.ProductVariant?.Product?.ImageUrl ?? "",
                    Size = item.ProductVariant?.Size ?? "N/A",
                    Price = item.ProductVariant?.Price ?? 0,
                    Quantity = item.Quantity,
                    Total = (item.ProductVariant?.Price ?? 0) * item.Quantity
                });

                var totalAmount = result.Sum(r => r.Total);

                return Ok(new { items = result, totalAmount });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to view your cart." });
            }
        }

        // POST: api/cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Validate variant exists and has stock
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.VariantId == request.VariantId);

                if (variant == null)
                {
                    return BadRequest(new { message = "Product variant not found." });
                }

                if (variant.StockQuantity < request.Quantity)
                {
                    return BadRequest(new { message = $"Only {variant.StockQuantity} items available in stock." });
                }

                // Check if item already exists in cart
                var existingItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.VariantId == request.VariantId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity += request.Quantity;
                    _context.ShoppingCarts.Update(existingItem);
                }
                else
                {
                    // Add new item
                    var cartItem = new ShoppingCart
                    {
                        UserId = userId,
                        VariantId = request.VariantId,
                        Quantity = request.Quantity,
                        AddedAt = DateTime.Now
                    };
                    await _context.ShoppingCarts.AddAsync(cartItem);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Item added to cart successfully!" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to add items to cart." });
            }
        }

        // PUT: api/cart/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartDto request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.CartItemId == request.CartItemId && c.UserId == userId);

                if (cartItem == null)
                {
                    return NotFound(new { message = "Cart item not found." });
                }

                if (request.Quantity <= 0)
                {
                    // Remove item if quantity is 0 or negative
                    _context.ShoppingCarts.Remove(cartItem);
                }
                else
                {
                    // Validate stock
                    var variant = await _context.ProductVariants
                        .FirstOrDefaultAsync(v => v.VariantId == cartItem.VariantId);

                    if (variant != null && variant.StockQuantity < request.Quantity)
                    {
                        return BadRequest(new { message = $"Only {variant.StockQuantity} items available in stock." });
                    }

                    cartItem.Quantity = request.Quantity;
                    _context.ShoppingCarts.Update(cartItem);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cart updated successfully!" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to update your cart." });
            }
        }

        // DELETE: api/cart/remove/{id}
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.CartItemId == id && c.UserId == userId);

                if (cartItem == null)
                {
                    return NotFound(new { message = "Cart item not found." });
                }

                _context.ShoppingCarts.Remove(cartItem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Item removed from cart." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to manage your cart." });
            }
        }

        // DELETE: api/cart/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItems = await _context.ShoppingCarts
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (cartItems.Any())
                {
                    _context.ShoppingCarts.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Cart cleared successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to manage your cart." });
            }
        }
    }
}