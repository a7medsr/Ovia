namespace Ovia.DTO
{
    public class GetTokensDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Token { get; set; }
        public decimal Value { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? Paidby { get; set; }
        public string?  BackOfficeId  { get; set; }
        public DateTime? PaidDate { get; set; }



    }
}
