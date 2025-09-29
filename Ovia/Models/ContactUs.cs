using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class ContactUs:Entity
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
     
    }
}
