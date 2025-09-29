using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovia.DTO
{
    public class AddComplaintDTO
    {
        public int UserId { get; set; }
        public string Subject { get; set; }
        public string Question { get; set; }
        public IFormFile? File { get; set; }

    }
}
