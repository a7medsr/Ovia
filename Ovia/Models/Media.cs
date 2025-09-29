using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Media:Entity
    {
        public int Id { get; set; }
        public string MediaType { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string Address { get; set; }
        public string Picture { get; set; }
        public int? NumberOfSeats { get; set; }
        public string Mobile { get; set; }
        public int? DisplayOrder { get; set; }
      
    }
}
