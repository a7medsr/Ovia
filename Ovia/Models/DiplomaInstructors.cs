using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public partial class DiplomaInstructors
    {
        public int Id { get; set; }
        public int? DiplomaDetailsId { get; set; }
        public int? InstructorId { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? EntryBy { get; set; }
        [ForeignKey("InstructorId")]

        public virtual Instructor Instructor { get; set; }
        [ForeignKey("DiplomaDetailsId")]

        public virtual DiplomaDetails DiplomaDetails { get; set; }

    }
}
