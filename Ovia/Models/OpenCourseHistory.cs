using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class OpenCourseHistory:Entity
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? InstructorId { get; set; }
        public int? CourseId { get; set; }
        public int? DisplayOrder { get; set; }
  

        public virtual Course Course { get; set; }
        public virtual Instructor Instructor { get; set; }
    }
}
