using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Profit
    {
        public int Id { get; set; }
        public int? DistributorId { get; set; }
        public decimal? Profit1 { get; set; }
        public DateTime? ProfitDate { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? ProcessTypeId { get; set; }
        public int? PointProcessId { get; set; }
        public virtual PointProcess PointProcess { get; set; }
      
    }
}
