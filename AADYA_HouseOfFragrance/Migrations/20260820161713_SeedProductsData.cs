using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AADYA_HouseOfFragrance.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // Insert Categories (if not already present)
            // ============================================
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 1, "Signature Blends" },
                    { 2, "Attars & Oud" },
                    { 3, "Contemporary Fragrances" }
                });

            // ============================================
            // Insert Products
            // ============================================
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Name", "Description", "ImageUrl", "ScentFamily", "RecommendedOccasion", "Intensity", "CreatedAt" },
                values: new object[,]
                {
                    // Attars & Oud (CategoryId = 2)
                    { 1, 2, "Oud Al Noor", "Rich oud accords beautifully balanced with warm, refined notes to create a fragrance that is deep, sophisticated, and effortlessly luxurious.", "/images/products/OUD Al Noor.jpeg", "Woody", "Evening", "Bold", DateTime.Now },
                    { 2, 2, "Royal Oud", "Crafted for those who appreciate timeless luxury, Royal Oud is a rich and refined fragrance that showcases the majestic character of oud with a smooth, elegant warmth.", "/images/products/Royal OUD.jpeg", "Woody", "Evening", "Bold", DateTime.Now },
                    { 3, 2, "Oud Habib", "Experience the perfect balance of depth and elegance with Oud Habib. Rich oud is complemented by warm, refined accords to create a fragrance that feels smooth, luxurious, and irresistibly captivating.", "/images/products/OUD Habib.jpeg", "Woody", "Evening", "Bold", DateTime.Now },
                    { 13, 2, "Black Onyx", "Dark, composed, and quietly intriguing—Black Onyx is made for a presence that doesn't need to compete for attention.", "/images/products/Black Onyx.jpeg", "Woody", "Evening", "Bold", DateTime.Now },
                    { 30, 2, "White Oud", "Oud, reimagined with a cleaner and more refined presence. White Oud carries the distinctive woody character of oud in an elegant profile that feels sophisticated, composed, and easier to wear than darker oud styles.", "/images/products/White OUD.jpeg", "Woody", "Evening", "Bold", DateTime.Now },
                    { 31, 2, "Musk", "Bold, intense, and undeniably captivating, Musk is crafted for those who appreciate the rich depth of traditional musk.", "/images/products/Musk.jpeg", "Woody", "Evening", "Bold", DateTime.Now },
                    { 33, 2, "Shahi Darbar", "A fragrance with the poise of a royal gathering—graceful, distinguished, and steeped in tradition.", "/images/products/Shahi Darbar.jpeg", "Woody", "Evening", "Bold", DateTime.Now },
                    { 34, 2, "Black Oud", "Darkness, refined into a fragrance. Black Oud carries the unmistakable depth of oud with a darker, more mysterious character—rich, woody, and composed for those who prefer their scent with genuine presence.", "/images/products/Black OUD.jpeg", "Woody", "Evening", "Bold", DateTime.Now },
                    { 35, 2, "Oud", "Deep, woody, and unmistakably commanding. Oud captures the timeless character of one of perfumery's most prized aromas—rich, smoky warmth with an earthy depth that develops beautifully on the skin.", "/images/products/OUD.jpeg", "Woody", "Evening", "Bold", DateTime.Now },

                    // Signature Blends (CategoryId = 1)
                    { 4, 1, "Ridhaan", "Refined, confident, and effortlessly elegant, Ridhaan is a fragrance crafted for those who appreciate understated luxury.", "/images/products/Ridhaan.jpeg", "Fresh", "Daily", "Subtle", DateTime.Now },
                    { 7, 1, "Lovely", "Soft sweetness has a way of being remembered. Lovely wraps you in a delicate, graceful character that feels warm, charming, and beautifully feminine without becoming overwhelming.", "/images/products/Lovely.jpeg", "Floral", "Daily", "Subtle", DateTime.Now },
                    { 8, 1, "Jack Hobbs", "Sweetness, with a sharper sense of character. Jack Hobbs carries a rich, confident sweetness that feels polished rather than delicate, giving it a distinctive personality that works effortlessly on both men and women.", "/images/products/Jack Hobbs.jpeg", "Spicy", "Evening", "Bold", DateTime.Now },
                    { 9, 1, "Cairo Evening", "Warm, mysterious, and irresistibly elegant. Cairo Evening blends rich oriental warmth with smooth woody accords and a touch of soft sweetness, creating a fragrance that evokes golden sunsets, ancient streets, and unforgettable nights.", "/images/products/Cairo Evening.jpeg", "Spicy", "Evening", "Bold", DateTime.Now },
                    { 12, 1, "Fantasy", "Delicate, enchanting, and effortlessly captivating. Fantasy opens with soft fruity sweetness that melts into elegant floral notes, resting on a smooth musky base to create a fragrance that feels graceful, dreamy, and unforgettable.", "/images/products/Fantasy.jpeg", "Floral", "Daily", "Subtle", DateTime.Now },
                    { 14, 1, "Honey Tobacco", "Indulge in the rich harmony of golden honey and smooth tobacco. Sweet yet bold, Honey Tobacco creates a warm, sophisticated fragrance with a deep, luxurious character that feels both comforting and unforgettable.", "/images/products/Honey Tobacco.jpeg", "Spicy", "Evening", "Bold", DateTime.Now },
                    { 15, 1, "Kesar Chandan", "Experience the richness of tradition with Kesar Chandan. The luxurious warmth of saffron blends beautifully with the creamy depth of sandalwood, creating a fragrance that is soothing, elegant, and deeply comforting.", "/images/products/Kesar Chandan.jpeg", "Spicy", "Evening", "Bold", DateTime.Now },
                    { 18, 1, "Chocolate", "Rich, creamy, and irresistibly indulgent. Chocolate wraps you in the comforting aroma of smooth dark cocoa, blended with soft gourmand sweetness for a fragrance that feels warm, cozy, and deliciously addictive.", "/images/products/Chocolate.jpeg", "Spicy", "Evening", "Bold", DateTime.Now },
                    { 20, 1, "Jasmine", "Immerse yourself in the enchanting beauty of blooming jasmine. Rich, floral, and naturally captivating, Jasmine captures the essence of freshly blossomed flowers, creating a fragrance that feels elegant, graceful, and timeless.", "/images/products/Jasmine.jpeg", "Floral", "Daily", "Subtle", DateTime.Now },
                    { 21, 1, "Golden Ash", "Discover a fragrance that embodies quiet confidence and timeless elegance. Golden Ash opens with a refined freshness before unfolding into a warm, sophisticated character that feels rich, smooth, and effortlessly luxurious.", "/images/products/Golden Ash.jpeg", "Woody", "Evening", "Bold", DateTime.Now },
                    { 22, 1, "Maya", "Bright, joyful, and effortlessly captivating, Maya opens with refreshing citrus notes that blend beautifully into a delicate floral heart, finished with a soft touch of sweetness.", "/images/products/Maya.jpeg", "Floral", "Daily", "Subtle", DateTime.Now },
                    { 23, 1, "Honey Ember", "Wrap yourself in the comforting warmth of Honey Amber. This rich fragrance blends the smooth sweetness of honey with the deep, resinous elegance of amber, creating a scent that feels warm, inviting, and effortlessly luxurious.", "/images/products/Honey Ember.jpeg", "Spicy", "Evening", "Bold", DateTime.Now },
                    { 24, 1, "Echo", "A sweetness that lingers softly, like a memory you want to return to. Echo blends a sweet character with floral elegance, creating a fragrance that feels graceful, inviting, and effortlessly polished without becoming overly delicate.", "/images/products/Echo.jpeg", "Floral", "Daily", "Subtle", DateTime.Now },
                    { 27, 1, "Turkish Rose", "Experience the timeless beauty of blooming Turkish roses. Delicate yet captivating, Turkish Rose unfolds with a rich floral elegance, creating a fragrance that feels romantic, graceful, and effortlessly luxurious.", "/images/products/Turkish Rose.jpeg", "Floral", "Evening", "Bold", DateTime.Now },
                    { 29, 1, "S.P. Sandal", "Pure, creamy, and deeply calming. S.P. Sandal celebrates the timeless beauty of authentic sandalwood with its smooth, woody warmth and soft, meditative character that lingers gracefully throughout the day.", "/images/products/S.P.Sandal.jpeg", "Woody", "Spiritual", "Subtle", DateTime.Now },
                    { 32, 1, "Royal 96", "Some fragrances don't simply accompany an occasion—they define it. Royal 96 carries a rich, distinguished character that reflects quiet confidence and timeless refinement, creating a presence that feels composed from the moment it is worn.", "/images/products/Royal 96.jpeg", "Spicy", "Evening", "Bold", DateTime.Now },

                    // Contemporary Fragrances (CategoryId = 3)
                    { 5, 3, "Midnight Ice", "Cool, crisp, and effortlessly refined, Midnight Ice opens with refreshing citrus and aromatic notes, unfolding into a clean, invigorating fragrance that feels modern and sophisticated.", "/images/products/Midnight Ice.jpeg", "Citrus", "Daily", "Subtle", DateTime.Now },
                    { 6, 3, "Maverick", "Step into every moment with confidence. Maverick is a vibrant blend of zesty citrus and juicy fruity notes, creating a fragrance that's fresh, energetic, and effortlessly modern.", "/images/products/Maverick.jpeg", "Citrus", "Daily", "Subtle", DateTime.Now },
                    { 10, 3, "Cool Water", "Freshness that feels like stepping into a cool ocean breeze on a clear summer morning. Cool Water carries a crisp, aquatic character that feels clean, refreshing, and effortlessly confident.", "/images/products/Cool Water.jpeg", "Citrus", "Daily", "Subtle", DateTime.Now },
                    { 11, 3, "Red Crystal", "Radiate confidence with Red Crystal, a fragrance that blends vibrant freshness with refined elegance. Its smooth and captivating character creates a scent that feels modern, sophisticated, and effortlessly memorable.", "/images/products/Red Crystal.jpeg", "Floral", "Daily", "Subtle", DateTime.Now },
                    { 16, 3, "Velmont", "Fresh, refined, and effortlessly sophisticated. Velmont opens with a clean, uplifting freshness before settling into a smooth, elegant character that feels polished without becoming overpowering.", "/images/products/Velmont.jpeg", "Fresh", "Daily", "Subtle", DateTime.Now },
                    { 17, 3, "Hawas", "Freshness with an unmistakable edge of confidence. Hawas carries a crisp, aquatic character that feels energetic, modern, and effortlessly attractive.", "/images/products/Hawas.jpeg", "Citrus", "Daily", "Subtle", DateTime.Now },
                    { 19, 3, "Polo Sport", "Stay fresh, confident, and ready for every moment with Polo Sport. This energetic fragrance combines crisp citrus, refreshing aquatic accords, and clean aromatic notes to create a vibrant scent that's both modern and effortlessly masculine.", "/images/products/Polo Sport.jpeg", "Citrus", "Daily", "Subtle", DateTime.Now },
                    { 25, 3, "Berry Blast", "Bursting with the irresistible sweetness of ripe berries, Berry Blast is a vibrant fragrance that feels playful, refreshing, and full of life. Its fruity character is perfectly balanced to create a scent that's uplifting, smooth, and effortlessly charming.", "/images/products/Berry Blast.jpeg", "Citrus", "Daily", "Subtle", DateTime.Now },
                    { 26, 3, "Marine Haze", "Feel the refreshing essence of the ocean with Marine Haze. This invigorating fragrance combines crisp citrus, fresh aquatic accords, and clean aromatic notes to create a light, uplifting scent that keeps you feeling fresh all day.", "/images/products/Marine Haze.jpeg", "Citrus", "Daily", "Subtle", DateTime.Now },
                    { 28, 3, "White Onyx", "Clean, elegant, and effortlessly luxurious. White Onyx carries a smooth, sophisticated aroma with a fresh refined character that feels polished from the first impression to the final lingering trail.", "/images/products/White Onyx.jpeg", "Fresh", "Daily", "Subtle", DateTime.Now }
                });

            // ============================================
            // Insert ProductVariants
            // ============================================
            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "VariantId", "ProductId", "Size", "Price", "StockQuantity" },
                values: new object[,]
                {
                    // Oud Al Noor (1)
                    { 101, 1, "6ml", 240, 50 },
                    { 102, 1, "12ml", 410, 30 },
                    // Royal Oud (2)
                    { 201, 2, "6ml", 330, 40 },
                    { 202, 2, "12ml", 600, 25 },
                    // Oud Habib (3)
                    { 301, 3, "6ml", 200, 45 },
                    { 302, 3, "12ml", 350, 30 },
                    // Ridhaan (4)
                    { 401, 4, "6ml", 240, 50 },
                    { 402, 4, "12ml", 430, 30 },
                    // Midnight Ice (5)
                    { 501, 5, "6ml", 150, 60 },
                    { 502, 5, "12ml", 260, 40 },
                    // Maverick (6)
                    { 601, 6, "6ml", 200, 50 },
                    { 602, 6, "12ml", 350, 30 },
                    // Lovely (7)
                    { 701, 7, "6ml", 135, 55 },
                    { 702, 7, "12ml", 225, 35 },
                    // Jack Hobbs (8)
                    { 801, 8, "6ml", 200, 45 },
                    { 802, 8, "12ml", 350, 30 },
                    // Cairo Evening (9)
                    { 901, 9, "6ml", 135, 60 },
                    { 902, 9, "12ml", 225, 40 },
                    // Cool Water (10)
                    { 1001, 10, "6ml", 150, 50 },
                    { 1002, 10, "12ml", 250, 35 },
                    // Red Crystal (11)
                    { 1101, 11, "6ml", 200, 45 },
                    { 1102, 11, "12ml", 350, 30 },
                    // Fantasy (12)
                    { 1201, 12, "6ml", 100, 70 },
                    { 1202, 12, "12ml", 160, 50 },
                    // Black Onyx (13)
                    { 1301, 13, "6ml", 200, 40 },
                    { 1302, 13, "12ml", 350, 25 },
                    // Honey Tobacco (14)
                    { 1401, 14, "6ml", 200, 45 },
                    { 1402, 14, "12ml", 350, 30 },
                    // Kesar Chandan (15)
                    { 1501, 15, "6ml", 240, 50 },
                    { 1502, 15, "12ml", 450, 30 },
                    // Velmont (16)
                    { 1601, 16, "6ml", 200, 50 },
                    { 1602, 16, "12ml", 350, 35 },
                    // Hawas (17)
                    { 1701, 17, "6ml", 150, 60 },
                    { 1702, 17, "12ml", 250, 40 },
                    // Chocolate (18)
                    { 1801, 18, "6ml", 150, 55 },
                    { 1802, 18, "12ml", 250, 35 },
                    // Polo Sport (19)
                    { 1901, 19, "6ml", 135, 60 },
                    { 1902, 19, "12ml", 225, 40 },
                    // Jasmine (20)
                    { 2001, 20, "6ml", 135, 55 },
                    { 2002, 20, "12ml", 225, 35 },
                    // Golden Ash (21)
                    { 2101, 21, "6ml", 200, 45 },
                    { 2102, 21, "12ml", 350, 30 },
                    // Maya (22)
                    { 2201, 22, "6ml", 120, 70 },
                    { 2202, 22, "12ml", 200, 50 },
                    // Honey Ember (23)
                    { 2301, 23, "6ml", 150, 50 },
                    { 2302, 23, "12ml", 260, 35 },
                    // Echo (24)
                    { 2401, 24, "6ml", 150, 55 },
                    { 2402, 24, "12ml", 260, 35 },
                    // Berry Blast (25)
                    { 2501, 25, "6ml", 150, 60 },
                    { 2502, 25, "12ml", 250, 40 },
                    // Marine Haze (26)
                    { 2601, 26, "6ml", 200, 50 },
                    { 2602, 26, "12ml", 250, 35 },
                    // Turkish Rose (27)
                    { 2701, 27, "6ml", 390, 30 },
                    { 2702, 27, "12ml", 750, 20 },
                    // White Onyx (28)
                    { 2801, 28, "6ml", 200, 45 },
                    { 2802, 28, "12ml", 350, 30 },
                    // S.P. Sandal (29)
                    { 2901, 29, "6ml", 150, 50 },
                    { 2902, 29, "12ml", 250, 35 },
                    // White Oud (30)
                    { 3001, 30, "6ml", 200, 40 },
                    { 3002, 30, "12ml", 350, 25 },
                    // Musk (31)
                    { 3101, 31, "6ml", 200, 45 },
                    { 3102, 31, "12ml", 350, 30 },
                    // Royal 96 (32)
                    { 3201, 32, "6ml", 160, 50 },
                    { 3202, 32, "12ml", 300, 35 },
                    // Shahi Darbar (33)
                    { 3301, 33, "6ml", 420, 25 },
                    { 3302, 33, "12ml", 800, 15 },
                    // Black Oud (34)
                    { 3401, 34, "6ml", 600, 20 },
                    { 3402, 34, "12ml", 1100, 10 },
                    // Oud (35)
                    { 3501, 35, "6ml", 725, 15 },
                    { 3502, 35, "12ml", 1350, 8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ProductVariants");
            migrationBuilder.Sql("DELETE FROM Products");
            migrationBuilder.Sql("DELETE FROM Categories");
        }
    }
}