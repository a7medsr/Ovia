using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public partial class CustomerAnnualRenewal
    {
        [Key]
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? OldRenewalDate { get; set; }
        public DateTime? NextRenewalDate { get; set; }
        public int? RenewalMethodId { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
