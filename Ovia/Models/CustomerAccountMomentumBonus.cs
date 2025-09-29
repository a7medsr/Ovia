using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CustomerAccountMomentumBonus
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? Balance { get; set; }
        public string Description { get; set; }
        public bool? Used { get; set; }
    }
}
