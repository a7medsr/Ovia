using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
    public class PackageDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string FullDesc { get; set; }

        public string Picture { get; set; }
        public string Type { get; set; }
        public int DisplayOrder { get; set; }
        public decimal Price { get; set; }
        public int CoursesNumber { get; set; }
        public List<CourseDTO> Courses { get; set; }


    }
}
