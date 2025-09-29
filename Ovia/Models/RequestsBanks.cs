using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class RequestsBanks
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string SwiftCode { get; set; }
        public bool? IsActve { get; set; }
    }
}
