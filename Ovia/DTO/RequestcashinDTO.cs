using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
    public class RequestcashinDTO
    {
        public int  UserId { get; set; }
        public int RequestTypeId { get; set; }
        public int payedpy { get; set; }
        public DateTime requestdate { get; set; }
    }
}
