namespace BoutiqueQuantity.Models
{
    public class VariantEditViewModel
    {
        public int Id { get; set; }

        public string Color { get; set; }

        public List<InventoryEditViewModel>
        Inventories
        { get; set; }
        =
        new();
    }
}
