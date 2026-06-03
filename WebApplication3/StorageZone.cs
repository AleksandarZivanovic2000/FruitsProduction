namespace WebApplication3.Models
{
    public class StorageZone
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public decimal TempMin { get; set; }
        public decimal TempMax { get; set; }

        public decimal HumMin { get; set; }
        public decimal HumMax { get; set; }
    }
}