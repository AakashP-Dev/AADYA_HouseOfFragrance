using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADYA_HouseOfFragrance.Models.Entities
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceAtSale { get; set; }

        // Navigation Properties
        public Order? Order { get; set; }
        public ProductVariant? ProductVariant { get; set; }
    }
}