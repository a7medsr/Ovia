using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bots.Types;
using Twilio.TwiML.Voice;

namespace Ovia.Models
{
    public class Room : Entity
    {
        public int Id { get; set; }

        public string RoomName { get; set; }

        [ForeignKey("Sender")]
        public int SenderId { get; set; }

        [ForeignKey("Receiver")]
        public int ReceiverId { get; set; }

        public virtual CustomerAttribute Sender { get; set; }
        public virtual CustomerAttribute Receiver { get; set; }



    }
}
