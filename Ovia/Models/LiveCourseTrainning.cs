using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ovia.Models
{
    public class LiveCourseTrainning : Entity
    {
        [Key]
        public int Id {get; set;}
        public int CourseId {get; set;}
        public int InstructorId { get; set; }
        public string LectureLiveName {get; set;}
        public string MeetingCode { get; set; }

        public DateTime Date {get; set;}
       // public string Url {get; set;}


        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }  
        
        [ForeignKey(nameof(InstructorId))]
        public virtual Instructor Instructor { get; set; }

    }
}
