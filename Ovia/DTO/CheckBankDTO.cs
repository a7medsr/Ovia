namespace Ovia.DTO
{
    public class CheckBankDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Points { get; set; }


    }
}
