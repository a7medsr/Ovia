using System;
using System.Collections.Generic;

namespace Ovia.DTO
{
    public class CourseDTO : BasicDataDTO
    {
        public CourseDTO()
        {
            sectionCourseD = new List<SectionCourseDTO>();
            Instructors = new List<InstructorDTO>();
            Aboutcourse = new List<AboutcourseDTO>();
            Relatedcoures = new List<CourseDTO>();
        }

        public int? CourseId { get; set; }
        public int? CourseTypeId { get; set; }
        public BasicDataDTO? Category { get; set; }
        public string? CategoryName { get; set; }
        public string[]? Tags { get; set; }
        public string? Photo { get; set; }
        public int? TotalHouers { get; set; }
        public int? NoOfVideos { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string[]? InstractorName { get; set; }
        public string? FullDesc { get; set; }
        public bool? IsEnroll { get; set; }
        public string? CreationDate { get; set; }
        public List<InstructorDTO>? Instructors { get; set; }
        public List<CommentDetailsDTO>? Comments { get; set; }
        public string? Language { get; set; }
        public float? WatchPercentage { get; set; }
        public int? DurationBySecond { get; set; }
        public int? displayorder { get; set; }
        public string? Promo { get; set; }
        public string? pdfurl { get; set; }
        public CertificateDTO? Certificate { get; set; }
        public string? Rate { get; set; }
        public int? numofuserRate { get; set; }
        public int? numofsections { get; set; }
        public bool? IsRated { get; set; }
        public string? uricouresname { get; set; }
        public string? GuId { get; set; }
        public MetatagDTO? metatag { get; set; }
        // public CourseLevelDTO CourseLevel { get; set; }
        public string? levelname { get; set; }
        public string? urlname { get; set; }
        public List<SectionCourseDTO>? sectionCourseD { get; set; }
        public List<AboutcourseDTO>? Aboutcourse { get; set; }
        public List<CourseDTO>? Relatedcoures { get; set; }
        public object? Relatedpath { get; set; }
        public List<CourseDetailsDTO>? CourseDetail { get; set; }
        public object? litRatecount { get; set; }
    }

    public class CertificateDTO
    {
        public string? CertificateURL { get; set; }
        public string? CertificateId { get; set; }
        public DateTime? CertificateDate { get; set; }
    }
}




//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;

//namespace Ovia.DTO
//{
//    public class CourseDTO : BasicDataDTO
//    {

//        public CourseDTO()
//        {
//            sectionCourseD = new List<SectionCourseDTO>();
//            Instructors = new List<InstructorDTO>();
//            Aboutcourse = new List<AboutcourseDTO>();
//            Relatedcoures = new List<CourseDTO>();
//            //litRatecount = new List<ratestarcountDTO>();
//        }

//        public int CourseId { get; set; }
//        public int CourseTypeId { get; set; }
//        public BasicDataDTO Category { get; set; }
//        public string CategoryName { get; set; }
//        public string[] Tags { get; set; }
//        public string Photo { get; set; }
//        public int TotalHouers { get; set; }
//        public int NoOfVideos { get; set; }
//        public decimal Price { get; set; } 
//        public decimal OldPrice { get; set; }
//        public string[] InstractorName { get; set; }
//        public string FullDesc { get; set; }
//        public bool IsEnroll { get; set; }
//        public string CreationDate { get; set; }
//        public List<InstructorDTO> Instructors { get; set; }
//        public List<CommentDetailsDTO> Comments { get; set; }
//        public string Language { get; set; }
//        public float WatchPercentage { get; set; }
//        public int DurationBySecond { get; set; }
//        public int displayorder { get; set; }
//        public string Promo { get; set; }
//        public string pdfurl { get; set; }
//        public CertificateDTO Certificate{ get; set; }
//        public string Rate { get; set; }
//        public int numofuserRate { get; set; }
//        public int numofsections { get; set; }
//        public bool IsRated { get; set; }
//        public string uricouresname { get; set; }
//        public string GuId { get; set; }
//        public MetatagDTO metatag { get; set; }
//       // public CourseLevelDTO CourseLevel { get; set; }
//        public string levelname { get; set; }
//        public string urlname { get; set; }
//        public List< SectionCourseDTO> sectionCourseD { get; set; }
//        public List< AboutcourseDTO> Aboutcourse { get; set; }
//        public List<CourseDTO> Relatedcoures { get; set; }
//        public object Relatedpath { get; set; }
//        public List<CourseDetailsDTO> CourseDetail { get; set; }

//        public object litRatecount { get; set; }
//    }
//    public class CertificateDTO
//    {
//        public string CertificateURL { get; set; }
//        public string CertificateId { get; set; }
//        public DateTime? CertificateDate { get; set; }
//    }
//    //public class CourseLevelDTO
//    //{
//    //    public string levelname { get; set; }
//    //}
//}
