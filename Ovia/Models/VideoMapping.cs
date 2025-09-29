using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class VideoMapping:Entity
    {
        public int Id { get; set; }
        public int? VideoId { get; set; }
        public int? EntityId { get; set; }
        public string EntityType { get; set; }


        public virtual Video Video { get; set; }
    }
}
