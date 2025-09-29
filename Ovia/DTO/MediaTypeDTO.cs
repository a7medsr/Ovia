using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.DTO
{
     public class MediaTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public string Picture { get; set; }
        public string Mobile { get; set; }
        public List<PictureDTO> Pictures { get; set; }

    }
}
