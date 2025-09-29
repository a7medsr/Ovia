using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
    public  class DirectcomitionDto
    {

        public int? CustomerId { get; set; }
        public int? ForCustomerId { get; set; }
        public int? ProcessTypeId { get; set; }
        public int ?Value { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsBooker { get; set; }
        public string? Process { get; set; }
        public string? NameEn { get; set; }
        public string? mobile { get; set; }
        public decimal? profit { get; set; }
    }
}
