using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class OnlineAttendance:Entity
    {
        public int Id { get; set; }
        public int CourseDetailsId { get; set; }
        public int CustomerId { get; set; }
    

        public virtual CourseDetails CourseDetails { get; set; }
        public virtual CustomerAttribute Customer { get; set; }
    }
}
