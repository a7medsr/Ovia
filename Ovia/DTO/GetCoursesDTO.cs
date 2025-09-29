namespace Ovia.DTO
{
    public class GetCoursesDTO
    {
        public int Id { get; set; }
        public string? NameEn { get; set; }
        public string? ShortDescriptionEn { get; set; }
        public string? FullDescriptionEn { get; set; }
        public int? CourseTypeId { get; set; }
        public int? TotalHour { get; set; }
        public bool? IsActive { get; set; }
        public int? NumberLecture { get; set; }
        public string? URL { get; set; }
        public decimal? Price { get; set; }
    }
}
