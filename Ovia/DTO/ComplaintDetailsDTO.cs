using Ovia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovia.DTO
{
    public class ComplaintDetailsDTO
    {
        public int ComplaintId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Subject { get; set; }
        public string Question { get; set; }
        public TicketEnum Status { get; set; }
        public List<ComplaintReplyDTO>? Replies { get; set; }
        public List<String>? AttachmentsUrl { get; set; }


    }
}
