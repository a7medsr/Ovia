using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.Models
{
    public class Allpathviewsql
    {
        //public Allpathviewsql()
        //{
        //    price = decimal.Parse(price.ToString("0.00"));
        //}

        public int Id { get; set; }
        public string photo { get; set; }
        public string uripackagname { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal oldprice { get; set; }
        public int Noofcourse { get; set; }
        public int DurationBySecond { get; set; }
        public int countrate { get; set; }
        public decimal rate { get; set; }
        public int lessonsnumber { get; set; }
        public string categoryName { get; set; }
        public int? ordring { get; set; }


    }
}
