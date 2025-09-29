using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public partial class Talks:Entity
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string Picture { get; set; }
        public string Video { get; set; }
        public bool? Published { get; set; }
        public int? InstructorId { get; set; }
        public int DisplayOrder { get; set; }
        public string UriTalkName { get; set; }
        public decimal timeduration { get; set; }
        [Required]
        public int  CategoryId { get; set; }
        public Category Category { get; set; }

        public virtual Instructor Instructor { get; set; }
    }

    public class TagTalks
    {
        public int Id { get; set; }
        public int TalksId { get; set; }
        public Talks Talks { get; set; }

        public int TalktagId { get; set; }
        public Talktag Talktag { get; set; }
    }
}
