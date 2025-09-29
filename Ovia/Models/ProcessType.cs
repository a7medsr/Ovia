using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class ProcessType
    {
        public int Id { get; set; }
        public string Process { get; set; }
        public bool? IsActive { get; set; }
    }
}
