namespace Api.Models
{
    public class InventoryAllocation
    {
        public int InventoryAllocationId { get; set; }
        public int InventoryId { get; set; }
        public int CustomerOrderItemId { get; set; }
        public double AllocatedQuantityKg { get; set; }
    }
}