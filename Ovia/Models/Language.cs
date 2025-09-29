using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Language:Entity
    {
        public Language()
        {
            Course = new HashSet<Course>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueSeoCode { get; set; }
        public string FlagImageFileName { get; set; }
        public bool? Published { get; set; }
        public int? DisplayOrder { get; set; }
 

        public virtual ICollection<Course> Course { get; set; }
    }
}
