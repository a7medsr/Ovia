using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class RequestCash:Entity
    {
        public int Id { get; set; }
        public int? RequestTypeId { get; set; }
        public int? CustomerId { get; set; }
        public bool? IsPaid { get; set; }
        public int? PaidBy { get; set; }
        public DateTime?  PaidDate { get; set; }
        public DateTime?  RequestDate { get; set; }
        public decimal? RequestedAmount { get; set; }
        public bool? UnPaid { get; set; } = false;
        public DateTime? UnPaidDate { get; set; }

        public virtual CustomerAttribute Customer { get; set; }
    }
}
