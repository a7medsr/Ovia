using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class PointProcess:Entity
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? ForCustomerId { get; set; }
        public decimal? Value { get; set; }
        public int? ProcessTypeId { get; set; }
   
        public int? PackageId { get; set; }

        public virtual Course Course { get; set; }

       // public virtual ICollection<Profit> Profits { get; set; }

    }
}
