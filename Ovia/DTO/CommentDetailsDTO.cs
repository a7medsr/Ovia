using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
     public class CommentDetailsDTO
    {

        public int Rate { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string By { get; set; }
        public string Avatar { get; set; }
    }
}
