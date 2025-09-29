using System.ComponentModel.DataAnnotations;

namespace Ovia.Models
{
    public class HomePhotos : Entity
    {
        [Key]
        public int Id { get; set; }
        public string? Key { get; set; }
        public string? FileName { get; set; }
        public string? Extension { get; set; }
        public decimal? Size { get; set; }
        public string Description { get; set; }

        public bool ShowInHome { get; set; }



    }
}
