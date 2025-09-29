namespace Ovia.DTO
{
    public class UpdateCourseDTO
    {
        public int CourseId { get; set; }   
        public string? NameEn { get; set; }
        public string? ShortDescriptionEn { get; set; }
        public string? FullDescriptionEn { get; set; }
        public int? TotalHour { get; set; }
        public decimal? Price { get; set; }
        public int? NumberLecture { get; set; }
        public IFormFile? Img { get; set; }
    }
}
