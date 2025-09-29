using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ovia.Models
{
    public class UserImg
    {
        [Key]
        public int Id { get; set; }
      
        [ForeignKey("CustomerInfo")]
        public int UserId { get; set; }
        public string? Key { get; set; } = null!;
        public string? FileName { get; set; }
        public string? Extension { get; set; }
        public long? FileSize { get; set; }

        [JsonIgnore]
        public virtual CustomerInfo CustomerInfo { get; set; }


    }
}
