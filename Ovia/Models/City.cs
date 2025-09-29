using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public partial class City : Entity
    {
        [Key]
     //   [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int? GovernorateId { get; set; }

        public virtual Governorate Governorate { get; set; }


    }
}
