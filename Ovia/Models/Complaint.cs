using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovia.Models
{
    public class Complaint : Entity
    {
      
    

        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public TicketEnum Status { get; set; }

        public virtual ICollection<ComplaintAttachment> Attachments { get; set; }
        public virtual ICollection<ComplaintReply> Replies { get; set; }

    }





    public enum TicketEnum
    {
        Open = 1,
        Reply = 2,
        Close = 3

    }
}
