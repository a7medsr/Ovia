using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public class PayToken :Entity
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Token { get; set; }
        public decimal Value { get; set; }
        public bool IsUsed { get; set; }    
    }
}
