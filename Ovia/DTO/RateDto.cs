using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
  public  class RateDto
    {
        public double? AvrgRate { get; set; }
        public Dictionary<int?, int> GruopsRates { get; set; }

        public int NoOfRates { get; set; }

    }
    //public class StarsRate
    //{
    //    public int? Star { get; set; }
    //    public float? RateValue { get; set; }
    //}
}
