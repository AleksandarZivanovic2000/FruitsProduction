namespace WebApplication3.Models
{
    public class StorageConditionLog
    {
        public int ZoneId { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public DateTime LoggedAt { get; set; }
    }
}