using Ovia.Models;

namespace Ovia.DTO
{
    public class TicketSupportDetailsDTO
    {
        public int TicketId { get; set; }
        public string UserEmail { get; set; }
        public string Subject { get; set; }
        public string Question { get; set; }
        public TicketEnum Status { get; set; }
        public List<ComplaintReplyDTO>? Replies { get; set; }
        public List<String>? AttachmentsUrl { get; set; }


    }
}
