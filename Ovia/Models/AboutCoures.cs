using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ovia.Models
{
    public class AboutCoures:Entity
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int MapdiscreptiioncouresId { get; set; }
        public string? DiscriptionAR { get; set; }
        public string? DiscriptionEN { get; set; }
        public string? photourl { get; set; }

        public Course course { get; set; }
       
        public Mapdiscreptiioncoures Mapdiscreptiioncoure { get; set; }
        [NotMapped]
        public virtual ICollection<Mapdiscreptiioncoures> mapdiscreptiioncoures { get; set; }

      
    }
}
