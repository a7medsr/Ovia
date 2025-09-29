using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.Models
{
    public class DistributorsAddonCoins : Entity
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Description { get; set; }
        public string NsTransactionId { get; set; }
    }
}
