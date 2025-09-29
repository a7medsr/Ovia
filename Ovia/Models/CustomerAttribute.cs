using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ovia.Models
{
    public class CustomerAttribute : Entity
    {


        
                public CustomerAttribute()
                {
                  //  ActivityLog = new HashSet<ActivityLog>();
                    BalanceActionHistory = new HashSet<BalanceActionHistory>();
                    CourseComment = new HashSet<CourseComment>();
                    CourseCustomerMapping = new HashSet<CourseCustomerMapping>();
                    CustomerCustomerRoleMapping = new HashSet<CustomerCustomerRoleMapping>();
                    CustomerNetworkChild = new HashSet<CustomerNetwork>();
                    CustomerNetworkParent = new HashSet<CustomerNetwork>();
                  //  CustomerPasswordHistory = new HashSet<CustomerPasswordHistory>();
                  //  CustomerTransactions = new HashSet<CustomerTransaction>();
                   // CustomerTransactionNavigations = new HashSet<CustomerTransaction>();
                  //  ExternalCustomersTransactions = new HashSet<ExternalCustomersTransactions>();
                  //  ExternalCustomersTransactionsNavigation = new HashSet<ExternalCustomersTransactions>();
                    Log = new HashSet<Log>();
                    //OfflineAttendance = new HashSet<OfflineAttendance>();
                    OnlineAttendance = new HashSet<OnlineAttendance>();
                    RequestCash = new HashSet<RequestCash>();
                }
        

        
        [Key]
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("CustomerInfo")]
        public int? CustomerInfoId { get; set; }
        public string? ReferId { get; set; }
        public virtual CustomerInfo CustomerInfo { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public DateTime? RenewalDateUtc { get; set; }
        public DateTime? ActualRenewalDateUtc { get; set; }
        public string? UserId { get; set; }
        public int? MembershipID { get; set; }
        public DateTime? EligibleDate { get; set; }
        public bool? Isadd { get; set; }
        public bool? IsEligible { get; set; }
        public int? RankId { get; set; }
        public bool? Renewal { get; set; }

        public virtual Rank Rank { get; set; }
       public List<RefreshTokens>? RefreshTokens { get; set; }
     //   public virtual ICollection<CustomerDiploma> CustomerDiploma { get; set; }
     //   public virtual ICollection<ActivityLog> ActivityLog { get; set; }
        public virtual ICollection<BalanceActionHistory> BalanceActionHistory { get; set; }
        public virtual ICollection<CourseComment> CourseComment { get; set; }
        public virtual ICollection<CourseCustomerMapping> CourseCustomerMapping { get; set; }
       public virtual ICollection<CustomerCustomerRoleMapping> CustomerCustomerRoleMapping { get; set; }

        [JsonIgnore]
        public virtual ICollection<CustomerNetwork> CustomerNetworkChild { get; set; }
        public virtual ICollection<CustomerNetwork> CustomerNetworkParent { get; set; }
       // public virtual ICollection<CustomerPasswordHistory> CustomerPasswordHistory { get; set; }
    //    public virtual ICollection<CustomerTransaction> CustomerTransactions { get; set; }
     //   public virtual ICollection<CustomerTransaction> CustomerTransactionNavigations { get; set; }
     //   public virtual ICollection<ExternalCustomersTransactions> ExternalCustomersTransactions { get; set; }
     //   public virtual ICollection<ExternalCustomersTransactions> ExternalCustomersTransactionsNavigation { get; set; }
        public virtual ICollection<Log> Log { get; set; }
    //    public virtual ICollection<OfflineAttendance> OfflineAttendance { get; set; }
    //    public virtual ICollection<CustomerDiplomaAttendance> CustomerDiplomaAttendances { get; set; }
        public virtual ICollection<OnlineAttendance> OnlineAttendance { get; set; }
        public virtual ICollection<RequestCash> RequestCash { get; set; }
       // public virtual ICollection<AdminUserPages> AdminUsersPages { get; set; }



    }
}
