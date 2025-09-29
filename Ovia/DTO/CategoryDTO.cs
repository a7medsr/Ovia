using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
    public class CategoryDTO : BasicDataDTO
    {
        public List<CourseDTO> Courses { get; set; }
        public string uricatigoryname { get; set; }
        public string GuId { get; set; }
    }
}
