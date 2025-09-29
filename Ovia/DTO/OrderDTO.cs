namespace Ovia.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? PackagesId { get; set; }
        public string? PackageName { get; set; }
        public decimal? Cost { get; set; }
        public decimal? OrignalCost { get; set; }
        public DateTime CreationDate { get; set; }
        public string InvoiceSerial { get; set; }
        public bool IsCompleted { get; set; }

    }
}
