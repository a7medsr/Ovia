using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ovia.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        [ForeignKey(nameof(CustomerAttribute))]
        public int CustomerId { get; set; } 
        public DateTime UploadDate { get; set; }


        [JsonIgnore]
        public virtual CustomerAttribute CustomerAttribute { get; set; }  


    }
}
