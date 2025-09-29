using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ovia.Models
{
    public class PackageImage
    {


        [Key]
        public int Id { get; set; }

        [ForeignKey("Package")]
        public int PackageId { get; set; }
        public string? Key { get; set; } = null!;
        public string? FileName { get; set; }
        public string? Extension { get; set; }
        public long? FileSize { get; set; }


        [JsonIgnore]
        public virtual Package Package { get; set; }


    }
}
