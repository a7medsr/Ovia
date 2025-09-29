using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovia.Models
{
    public class ComplaintReply : Entity
    {
      



        [Key]
        public int Id { get; set; }

        [Required]
        public int ComplaintId { get; set; }

        public string? Reply { get; set; }

        public int AdminId { get; set; }

        [ForeignKey(nameof(ComplaintId))]
        public virtual Complaint Complaint { get; set; }

        [ForeignKey(nameof(AdminId))]
        public virtual CustomerAttribute CustomerAttribute { get; set; }

    }
}
