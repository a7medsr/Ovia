using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public partial class Governorate : Entity
    {
        [Key]
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int? Countries_ID { get; set; }

        [ForeignKey("Countries_ID")]
        public virtual Countries Country { get; set; }

    }

}
