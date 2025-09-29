using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ovia.DTO
{
    public class CourseCommentDTO
    {

        public int userId { get; set; }

       public int CourseId { get; set; }
        [Range(1, 5)]

        public int Rate { get; set; }
        [Required]
        public string Comment { get; set; }

    }
}
