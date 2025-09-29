using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public class TicketSupportReply : Entity
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }

        public string? Reply { get; set; }

        public int AdminId { get; set; }
        public string? Key { get; set; }


        [ForeignKey(nameof(TicketId))]
        public virtual TicketSupport Ticket { get; set; }

        [ForeignKey(nameof(AdminId))]
        public virtual CustomerAttribute CustomerAttribute { get; set; }


    }
}
