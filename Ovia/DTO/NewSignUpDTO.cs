namespace Ovia.DTO
{
    public class NewSignUpDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int CountryId { get; set; }
        public string? NationalId { get; set; }
        public string Email { get; set; }
        // public string Pass { get; set; }
        // public RoleEnum Role { get; set; }
        // public string LiveStyleId { get; set; }
        public string? SponsorId { get; set; }

    }
}
