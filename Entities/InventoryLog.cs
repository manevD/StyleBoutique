using BoutiqueQuantity.Data;

namespace BoutiqueQuantity.Entities
{
    public class InventoryLog
    {
        public int Id { get; set; }

        public int ProductVariantId { get; set; }

        public ProductVariant ProductVariant { get; set; }

        public string Size { get; set; }

        public int Quantity { get; set; }

        // IN / OUT / ADJUST
        public InventoryAction ActionType { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;

        // User кој ја направил акцијата
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
    public enum InventoryAction
    {
        In = 1,
        Out = 2,
        Adjust = 3
    }
}