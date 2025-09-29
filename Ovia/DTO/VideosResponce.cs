using Ovia.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
    public class VideosResponce
    {
        public int Count { get; set; }
        public List<Datum> Rows { get; set; }
    }
    public class Datum
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public int Upload_time { get; set; }
        public bool Public { get; set; }
        public string Status { get; set; }
        public List<PosterDTO> Posters { get; set; }
       

    }


}
