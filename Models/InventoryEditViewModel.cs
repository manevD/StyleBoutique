namespace BoutiqueQuantity.Models
{
    public class InventoryEditViewModel
    {
        public int Id { get; set; }

        public string Size { get; set; }

        public int Quantity { get; set; }

        // за X бришење
        public bool Delete { get; set; } 
    }
}
