namespace WebApplication3.Models
{
    public class InventoryMovement
    {
        public int LotId { get; set; }
        public int? ToBinId { get; set; }
        public decimal Quantity { get; set; }
    }
}