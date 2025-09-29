using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public class Subscribe : Entity
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }


    }
}
