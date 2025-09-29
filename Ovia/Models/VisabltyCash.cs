using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.Models
{
    public  class VisabltyCash:Entity
    {
        public int id { get; set; }
        public DateTime timefrom { get; set; }
        public DateTime timeto { get; set; }
        public string day { get; set; }
    }
}
