using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public class TrainningMapping
    {
        [Key]
        public int Id { get; set; }
        public int TrainningId { get; set; }
        public int CustomerId { get; set; }

    }
}
