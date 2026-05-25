using System.ComponentModel.DataAnnotations;

namespace BoutiqueQuantity.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductCode { get; set; }

        public string Name { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public ICollection<ProductVariant>
            Variants
        { get; set; }
    }
}
