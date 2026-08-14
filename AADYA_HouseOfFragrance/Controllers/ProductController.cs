using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;

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
    }
}