using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CourseType: Entity
    {
        public CourseType()
        {
            Course = new HashSet<Course>();
            Instructor = new HashSet<Instructor>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public bool? Published { get; set; }


        public virtual ICollection<Course> Course { get; set; }
        public virtual ICollection<Instructor> Instructor { get; set; }
    }
}
