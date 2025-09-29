using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
    public class SectionCourseDTO
    {
        public SectionCourseDTO()
        {
            Data = new List<VideoDTO>();
        }

        
        public int? SectionNo { get; set; }
        //dscds  hhh
        public string DetailsName { get; set; }
        public string setionnameName { get; set; }
        public string description { get; set; }
      
        public List<VideoDTO> Data { get; set; }

       
    }
}
