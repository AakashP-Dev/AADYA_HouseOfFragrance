using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AADYA_HouseOfFragrance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        // Optional query params: category, scentFamily, search, sort
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? category = null,
            [FromQuery] string? scentFamily = null,
            [FromQuery] string? search = null,
            [FromQuery] string sort = "popular")
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .AsQueryable();

            // Filter by category (optional)
            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                query = query.Where(p => p.Category.CategoryName.ToLower() == category.ToLower());
            }

            // Filter by scent family (optional)
            if (!string.IsNullOrEmpty(scentFamily))
            {
                query = query.Where(p => p.ScentFamily.ToLower() == scentFamily.ToLower());
            }

            // Search by name or description (optional)
            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower) ||
                                         (p.Description != null && p.Description.ToLower().Contains(searchLower)));
            }

            // Sorting
            query = sort?.ToLower() switch
            {
                "low-high" => query.OrderBy(p => p.Variants.Min(v => v.Price)),
                "high-low" => query.OrderByDescending(p => p.Variants.Max(v => v.Price)),
                _ => query.OrderBy(p => p.ProductId) // Default: popular (we'll add popularity tracking later)
            };

            var products = await query.ToListAsync();

            // Map to DTO (avoid circular references)
            var result = products.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                ScentFamily = p.ScentFamily,
                RecommendedOccasion = p.RecommendedOccasion,
                Intensity = p.Intensity,
                CategoryName = p.Category?.CategoryName ?? "Uncategorized",
                Variants = p.Variants?.Select(v => new VariantDto
                {
                    VariantId = v.VariantId,
                    Size = v.Size,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity
                }).ToList() ?? new List<VariantDto>()
            });

            return Ok(result);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            var result = new ProductResponseDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                ScentFamily = product.ScentFamily,
                RecommendedOccasion = product.RecommendedOccasion,
                Intensity = product.Intensity,
                CategoryName = product.Category?.CategoryName ?? "Uncategorized",
                Variants = product.Variants?.Select(v => new VariantDto
                {
                    VariantId = v.VariantId,
                    Size = v.Size,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity
                }).ToList() ?? new List<VariantDto>()
            };

            return Ok(result);
        }

        // POST: api/products/recommend
        [HttpPost("recommend")]
        public async Task<IActionResult> GetRecommendations([FromBody] WizardRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.ScentFamily))
                return BadRequest(new { message = "Invalid preferences. ScentFamily is required." });

            // Query products that match the user's preferences
            var query = _context.Products
                .Include(p => p.Variants)
                .Where(p =>
                    p.ScentFamily.ToLower() == request.ScentFamily.ToLower() &&
                    (string.IsNullOrEmpty(request.Occasion) ||
                     p.RecommendedOccasion.ToLower() == request.Occasion.ToLower()) &&
                    (string.IsNullOrEmpty(request.Intensity) ||
                     p.Intensity.ToLower() == request.Intensity.ToLower())
                )
                .Take(6); // Limit to 6 recommendations

            var products = await query.ToListAsync();

            // If no exact match, fallback to just matching ScentFamily
            if (!products.Any())
            {
                query = _context.Products
                    .Include(p => p.Variants)
                    .Where(p => p.ScentFamily.ToLower() == request.ScentFamily.ToLower())
                    .Take(4);
                products = await query.ToListAsync();
            }

            // Map to DTO
            var result = products.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                ScentFamily = p.ScentFamily,
                RecommendedOccasion = p.RecommendedOccasion,
                Intensity = p.Intensity,
                CategoryName = p.Category?.CategoryName ?? "Uncategorized",
                Variants = p.Variants?.Select(v => new VariantDto
                {
                    VariantId = v.VariantId,
                    Size = v.Size,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity
                }).ToList() ?? new List<VariantDto>()
            });

            return Ok(result);
        }

        // GET: api/products/recommend/me
        [HttpGet("recommend/me")]
        public async Task<IActionResult> GetPersonalizedRecommendations()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    // Not logged in → return trending only
                    var trending = await GetTrendingProducts(6);
                    return Ok(new { triggerProductName = "", recommendations = trending });
                }

                var userId = int.Parse(userIdClaim.Value);

                // 1. Get user's order history (last 10 orders)
                var userOrders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                            .ThenInclude(v => v.Product)
                    .ToListAsync();

                // If user has no orders → fallback to trending
                if (!userOrders.Any() || !userOrders.Any(o => o.OrderItems.Any()))
                {
                    var trending = await GetTrendingProducts(6);
                    return Ok(new { triggerProductName = "", recommendations = trending });
                }

                // 2. Extract the most recent purchased product name (first item of first order)
                var mostRecentOrder = userOrders.First();
                var firstItem = mostRecentOrder.OrderItems.FirstOrDefault();
                string triggerProductName = firstItem?.ProductVariant?.Product?.Name ?? "";

                // 3. Extract ScentFamily, Occasion, Intensity and bought Product IDs
                var purchasedProductIds = new HashSet<int>();
                var scentFamilies = new List<string>();
                var occasions = new List<string>();
                var intensities = new List<string>();

                foreach (var order in userOrders)
                {
                    foreach (var item in order.OrderItems)
                    {
                        var product = item.ProductVariant?.Product;
                        if (product != null)
                        {
                            purchasedProductIds.Add(product.ProductId);
                            if (!string.IsNullOrEmpty(product.ScentFamily))
                                scentFamilies.Add(product.ScentFamily);
                            if (!string.IsNullOrEmpty(product.RecommendedOccasion))
                                occasions.Add(product.RecommendedOccasion);
                            if (!string.IsNullOrEmpty(product.Intensity))
                                intensities.Add(product.Intensity);
                        }
                    }
                }

                // 4. Find the most common scent family (or fallback)
                var topScent = scentFamilies.GroupBy(s => s)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                var topOccasion = occasions.GroupBy(s => s)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                var topIntensity = intensities.GroupBy(s => s)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                // 5. Build recommendation query
                var query = _context.Products
                    .Include(p => p.Variants)
                    .Where(p => !purchasedProductIds.Contains(p.ProductId));

                if (!string.IsNullOrEmpty(topScent))
                    query = query.Where(p => p.ScentFamily == topScent);
                else if (!string.IsNullOrEmpty(topOccasion))
                    query = query.Where(p => p.RecommendedOccasion == topOccasion);
                else if (!string.IsNullOrEmpty(topIntensity))
                    query = query.Where(p => p.Intensity == topIntensity);

                var recommendations = await query.Take(6).ToListAsync();

                if (!recommendations.Any())
                {
                    // Fallback to trending
                    var trending = await GetTrendingProducts(6);
                    return Ok(new { triggerProductName, recommendations = trending });
                }

                // Map to DTO
                var result = recommendations.Select(p => new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    ScentFamily = p.ScentFamily,
                    RecommendedOccasion = p.RecommendedOccasion,
                    Intensity = p.Intensity,
                    CategoryName = p.Category?.CategoryName ?? "Uncategorized",
                    Variants = p.Variants?.Select(v => new VariantDto
                    {
                        VariantId = v.VariantId,
                        Size = v.Size,
                        Price = v.Price,
                        StockQuantity = v.StockQuantity
                    }).ToList() ?? new List<VariantDto>()
                });

                return Ok(new { triggerProductName, recommendations = result });
            }
            catch (Exception)
            {
                // Fallback to trending on any error
                var trending = await GetTrendingProducts(6);
                return Ok(new { triggerProductName = "", recommendations = trending });
            }
        }

        // Helper: Get trending products (most ordered)
        private async Task<IEnumerable<ProductResponseDto>> GetTrendingProducts(int count)
        {
            var trendingProductIds = await _context.OrderItems
                .GroupBy(oi => oi.VariantId)
                .Select(g => new { VariantId = g.Key, Count = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.Count)
                .Take(count * 2) // Get more to account for duplicate products
                .Join(_context.ProductVariants,
                    g => g.VariantId,
                    v => v.VariantId,
                    (g, v) => v.ProductId)
                .Distinct()
                .Take(count)
                .ToListAsync();

            var products = await _context.Products
                .Include(p => p.Variants)
                .Include(p => p.Category)
                .Where(p => trendingProductIds.Contains(p.ProductId))
                .ToListAsync();

            // If no trending products (fresh DB), fallback to newest products
            if (!products.Any())
            {
                products = await _context.Products
                    .Include(p => p.Variants)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(count)
                    .ToListAsync();
            }

            return products.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                ScentFamily = p.ScentFamily,
                RecommendedOccasion = p.RecommendedOccasion,
                Intensity = p.Intensity,
                CategoryName = p.Category?.CategoryName ?? "Uncategorized",
                Variants = p.Variants?.Select(v => new VariantDto
                {
                    VariantId = v.VariantId,
                    Size = v.Size,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity
                }).ToList() ?? new List<VariantDto>()
            });
        }
    }
}