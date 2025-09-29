namespace Ovia.DTO
{
    public class GetRequestCash
    {
        public int Id { get; set; }
        public int? RequestTypeId { get; set; }
        public string? BinanceId { get; set; }
        public string? CustomerName { get; set; }
        public string?CustomerEmail  { get; set; }
        public string? ReferId { get; set; }

        public int? CustomerId { get; set; }
        public bool? IsPaid { get; set; }
        public int? PaidBy { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? RequestDate { get; set; }
        public decimal? RequestedAmount { get; set; }
        public bool? UnPaid { get; set; } = false;
        public DateTime? UnPaidDate { get; set; }



    }
}
