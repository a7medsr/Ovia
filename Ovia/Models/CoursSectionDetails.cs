using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CoursSectionDetails :Entity 
    {
        public int Id { get; set; }
        public string SectionNameAr { get; set; }
        public string SectionNameEn { get; set; }
        public string DescAr { get; set; }
        public string DescEn { get; set; }
        public int SectionNo { get; set; }
        public int Coursid { get; set; }

      

    }
}
