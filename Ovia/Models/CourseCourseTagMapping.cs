using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CourseCourseTagMapping:Entity
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? CourseTagId { get; set; }
  

        public virtual Course Course { get; set; }
        public virtual CourseTag CourseTag { get; set; }
    }
}
