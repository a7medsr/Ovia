using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Content: Entity
    {
        public int Id { get; set; }
        public string Section { get; set; }
        public string TextAr { get; set; }
        public string TextEn { get; set; }
        public string Picture { get; set; }
    }
}
