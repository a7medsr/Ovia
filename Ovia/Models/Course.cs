using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class Course:Entity
    {
       public Course()
        {
            BalanceActionHistory = new HashSet<BalanceActionHistory>();
            CourseCourseTagMappings = new HashSet<CourseCourseTagMapping>();
            CourseCustomerMapping = new HashSet<CourseCustomerMapping>();
            CourseDetails = new HashSet<CourseDetails>();
           CourseInstructorMapping = new HashSet<CourseInstructorMapping>();
            CoursePackageMapping = new HashSet<CoursePackageMapping>();
            OpenCourseHistory = new HashSet<OpenCourseHistory>();
            PointProcess = new HashSet<PointProcess>();
        }

        public int Id { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? ShortDescriptionEn { get; set; }
        public string? ShortDescriptionAr { get; set; }
        public string? FullDescriptionEn { get; set; }
        public string? FullDescriptionAr { get; set; }
        public int? CategoryId { get; set; }
        public int? CourseTypeId { get; set; }
        public int? LanguageId { get; set; }
        public int? TotalHour { get; set; }
        public int? LevelId { get; set; }
        public int? NumberLecture { get; set; }
        public string? Key { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? CourseCost { get; set; }
        public  int?   DisplayOrder { get; set; }
        public bool? Published { get; set; }
        public bool? Deleted { get; set; }
        public decimal? CostPoint { get; set; }
        public decimal? ProfitPoint { get; set; }
        public decimal? SponsorPoint { get; set; }
        public decimal? ManagementPoint { get; set; }
        public decimal? ContractCost { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? SummitCoin { get; set; }
        public decimal? SummitPoint { get; set; }
        public decimal? SummitCost { get; set; }
        public bool? DisableEnrollButton { get; set; }
        public bool? DisableWishlistButton { get; set; }
        public bool? AllowCustomerReviews { get; set; }
        public string?   Requirement { get; set; }
     
        public string? OnlineTag { get; set; }
        public int? DurationBySecond { get; set; }
        public string? Promo { get; set; }
        public bool? ShowOnHomePage { get; set; }
        public bool? IsSection { get; set; }
        public int? NoOfSection { get; set; }
        public string? uricouresname { get; set; }
        public string? GuId { get; set; }



        public virtual Category Category { get; set; }
        public virtual CourseType CourseType { get; set; }
       public virtual Language Language { get; set; }
        public virtual CourseLevel Level { get; set; }
       // public virtual ICollection<CustomerDiploma> CustomerDiploma { get; set; }

        public virtual ICollection<BalanceActionHistory> BalanceActionHistory { get; set; }
        public virtual ICollection<CourseCourseTagMapping> CourseCourseTagMappings { get; set; }
        public virtual ICollection<CourseCustomerMapping> CourseCustomerMapping { get; set; }
        public virtual ICollection<CourseDetails> CourseDetails { get; set; }
        public virtual ICollection<CourseInstructorMapping> CourseInstructorMapping { get; set; }
        public virtual ICollection<CoursePackageMapping> CoursePackageMapping { get; set; }
        public virtual ICollection<OpenCourseHistory> OpenCourseHistory { get; set; }
        public virtual ICollection<PointProcess> PointProcess { get; set; }
        public virtual ICollection<AboutCoures> AboutCoures { get; set; }
    }
}
