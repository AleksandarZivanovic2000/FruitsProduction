namespace WebApplication2
{
    public class HarvestRequestDto
    {
        public string LotCode { get; set; }
        public string Shift { get; set; }
        public string PickerTeam { get; set; }
        public decimal Quantity { get; set; }
        public string UnitCode { get; set; }
    }
}