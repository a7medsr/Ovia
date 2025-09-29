using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public class RankHistory
    {
        public int Id { get; set; }

        [ForeignKey(nameof(CustomerAttribute))]
        public int DistributerId { get; set; }

        public int? OldRankId { get; set; }

        public int? NewRankId { get; set; }

        public DateTime CreationDate { get; set; }

      
        public virtual CustomerAttribute CustomerAttribute { get; set; }

    }
}
