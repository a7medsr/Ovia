using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public partial class Role : Entity
    {
        public Role()
        {
            CustomerInfo = new HashSet<CustomerInfo>();
        }
        [Key]
     //   [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }


        public virtual ICollection<CustomerInfo> CustomerInfo { get; set; }
    }

}
