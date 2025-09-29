namespace Ovia.Models
{
    public class Entity
    {
        public DateTime? CreationDate { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public bool? IsActive { get; set; } = true;
        public bool? IsDeleted { get; set; } = false;
        public DateTime? LastUpdateDate { get; set; } = DateTime.Now;
        public string? Notes { get; set; }
    }
}
