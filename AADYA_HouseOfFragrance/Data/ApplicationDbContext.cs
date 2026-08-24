using Microsoft.EntityFrameworkCore;
using AADYA_HouseOfFragrance.Models.Entities;

namespace AADYA_HouseOfFragrance.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique constraint for Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ============================================
            // SEED CATEGORIES
            // ============================================
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Signature Blends" },
                new Category { CategoryId = 2, CategoryName = "Attars & Oud" },
                new Category { CategoryId = 3, CategoryName = "Contemporary Fragrances" }
            );

            // ============================================
            // SEED PRODUCTS (35 Products)
            // ============================================
            modelBuilder.Entity<Product>().HasData(
                // Attars & Oud (CategoryId = 2)
                new Product { ProductId = 1, CategoryId = 2, Name = "Oud Al Noor", Description = "Rich oud accords beautifully balanced with warm, refined notes to create a fragrance that is deep, sophisticated, and effortlessly luxurious.", ImageUrl = "/images/products/OUD Al Noor.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 2, CategoryId = 2, Name = "Royal Oud", Description = "Crafted for those who appreciate timeless luxury, Royal Oud is a rich and refined fragrance that showcases the majestic character of oud with a smooth, elegant warmth.", ImageUrl = "/images/products/Royal OUD.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 3, CategoryId = 2, Name = "Oud Habib", Description = "Experience the perfect balance of depth and elegance with Oud Habib. Rich oud is complemented by warm, refined accords to create a fragrance that feels smooth, luxurious, and irresistibly captivating.", ImageUrl = "/images/products/OUD Habib.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 13, CategoryId = 2, Name = "Black Onyx", Description = "Dark, composed, and quietly intriguing—Black Onyx is made for a presence that doesn't need to compete for attention.", ImageUrl = "/images/products/Black Onyx.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 30, CategoryId = 2, Name = "White Oud", Description = "Oud, reimagined with a cleaner and more refined presence. White Oud carries the distinctive woody character of oud in an elegant profile that feels sophisticated, composed, and easier to wear than darker oud styles.", ImageUrl = "/images/products/White OUD.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 31, CategoryId = 2, Name = "Musk", Description = "Bold, intense, and undeniably captivating, Musk is crafted for those who appreciate the rich depth of traditional musk.", ImageUrl = "/images/products/Musk.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 33, CategoryId = 2, Name = "Shahi Darbar", Description = "A fragrance with the poise of a royal gathering—graceful, distinguished, and steeped in tradition.", ImageUrl = "/images/products/Shahi Darbar.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 34, CategoryId = 2, Name = "Black Oud", Description = "Darkness, refined into a fragrance. Black Oud carries the unmistakable depth of oud with a darker, more mysterious character—rich, woody, and composed for those who prefer their scent with genuine presence.", ImageUrl = "/images/products/Black OUD.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 35, CategoryId = 2, Name = "Oud", Description = "Deep, woody, and unmistakably commanding. Oud captures the timeless character of one of perfumery's most prized aromas—rich, smoky warmth with an earthy depth that develops beautifully on the skin.", ImageUrl = "/images/products/OUD.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },

                // Signature Blends (CategoryId = 1)
                new Product { ProductId = 4, CategoryId = 1, Name = "Ridhaan", Description = "Refined, confident, and effortlessly elegant, Ridhaan is a fragrance crafted for those who appreciate understated luxury.", ImageUrl = "/images/products/Ridhaan.jpeg", ScentFamily = "Fresh", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 7, CategoryId = 1, Name = "Lovely", Description = "Soft sweetness has a way of being remembered. Lovely wraps you in a delicate, graceful character that feels warm, charming, and beautifully feminine without becoming overwhelming.", ImageUrl = "/images/products/Lovely.jpeg", ScentFamily = "Floral", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 8, CategoryId = 1, Name = "Jack Hobbs", Description = "Sweetness, with a sharper sense of character. Jack Hobbs carries a rich, confident sweetness that feels polished rather than delicate, giving it a distinctive personality that works effortlessly on both men and women.", ImageUrl = "/images/products/Jack Hobbs.jpeg", ScentFamily = "Spicy", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 9, CategoryId = 1, Name = "Cairo Evening", Description = "Warm, mysterious, and irresistibly elegant. Cairo Evening blends rich oriental warmth with smooth woody accords and a touch of soft sweetness, creating a fragrance that evokes golden sunsets, ancient streets, and unforgettable nights.", ImageUrl = "/images/products/Cairo Evening.jpeg", ScentFamily = "Spicy", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 12, CategoryId = 1, Name = "Fantasy", Description = "Delicate, enchanting, and effortlessly captivating. Fantasy opens with soft fruity sweetness that melts into elegant floral notes, resting on a smooth musky base to create a fragrance that feels graceful, dreamy, and unforgettable.", ImageUrl = "/images/products/Fantasy.jpeg", ScentFamily = "Floral", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 14, CategoryId = 1, Name = "Honey Tobacco", Description = "Indulge in the rich harmony of golden honey and smooth tobacco. Sweet yet bold, Honey Tobacco creates a warm, sophisticated fragrance with a deep, luxurious character that feels both comforting and unforgettable.", ImageUrl = "/images/products/Honey Tobacco.jpeg", ScentFamily = "Spicy", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 15, CategoryId = 1, Name = "Kesar Chandan", Description = "Experience the richness of tradition with Kesar Chandan. The luxurious warmth of saffron blends beautifully with the creamy depth of sandalwood, creating a fragrance that is soothing, elegant, and deeply comforting.", ImageUrl = "/images/products/Kesar Chandan.jpeg", ScentFamily = "Spicy", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 18, CategoryId = 1, Name = "Chocolate", Description = "Rich, creamy, and irresistibly indulgent. Chocolate wraps you in the comforting aroma of smooth dark cocoa, blended with soft gourmand sweetness for a fragrance that feels warm, cozy, and deliciously addictive.", ImageUrl = "/images/products/Chocolate.jpeg", ScentFamily = "Spicy", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 20, CategoryId = 1, Name = "Jasmine", Description = "Immerse yourself in the enchanting beauty of blooming jasmine. Rich, floral, and naturally captivating, Jasmine captures the essence of freshly blossomed flowers, creating a fragrance that feels elegant, graceful, and timeless.", ImageUrl = "/images/products/Jasmine.jpeg", ScentFamily = "Floral", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 21, CategoryId = 1, Name = "Golden Ash", Description = "Discover a fragrance that embodies quiet confidence and timeless elegance. Golden Ash opens with a refined freshness before unfolding into a warm, sophisticated character that feels rich, smooth, and effortlessly luxurious.", ImageUrl = "/images/products/Golden Ash.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 22, CategoryId = 1, Name = "Maya", Description = "Bright, joyful, and effortlessly captivating, Maya opens with refreshing citrus notes that blend beautifully into a delicate floral heart, finished with a soft touch of sweetness.", ImageUrl = "/images/products/Maya.jpeg", ScentFamily = "Floral", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 23, CategoryId = 1, Name = "Honey Ember", Description = "Wrap yourself in the comforting warmth of Honey Amber. This rich fragrance blends the smooth sweetness of honey with the deep, resinous elegance of amber, creating a scent that feels warm, inviting, and effortlessly luxurious.", ImageUrl = "/images/products/Honey Ember.jpeg", ScentFamily = "Spicy", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 24, CategoryId = 1, Name = "Echo", Description = "A sweetness that lingers softly, like a memory you want to return to. Echo blends a sweet character with floral elegance, creating a fragrance that feels graceful, inviting, and effortlessly polished without becoming overly delicate.", ImageUrl = "/images/products/Echo.jpeg", ScentFamily = "Floral", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 27, CategoryId = 1, Name = "Turkish Rose", Description = "Experience the timeless beauty of blooming Turkish roses. Delicate yet captivating, Turkish Rose unfolds with a rich floral elegance, creating a fragrance that feels romantic, graceful, and effortlessly luxurious.", ImageUrl = "/images/products/Turkish Rose.jpeg", ScentFamily = "Floral", RecommendedOccasion = "Evening", Intensity = "Bold" },
                new Product { ProductId = 29, CategoryId = 1, Name = "S.P. Sandal", Description = "Pure, creamy, and deeply calming. S.P. Sandal celebrates the timeless beauty of authentic sandalwood with its smooth, woody warmth and soft, meditative character that lingers gracefully throughout the day.", ImageUrl = "/images/products/S.P.Sandal.jpeg", ScentFamily = "Woody", RecommendedOccasion = "Spiritual", Intensity = "Subtle" },
                new Product { ProductId = 32, CategoryId = 1, Name = "Royal 96", Description = "Some fragrances don't simply accompany an occasion—they define it. Royal 96 carries a rich, distinguished character that reflects quiet confidence and timeless refinement, creating a presence that feels composed from the moment it is worn.", ImageUrl = "/images/products/Royal 96.jpeg", ScentFamily = "Spicy", RecommendedOccasion = "Evening", Intensity = "Bold" },

                // Contemporary Fragrances (CategoryId = 3)
                new Product { ProductId = 5, CategoryId = 3, Name = "Midnight Ice", Description = "Cool, crisp, and effortlessly refined, Midnight Ice opens with refreshing citrus and aromatic notes, unfolding into a clean, invigorating fragrance that feels modern and sophisticated.", ImageUrl = "/images/products/Midnight Ice.jpeg", ScentFamily = "Citrus", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 6, CategoryId = 3, Name = "Maverick", Description = "Step into every moment with confidence. Maverick is a vibrant blend of zesty citrus and juicy fruity notes, creating a fragrance that's fresh, energetic, and effortlessly modern.", ImageUrl = "/images/products/Maverick.jpeg", ScentFamily = "Citrus", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 10, CategoryId = 3, Name = "Cool Water", Description = "Freshness that feels like stepping into a cool ocean breeze on a clear summer morning. Cool Water carries a crisp, aquatic character that feels clean, refreshing, and effortlessly confident.", ImageUrl = "/images/products/Cool Water.jpeg", ScentFamily = "Citrus", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 11, CategoryId = 3, Name = "Red Crystal", Description = "Radiate confidence with Red Crystal, a fragrance that blends vibrant freshness with refined elegance. Its smooth and captivating character creates a scent that feels modern, sophisticated, and effortlessly memorable.", ImageUrl = "/images/products/Red Crystal.jpeg", ScentFamily = "Floral", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 16, CategoryId = 3, Name = "Velmont", Description = "Fresh, refined, and effortlessly sophisticated. Velmont opens with a clean, uplifting freshness before settling into a smooth, elegant character that feels polished without becoming overpowering.", ImageUrl = "/images/products/Velmont.jpeg", ScentFamily = "Fresh", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 17, CategoryId = 3, Name = "Hawas", Description = "Freshness with an unmistakable edge of confidence. Hawas carries a crisp, aquatic character that feels energetic, modern, and effortlessly attractive.", ImageUrl = "/images/products/Hawas.jpeg", ScentFamily = "Citrus", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 19, CategoryId = 3, Name = "Polo Sport", Description = "Stay fresh, confident, and ready for every moment with Polo Sport. This energetic fragrance combines crisp citrus, refreshing aquatic accords, and clean aromatic notes to create a vibrant scent that's both modern and effortlessly masculine.", ImageUrl = "/images/products/Polo Sport.jpeg", ScentFamily = "Citrus", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 25, CategoryId = 3, Name = "Berry Blast", Description = "Bursting with the irresistible sweetness of ripe berries, Berry Blast is a vibrant fragrance that feels playful, refreshing, and full of life. Its fruity character is perfectly balanced to create a scent that's uplifting, smooth, and effortlessly charming.", ImageUrl = "/images/products/Berry Blast.jpeg", ScentFamily = "Citrus", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 26, CategoryId = 3, Name = "Marine Haze", Description = "Feel the refreshing essence of the ocean with Marine Haze. This invigorating fragrance combines crisp citrus, fresh aquatic accords, and clean aromatic notes to create a light, uplifting scent that keeps you feeling fresh all day.", ImageUrl = "/images/products/Marine Haze.jpeg", ScentFamily = "Citrus", RecommendedOccasion = "Daily", Intensity = "Subtle" },
                new Product { ProductId = 28, CategoryId = 3, Name = "White Onyx", Description = "Clean, elegant, and effortlessly luxurious. White Onyx carries a smooth, sophisticated aroma with a fresh refined character that feels polished from the first impression to the final lingering trail.", ImageUrl = "/images/products/White Onyx.jpeg", ScentFamily = "Fresh", RecommendedOccasion = "Daily", Intensity = "Subtle" }
            );

            // ============================================
            // SEED PRODUCT VARIANTS (6ml & 12ml)
            // ============================================
            modelBuilder.Entity<ProductVariant>().HasData(
                // Oud Al Noor (1)
                new ProductVariant { VariantId = 101, ProductId = 1, Size = "6ml", Price = 240, StockQuantity = 50 },
                new ProductVariant { VariantId = 102, ProductId = 1, Size = "12ml", Price = 410, StockQuantity = 30 },
                // Royal Oud (2)
                new ProductVariant { VariantId = 201, ProductId = 2, Size = "6ml", Price = 330, StockQuantity = 40 },
                new ProductVariant { VariantId = 202, ProductId = 2, Size = "12ml", Price = 600, StockQuantity = 25 },
                // Oud Habib (3)
                new ProductVariant { VariantId = 301, ProductId = 3, Size = "6ml", Price = 200, StockQuantity = 45 },
                new ProductVariant { VariantId = 302, ProductId = 3, Size = "12ml", Price = 350, StockQuantity = 30 },
                // Ridhaan (4)
                new ProductVariant { VariantId = 401, ProductId = 4, Size = "6ml", Price = 240, StockQuantity = 50 },
                new ProductVariant { VariantId = 402, ProductId = 4, Size = "12ml", Price = 430, StockQuantity = 30 },
                // Midnight Ice (5)
                new ProductVariant { VariantId = 501, ProductId = 5, Size = "6ml", Price = 150, StockQuantity = 60 },
                new ProductVariant { VariantId = 502, ProductId = 5, Size = "12ml", Price = 260, StockQuantity = 40 },
                // Maverick (6)
                new ProductVariant { VariantId = 601, ProductId = 6, Size = "6ml", Price = 200, StockQuantity = 50 },
                new ProductVariant { VariantId = 602, ProductId = 6, Size = "12ml", Price = 350, StockQuantity = 30 },
                // Lovely (7)
                new ProductVariant { VariantId = 701, ProductId = 7, Size = "6ml", Price = 135, StockQuantity = 55 },
                new ProductVariant { VariantId = 702, ProductId = 7, Size = "12ml", Price = 225, StockQuantity = 35 },
                // Jack Hobbs (8)
                new ProductVariant { VariantId = 801, ProductId = 8, Size = "6ml", Price = 200, StockQuantity = 45 },
                new ProductVariant { VariantId = 802, ProductId = 8, Size = "12ml", Price = 350, StockQuantity = 30 },
                // Cairo Evening (9)
                new ProductVariant { VariantId = 901, ProductId = 9, Size = "6ml", Price = 135, StockQuantity = 60 },
                new ProductVariant { VariantId = 902, ProductId = 9, Size = "12ml", Price = 225, StockQuantity = 40 },
                // Cool Water (10)
                new ProductVariant { VariantId = 1001, ProductId = 10, Size = "6ml", Price = 150, StockQuantity = 50 },
                new ProductVariant { VariantId = 1002, ProductId = 10, Size = "12ml", Price = 250, StockQuantity = 35 },
                // Red Crystal (11)
                new ProductVariant { VariantId = 1101, ProductId = 11, Size = "6ml", Price = 200, StockQuantity = 45 },
                new ProductVariant { VariantId = 1102, ProductId = 11, Size = "12ml", Price = 350, StockQuantity = 30 },
                // Fantasy (12)
                new ProductVariant { VariantId = 1201, ProductId = 12, Size = "6ml", Price = 100, StockQuantity = 70 },
                new ProductVariant { VariantId = 1202, ProductId = 12, Size = "12ml", Price = 160, StockQuantity = 50 },
                // Black Onyx (13)
                new ProductVariant { VariantId = 1301, ProductId = 13, Size = "6ml", Price = 200, StockQuantity = 40 },
                new ProductVariant { VariantId = 1302, ProductId = 13, Size = "12ml", Price = 350, StockQuantity = 25 },
                // Honey Tobacco (14)
                new ProductVariant { VariantId = 1401, ProductId = 14, Size = "6ml", Price = 200, StockQuantity = 45 },
                new ProductVariant { VariantId = 1402, ProductId = 14, Size = "12ml", Price = 350, StockQuantity = 30 },
                // Kesar Chandan (15)
                new ProductVariant { VariantId = 1501, ProductId = 15, Size = "6ml", Price = 240, StockQuantity = 50 },
                new ProductVariant { VariantId = 1502, ProductId = 15, Size = "12ml", Price = 450, StockQuantity = 30 },
                // Velmont (16)
                new ProductVariant { VariantId = 1601, ProductId = 16, Size = "6ml", Price = 200, StockQuantity = 50 },
                new ProductVariant { VariantId = 1602, ProductId = 16, Size = "12ml", Price = 350, StockQuantity = 35 },
                // Hawas (17)
                new ProductVariant { VariantId = 1701, ProductId = 17, Size = "6ml", Price = 150, StockQuantity = 60 },
                new ProductVariant { VariantId = 1702, ProductId = 17, Size = "12ml", Price = 250, StockQuantity = 40 },
                // Chocolate (18)
                new ProductVariant { VariantId = 1801, ProductId = 18, Size = "6ml", Price = 150, StockQuantity = 55 },
                new ProductVariant { VariantId = 1802, ProductId = 18, Size = "12ml", Price = 250, StockQuantity = 35 },
                // Polo Sport (19)
                new ProductVariant { VariantId = 1901, ProductId = 19, Size = "6ml", Price = 135, StockQuantity = 60 },
                new ProductVariant { VariantId = 1902, ProductId = 19, Size = "12ml", Price = 225, StockQuantity = 40 },
                // Jasmine (20)
                new ProductVariant { VariantId = 2001, ProductId = 20, Size = "6ml", Price = 135, StockQuantity = 55 },
                new ProductVariant { VariantId = 2002, ProductId = 20, Size = "12ml", Price = 225, StockQuantity = 35 },
                // Golden Ash (21)
                new ProductVariant { VariantId = 2101, ProductId = 21, Size = "6ml", Price = 200, StockQuantity = 45 },
                new ProductVariant { VariantId = 2102, ProductId = 21, Size = "12ml", Price = 350, StockQuantity = 30 },
                // Maya (22)
                new ProductVariant { VariantId = 2201, ProductId = 22, Size = "6ml", Price = 120, StockQuantity = 70 },
                new ProductVariant { VariantId = 2202, ProductId = 22, Size = "12ml", Price = 200, StockQuantity = 50 },
                // Honey Ember (23)
                new ProductVariant { VariantId = 2301, ProductId = 23, Size = "6ml", Price = 150, StockQuantity = 50 },
                new ProductVariant { VariantId = 2302, ProductId = 23, Size = "12ml", Price = 260, StockQuantity = 35 },
                // Echo (24)
                new ProductVariant { VariantId = 2401, ProductId = 24, Size = "6ml", Price = 150, StockQuantity = 55 },
                new ProductVariant { VariantId = 2402, ProductId = 24, Size = "12ml", Price = 260, StockQuantity = 35 },
                // Berry Blast (25)
                new ProductVariant { VariantId = 2501, ProductId = 25, Size = "6ml", Price = 150, StockQuantity = 60 },
                new ProductVariant { VariantId = 2502, ProductId = 25, Size = "12ml", Price = 250, StockQuantity = 40 },
                // Marine Haze (26)
                new ProductVariant { VariantId = 2601, ProductId = 26, Size = "6ml", Price = 200, StockQuantity = 50 },
                new ProductVariant { VariantId = 2602, ProductId = 26, Size = "12ml", Price = 250, StockQuantity = 35 },
                // Turkish Rose (27)
                new ProductVariant { VariantId = 2701, ProductId = 27, Size = "6ml", Price = 390, StockQuantity = 30 },
                new ProductVariant { VariantId = 2702, ProductId = 27, Size = "12ml", Price = 750, StockQuantity = 20 },
                // White Onyx (28)
                new ProductVariant { VariantId = 2801, ProductId = 28, Size = "6ml", Price = 200, StockQuantity = 45 },
                new ProductVariant { VariantId = 2802, ProductId = 28, Size = "12ml", Price = 350, StockQuantity = 30 },
                // S.P. Sandal (29)
                new ProductVariant { VariantId = 2901, ProductId = 29, Size = "6ml", Price = 150, StockQuantity = 50 },
                new ProductVariant { VariantId = 2902, ProductId = 29, Size = "12ml", Price = 250, StockQuantity = 35 },
                // White Oud (30)
                new ProductVariant { VariantId = 3001, ProductId = 30, Size = "6ml", Price = 200, StockQuantity = 40 },
                new ProductVariant { VariantId = 3002, ProductId = 30, Size = "12ml", Price = 350, StockQuantity = 25 },
                // Musk (31)
                new ProductVariant { VariantId = 3101, ProductId = 31, Size = "6ml", Price = 200, StockQuantity = 45 },
                new ProductVariant { VariantId = 3102, ProductId = 31, Size = "12ml", Price = 350, StockQuantity = 30 },
                // Royal 96 (32)
                new ProductVariant { VariantId = 3201, ProductId = 32, Size = "6ml", Price = 160, StockQuantity = 50 },
                new ProductVariant { VariantId = 3202, ProductId = 32, Size = "12ml", Price = 300, StockQuantity = 35 },
                // Shahi Darbar (33)
                new ProductVariant { VariantId = 3301, ProductId = 33, Size = "6ml", Price = 420, StockQuantity = 25 },
                new ProductVariant { VariantId = 3302, ProductId = 33, Size = "12ml", Price = 800, StockQuantity = 15 },
                // Black Oud (34)
                new ProductVariant { VariantId = 3401, ProductId = 34, Size = "6ml", Price = 600, StockQuantity = 20 },
                new ProductVariant { VariantId = 3402, ProductId = 34, Size = "12ml", Price = 1100, StockQuantity = 10 },
                // Oud (35)
                new ProductVariant { VariantId = 3501, ProductId = 35, Size = "6ml", Price = 725, StockQuantity = 15 },
                new ProductVariant { VariantId = 3502, ProductId = 35, Size = "12ml", Price = 1350, StockQuantity = 8 }
            );
        }
    }
}