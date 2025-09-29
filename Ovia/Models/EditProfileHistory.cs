namespace Ovia.Models
{
    public class EditProfileHistory : Entity
    {
        public int Id { get; set; } 
        public int UserId { get; set; }

        public string? MobileNumber { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Email { get; set; }
    }
}
