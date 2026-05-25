namespace BoutiqueQuantity.Entities
{
    public class ProductVariant
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Color { get; set; }

        public string Barcode { get; set; }

        public Product Product { get; set; }

        public ICollection<Inventory> Inventories { get; set; }
    }
}
