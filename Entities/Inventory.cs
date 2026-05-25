namespace BoutiqueQuantity.Entities
{
    public class Inventory
    {
        public int Id { get; set; }

        public int ProductVariantId { get; set; }

        public ProductVariant ProductVariant { get; set; }

        // XS,S,M,L или 36,38,40...
        public string Size { get; set; }

        public int Quantity { get; set; }
    }
}
