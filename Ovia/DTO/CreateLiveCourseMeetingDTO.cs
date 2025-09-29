namespace Ovia.DTO
{
    public class CreateLiveCourseMeetingDTO
    {
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        public string LectureLiveName { get; set; }
        public DateTime Date { get; set; }
       // public string Url { get; set; }

    }
}
