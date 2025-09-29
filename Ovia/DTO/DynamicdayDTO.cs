using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
   public  class DynamicdayDTO
    {
        
        public string dayfrom { get; set; }
        public string dayto { get; set; }
        public DateTime? CreationDate { get; set; } = DateTime.Now;
        public bool? IsActive { get; set; } = true;
        public bool? IsDeleted { get; set; } = false;
        public DateTime? LastUpdateDate { get; set; } = DateTime.Now;
        public string? Notes { get; set; }

    }
}
