using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovia.DTO
{
    public class ComplaintReplyDTO
    {
        public int ComplaintReplyId { get; set; }

        public int ComplaintId { get; set; }
        public string? Reply { get; set; }
        public int? AdminId { get; set; }
        public string? AdminName { get; set; }

        public DateTimeOffset CreateDate { get; set; }





    }

}
