using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Video:Entity
    {
        public Video(){   VideoMapping = new HashSet<VideoMapping>(); }
        public int Id { get; set; }
        public string VideoName { get; set; }
        public string Url { get; set; }
        public string MimeType { get; set; }
        public bool Published { get; set; }
        public virtual ICollection<VideoMapping> VideoMapping { get; set; }
    }
}
