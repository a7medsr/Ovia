using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovia.DTO
{
    public class AddComplaintReplyDTO
    {
        public int ComplaintId { get; set; }
        public string? Reply { get; set; }
        public int? AdminId { get; set; }
        public IFormFile? File { get; set; }


    }
}
