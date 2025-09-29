using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{ 
    public class newcashbackmethodDTO
    {
        public int userid {  get; set; }   
        public int methodid { get; set; }
        public string cardNumber { get; set; }
        public IFormFile nationalIdfront { get; set; }
        public IFormFile nationalIdback { get; set; }
    }
}
