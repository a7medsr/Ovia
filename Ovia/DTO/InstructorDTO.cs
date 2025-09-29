using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
   public class InstructorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string About { get; set; }
        public string Photo { get; set; }
        public List<CourseDTO> Courses { get; set; }
        public int NumberOfCourses { get; set; }
        public int NumberOfTalks { get; set; }
        public int NumberOfStudent { get { return Students; } }
        public int Talks { get; set; }
        public int Students { get; set; }
        public int NumperOfViews { get; set; }
        public string uriinstractorname { get; set; }
        public string GuId { get; set; }
        public MetatagDTO metatag { get; set; }

    }

}
