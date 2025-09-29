using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public class TicketSupport : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string  Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Question { get; set; }


        [Required]
        public TicketEnum Status { get; set; }

        public string? Key { get; set; }

        public virtual ICollection<TicketSupportReply> Replies { get; set; }
    }
}
