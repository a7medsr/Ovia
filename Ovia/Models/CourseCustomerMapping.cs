using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public partial class CourseCustomerMapping : Entity
    {
        public CourseCustomerMapping()
        {
            CourseComment = new HashSet<CourseComment>();
        }

        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? CustomerId { get; set; }
        public string? Certificate { get; set; }
        public string? CertificateId { get; set; }
        public DateTime? CertificateDate { get; set; }

        public decimal? Cost { get; set; }
        public int? PackageSelectId { get; set; }

        public virtual Course Course { get; set; }
        public virtual CustomerAttribute Customer { get; set; }
        [ForeignKey("PackageSelectId")]
        public virtual CustomerPackageSelect Package { get; set; }
        public virtual ICollection<CourseComment> CourseComment { get; set; }
    }
}
