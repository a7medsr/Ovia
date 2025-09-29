using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CustomerPointMapping:Entity
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? PointProcessId { get; set; }
        public bool? IsBooker { get; set; }
      
    }
}
