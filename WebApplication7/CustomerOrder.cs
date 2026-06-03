namespace Api.Models
{
    public class CustomerOrder
    {
        public int CustomerOrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}