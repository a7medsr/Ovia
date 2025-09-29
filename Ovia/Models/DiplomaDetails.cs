using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public partial class DiplomaDetails
    {
        public int Id { get; set; }
        public int? DiplomaTypeId { get; set; }
        public int? CourseLocationId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDay { get; set; }
        public int? AvailableSeats { get; set; }
        public int? NumberOfGroups { get; set; }
        public string Note { get; set; }
        public int? AllowedGovId { get; set; }
       /// public virtual ICollection<CustomerDiploma> CustomerDiploma { get; set; }
       
        //[ForeignKey("DiplomaTypeId")]

       // public virtual DiplomaType DiplomaType { get; set; }
       // [ForeignKey("CourseLocationId")]

        //public virtual DiplomaLocation  DiplomaLocation { get; set; }

        public virtual ICollection<DiplomaInstructors>  DiplomaInstructors { get; set; }



    }
}
