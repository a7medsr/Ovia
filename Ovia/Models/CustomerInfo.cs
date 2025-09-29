using Ovia.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;

namespace Ovia.Models
{
    public partial class CustomerInfo : Entity
    {
        public CustomerInfo()
        {
            CustomerAttributes = new HashSet<CustomerAttribute>();
        }
        
       [Key]
     //  [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }

        public string? Username { get; set; }
        [Required]
        public string Email { get; set; }
        public string? LiveStyleId { get; set; }

        public string? Mobile { get; set; }
        public string? whatsappmobile { get; set; }
        public string? NationalId { get; set; }
        public string? Address { get; set; }
        public int? WorkZone { get; set; }
        public string?   Gendar { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public DateTime? BirthDate { get; set; }

        public string? Password { get; set; }
        public string? PasswordResetCode { get; set; }
        public DateTime? RequestPasswordResetDate { get; set; }
        public int? AttrMasterID { get; set; }

        public int? RoleId { get; set; }
        public string? Picture { get; set; }
        public int? CountryId { get; set; }
        public int? GovId { get; set; }
        public int? TeamId { get; set; }
        public int? RanksID { get; set; }
        public int? RanksIDold { get; set; }

        public bool? ISEdited { get; set; }
        public bool? AllowToCreateMeetings { get; set; } 
        public DateTime? LeadExpiry { get; set; }
        public bool? ISEnglish { get; set; }
        public bool? ISDark { get; set; }
        public bool? ISEmailconfirmed { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [ForeignKey("CountryId")]
        public virtual Countries Country { get; set; }

        // public virtual ICollection<ComplainsReply> ComplainsReply { get; set; }
        public virtual ICollection<CustomerAttribute> CustomerAttributes { get; set; }
        public virtual ICollection<Employe> Employes { get; set; }
        
        //public virtual ICollection<Complains> Complains { get; set; }




    }



    //public class CustomerInfo
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    //    public int Id { get; set; }
    //    public string? NameEn { get; set; }
    //    [Required]
    //    public string Email { get; set; }
    //    public string Pass {get; set;}
    //    public RoleEnum Role { get; set;}
    //    public string LiveStyleId { get; set;}

    //    public string? UserName { get; set; }


    //    public string? Mobile { get; set; }
    //    public string? whatsappmobile { get; set; }
    //    public string? NationalId { get; set; }
    //    public string?  Address { get; set; }
    //    public int? WorkZone { get; set; }
    //    public string?   Gendar { get; set; }
    //    public string? NameAr { get; set; }

    //    public DateTime? birthdate { get; set; }
    //    public string? PasswordResetCode { get; set; }
    //    public DateTime? RequestPasswordResetDate { get; set; }
    //    public int? AttrMasterID { get; set; }

    //    public int? RoleId { get; set; }
    //    public string? Picture { get; set; }
    //    public int? CountryId { get; set; }
    //    public int? GovId { get; set; }
    //    public int? TeamId { get; set; }
    //    public int? RanksID { get; set; }
    //    public int? RanksIDold { get; set; }

    //    public bool? ISEdited { get; set; }
    //    public DateTime? LeadExpiry { get; set; }
    //    public bool? ISEnglish { get; set; }
    //    public bool? ISDark { get; set; }
    //    public bool? ISEmailconfirmed { get; set; }

    //   // public virtual Role Role { get; set; }
    //   // [ForeignKey("CountryId")]
    //  //  public virtual Countries Country { get; set; }

    //    //public virtual ICollection<ComplainsReply> ComplainsReply { get; set; }
    //    //public virtual ICollection<CustomerAttributes> CustomerAttributes { get; set; }
    //    //public virtual ICollection<Employe> Employes { get; set; }
    //    //public virtual ICollection<Complains> Complains { get; set; }


    //}
}
