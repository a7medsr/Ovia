using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public partial class CourseComment:Entity
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? CourseId { get; set; }
        public int? Rate { get; set; }
        public string Comment { get; set; }
        public bool? Approved { get; set; }
      
       
        [ForeignKey("Id")]
        public virtual CourseCustomerMapping CourseCustomerMapping { get; set; }
        public virtual CustomerAttribute Customer { get; set; }
    }
}
