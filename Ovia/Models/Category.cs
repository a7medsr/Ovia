using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Category: Entity
    {
        public Category()
        {
            Course = new HashSet<Course>();
        }

        public int Id { get; set; }
        public string? NameAr { get; set; }
        public string NameEn { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? Picture { get; set; }
        public bool?  Published { get; set; }
        public bool?  Deleted { get; set; }
        public int? DisplayOrder { get; set; }
        public string? uricatigoryname { get; set; }
        public string? GuId { get; set; }
     

        public virtual ICollection<Course> Course { get; set; }
    }
}
