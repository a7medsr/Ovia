using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class BalanceActionHistory: Entity
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? CourseId { get; set; }
        public int? Amount { get; set; }
        public int? ActionTypeId { get; set; }
        public int? BalanceBefore { get; set; }
        public int? BalanceAfter { get; set; }
        public int? CustomerLoginId { get; set; }
        

        public virtual Course Course { get; set; }
        public virtual CustomerAttribute Customer { get; set; }
    }
}
