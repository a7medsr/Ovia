using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Rank
    {
        public int Id { get; set; }
        public string Rank1 { get; set; }
        public decimal CheckPercentage { get; set; }
        public int SponsorFor { get; set; }
        public decimal CvOn2Sides { get; set; }
        public decimal MaxOut { get; set; }

        public virtual ICollection<CustomerAttribute> Customers { get; set; }
    }
}
