using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{


    public class instractorDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        //public string nameAR{ get; set; }
        public string email{ get; set; }
        public string mobil{ get; set; }
        public string aboutEN{ get; set; }
        public string aboutAR{ get; set; }
        public string UriInstractorName{ get; set; }
        public IFormFile photo { get; set; }
        public string urlphoto { get; set; }
    }
}
