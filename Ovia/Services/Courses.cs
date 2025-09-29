//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Ovia.DTO;
//using Ovia.Models;
//using Ovia.Services.
//Files;
//using RestSharp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Ovia.Services
//{
//    public class Courses
//    {
//        MomEntity Context = new MomEntity();
//        private readonly IStorageService storage;
//        //public List<CourseDTO> GetMyCoruses(int UserId, string lang)
//        //{

//        //    List<CourseDTO> Courses;
//        //    switch (lang.ToLower())
//        //    {
//        //        case "ar":
//        //            Courses = Context.CourseCustomerMapping.Where(c => c.CustomerId == UserId && c.IsActive == true && c.IsDeleted != true && c.Course.IsActive == true && c.Course.IsDeleted != true).Select(s => new CourseDTO
//        //            {
//        //                Category = new BasicDataDTO { Id = s.Course.Category.Id, Name = s.Course.Category.NameAr },
//        //                Photo = s.Course.Key.SetDownloadFileUrlByKey(storage),
//        //                Desc = s.Course.FullDescriptionAr,
//        //                Id = s.Course.Id,
//        //                Name = s.Course.NameAr,
//        //                NoOfVideos = s.Course.NumberLecture.GetValueOrDefault(),
//        //                Price = (decimal)s.Course.Price,
//        //                Tags = s.Course.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//        //                TotalHouers = s.Course.TotalHour.GetValueOrDefault(),
//        //                WatchPercentage = (float)System.Math.Round((s.Course.CourseDetails.SelectMany(ss => ss.OnlineAttendance.Where(c => c.CustomerId == UserId).Select(d => d.CourseDetails)).Sum(a => (decimal)a.Duration) * 100 / (decimal)s.Course.DurationBySecond), 2)
//        //            }).ToList();
//        //            return Courses;

//        //        default:
//        //            Courses = Context.CourseCustomerMapping.Where(c => c.CustomerId == UserId && c.IsActive == true && c.IsDeleted != true).Select(s => new CourseDTO
//        //            {
//        //                Category = new BasicDataDTO { Id = s.Course.Category.Id, Name = s.Course.Category.NameEn },
//        //                Photo = s.Course.Key.SetDownloadFileUrlByKey(storage),
//        //                Desc = s.Course.FullDescriptionEn,
//        //                Id = s.Course.Id,
//        //                Name = s.Course.NameEn,
//        //                NoOfVideos = s.Course.NumberLecture.GetValueOrDefault(),
//        //                Price = (decimal)s.Course.Price,
//        //                Tags = s.Course.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//        //                TotalHouers = s.Course.TotalHour.GetValueOrDefault(),

//        //                WatchPercentage = (float)System.Math.Round((s.Course.CourseDetails.SelectMany(ss => ss.OnlineAttendance.Where(c => c.CustomerId == UserId).Select(d => d.CourseDetails)).Sum(a => (decimal)a.Duration) * 100 / (decimal)s.Course.DurationBySecond), 2)

//        //            }).ToList();
//        //            return Courses;
//        //    }
//        //}

//        //public RateDto GetCoruseRates(int courseId, string lang)
//        //{
//        //    var CourseCommentes = Context.CourseComment
//        //        .Where(c => c.IsActive == true && c.IsDeleted != true &&
//        //        c.CourseId == courseId).ToList();
//        //    return new RateDto()
//        //    {
//        //        AvrgRate = CourseCommentes.Average(a => a.Rate),
//        //        GruopsRates = CourseCommentes.GroupBy(g => g.Rate).ToDictionary(d => d.Key, d => d.Count()),
//        //        NoOfRates = CourseCommentes.Count
//        //    };
//        //}

//        //public List<CourseDTO> GetAllCourses(
//        //    bool? showInHomePage, bool? latest, string lang, int limit, int page, long customerId)
//        //{
//        //    List<CourseDTO> Courses;
//        //    IQueryable<Course> filter;
//        //    if (showInHomePage == true)
//        //        filter = Context.Courses.Where(c => c.Published == true && c.CourseTypeId == 3 && c.ShowOnHomePage == true && c.IsDeleted != true && c.IsActive == true);
//        //    else
//        //        filter = Context.Courses.Where(c => c.Published == true && c.CourseTypeId == 3 && c.IsDeleted != true && c.IsActive == true);
//        //    if (latest == true)
//        //        filter = filter.Skip(limit * (page - 1)).Take(limit)
//        //                .OrderByDescending(c => c.Id);

//        //    switch (lang.ToLower())
//        //    {
//        //        case "ar":

//        //            Courses = filter.Select(s => new CourseDTO
//        //            {

//        //                Desc = s.ShortDescriptionAr,
//        //                Id = s.Id,
//        //                Name = s.NameAr,
//        //                NoOfVideos = s.NumberLecture.GetValueOrDefault(),
//        //                Price = (decimal)s.Price,
//        //                Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//        //                TotalHouers = s.TotalHour.GetValueOrDefault(),
//        //                DurationBySecond = s.DurationBySecond.GetValueOrDefault(),
//        //                FullDesc = s.FullDescriptionAr,
//        //                InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameAr).ToArray(),
//        //                Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameAr },
//        //                Language = s.Language.Name,
//        //                Photo = s.Key.SetDownloadFileUrlByKey(storage),
//        //                Promo = s.Promo,
//        //                IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == customerId),
//        //                Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
//        //                {
//        //                    Id = s1.Instructor.Id,
//        //                    Name = s1.Instructor.NameAr,
//        //                    JobTitle = s1.Instructor.JobTitleAr,
//        //                    About = s1.Instructor.AboutAr,
//        //                    Photo = s1.Instructor.Picture
//        //                }).ToList()

//        //            }).ToList();

//        //            return Courses;
//        //        default:
//        //            Courses = filter

//        //                .Select(s => new CourseDTO
//        //                {

//        //                    Desc = s.ShortDescriptionEn,
//        //                    Id = s.Id,
//        //                    Name = s.NameEn,
//        //                    NoOfVideos = s.NumberLecture.GetValueOrDefault(),
//        //                    Price = (decimal)s.Price,
//        //                    Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//        //                    TotalHouers = s.TotalHour.GetValueOrDefault(),
//        //                    DurationBySecond = s.DurationBySecond.GetValueOrDefault(),
//        //                    FullDesc = s.FullDescriptionEn,
//        //                    InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray(),
//        //                    IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == customerId),
//        //                    Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameEn },
//        //                    Language = s.Language.Name,
//        //                    Photo = s.Key.SetDownloadFileUrlByKey(storage),
//        //                    Promo = s.Promo,
//        //                    Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
//        //                    {
//        //                        Id = s1.Instructor.Id,
//        //                        Name = s1.Instructor.NameEn,
//        //                        JobTitle = s1.Instructor.JobTitleEn,
//        //                        About = s1.Instructor.AboutEn,
//        //                        Photo = s1.Instructor.Picture
//        //                    }).ToList()

//        //                }).ToList();
//        //            return Courses;
//        //    }

//            //}
//            //public List<CourseDTO> GetCoursesPerPage(bool? showInHomePage,
//            //    bool? latest, string lang, int limit, int page, long customerId)
//            //{
//            //    List<CourseDTO> Courses;
//            //    IQueryable<Course> filter;
//            //    if (showInHomePage == true)
//            //        filter = Context.Courses.Where(c => c.Published == true && c.CourseTypeId == 3 && c.ShowOnHomePage == true && c.IsDeleted != true && c.IsActive == true);
//            //    else
//            //        filter = Context.Courses.Where(c => c.Published == true && c.CourseTypeId == 3 && c.IsDeleted != true && c.IsActive == true);
//            //    if (latest == true)
//            //        filter = filter.OrderByDescending(c => c.Id);

//            //    filter = filter.Skip(limit * (page - 1)).Take(limit);
//            //    switch (lang.ToLower())
//            //    {
//            //        case "ar":

//            //            Courses = filter.Select(s => new CourseDTO
//            //            {

//            //                Desc = s.ShortDescriptionAr,
//            //                Id = s.Id,
//            //                Name = s.NameAr,
//            //                NoOfVideos = s.NumberLecture.GetValueOrDefault(),
//            //                Price = (decimal)s.Price,
//            //                Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//            //                TotalHouers = s.TotalHour.GetValueOrDefault(),
//            //                FullDesc = s.FullDescriptionAr,
//            //                InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameAr).ToArray(),
//            //                Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameAr },
//            //                Language = s.Language.Name,
//            //                Photo = s.Key.SetDownloadFileUrlByKey(storage),
//            //                IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == customerId),
//            //                Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
//            //                {
//            //                    Id = s1.Instructor.Id,
//            //                    Name = s1.Instructor.NameAr,
//            //                    JobTitle = s1.Instructor.JobTitleAr,
//            //                    About = s1.Instructor.AboutAr,
//            //                    Photo = s1.Instructor.Picture
//            //                }).ToList()

//            //            }).ToList();

//            //            return Courses;
//            //        default:
//            //            Courses = filter

//            //                .Select(s => new CourseDTO
//            //                {

//            //                    Desc = s.ShortDescriptionEn,
//            //                    Id = s.Id,
//            //                    Name = s.NameEn,
//            //                    NoOfVideos = s.NumberLecture.GetValueOrDefault(),
//            //                    Price = (decimal)s.Price,
//            //                    Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//            //                    TotalHouers = s.TotalHour.GetValueOrDefault(),
//            //                    FullDesc = s.FullDescriptionEn,
//            //                    InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray(),
//            //                    IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == customerId),
//            //                    Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameEn },
//            //                    Language = s.Language.Name,
//            //                    Photo = s.Key.SetDownloadFileUrlByKey(storage),
//            //                    Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
//            //                    {
//            //                        Id = s1.Instructor.Id,
//            //                        Name = s1.Instructor.NameEn,
//            //                        JobTitle = s1.Instructor.JobTitleEn,
//            //                        About = s1.Instructor.AboutEn,
//            //                        Photo = s1.Instructor.Picture
//            //                    }).ToList()

//            //                }).ToList();
//            //            return Courses;
//            //    }

//            //}

//        //    public bool EnroleInCourse(int userId, int coursId)
//        //{
//        //    Context.CourseCustomerMapping.Add(new CourseCustomerMapping()
//        //    {
//        //        CourseId = coursId,
//        //        CustomerId = userId,

//        //    });
//        //    Context.SaveChanges();

//        //    return true;
//        //}

//        //public List<CourseDTO> GetAllDiplomas(string lang, int limit, int page)
//        //{
//        //    List<CourseDTO> Diplomas;
//        //    switch (lang.ToLower())
//        //    {
//        //        case "ar":

//        //            Diplomas = Context.Courses.Where(c => c.CourseTypeId == 1 && c.IsDeleted != true && c.IsActive == true)
//        //               .Skip(limit * (page - 1)).Take(limit)
//        //                .Select(s => new CourseDTO
//        //                {

//        //                    Desc = s.ShortDescriptionAr,
//        //                    Id = s.Id,
//        //                    Name = s.NameAr,
//        //                    Price = (decimal)s.Price,
//        //                    FullDesc = s.FullDescriptionAr,
//        //                    Language = s.Language.Name,
//        //                    Photo = s.Key.SetDownloadFileUrlByKey(storage),
//        //                    Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
//        //                    {
//        //                        Id = s1.Instructor.Id,
//        //                        Name = s1.Instructor.NameAr,
//        //                        JobTitle = s1.Instructor.JobTitleAr,
//        //                        About = s1.Instructor.AboutAr,
//        //                        Photo = s1.Instructor.Picture
//        //                    }).ToList()

//        //                }).ToList();

//        //            return Diplomas;
//        //        default:
//        //            Diplomas = Context.Courses.Where(c => c.CourseTypeId == 1 && c.IsDeleted != true && c.IsActive == true)
//        //                .Skip(limit * (page - 1)).Take(limit)
//        //                .Select(s => new CourseDTO
//        //                {

//        //                    Desc = s.ShortDescriptionEn,
//        //                    Id = s.Id,
//        //                    Name = s.NameEn,
//        //                    Price = (decimal)s.Price,
//        //                    FullDesc = s.FullDescriptionEn,
//        //                    Language = s.Language.Name,
//        //                    Photo = s.Key.SetDownloadFileUrlByKey(storage),
//        //                    Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
//        //                    {
//        //                        Id = s1.Instructor.Id,
//        //                        Name = s1.Instructor.NameEn,
//        //                        JobTitle = s1.Instructor.JobTitleEn,
//        //                        About = s1.Instructor.AboutEn,
//        //                        Photo = s1.Instructor.Picture
//        //                    }).ToList()

//        //                }).ToList();
//        //            return Diplomas;
//        //    }

//        //}

//        public BasicDataDTO[] GetAllCategories (  int? parentId, string lang)
//        {
//            BasicDataDTO[] Categories;
//            IQueryable<Category> filter;
//            if (parentId != null)
//                filter = Context.Category.Where(c => c.Published == true && c.ParentCategoryId == parentId);
//            else filter = Context.Category.Where(c => c.Published == true);
//            switch (lang.ToLower())
//            {

//                case "ar":
//                    Categories = filter.Select(s => new BasicDataDTO { Id = s.Id, Name = s.NameAr, Picture = s.Picture, NumberOfCourses = s.Course.Where(c => c.IsActive == true && c.IsDeleted != true).Count() }).ToArray();
//                    return Categories;
//                default:
//                    Categories = filter.Select(s => new BasicDataDTO { Id = s.Id, Name = s.NameEn, Picture = s.Picture, NumberOfCourses = s.Course.Where(c => c.IsActive == true && c.IsDeleted != true).Count() }).ToArray();
//                    return Categories;
//            }
//        }

//        public CategoryDTO GetCategoryCourses(int CategoryId, string lang)
//        {
//            CategoryDTO Category;
//            switch (lang.ToLower())
//            {
//                case "ar":


//                    Category = Context.Category.Where(c => c.Id == CategoryId && c.Published == true && c.IsActive == true && c.IsDeleted != true).Select(s => new CategoryDTO
//                    {
//                        Id = s.Id,
//                        Name = s.NameAr,

//                        Courses = s.Course.Where(c => c.IsDeleted != true && c.IsActive == true).Select(s1 => new CourseDTO
//                        {

//                            Desc = s1.ShortDescriptionAr,
//                            Id = s1.Id,
//                            Name = s1.NameAr,
//                            NoOfVideos = s1.NumberLecture.GetValueOrDefault(),
//                            Price = (decimal)s1.Price,
//                            Tags = s1.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//                            TotalHouers = s1.TotalHour.GetValueOrDefault(),
//                            FullDesc = s1.FullDescriptionAr,
//                            Photo = s1.Key.SetDownloadFileUrlByKey(storage),
//                            InstractorName = s1.CourseInstructorMapping.Select(s2 => s2.Instructor.NameAr).ToArray()


//                        }).ToList()



//                    }).FirstOrDefault();

//                    return Category;

//                default:
//                    Category = Context.Category.Where(c => c.Id == CategoryId && c.IsActive == true && c.IsDeleted != true).Select(s => new CategoryDTO
//                    {
//                        Id = s.Id,
//                        Name = s.NameEn,

//                        Courses = s.Course.Where(c => c.IsDeleted != true && c.IsActive == true).Select(s1 => new CourseDTO
//                        {

//                            Desc = s1.ShortDescriptionEn,
//                            Id = s1.Id,
//                            Name = s1.NameEn,
//                            NoOfVideos = s1.NumberLecture.GetValueOrDefault(),
//                            Price = (decimal)s1.Price,
//                            Tags = s1.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//                            TotalHouers = s1.TotalHour.GetValueOrDefault(),
//                            FullDesc = s1.FullDescriptionEn,
//                            Photo = s1.Key.SetDownloadFileUrlByKey(storage),
//                            InstractorName = s1.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray()


//                        }).ToList()



//                    }).FirstOrDefault();

//                    return Category;
//            }
//        }

//        public CourseDTO GetCourseDetails(int courseId, int userId, string lang)
//        {
//            CourseDTO Course;

//            switch (lang.ToLower())
//            {
//                case "ar":

//                    Course = Context.Courses.Where(c => c.Id == courseId && 
//                    c.IsDeleted != true && c.IsActive == true).Include(i => i.CourseCustomerMapping).Select(s => new CourseDTO
//                    {

//                        Desc = s.ShortDescriptionAr,
//                        Id = s.Id,
//                        Name = s.NameAr,
//                        NoOfVideos = s.NumberLecture.GetValueOrDefault(),
//                        Photo = s.Key.SetDownloadFileUrlByKey(storage),
//                        Price = (decimal)s.Price,
//                        Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//                        TotalHouers = s.TotalHour.GetValueOrDefault(),
//                        DurationBySecond = s.DurationBySecond.GetValueOrDefault(),
//                        FullDesc = s.FullDescriptionAr,
//                        InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameAr).ToArray(),
//                        IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == userId && a.IsActive == true && a.IsDeleted != true),
//                        Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameAr },
//                        Language = s.Language.Name,
//                        Promo = s.Promo,
//                        Certificate = s.CourseCustomerMapping.Where(c => c.CustomerId == userId).Select(s1 => new CertificateDTO() { CertificateURL = s1.Certificate, CertificateDate = s1.CertificateDate, CertificateId = s1.CertificateId }).FirstOrDefault(),
//                        Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
//                        {
//                            Id = s1.Instructor.Id,
//                            Name = s1.Instructor.NameAr,
//                            JobTitle = s1.Instructor.JobTitleAr,
//                            About = s1.Instructor.AboutAr,
//                            Photo = s1.Instructor.Picture
//                        }).ToList()

//                    }).FirstOrDefault();

//                    //return Course;
//                    break;
//                default:
//                    Course = Context.Courses.Where(c => c.Id == courseId).Include(i => i.CourseCustomerMapping).Select(s => new CourseDTO
//                    {

//                        Desc = s.ShortDescriptionEn,
//                        Id = s.Id,
//                        Name = s.NameEn,
//                        NoOfVideos = s.NumberLecture.GetValueOrDefault(),
//                        Photo = s.Key.SetDownloadFileUrlByKey(storage),
//                        Price = (decimal)s.Price,
//                        Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//                        TotalHouers = s.TotalHour.GetValueOrDefault(),
//                        DurationBySecond = s.DurationBySecond.GetValueOrDefault(),
//                        FullDesc = s.FullDescriptionEn,
//                        InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray(),
//                        IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == userId && a.IsActive == true && a.IsDeleted != true),
//                        Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameEn },
//                        Language = s.Language.Name,
//                        Promo = s.Promo,
//                        Certificate = s.CourseCustomerMapping.Where(c => c.CustomerId == userId).Select(s1 => new CertificateDTO() { CertificateURL = s1.Certificate, CertificateDate = s1.CertificateDate, CertificateId = s1.CertificateId }).FirstOrDefault(),
//                        Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
//                        {
//                            Id = s1.Instructor.Id,
//                            Name = s1.Instructor.NameEn,
//                            JobTitle = s1.Instructor.JobTitleEn,
//                            About = s1.Instructor.AboutEn,
//                            Photo = s1.Instructor.Picture
//                        }).ToList()

//                    }).FirstOrDefault();

//                    break;

//            }
//            if (Course != null)
//            {
//                // Course.Videos = GetAllVideosByCourseId(courseId, userId, lang.ToLower());

//                Course.Comments = GetAllCommentByCourseId(courseId);
//                Course.IsRated = Context.CourseComment.Any(a => a.CustomerId == userId && a.CourseId == courseId);

//                Course.WatchPercentage = Context.OnlineAttendance.Where(c => c.CourseDetails != null && c.CourseDetails.CourseId == courseId && c.CustomerId == userId).Sum(s => (float)s.CourseDetails.Duration) * 100 / Course.DurationBySecond;
//                Course.WatchPercentage = (decimal)System.Math.Round(Course.WatchPercentage, 2);
//                Certificate certificate = new Certificate();
//                if (Course.Certificate != null && (Course.Certificate.CertificateURL == null || certificate.CheckFileCreated(Course.Certificate.CertificateURL)) && Course.WatchPercentage > 85)
//                {
//                    var userName = Context.CustomerAttributes.Where(c => c.Id == userId).Select(s => s.CustomerInfo.NameEn).FirstOrDefault();
//                    Course.Name = Context.Courses.Where(c => c.Id == courseId).Select(s => s.NameEn).FirstOrDefault();
//                    Course.Certificate = certificate.getCertificateAsync(Course.Name, Course.Instructors[0].Name, userName, courseId, userId, Course.Certificate).Result;

//                }
//            }
//            return Course;
//        }

//        public CourseDTO GetCourseDetailsIfSectionOrNot(int courseId, int userId, string lang)
//        {

//            CourseDTO Course = new CourseDTO();
//            SectionCourseDTO Course11fullDTO = new SectionCourseDTO();

//            try
//            {

//                var courseData = Context.Courses.FirstOrDefault(c => c.Id == courseId);
//                if (courseData == null)
//                {
//                    return null;
//                }

//                var data = Context.CourseDetails.Where(c => c.CourseId == courseId && c.IsActive == true && c.IsDeleted != true).ToList();
//                if (data.Count > 0)
//                {
//                    Course = Context.Courses.Where(c => c.Id == courseId).Include(i => i.CourseCustomerMapping).Select(s => new CourseDTO
//                    {

//                        Desc = lang == "ar" ? s.ShortDescriptionAr : s.ShortDescriptionEn,
//                        Id = s.Id,
//                        Name = lang == "ar" ? s.NameAr : s.NameEn,
//                        NoOfVideos = s.NumberLecture.GetValueOrDefault(),
//                        Photo = s.Key.SetDownloadFileUrlByKey(storage),
//                        Price = (decimal)s.Price,
//                        Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//                        TotalHouers = s.TotalHour.GetValueOrDefault(),
//                        DurationBySecond = s.DurationBySecond.GetValueOrDefault(),
//                        FullDesc = lang == "ar" ? s.FullDescriptionAr : s.FullDescriptionEn,
//                        InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray(),
//                        IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == userId && a.IsActive == true && a.IsDeleted != true),
//                        Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameEn },
//                        Language = s.Language.Name,
//                        Promo = s.Promo,
//                        Certificate = s.CourseCustomerMapping.Where(c => c.CustomerId == userId).Select(s1 => new CertificateDTO() { CertificateURL = s1.Certificate, CertificateDate = s1.CertificateDate, CertificateId = s1.CertificateId }).FirstOrDefault(),
//                        Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
//                        {
//                            Id = s1.Instructor.Id,
//                            Name = lang == "ar" ? s1.Instructor.NameAr : s1.Instructor.NameEn,
//                            JobTitle = lang == "ar" ? s1.Instructor.JobTitleAr : s1.Instructor.JobTitleEn,
//                            About = lang == "ar" ? s1.Instructor.AboutAr : s1.Instructor.AboutEn,
//                            Photo = s1.Instructor.Picture
//                        }).ToList(),


//                        Comments = GetAllCommentByCourseId(courseId)



//                    }).FirstOrDefault();

//                    var sections = Context.CoursSectionDetails.Where(x => x.Coursid == courseId).ToList();

//                    var groups = data.GroupBy(s => s.SectionNo);
//                    foreach (var group in groups)
//                    {


//                        Course11fullDTO = sections.Where(z => z.SectionNo == group.Key).Select(s => new SectionCourseDTO
//                        {

//                            SectionNo = group.Key,
//                            setionnameName = lang == "ar" ? s.SectionNameAr.ToUpper() : s.SectionNameEn.ToUpper(),

//                            description = lang == "ar" ? s.DescAr : s.DescEn,

//                        }).FirstOrDefault();

//                        if (Course11fullDTO == null)
//                        {
//                            Course11fullDTO = new SectionCourseDTO();
//                        }

//                        Course11fullDTO.Data = GetAllVideosBySectionId(courseId, group.Key, userId, lang);

//                        Course.sectionCourseD.Add(Course11fullDTO);
//                    }
//                    if (Course != null)
//                    {

//                        Course.IsRated = Context.CourseComment.Any(a => a.CustomerId == userId && a.CourseId == courseId);

//                        Course.WatchPercentage = Context.OnlineAttendance.Where(c => c.CourseDetails != null && c.CourseDetails.CourseId == courseId && c.CustomerId == userId).Sum(s => (float)s.CourseDetails.Duration) * 100 / Course.DurationBySecond;
//                        Course.WatchPercentage = (decimal)System.Math.Round(Course.WatchPercentage, 2);
//                        Certificate certificate = new Certificate();
//                        if (Course.Certificate != null && (Course.Certificate.CertificateURL == null || certificate.CheckFileCreated(Course.Certificate.CertificateURL)) && Course.WatchPercentage > 85)
//                        {
//                            var userName = Context.CustomerAttributes.Where(c => c.Id == userId).Select(s => s.CustomerInfo.NameEn).FirstOrDefault();
//                            Course.Name = Context.Courses.Where(c => c.Id == courseId).Select(s => s.NameEn).FirstOrDefault();
//                            Course.Certificate = certificate.getCertificateAsync(Course.Name, Course.Instructors[0].Name, userName, courseId, userId, Course.Certificate).Result;

//                        }
//                    }

//                }

//                return Course;

//            }
//            catch (Exception ex)
//            {

//                return null;

//            }
//        }

//        private List<VideoDTO> GetAllVideosBySectionId(int courseId, int? sectionId, int userId, string lang)
//        {
//            //  var rng = new Random();

//            int videosNo = Context.CourseDetails.Where(c => c.CourseId == courseId && c.SectionNo == sectionId && c.IsActive == true && c.IsDeleted != true).Count();
//            string tag = Context.Courses.Where(c => c.Id == courseId).FirstOrDefault()?.OnlineTag;
//            var stop = videosNo / 40 + 1;
//            List<VideoDTO> Videos = new List<VideoDTO>();
//            var res = new VideosResponce();
//            int limit;
//            for (int i = 1; i < stop + 1; i++)
//            {
//                limit = videosNo;
//                if (videosNo < 1)
//                    break;

//                RestClient client = new RestClient(" https://dev.vdocipher.com/api/videos?page=" + i + "&limit=40&tags=" + tag);


//                client.AddDefaultHeader("Authorization", "Apisecret Cf4y6Kkdl2GNQOen9vYwBCZYmepJL4TDFK4hHukbxxTn9kNiWW0ls2HRRcHB1zIC");



//                res = CallCustomerAPI.CallOutAPIWithAuth(client, new VideosResponce(), 0).Result;

//                VideoDTO Video;
//                if (res != null && res.Rows != null)
//                {
//                    foreach (Datum item in res.Rows)
//                    {
//                        Video = Context.CourseDetails.Where(c => c.CourseId == courseId && c.SectionNo == sectionId && c.OnlineId == item.Id).Select(s => new VideoDTO
//                        {
//                            // pdfurl= Context.CourseDetails.Where(x => x.CourseId == courseId && x.IsPdf == true && x.SectionNo == sectionId&&x.Id==s.Id).FirstOrDefault().PdfUrl ,
//                            pdfurl = s.PdfUrl,
//                            Id = s.Id,
//                            GuidId = s.OnlineId,
//                            IsWatched = s.OnlineAttendance.Any(a => a.CourseDetailsId == s.Id & a.CustomerId == userId),
//                            Name = lang == "ar" ? s.NameAr : s.NameEn,
//                            Posters = item.Posters.ToArray(),
//                            TotalTimeBySec = item.Length,
//                            Desc = item.Description
//                        }).FirstOrDefault();
//                        if (Video != null)
//                            Videos.Add(Video);
//                    }
//                }
//                videosNo -= 40;
//            }
//            Videos = Videos.OrderBy(o => o.Id).ToList();
//            return Videos;
//        }
//        private List<CommentDetailsDTO> GetAllCommentByCourseId(int courseId)
//        {
//            List<CommentDetailsDTO> comments;
//            comments = Context.CourseComment.Where(c => c.CourseId == courseId && c.Approved == true)
//                .Select(s => new CommentDetailsDTO
//                {
//                    Comment = s.Comment,
//                    Rate = s.Rate.GetValueOrDefault(),
//                    CreatedAt = (DateTime)s.CreationDate,
//                    By = s.Customer.CustomerInfo.NameEn,
//                    Avatar = s.Customer.CustomerInfo.Picture

//                }).ToList();
//            return comments;

//        }

//        public bool AddCourseComment(CourseCommentDTO data, int userId)
//        {
//            try
//            {
//                var course = Context.CourseCustomerMapping.Where(c => c.CustomerId == userId && c.CourseId == data.CourseId).FirstOrDefault();
//                if (course == null) return false;
//                Context.CourseComment.Add(new CourseComment
//                {
//                    Id = course.Id,
//                    CourseId = data.CourseId,
//                    Comment = data.Comment,
//                    CustomerId = userId,
//                    Rate = data.Rate,
//                    Approved = false
//                }
//         );
//                Context.SaveChanges();

//                return true;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }
//        public List<PackageDTO> GetAllPackages(
//            bool? showInHomePage, string lang, int limit, int page)
//        {
//            List<PackageDTO> Packages;
//            IQueryable<Package> filter;
//            if (showInHomePage == true)
//                filter = Context.Packages.Where(c => c.Published == true && c.ShowOnHomePage == true && c.IsActive == true && c.IsDeleted != true);
//            else
//                filter = Context.Packages.Where(c => c.Published == true && c.IsActive == true && c.IsDeleted != true);

//            switch (lang.ToLower())
//            {
//                case "ar":

//                    Packages = filter
//                       .Skip(limit * (page - 1)).Take(limit)
//                        .Select(s => new PackageDTO
//                        {

//                            ShortDesc = s.ShortDescription,
//                            Id = s.Id,
//                            Name = s.Name,
//                            Picture = s.Photo,
//                            Type = s.PackageType.Name,
//                            DisplayOrder = (int)s.DisplayOrder,
//                            Price = (decimal)s.Price,
//                            CoursesNumber = (int)s.CoursesNumber
//                        }).OrderByDescending(s => s.DisplayOrder).ToList();

//                    return Packages;
//                default:
//                    Packages = filter
//                         .Skip(limit * (page - 1)).Take(limit)
//                          .Select(s => new PackageDTO
//                          {
//                              ShortDesc = s.ShortDescription,
//                              Id = s.Id,
//                              Name = s.Name,
//                              Picture = s.Photo,
//                              Type = s.PackageType.Name,
//                              DisplayOrder = (int)s.DisplayOrder,
//                              Price = (decimal)s.Price,
//                              CoursesNumber = (int)s.CoursesNumber

//                          }).OrderByDescending(s => s.DisplayOrder).ToList();

//                    return Packages;
//            }

//        }

//        //[HttpGet]
//        //[Route("api/Packages/GetPackageCourses")]
//        //public async Task<IActionResult> GetPackageCourses(int PackageId)
//        //{
//        //    try
//        //    {
//        //        PackageDTO package;
//        //        package = await Context.Packages
//        //            .Where(c => c.Id == PackageId && c.Published == true && c.IsDeleted != true && c.IsActive == true)
//        //            .Select(s => new PackageDTO
//        //            {
//        //                ShortDesc = s.ShortDescription,
//        //                FullDesc = s.FullDescription,
//        //                Id = s.Id,
//        //                Name = s.Name,
//        //                Picture = s.Photo,
//        //                Type = s.PackageType.Name,
//        //                DisplayOrder = (int)s.DisplayOrder,
//        //                Price = (decimal)s.Price,
//        //                CoursesNumber = (int)s.CoursesNumber,

//        //                Courses = s.CoursePackageMappings
//        //                    .Where(c => c.IsActive == true && c.IsDeleted != true && c.Course.IsActive == true && c.Course.IsDeleted != true)
//        //                    .Select(s1 => new CourseDTO
//        //                    {
//        //                        Desc = s1.Course.ShortDescriptionEn,
//        //                        Id = s1.Course.Id,
//        //                        Name = s1.Course.NameEn,
//        //                        NoOfVideos = s1.Course.NumberLecture.GetValueOrDefault(),
//        //                        Price = (decimal)s1.Course.Price,
//        //                        Tags = s1.Course.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
//        //                        TotalHouers = s1.Course.TotalHour.GetValueOrDefault(),
//        //                        FullDesc = s1.Course.FullDescriptionEn,
//        //                        Photo = s1.Course.Key.SetDownloadFileUrlByKey(storage),
//        //                        InstractorName = s1.Course.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray(),
//        //                        Instructors = s1.Course.CourseInstructorMapping
//        //                            .Select(s1 => new InstructorDTO
//        //                            {
//        //                                Id = s1.Instructor.Id,
//        //                                Name = s1.Instructor.NameEn,
//        //                                JobTitle = s1.Instructor.JobTitleEn,
//        //                                About = s1.Instructor.AboutEn,
//        //                                Photo = s1.Instructor.Picture
//        //                            }).ToList()
//        //                    })
//        //            }).FirstOrDefaultAsync();

//        //        if (package != null)
//        //            return Ok(package);
//        //        else
//        //            return NotFound("Not found courses in this package");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return StatusCode(500, ex);
//        //    }
//        //}













//        private List<VideoDTO> GetAllVideosByCourseId(int courseId, int userId, string lang)
//        {
//            //  var rng = new Random();

//            int videosNo = Context.CourseDetails.Where(c => c.CourseId == courseId && c.IsActive == true && c.IsDeleted != true).Count();
//            string tag = Context.Courses.Where(c => c.Id == courseId).FirstOrDefault()?.OnlineTag;
//            var stop = videosNo / 40 + 1;
//            List<VideoDTO> Videos = new List<VideoDTO>();
//            var res = new VideosResponce();
//            int limit;
//            for (int i = 1; i < stop + 1; i++)
//            {
//                limit = videosNo;
//                if (videosNo < 1)
//                    break;

//                RestClient client = new RestClient(" https://dev.vdocipher.com/api/videos?page=" + i + "&limit=40&tags=" + tag);


//                client.AddDefaultHeader("Authorization", "Apisecret Cf4y6Kkdl2GNQOen9vYwBCZYmepJL4TDFK4hHukbxxTn9kNiWW0ls2HRRcHB1zIC");



//                res = CallCustomerAPI.CallOutAPIWithAuth(client, new VideosResponce(), 0).Result;

//                VideoDTO Video;
//                if (res != null && res.Rows != null)
//                {
//                    foreach (Datum item in res.Rows)
//                    {
//                        Video = Context.CourseDetails.Where(c => c.CourseId == courseId & c.OnlineId == item.Id).Select(s => new VideoDTO
//                        {
//                            Id = s.Id,
//                            GuidId = s.OnlineId,
//                            IsWatched = s.OnlineAttendance.Any(a => a.CourseDetailsId == s.Id & a.CustomerId == userId),
//                            Name = lang == "ar" ? s.NameAr : s.NameEn,
//                            Posters = item.Posters.ToArray(),
//                            TotalTimeBySec = item.Length,
//                            Desc = item.Description
//                        }).FirstOrDefault();
//                        if (Video != null)
//                            Videos.Add(Video);
//                    }
//                }
//                videosNo -= 40;
//            }
//            Videos = Videos.OrderBy(o => o.Id).ToList();
//            return Videos;
//        }


//    }
//}





