using System;
using System.Collections.Generic;

namespace Ovia.Models
{ 
    public partial class Picture:Entity
    {
        public Picture()
        {
            PictureMapping = new HashSet<PictureMapping>();
        }

        public int Id { get; set; }
        public string PictureName { get; set; }
        public string Url { get; set; }
        public string MimeType { get; set; }
        public bool Published { get; set; }


        public virtual ICollection<PictureMapping> PictureMapping { get; set; }
    }
}
