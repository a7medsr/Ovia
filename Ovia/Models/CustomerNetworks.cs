using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public class CustomerNetwork : Entity
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int? ChildId { get; set; }
        public string?   HandSide { get; set; } // You might not need nullable string here
        public int? SponsorId { get; set; }
        public string?   UplineHistoryId { get; set; } // You might not need nullable string here

        // Navigation properties
        [ForeignKey("ChildId")]
        [InverseProperty("CustomerNetworkChild")]
        public virtual CustomerAttribute Child { get; set; }

       [ForeignKey("ParentId")]
        [InverseProperty("CustomerNetworkParent")]
        public virtual CustomerAttribute Parent { get; set; }


       


    }
}
