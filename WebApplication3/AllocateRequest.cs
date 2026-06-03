namespace WebApplication3.DTOs
{
    public class AllocateRequest
    {
        public int LotId { get; set; }
        public int BinId { get; set; }
        public decimal Quantity { get; set; }
    }
}