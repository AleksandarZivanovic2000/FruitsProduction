namespace Api.Models
{
    public class CustomerOrderItem
    {
        public int CustomerOrderItemId { get; set; }
        public int CustomerOrderId { get; set; }
        public string ProduceType { get; set; }
        public double RequestedQuantityKg { get; set; }
        public double AllocatedQuantityKg { get; set; }
    }
}