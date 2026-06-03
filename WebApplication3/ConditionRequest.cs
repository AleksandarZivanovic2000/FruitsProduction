namespace WebApplication3.DTOs
{
    public class ConditionRequest
    {
        public int ZoneId { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
    }
}