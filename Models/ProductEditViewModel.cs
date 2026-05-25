using BoutiqueQuantity.Entities;

namespace BoutiqueQuantity.Models
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ProductEditViewModel
    {
        public int Id { get; set; }

        public string ProductCode { get; set; }

        public string Name { get; set; }

        public int CategoryId { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public List<SelectListItem>
        Categories
        { get; set; }
        =
        new();

        public List<VariantEditViewModel>
        Variants
        { get; set; }
        =
        new();
    }
}
