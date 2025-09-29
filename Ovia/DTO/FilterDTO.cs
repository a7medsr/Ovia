using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
    public  class FilterDTO
    {
        public int courstypeid { get; set; }
        public List<int>? categoryid { get; set; }
        public string? duration { get; set; }
        public List<int>? langid { get; set; }
        public string? ordaring { get; set; }
         
    }
}
