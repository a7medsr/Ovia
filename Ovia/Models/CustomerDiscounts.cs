using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CustomerDiscounts
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public decimal? Credit { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? Balance { get; set; }
        public string? Description { get; set; }
    }
}
