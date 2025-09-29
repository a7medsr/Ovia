namespace Ovia.DTO
{
    public class TokenDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? Token { get; set; }
        public decimal Value { get; set; }
        public string? ImgUrl { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? WhatsApp { get; set; }
        public string? NationalId { get; set; }
        public string? Country { get; set; }
        public string? Role { get; set; }
        public DateTime? StartDate { get; set; }
        
        public bool IsUsed { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? Paidby { get; set; }
        public string? BackOfficeId { get; set; }
        public string? SponsorId { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
