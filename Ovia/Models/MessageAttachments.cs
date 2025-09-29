using Ovia.DTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public class MessageAttachments : Entity
    {
        public int Id { get; set; }
        [ForeignKey("Room")]
        public int RoomID { get; set; }

        [ForeignKey("customerattribute")]
        public int SenderID { get; set; }

        public string? FileKey { get; set; }

        public string? FileExtension { get; set; }

        public string? FileName { get; set; }

        public long? FileSize { get; set; }


        public CustomerAttribute customerattribute { get; set; }
        public Room Room { get; set; }



    }
}
