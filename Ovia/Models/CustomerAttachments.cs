using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CustomerAttachments
    {
        public int Id { get; set; }
      //  public int? AttachedId { get; set; }
        public int? CustomerId { get; set; }
        //public string? AttachedPass { get; set; }
        public string? FrontKey { get; set; }
        public string? BackKey { get; set; }
        public DateTime? EntryDate { get; set; }
        //    public virtual ICollection<ComplainAttachments> ComplainAttachments { get; set; }



    }
}
