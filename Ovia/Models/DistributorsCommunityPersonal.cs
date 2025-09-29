using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class DistributorsCommunityPersonal
    {
        public int Id { get; set; }
        public int? DistributorsId { get; set; }
        public int? CommunityNo { get; set; }
        public int? PersonalNo { get; set; }
    }
}
