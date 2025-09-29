using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class CustomerCustomerRoleMapping:Entity
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerRoleId { get; set; }
   

        public virtual CustomerAttribute Customer { get; set; }
    }
}
