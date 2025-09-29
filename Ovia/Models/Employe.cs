using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Employe:Entity
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string JobTitleAr { get; set; }
        public string JobTitleEn { get; set; }
        public string Picture { get; set; }

        public int? AdminTypeId { get; set; }
        public int? CustomerInfoId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual CustomerInfo CustomerInfo { get; set; }
    }
}
