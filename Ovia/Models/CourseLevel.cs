using Ovia.Models;
using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CourseLevel: Entity
    {
        public CourseLevel()
        {
            Course = new HashSet<Course>();
        }

        public int Id { get; set; }
        public string LevelEn { get; set; }
        public string LevelAr { get; set; }
        public bool? Published { get; set; }
   

        public virtual ICollection<Course> Course { get; set; }
    }
}
