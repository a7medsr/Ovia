using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CustomerRequestsData:Entity
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
        public string OldMobile { get; set; }
        public string NewMobile { get; set; }
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public DateTime? RequestDate { get; set; }
        public bool? IsConfirmed { get; set; }
        public int? Action_By { get; set; }
        public DateTime? Action_Date { get; set; }

        
        public CustomerInfo Customer { get; set; }



    }
}
