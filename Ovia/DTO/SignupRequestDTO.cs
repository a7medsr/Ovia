using Ovia.Enum;

namespace Ovia.DTO
{
    public class SignupRequestDTO
    {
        public string Name { get; set; }
        public string Pass { get; set; }
       // public RoleEnum Role { get; set; }
        public string Email { get; set; }
        
        public string? SponsorId { get; set; }


    }
}
