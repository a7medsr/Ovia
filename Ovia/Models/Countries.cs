using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public partial class Countries
    {
        [Key]
      //  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string code { get; set; } // Add this property
        public string dial_code { get; set; } // Add this property
        public string name { get; set; } // Assuming this property already exists

     //   public virtual ICollection<City> Cities { get; set; }
     //   public virtual ICollection<CustomerInfo> CustomerInfo { get; set; }
      //  public virtual ICollection<Governorate> Governorates { get; set; }

    }
}
