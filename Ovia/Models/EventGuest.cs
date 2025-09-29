namespace Ovia.Models
{
    public class EventGuest : Entity
    {
        public int Id { get; set; } 
        public int CustomerEventID {get; set;}
        public int CustomerID {get; set;}
        public string TicketID {get; set;}
        public string Name {get; set;}
        public string Email {get; set;}
        public string Mobile {get; set;}  
        public string Token {get; set;}
        public decimal Value {get; set;}  
    

         

    }
}
