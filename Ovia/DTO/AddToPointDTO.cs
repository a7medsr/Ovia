namespace Ovia.DTO
{
    public class AddToPointDTO
    {
        public int parentId { get; set; }
        public int? businessValue { get; set; }
        public int pointCount { get; set; }
        public string handSide { get; set; }
        public int ProcessTypeId { get; set; }
        public DateTime creationDate { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsFlashed { get; set; }
    }
}
