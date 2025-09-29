using Ovia.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
    public class AboutcourseDTO
    {
       

        public int Id { get; set; }
        public int CourseId { get; set; }
       
        public int MapdiscreptiioncouresId { get; set; }
        public Mapdiscreptiioncoures Mapdiscreptiioncoures { get; set; }
        public string? DiscriptionAR { get; set; }
        public string? DiscriptionEN { get; set; }
        public string? photourl { get; set; }
    }
}
