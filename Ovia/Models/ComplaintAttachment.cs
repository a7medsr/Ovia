using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovia.Models
{
    public class ComplaintAttachment : Entity
    {
      

        [Key]
        public int Id { get; set; }

        [Required]
        public int ComplaintId { get; set; }

        public string? FileKey { get; set; }

        public string? FileExtension { get; set; }

        public string? FileName { get; set; }

        public long? FileSize { get; set; }

        [ForeignKey(nameof(ComplaintId))]
        public virtual Complaint Complaint { get; set; }






    }
}
