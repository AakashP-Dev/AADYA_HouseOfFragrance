#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;

namespace AADYA_HouseOfFragrance.Controllers
{
    /// <summary>
    /// Provides shopping cart API endpoints for managing cart items.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
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

        // ===================== CART OPERATIONS =====================

        /// <summary>
        /// GET: api/cart
        /// Retrieves the current user's cart items with product details and totals.
        /// </summary>
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
                    ProductImage = item.ProductVariant?.Product?.ImageUrl ?? string.Empty,
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

        /// <summary>
        /// POST: api/cart/add
        /// Adds a product variant to the user's cart. If the item already exists,
        /// the quantity is increased. Validates stock availability before adding.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Validate variant exists
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.VariantId == request.VariantId);

                if (variant == null)
                    return BadRequest(new { message = "Product variant not found." });

                // Validate stock availability
                if (variant.StockQuantity < request.Quantity)
                    return BadRequest(new { message = $"Only {variant.StockQuantity} items available in stock." });

                // Check if item already exists in cart
                var existingItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.VariantId == request.VariantId);

                if (existingItem != null)
                {
                    // Update quantity of existing item
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
                        AddedAt = DateTime.UtcNow
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

        /// <summary>
        /// PUT: api/cart/update
        /// Updates the quantity of a specific cart item. If quantity is 0 or less,
        /// the item is removed from the cart. Validates stock availability.
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartDto request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.CartItemId == request.CartItemId && c.UserId == userId);

                if (cartItem == null)
                    return NotFound(new { message = "Cart item not found." });

                // If quantity is 0 or negative, remove the item
                if (request.Quantity <= 0)
                {
                    _context.ShoppingCarts.Remove(cartItem);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Item removed from cart." });
                }

                // Validate stock availability
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.VariantId == cartItem.VariantId);

                if (variant != null && variant.StockQuantity < request.Quantity)
                    return BadRequest(new { message = $"Only {variant.StockQuantity} items available in stock." });

                // Update quantity
                cartItem.Quantity = request.Quantity;
                _context.ShoppingCarts.Update(cartItem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cart updated successfully!" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to update your cart." });
            }
        }

        /// <summary>
        /// DELETE: api/cart/remove/{id}
        /// Removes a specific item from the user's cart.
        /// </summary>
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var cartItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.CartItemId == id && c.UserId == userId);

                if (cartItem == null)
                    return NotFound(new { message = "Cart item not found." });

                _context.ShoppingCarts.Remove(cartItem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Item removed from cart." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to manage your cart." });
            }
        }

        /// <summary>
        /// DELETE: api/cart/clear
        /// Removes all items from the user's cart.
        /// </summary>
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