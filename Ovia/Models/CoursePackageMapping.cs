using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public partial class CoursePackageMapping : Entity
    {
        [Key]
      //  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? PackageId { get; set; }
        public int? DisplayOrder { get; set; }
        public string UriPathname { get; set; }

       public virtual Course Course { get; set; }
        public virtual Package Package { get; set; }
    }

}
