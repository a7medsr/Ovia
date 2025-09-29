using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CourseTag:Entity
    {
        public CourseTag()
        {
            CourseCourseTagMapping = new HashSet<CourseCourseTagMapping>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
      

        public virtual ICollection<CourseCourseTagMapping> CourseCourseTagMapping { get; set; }
    }

    public class Tagcoures
    {
        public int Id { get; set; }
        public int couresId { get; set; }
        public Course coures { get; set; }

        public int couresTagId { get; set; }
        public CourseTag couresTag { get; set; }
    }
}
