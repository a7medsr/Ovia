namespace Ovia.Models
{
    public class MessageNotifications : Entity
    {
      
        public int Id { get; set; } 
        public string Title { get; set; }
        public string? Content { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? RecipientId { get; set; }
        public Guid? SenderId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTimeOffset? DateRead { get; set; }

    }
}
