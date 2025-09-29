using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class AboutContent: Entity
    {
        public int Id { get; set; }
        public string ShortDescriptionEn { get; set; }
        public string ShortDescriptionAr { get; set; }
        public string FullDescriptionEn { get; set; }
        public string FullDescriptionAr { get; set; }
        public string GoalEn { get; set; }
        public string GoalAr { get; set; }
        public string MissionEn { get; set; }
        public string MissionAr { get; set; }
        public string ValueEn { get; set; }
        public string ValueAr { get; set; }
        public string VisionEn { get; set; }
        public string VisionAr { get; set; }
        public string Video { get; set; }
     
    }
}
