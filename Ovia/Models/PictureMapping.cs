using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class PictureMapping:Entity
    {
        public int Id { get; set; }
        public int? PictureId { get; set; }
        public int? EntityId { get; set; }
        public string EntityType { get; set; }
 

        public virtual Picture Picture { get; set; }
    }
}
