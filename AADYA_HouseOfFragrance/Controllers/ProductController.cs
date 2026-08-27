#nullable disable

using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AADYA_HouseOfFragrance.Controllers
{
    /// <summary>
    /// Provides product catalog operations, including listing, filtering, searching,
    /// sorting, and recommendation generation.
    /// Most endpoints are public (no authentication required) except for personalized recommendations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== PRODUCT CATALOG =====================

        /// <summary>
        /// GET: api/products
        /// Retrieves all products with optional filtering, searching, and sorting.
        /// </summary>
        /// <param name="category">Filter by category name (e.g., "Attars & Oud")</param>
        /// <param name="scentFamily">Filter by scent family (e.g., "Woody", "Floral")</param>
        /// <param name="search">Search by product name or description</param>
        /// <param name="sort">Sort order: "popular", "low-high", "high-low"</param>
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string category = null,
            [FromQuery] string scentFamily = null,
            [FromQuery] string search = null,
            [FromQuery] string sort = "popular")
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .AsQueryable();

            // Apply category filter
            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                query = query.Where(p => p.Category.CategoryName.ToLower() == category.ToLower());
            }

            // Apply scent family filter
            if (!string.IsNullOrEmpty(scentFamily))
            {
                query = query.Where(p => p.ScentFamily.ToLower() == scentFamily.ToLower());
            }

            // Apply search filter (name or description)
            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchLower)));
            }

            // Apply sorting
            query = sort?.ToLower() switch
            {
                "low-high" => query.OrderBy(p => p.Variants.Min(v => v.Price)),
                "high-low" => query.OrderByDescending(p => p.Variants.Max(v => v.Price)),
                _ => query.OrderBy(p => p.ProductId) // Default: ID order
            };

            var products = await query.ToListAsync();

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

        /// <summary>
        /// GET: api/products/{id}
        /// Retrieves a single product by its ID with all related data.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound(new { message = "Product not found." });

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

        // ===================== RECOMMENDATIONS (Wizard) =====================

        /// <summary>
        /// POST: api/products/recommend
        /// Returns product recommendations based on user preferences
        /// selected in the Fragrance Wizard (scent family, occasion, intensity).
        /// </summary>
        [HttpPost("recommend")]
        public async Task<IActionResult> GetRecommendations([FromBody] WizardRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.ScentFamily))
                return BadRequest(new { message = "Invalid preferences. ScentFamily is required." });

            // Query products matching all selected preferences
            var query = _context.Products
                .Include(p => p.Variants)
                .Where(p =>
                    p.ScentFamily.ToLower() == request.ScentFamily.ToLower() &&
                    (string.IsNullOrEmpty(request.Occasion) ||
                     p.RecommendedOccasion.ToLower() == request.Occasion.ToLower()) &&
                    (string.IsNullOrEmpty(request.Intensity) ||
                     p.Intensity.ToLower() == request.Intensity.ToLower()))
                .Take(6);

            var products = await query.ToListAsync();

            // If no exact matches, fallback to scent family only
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

        // ===================== PERSONALIZED & TRENDING RECOMMENDATIONS =====================

        /// <summary>
        /// GET: api/products/recommend/me
        /// Returns personalized product recommendations based on the user's order history.
        /// If the user is not logged in or has no orders, returns trending products.
        /// </summary>
        [HttpGet("recommend/me")]
        public async Task<IActionResult> GetPersonalizedRecommendations()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                // Not logged in → return trending products
                if (userIdClaim == null)
                {
                    var trending = await GetTrendingProducts(6);
                    return Ok(new { triggerProductName = string.Empty, recommendations = trending });
                }

                var userId = int.Parse(userIdClaim.Value);

                // Get user's last 10 orders with their items
                var userOrders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                            .ThenInclude(v => v.Product)
                    .ToListAsync();

                // If no orders → return trending
                if (!userOrders.Any() || !userOrders.Any(o => o.OrderItems.Any()))
                {
                    var trending = await GetTrendingProducts(6);
                    return Ok(new { triggerProductName = string.Empty, recommendations = trending });
                }

                // Extract the most recent purchased product name
                var mostRecentOrder = userOrders.First();
                var firstItem = mostRecentOrder.OrderItems.FirstOrDefault();
                var triggerProductName = firstItem?.ProductVariant?.Product?.Name ?? string.Empty;

                // Collect purchased product IDs and their attributes
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

                // Find the most common attributes
                var topScent = scentFamilies
                    .GroupBy(s => s)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                var topOccasion = occasions
                    .GroupBy(s => s)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                var topIntensity = intensities
                    .GroupBy(s => s)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                // Build recommendation query (exclude already purchased products)
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

                // If no recommendations, fallback to trending
                if (!recommendations.Any())
                {
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
                // On any error, fallback to trending products
                var trending = await GetTrendingProducts(6);
                return Ok(new { triggerProductName = string.Empty, recommendations = trending });
            }
        }

        // ===================== DATA MINING: TRENDING & FREQUENTLY BOUGHT =====================

        /// <summary>
        /// Gets trending products based on order frequency (most ordered).
        /// If no orders exist in the system, returns the newest products.
        /// </summary>
        /// <param name="count">Number of products to return</param>
        private async Task<IEnumerable<ProductResponseDto>> GetTrendingProducts(int count)
        {
            var trendingProductIds = await _context.OrderItems
                .GroupBy(oi => oi.VariantId)
                .Select(g => new { VariantId = g.Key, Count = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.Count)
                .Take(count * 2) // Get extra to account for duplicate products
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

        /// <summary>
        /// GET: api/products/{variantId}/frequently-bought-with
        /// Implements Market Basket Mining – returns products that are frequently
        /// purchased together with the specified product variant.
        /// </summary>
        /// <param name="id">The variant ID to find related products for</param>
        [HttpGet("{id}/frequently-bought-with")]
        public async Task<IActionResult> GetFrequentlyBoughtWith(int id)
        {
            // Get all order IDs that contain this variant
            var orderIds = await _context.OrderItems
                .Where(oi => oi.VariantId == id)
                .Select(oi => oi.OrderId)
                .Distinct()
                .ToListAsync();

            if (!orderIds.Any())
                return Ok(new List<ProductResponseDto>());

            // Get other variants bought in the same orders (excluding the current one)
            var relatedVariantIds = await _context.OrderItems
                .Where(oi => orderIds.Contains(oi.OrderId) && oi.VariantId != id)
                .GroupBy(oi => oi.VariantId)
                .Select(g => new { VariantId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(4)
                .Select(x => x.VariantId)
                .ToListAsync();

            if (!relatedVariantIds.Any())
                return Ok(new List<ProductResponseDto>());

            // Get full product details with variants and category
            var products = await _context.Products
                .Include(p => p.Variants)
                .Include(p => p.Category)
                .Where(p => p.Variants.Any(v => relatedVariantIds.Contains(v.VariantId)))
                .ToListAsync();

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
    }
}