using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CustomerPackageSelect
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? PackagesId { get; set; }
        public int? PackageTypeId { get; set; }
        public decimal? Cost { get; set; }
        public decimal? OrignalCost { get; set; }
        public DateTime CreationDate { get; set; }
        public string InvoiceSerial { get; set; }
        public bool IsCompleted { get; set; }       
        public string ? Notes { get; set; }
        public virtual ICollection<CustomerTransaction> CustomersTransactions { get; set; }
        public virtual ICollection<CourseCustomerMapping> CourseCustomerMapping { get; set; }
     
       // public virtual ICollection<ExternalCustomersTransactions> ExternalCustomersTransactions { get; set; }
       // public virtual Package Package { get; set; }
       // public virtual CustomerAttribute Customer { get; set; }
    }
}
