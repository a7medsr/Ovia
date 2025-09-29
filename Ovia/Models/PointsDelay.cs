namespace Ovia.Models
{
    public class PointsDelay : Entity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int PointProcessId { get; set; }
        public int PointCount { get; set; }
        public string Side { get; set; }
        public int ProcessTypeId { get; set; }
        public DateTime PointDate { get; set; }
        public DateTime? FlashDate { get; set; }
        public bool IsCalculated { get; set; } = true;
        public bool IsFlashed { get; set; } = true;


   

    }
}
