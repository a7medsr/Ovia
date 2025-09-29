using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
   public class VideoDTO:BasicDataDTO
    {
        public int TotalTimeBySec { get; set; }
        public string GuidId { get; set; }
        public string pdfurl { get; set; }
        public PosterDTO[] Posters { get; set; }
        public bool IsWatched { get; set; }
        
        
    }
}
