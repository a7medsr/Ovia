using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CustomerTransaction: Entity
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? BalanceBefore { get; set; }
        public decimal? BalanceAfter { get; set; }
        public int? TractionTypeId { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public int? PaymentId { get; set; }
        public int? TellerId { get; set; }
        public string ReferenceNo { get; set; }
        public string Status { get; set; }
        public int? PackageSelectId { get; set; }
    
        public int? CustomerNavigationId { get; set; }

        public virtual CustomerAttribute Customer { get; set; }
        public virtual CustomerAttribute CustomerNavigation { get; set; }
        public virtual CustomerPackageSelect PackageSelect { get; set; }
    }
}
