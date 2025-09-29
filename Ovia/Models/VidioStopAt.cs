using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.Models
{
    public  class VidioStopAt:Entity
    {
        public int Id { get; set; }
        public int userId  { get; set; }
        public int couresId { get; set; }
        public int couresdetailsId { get; set; }
        public string time { get; set; }

        public CustomerAttribute user { get; set; }
        public Course coures { get; set; }
        public CourseDetails couresdetails { get; set; }
    }
}
