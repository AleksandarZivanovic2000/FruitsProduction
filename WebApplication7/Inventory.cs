namespace Api.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public string ProduceType { get; set; }
        public double QuantityKg { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; }
    }
}