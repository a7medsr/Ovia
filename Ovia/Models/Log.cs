using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Log:Entity
    {
        public int? Id { get; set; }
        public int? LogLevelId { get; set; }
        public string? ShortMessage { get; set; }
        public string? FullMessage { get; set; }
        public string? IpAddress { get; set; }
        public int? CustomerId { get; set; }
        public string? PageUrl { get; set; }


        public virtual CustomerAttribute Customer { get; set; }
    }
}
