using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace  Ovia.DTO
{
    public class TalkDTO
    {
        public TalkDTO()
        {
            Relatedtalks = new List<TalkDTO>();
        }

        public int Id { get; set; }
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public string Picture { get; set; }
        public IFormFile Pictureurl { get; set; }
        public string Video { get; set; }
        public int  IntractorId { get; set; }
        public string DescriptionAR { get; set; }
        public string DescriptionEN { get; set; }
        public string uritalkname { get; set; }
        public int DisplayOrder { get; set; }
        public int CategoryId { get; set; }
        public int watch { get; set; }
        public decimal timeduration { get; set; }
        public MetatagDTO metatag { get; set; }
        public CategoryDTO category { get; set; }
        public instractorDTO instractor { get; set; }
        public List<TalkDTO> Relatedtalks { get; set; }
    }
}
