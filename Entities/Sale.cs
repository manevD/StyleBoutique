using System.ComponentModel.DataAnnotations.Schema;

namespace BoutiqueQuantity.Entities
{
    public class Sale
    {
        public int Id { get; set; }


        // =========================
        // RELATIONS
        // =========================

        public int? ProductId { get; set; }

        public Product? Product { get; set; }


        public int? ProductVariantId { get; set; }

        public ProductVariant? ProductVariant { get; set; }


        public int? InventoryId { get; set; }

        public Inventory? Inventory { get; set; }


        // =========================
        // SNAPSHOT DATA
        // =========================

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string CategoryName { get; set; }

        public string Color { get; set; }

        public string Size { get; set; }


        // =========================
        // QUANTITY
        // =========================

        public int Quantity { get; set; }

        public int ReturnedQuantity { get; set; }


        // =========================
        // PRICES
        // =========================

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public decimal Total { get; set; }

        public decimal Profit { get; set; }
        public bool IsDeleted { get; set; }

        // =========================
        // CALCULATED
        // =========================
        [NotMapped]
        public int RealQuantity =>
        Quantity - ReturnedQuantity;

        [NotMapped]
        public decimal RealTotal =>
        RealQuantity * SalePrice;

        [NotMapped]
        public decimal RealProfit =>
        (SalePrice - PurchasePrice)
        * RealQuantity;


        // =========================
        // OPTIONAL
        // =========================

        public string? Note { get; set; }

        public string? SoldByUserId { get; set; }

        public string? CustomerName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? PaymentMethod { get; set; }


        // =========================
        // RETURNS
        // =========================
        [NotMapped]
        public bool IsReturned =>
        ReturnedQuantity >= Quantity;
        public DateTime? ReturnedAt { get; set; }

        public string? ReturnReason { get; set; }


        // =========================
        // DATES
        // =========================

        public DateTime CreatedAt { get; set; }
        =
        DateTime.UtcNow;
    }
}