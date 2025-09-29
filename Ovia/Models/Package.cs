using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ovia.Models
{
    public partial class Package : Entity
    {
        [Key]
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public bool? ShowOnHomePage { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? Published { get; set; }
        public int? MembershipID { get; set; }
        public decimal? BusinessValue { get; set; }
        public decimal? ContractCost { get; set; }
        public int? CoursesNumber { get; set; }

        public int? PackageTypeId { get; set; }
        public decimal? SponsorCustomerToCustomer { get; set; }
        public decimal? SponsorDistributorToCustomer { get; set; }
        public decimal? SponsorDistributorToDistributor { get; set; }

        public decimal? SponsorTeam { get; set; }
        public decimal? Summit_Coins { get; set; }
        public decimal? Summit_Cost { get; set; }
        public int? TeamId { get; set; }
        public string? Photo { get; set; }
        public string? urlpackageName { get; set; }

        [JsonIgnore]
        public virtual ICollection<CoursePackageMapping> CoursePackageMappings { get; set; }

        [ForeignKey("PackageTypeId")]
        [JsonIgnore]
        public virtual PackagesTypes PackageType { get; set; }
    }
}

