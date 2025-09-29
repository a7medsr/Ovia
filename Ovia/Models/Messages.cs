using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public class Messages
    {

        public int Id { get; set; }

        [ForeignKey("Room")]
        public int roomID { get; set; }

        [ForeignKey("customerattribute")]
        public int senderID { get; set; } 

        public string content { get; set; }

        public DateTime sentAt { get; set; }
        public bool isRead { get; set; } = false;
        public DateTimeOffset? dateRead { get; set; }

        public virtual ICollection<MessageAttachments> MessageAttachments { get; set; } = new List<MessageAttachments>();
        public CustomerAttribute customerattribute { get; set; }
        public Room Room { get; set; }



       



    }
}
