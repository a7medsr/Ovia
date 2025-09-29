using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.Models
{
    public class Distributors_Summit_Coins : Entity
    {
        public int Id { get; set; }
        public int? Distributors_ID { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? Transaction_Date { get; set; }
        public string Description { get; set; }
        public decimal? Summit_Cost { get; set; }
        public bool? Is_Used { get; set; }
    }
}
