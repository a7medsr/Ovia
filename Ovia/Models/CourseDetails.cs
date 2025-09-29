using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ovia.Models

{
    public partial class CourseDetails: Entity
    {
        public CourseDetails()
        {
           OnlineAttendance = new HashSet<OnlineAttendance>();
        }

        public int Id { get; set; }
        public string? NameAr { get; set; }
        public string NameEn { get; set; }
        public int? CourseId { get; set; }
        public string? Url { get; set; }
        public string? Picture { get; set; }
        public string OnlineId { get; set; }
        public string OnlineTag { get; set; }
        public double? Duration { get; set; }
        [MaxLength(350)]
        public string? PdfUrl { get; set; }
        public bool? IsPdf { get; set; }
        public int? SectionNo { get; set; }
        public bool? IsFree { get; set; }
        


        public virtual Course Course { get; set; }
        public virtual ICollection<OnlineAttendance> OnlineAttendance { get; set; }
    }
}
