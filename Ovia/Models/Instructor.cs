using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Instructor:Entity
    {
        public Instructor()
        {
            CourseInstructorMapping = new HashSet<CourseInstructorMapping>();
            OpenCourseHistory = new HashSet<OpenCourseHistory>();
            Talks = new HashSet<Talks>();
        }
        
        public int Id { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? JobTitleAr { get; set; }
        public string? JobTitleEn { get; set; }
        public int? CourseTypeId { get; set; }
        public string? Email { get; set; }
        public string? Picture { get; set; }
        public string? Address { get; set; }
        public string? Mobile { get; set; }
        public string? AboutAr { get; set; }
        public string? AboutEn { get; set; }
        public int? DisplayOrder { get; set; }
        public int? InsInfoID { get; set; }
        public string? uriinstractorname { get; set; }
        public string?   GuId { get; set; }

        public virtual CourseType CourseType { get; set; }
        public virtual ICollection<CourseInstructorMapping> CourseInstructorMapping { get; set; }
        public virtual ICollection<OpenCourseHistory> OpenCourseHistory { get; set; }
        public virtual ICollection<Talks> Talks { get; set; }
        public virtual ICollection<DiplomaInstructors> DiplomaInstructors { get; set; }
    }
}
