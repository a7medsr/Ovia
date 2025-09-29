namespace Ovia.DTO
{
    public class AddTicketSupporttDTO
    {
        public string  Email { get; set; }
        public string Subject { get; set; }
        public string Question { get; set; }
        public IFormFile? File { get; set; }
    }
}
