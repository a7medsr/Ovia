using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Points
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? PointProcessId { get; set; }
        public int? PointCount { get; set; }
        public string Side { get; set; }
        public int? ProcessTypeId { get; set; }
        public DateTime? PointDate { get; set; }
        public bool? IsCalculated { get; set; }
        public DateTime? CalculationDate { get; set; }
        public bool? IsFlashed { get; set; }
        public DateTime? FlashDate { get; set; }
    }
}
