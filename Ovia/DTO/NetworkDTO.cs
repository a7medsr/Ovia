using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
    public class NetworkDTO
    {
        public int ChildId { get; set; }
        public string ChildName { get; set; }
        public string ChildEmail { get; set; }

        public string HandSide { get; set; }
        public string referrid { get; set; }
        public string rank { get; set; }
        public string mobile { get; set; }

       // public List<NetworkDTO>? Children { get; set; } // Include property for children


    }
}
