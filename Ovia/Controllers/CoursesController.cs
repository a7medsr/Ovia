using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BunnyCDN.Api;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using Ovia.Services;
using Ovia.Services.StorageFiles;
using Polly;
using RestSharp;

namespace Ovia.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly MomEntity Context;
        private readonly IStorageService storage;
       //private int _userId { get; set; }
        public CoursesController(
            //IHttpContextAccessor httpContextAccessor,
            IStorageService _storage,
            MomEntity _Context)
        {

            //var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (userId != null)
            //{
            //    _userId = int.Parse(userId);
            //}

            this.Context = _Context;
            this.storage = _storage;
        }


       // private Courses BusinessCourses = new Courses();


        [HttpPost]
        [Route("api/Courses/AddCourseVideos")]
        public async Task<IActionResult> AddCourseVideos(AddVideoDTO dto)
        {
            var existingCourse = await Context.Courses.FirstOrDefaultAsync(c => c.Id == dto.CourseId);
            if (existingCourse == null)
            {
                return NotFound("This course not found");
            }
            else
            {
                var existingVideo = await Context.CourseDetails.FirstOrDefaultAsync(c => c.OnlineId == dto.OnlineId);

                if (existingVideo != null)
                {
                    return BadRequest("This video already exist");
                }
                else
                {
                    var course_detail = new CourseDetails
                    {
                        Id = 0,
                        NameEn = dto.NameEn,
                        CourseId = dto.CourseId,
                        OnlineId = dto.OnlineId,
                        OnlineTag = dto.OnlineTag,
                        Duration = dto.Duration,
                        IsActive = dto.IsActive,
                        IsDeleted = false
                    };
                    Context.CourseDetails.Add(course_detail);
                    await Context.SaveChangesAsync();
                    return Ok("Video added successfully");
                }




            }


        }
   
        [HttpGet]
        [Route("api/Courses/GetMyCoruses")]
        public async Task<IActionResult> GetMyCoruses(int userId)
        {
            try
            {
                var courses = await Context.CourseCustomerMapping
                    .Where(c => c.CustomerId == userId && c.IsActive == true && c.IsDeleted != true)
                    .Select(s => new CourseDTO
                    {
                        Category = s.Course.Category != null ? new BasicDataDTO { Id = s.Course.Category.Id, Name = s.Course.Category.NameEn } : null,
                        Photo = s.Course.Key != null ? s.Course.Key.SetDownloadFileUrlByKey(storage) : "",
                        Desc = s.Course.FullDescriptionEn,
                        Id = s.Course.Id,
                        Name = s.Course.NameEn,
                        NoOfVideos = s.Course.NumberLecture ?? 0,
                        Price = s.Course.Price ?? 0,
                        Tags = s.Course.CourseCourseTagMappings != null ? s.Course.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray() : new string[0],
                        TotalHouers = s.Course.TotalHour ?? 0,
                        WatchPercentage = CalculateWatchPercentage(s.Course, userId)
                    }).ToListAsync();

                if (courses == null || courses.Count == 0)
                    return NotFound("No courses found for the user.");
                else
                    return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        private static float CalculateWatchPercentage(Course course, int userId)
        {
            if (course == null || course.DurationBySecond == null)
                return 0;

            var totalDuration = course.CourseDetails
                .SelectMany(cd => cd.OnlineAttendance.Where(a => a.CustomerId == userId))
                .Sum(a => a.CourseDetails.Duration ?? 0);

            return (float)Math.Round(totalDuration * 100 / course.DurationBySecond.Value, 2);
        }



        [HttpGet]
        [Route("api/Courses/GetCourseDetails")]
        public async Task<IActionResult> GetCourseDetails(int courseId, int userId)
        {
            try
            {
                var course = await Context.Courses
                    .Where(c => c.Id == courseId)
                    .Include(c => c.CourseInstructorMapping) // Include CourseInstructorMapping
                        .ThenInclude(cim => cim.Instructor) // Include related Instructor entities
                    .Include(i => i.CourseCustomerMapping)
                    .FirstOrDefaultAsync();

                if (course != null)
                {
                    var courseDTO = new CourseDTO
                    {
                        Desc = course.ShortDescriptionEn,
                        Id = course.Id,
                        Name = course.NameEn,
                        NoOfVideos = course.NumberLecture.GetValueOrDefault(),
                        Photo = course.Key != null ? course.Key.SetDownloadFileUrlByKey(storage) : "",
                        Price = (decimal)course.Price,
                        Tags = course.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
                        TotalHouers = course.TotalHour.GetValueOrDefault(),
                        DurationBySecond = course.DurationBySecond.GetValueOrDefault(),
                        FullDesc = course.FullDescriptionEn,
                        InstractorName = course.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray(),
                        IsEnroll = course.CourseCustomerMapping?.Any(a => a.CustomerId == userId && a.IsActive == true && a.IsDeleted == false) ?? false,
                        Category = course.Category != null ? new BasicDataDTO { Id = course.Category.Id, Name = course.Category.NameEn } : null,
                        Language = course.Language?.Name,
                        Promo = course.Promo,
                        Certificate = course.CourseCustomerMapping
                            .Where(c => c.CustomerId == userId)
                            .Select(s1 => new CertificateDTO
                            {
                                CertificateURL = s1.Certificate,
                                CertificateDate = s1.CertificateDate,
                                CertificateId = s1.CertificateId
                            })
                            .FirstOrDefault(),

                        Instructors = course.CourseInstructorMapping.Select(s1 => new InstructorDTO
                        {
                            Id = s1.Instructor.Id,
                            Name = s1.Instructor.NameEn,
                            JobTitle = s1.Instructor.JobTitleEn,
                            About = s1.Instructor.AboutEn,
                            Photo = s1.Instructor.Picture.SetDownloadFileUrlByKey(storage)
                        }).ToList()
                    };

                    return Ok(courseDTO);
                }
                else
                {
                    return NotFound("Course not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }







        // GET: api/Courses
        // [Authorize]
        //        [HttpGet]
        //        [Route("api/Courses/GetMyCoruses")]
        //        public async Task<IActionResult> GetMyCoruses(int UserId)
        //        {
        //            try
        //            {
        //                List<CourseDTO> Courses;
        //                Courses = await Context.CourseCustomerMapping
        //                    .Where(c => c.CustomerId == UserId && c.IsActive == true && c.IsDeleted != true)
        //                    .Select(s => new CourseDTO
        //                    {
        //                        Category = new BasicDataDTO { Id = s.Course.Category.Id, Name = s.Course.Category.NameEn },
        //                        Photo = s.Course.Key != null ? s.Course.Key.SetDownloadFileUrlByKey(storage) : "",
        //                        Desc = s.Course.FullDescriptionEn,
        //                        Id = s.Course.Id,
        //                        Name = s.Course.NameEn,
        //                        NoOfVideos = s.Course.NumberLecture.GetValueOrDefault(),
        //                        Price = (decimal)s.Course.Price,
        //                        Tags = s.Course.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
        //                        TotalHouers = s.Course.TotalHour.GetValueOrDefault(),
        //                        WatchPercentage = (float)System.Math.Round((s.Course.CourseDetails.SelectMany(ss => ss.OnlineAttendance.Where(c => c.CustomerId == UserId).Select(d => d.CourseDetails)).Sum(a => (decimal)a.Duration) * 100 / (decimal)s.Course.DurationBySecond), 2)
        //                    }).ToListAsync();

        //                if (Courses == null || Courses.Count == 0)
        //                    return NotFound("Not Found");
        //else
        //                return Ok(Courses);
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, ex);
        //            }
        //        }


        // [AllowAnonymous]
        [HttpGet]
        [Route("api/Courses/GetCoruseRates")]
        public IActionResult GetCoruseRates(int courseId)
        {
            try
            {

                var CourseCommentes = Context.CourseComment
             .Where(c => c.IsActive == true && c.IsDeleted != true &&
             c.CourseId == courseId).ToList();
                var rate =  new RateDto()
                {
                    AvrgRate = CourseCommentes.Average(a => a.Rate),
                    GruopsRates = CourseCommentes.GroupBy(g => g.Rate).ToDictionary(d => d.Key, d => d.Count()),
                    NoOfRates = CourseCommentes.Count
                };

                if (rate == null)
                    return NotFound("not found any rates");
                else
                return Ok(rate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }

        [HttpGet]
        [Route("api/Courses/GetAllCategories")]
        public async Task<IActionResult> GetAllCategories(int? parentId)
        {
            try
            {


                BasicDataDTO[] Categories;
                IQueryable<Category> filter;
                if (parentId != null)
                    filter = Context.Category
                        .Where(c => c.Published == true && c.ParentCategoryId == parentId);
                else filter = Context.Category.Where(c => c.Published == true);
                Categories = filter.Select(s => new BasicDataDTO 
                { Id = s.Id, Name = s.NameEn, Picture = s.Picture,
                    NumberOfCourses = s.Course.Where(c => c.IsActive == true
                    && c.IsDeleted != true).Count() }).ToArray();

                if (Categories == null)
                    return NotFound("not found any category");
                else
                    return Ok(Categories);



            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }


        [HttpGet]
        [Route("api/Courses/GetCategoryCourses")]
        public async Task<IActionResult> GetCategoryCourses(int CategoryId)
        {
            try
            {
                var category = await Context.Category
                    .Where(c => c.Id == CategoryId && c.IsActive ==true && c.IsDeleted ==false)
                    .Select(s => new CategoryDTO
                    {
                        Id = s.Id,
                        Name = s.NameEn,
                        Courses = s.Course
                            .Where(c => c.IsActive == true && c.IsDeleted == false)
                            .Select(s1 => new CourseDTO
                            {
                                Desc = s1.ShortDescriptionEn,
                                Id = s1.Id,
                                Name = s1.NameEn,
                                NoOfVideos = s1.NumberLecture.GetValueOrDefault(),
                                Price = (decimal)s1.Price,
                                Tags = s1.CourseCourseTagMappings
                                .Select(s1 => s1.CourseTag.Name).ToArray(),
                                TotalHouers = s1.TotalHour.GetValueOrDefault(),
                                FullDesc = s1.FullDescriptionEn,
                                Photo = s1.Key.SetDownloadFileUrlByKey(storage),
                                InstractorName = s1.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray()
                            }).ToList()
                    }).FirstOrDefaultAsync();

                if (category != null)
                {
                    return Ok(category);
                }
                else
                {
                    return NotFound("Category not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }




        // [Authorize]
        [HttpPost]
        [Route("api/Courses/EnroleInCourse")]
        public async Task<IActionResult> EnroleInCourse(int userId, int courseId)
        {
            try
            {
                var enrolledCourse = await Context.CourseCustomerMapping
                    .Where(c => c.CourseId == courseId && c.CustomerId == userId)
                    .FirstOrDefaultAsync();

                if (enrolledCourse != null)
                    return BadRequest("You already enrolled in this course");

                var newEnrollment = new CourseCustomerMapping
                {
                    Id = 0,
                    CourseId = courseId,
                    CustomerId = userId,
                    CertificateId = null,
                    Certificate = "null",
                    CertificateDate = null,
                    Cost = 0,
                    PackageSelectId = null
                };

                Context.CourseCustomerMapping.Add(newEnrollment);
                await Context.SaveChangesAsync();

                return Ok("You enrolled successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        [HttpGet]
        [Route("api/Courses/GetAllCourses")]
        public async Task <IActionResult> GetAllCourses(long customerId, bool? showInHomePage, bool? latest, int limit =12, int page =1 )
        {
            try
            {
                List<CourseDTO> Courses;
                IQueryable<Course> filter;
                if (showInHomePage == true)
                    filter = Context.Courses.Where(c => c.Published == true && c.CourseTypeId == 1 && c.ShowOnHomePage == true && c.IsDeleted != true && c.IsActive == true);
                else
                    filter = Context.Courses.Where(c => c.Published == true && c.CourseTypeId == 1 && c.IsDeleted != true && c.IsActive == true);
                if (latest == true)
                    filter = filter.Skip(limit * (page - 1)).Take(limit)
                            .OrderByDescending(c => c.Id);

                Courses = filter

                     .Select(s => new CourseDTO
                     {

                         Desc = s.ShortDescriptionEn,
                         Id = s.Id,
                         Name = s.NameEn,
                         NoOfVideos = s.NumberLecture.GetValueOrDefault(),
                         Price = (decimal)s.Price,
                         Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
                         TotalHouers = s.TotalHour.GetValueOrDefault(),
                         DurationBySecond = s.DurationBySecond.GetValueOrDefault(),
                         FullDesc = s.FullDescriptionEn,
                         InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray(),
                         //IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == customerId),
                        Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameEn },
                         Language = s.Language.Name,
                         Photo = s.Key.SetDownloadFileUrlByKey(storage),
                         Promo = s.Promo,
                         Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
                         {
                             Id = s1.Instructor.Id,
                             Name = s1.Instructor.NameEn,
                             JobTitle = s1.Instructor.JobTitleEn,
                             About = s1.Instructor.AboutEn,
                             Photo = s1.Instructor.Picture
                         }).ToList()

                     }).ToList();


                if (Courses != null && Courses.Count != 0)
                    return Ok(Courses);
                else
                    return NotFound("not found any courses");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }

        [HttpGet]
        [Route("api/Courses/GetCoursesPerPage")]
        public async Task<IActionResult> GetCoursesPerPage(long customerId , bool? showInHomePage,
            bool? latest,  int limit =12, int page = 1)
        {
            try
            {
                List<CourseDTO> Courses;
                IQueryable<Course> filter;
                if (showInHomePage == true)
                    filter = Context.Courses
                        .Where(c => c.Published == true && c.CourseTypeId == 3 
                        && c.ShowOnHomePage == true && c.IsDeleted != true && c.IsActive == true);
                else
                    filter = Context.Courses
                        .Where(c => c.Published == true && c.CourseTypeId == 3 
                        && c.IsDeleted != true && c.IsActive == true);
                if (latest == true)
                    filter = filter.OrderByDescending(c => c.Id);

                filter = filter.Skip(limit * (page - 1)).Take(limit);
                Courses = filter

                       .Select(s => new CourseDTO
                       {

                           Desc = s.ShortDescriptionEn,
                           Id = s.Id,
                           Name = s.NameEn,
                           NoOfVideos = s.NumberLecture.GetValueOrDefault(),
                           Price = (decimal)s.Price,
                           Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
                           TotalHouers = s.TotalHour.GetValueOrDefault(),
                           FullDesc = s.FullDescriptionEn,
                           InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray(),
                           IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == customerId),
                           Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameEn },
                           Language = s.Language.Name,
                           Photo = s.Key.SetDownloadFileUrlByKey(storage),
                           Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
                           {
                               Id = s1.Instructor.Id,
                               Name = s1.Instructor.NameEn,
                               JobTitle = s1.Instructor.JobTitleEn,
                               About = s1.Instructor.AboutEn,
                               Photo = s1.Instructor.Picture
                           }).ToList()

                       }).ToList();

                if (Courses != null && Courses.Count != 0)
                    return Ok(Courses);
                else
                    return NotFound("not found any courses"); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }



        [HttpGet]
        [Route("api/Diplomas/GetAllDiplomas")]
        public async Task<IActionResult> GetAllDiplomas(int limit =10, int page = 1)
        {
            try
            {
                List<CourseDTO> Diplomas;
                Diplomas =await Context.Courses.Where(c => c.CourseTypeId == 2 && c.IsDeleted != true && c.IsActive == true)
                      .Skip(limit * (page - 1)).Take(limit)
                      .Select(s => new CourseDTO
                      {

                          Desc = s.ShortDescriptionEn,
                          Id = s.Id,
                          Name = s.NameEn,
                          Price = (decimal)s.Price,
                          FullDesc = s.FullDescriptionEn,
                          Language = s.Language.Name,
                          Photo = s.Key.SetDownloadFileUrlByKey(storage),
                          Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
                          {
                              Id = s1.Instructor.Id,
                              Name = s1.Instructor.NameEn,
                              JobTitle = s1.Instructor.JobTitleEn,
                              About = s1.Instructor.AboutEn,
                              Photo = s1.Instructor.Picture
                          }).ToList()

                      }).ToListAsync();

                if (Diplomas != null && Diplomas.Count != 0)
                    return Ok(Diplomas);
                else
                    return NotFound("Not found any diploma");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }








        [ApiExplorerSettings(IgnoreApi =true)]
        private List<CommentDetailsDTO> GetAllCommentByCourseId(int courseId)
        {
            List<CommentDetailsDTO> comments;
            comments = Context.CourseComment.Where(c => c.CourseId == courseId && c.Approved == true)
                .Select(s => new CommentDetailsDTO
                {
                    Comment = s.Comment,
                    Rate = s.Rate.GetValueOrDefault(),
                    CreatedAt = (DateTime)s.CreationDate,
                    By = s.Customer.CustomerInfo.NameEn,
                    Avatar = s.Customer.CustomerInfo.Picture

                }).ToList();
            return comments;

        }


        // // [Authorize]
        [HttpGet]
        [Route("api/Courses/GetCourseDetailsIfsectionOrNot")]
        public IActionResult GetCourseDetailsIfsectionOrNot(int courseId, int userId)
        {
            CourseDTO Course = new CourseDTO();
            SectionCourseDTO Course11fullDTO = new SectionCourseDTO();

            try
            {

                var courseData = Context.Courses.FirstOrDefault(c => c.Id == courseId);
                if (courseData == null)
                {
                    return null;
                }

                var data = Context.CourseDetails
                    .Where(c => c.CourseId == courseId &&
                    c.IsActive == true && c.IsDeleted != true).ToList();
                if (data.Count > 0)
                {
                    Course = Context.Courses.Where(c => c.Id == courseId)
                        .Include(i => i.CourseCustomerMapping).Select(s => new CourseDTO
                        {

                            Desc = s.ShortDescriptionEn,
                            Id = s.Id,
                            Name = s.NameEn,
                            NoOfVideos = s.NumberLecture.GetValueOrDefault(),
                            Photo = (s.Key).SetDownloadFileUrlByKey(storage),
                            Price = (decimal)s.Price,
                            Tags = s.CourseCourseTagMappings.Select(s1 => s1.CourseTag.Name).ToArray(),
                            TotalHouers = s.TotalHour.GetValueOrDefault(),
                            DurationBySecond = s.DurationBySecond.GetValueOrDefault(),
                            FullDesc = s.FullDescriptionEn,
                            InstractorName = s.CourseInstructorMapping.Select(s2 => s2.Instructor.NameEn).ToArray(),
                            IsEnroll = s.CourseCustomerMapping.Any(a => a.CustomerId == userId && a.IsActive == true && a.IsDeleted != true),
                            Category = new BasicDataDTO { Id = s.Category.Id, Name = s.Category.NameEn },
                            Language = s.Language.Name,
                            Promo = s.Promo,
                            Certificate = s.CourseCustomerMapping.Where(c => c.CustomerId == userId).
                        Select(s1 => new CertificateDTO() { CertificateURL = s1.Certificate, CertificateDate = s1.CertificateDate, CertificateId = s1.CertificateId }).FirstOrDefault(),
                            Instructors = s.CourseInstructorMapping.Select(s1 => new InstructorDTO
                            {
                                Id = s1.Instructor.Id,
                                Name = s1.Instructor.NameEn,
                                JobTitle = s1.Instructor.JobTitleEn,
                                About = s1.Instructor.AboutEn,
                                Photo = s1.Instructor.Picture
                            }).ToList(),


                            //      Comments = GetAllCommentByCourseId(courseId)



                        }).FirstOrDefault();

                    var sections = Context.CoursSectionDetails
                        .Where(x => x.Coursid == courseId).ToList();

                    var groups = data.GroupBy(s => s.SectionNo);
                    foreach (var group in groups)
                    {


                        Course11fullDTO = sections.Where(z => z.SectionNo == group.Key).Select(s => new SectionCourseDTO
                        {

                            SectionNo = group.Key,
                            setionnameName = s.SectionNameEn.ToUpper(),

                            description = s.DescEn,

                        }).FirstOrDefault();

                        if (Course11fullDTO == null)
                        {
                            Course11fullDTO = new SectionCourseDTO();
                        }

                        Course11fullDTO.Data = GetAllVideosBySectionId(courseId, group.Key, userId);

                        Course.sectionCourseD.Add(Course11fullDTO);
                    }
                    if (Course != null)
                    {

                        Course.IsRated = Context.CourseComment.Any(a => a.CustomerId == userId && a.CourseId == courseId);

                        //Course.WatchPercentage
                        //    = Context.OnlineAttendance
                        //    .Where(c => c.CourseDetails != null &&
                        //    c.CourseDetails.CourseId == courseId
                        //    && c.CustomerId == userId)
                        //    .Sum(s => (float)s.CourseDetails.Duration) 
                        //    * 100 / Course.DurationBySecond;

                        Course.WatchPercentage = Context.OnlineAttendance
       .Where(c => c.CourseDetails != null && c.CourseDetails.CourseId == courseId
       && c.CustomerId == userId)
       .Sum(s => (float?)s.CourseDetails.Duration) * 100 / (float)Course.DurationBySecond;




                        //Course.WatchPercentage = (decimal)System.Math.Round(Course.WatchPercentage, 2);
                        Course.WatchPercentage = (float?)System.Math.Round(Course.WatchPercentage.GetValueOrDefault(), 2);



                        Certificate certificate = new Certificate();
                        if (Course.Certificate != null && (Course.Certificate.CertificateURL == null || certificate.CheckFileCreated(Course.Certificate.CertificateURL)) && Course.WatchPercentage > 85)
                        {
                            var userName = Context.CustomerAttributes.Where(c => c.Id == userId).Select(s => s.CustomerInfo.NameEn).FirstOrDefault();
                            Course.Name = Context.Courses.Where(c => c.Id == courseId).Select(s => s.NameEn).FirstOrDefault();
                            Course.Certificate = certificate.getCertificateAsync(Course.Name, Course.Instructors[0].Name, userName, courseId, userId, Course.Certificate).Result;
                        }
                    }

                    return Ok(Course);
                }


                else
                    return NotFound("Not found any videos");
            }
            catch (Exception ex)
            {

                return null;

            }
        }

        [ApiExplorerSettings(IgnoreApi =true)]
        private List<VideoDTO> GetAllVideosBySectionId(int courseId, int? sectionId,
            int userId)
        {
            //  var rng = new Random();

            int videosNo = Context.CourseDetails
                .Where(c => c.CourseId == courseId
              //  && c.SectionNo == sectionId

                && c.IsActive == true && c.IsDeleted != true).Count();
            string tag = Context.Courses.Where(c => c.Id == courseId).FirstOrDefault()?.OnlineTag;
            var stop = videosNo / 40 + 1;
            List<VideoDTO> Videos = new List<VideoDTO>();
            var res = new VideosResponce();
            int limit;
            for (int i = 1; i < stop + 1; i++)
            {
                limit = videosNo;
                if (videosNo < 1)
                    break;

                RestClient client = new 
                    RestClient(" https://dev.vdocipher.com/api/videos?page=" + i +
                    "&limit=40&tags=" + tag);


                client.AddDefaultHeader
                    ("Authorization", "Apisecret Cf4y6Kkdl2GNQOen9vYwBCZYmepJL4TDFK4hHukbxxTn9kNiWW0ls2HRRcHB1zIC");



                res = CallCustomerAPI.CallOutAPIWithAuth(client, new VideosResponce(), 0).Result;

                VideoDTO Video;
                if (res != null && res.Rows != null)
                {
                    foreach (Datum item in res.Rows)
                    {
                        Video = Context.CourseDetails.Where(c => c.CourseId == courseId
                        //&& c.SectionNo == sectionId 
                        && c.OnlineId == item.Id)
                            .Select(s => new VideoDTO
                        {
                            // pdfurl= Context.CourseDetails.Where(x => x.CourseId == courseId && x.IsPdf == true && x.SectionNo == sectionId&&x.Id==s.Id).FirstOrDefault().PdfUrl ,
                            pdfurl = s.PdfUrl,
                            Id = s.Id,
                            GuidId = s.OnlineId,
                            IsWatched = s.OnlineAttendance.Any(a => a.CourseDetailsId == s.Id & a.CustomerId == userId),
                            Name = s.NameEn,
                            Posters = item.Posters.ToArray(),
                            TotalTimeBySec = item.Length,
                            Desc = item.Description
                        }).FirstOrDefault();
                        if (Video != null)
                            Videos.Add(Video);
                    }
                }
                videosNo -= 40;
            }
            Videos = Videos.OrderBy(o => o.Id).ToList();
            return Videos;
        }




        // [Authorize]
        [HttpPost]
        [Route("api/Courses/AddComment")]
        public async Task<IActionResult> AddComment(
            [FromBody] CourseCommentDTO data)
        {
            try
            {
                var course = Context.CourseCustomerMapping
                    .FirstOrDefault(c => c.CustomerId == data.userId && c.CourseId == data.CourseId);
                if (course == null) return NotFound("Course not found for this user");

                Context.CourseComment.Add(new CourseComment
                {
                    CourseId = data.CourseId,
                    Comment = data.Comment,
                    CustomerId = data.userId,
                    Rate = data.Rate,
                    Approved = false
                });

                await Context.SaveChangesAsync();

                return Ok("Comment added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet]
        [Route("api/Packages/GetAllPackages")]
        public async Task<IActionResult> GetAllPackages(bool? showInHomePage, string lang, int limit =10, int page =1)
        {
            try
            {

                List<PackageDTO> Packages;
                IQueryable<Package> filter;
                if (showInHomePage == true)
                    filter = Context.Packages
                        .Where(c => c.Published == true && c.ShowOnHomePage == true 
                        && c.IsActive == true && c.IsDeleted != true);
                else
                    filter = Context.Packages
                        .Where(c => c.Published == true && c.IsActive == true && c.IsDeleted != true);


                Packages = filter
                        .Skip(limit * (page - 1)).Take(limit)
                         .Select(s => new PackageDTO
                         {
                             ShortDesc = s.ShortDescription,
                             Id = s.Id,
                             Name = s.Name,
                             Picture = s.Photo,
                             Type = s.PackageType.Name,
                             DisplayOrder = (int)s.DisplayOrder,
                             Price = (decimal)s.Price,
                             CoursesNumber = (int)s.CoursesNumber

                         }).OrderByDescending(s => s.DisplayOrder).ToList();

                if (Packages != null)
                    return Ok(Packages);
                else
                    return NotFound("Not found courses in this package");




            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
        [HttpGet]
        [Route("api/Packages/GetPackageCourses")]
        public async Task<IActionResult> GetPackageCourses(int PackageId)
        {
            try
            {

                PackageDTO package;
                package =await Context.Packages.Where(c => c.Id == PackageId && c.Published == true && c.IsDeleted != true && c.IsActive == true).Select(s => new PackageDTO
                {
                    ShortDesc = s.ShortDescription,
                    FullDesc = s.FullDescription,
                    Id = s.Id,
                    Name = s.Name,
                    Picture = s.Photo,
                    Type = s.PackageType.Name,
                    DisplayOrder = (int)s.DisplayOrder,
                    Price = (decimal)s.Price,
                    CoursesNumber = (int)s.CoursesNumber,

                    Courses = s.CoursePackageMappings
                    .Where(c => c.IsActive == true && c.IsDeleted != true && c.Course.IsActive == true && c.Course.IsDeleted != true).Select(s1 => new CourseDTO
                    {

                        Desc = s1.Course.ShortDescriptionEn,
                        Id = s1.Course.Id,
                        Name = s1.Course.NameEn,
                        NoOfVideos = s1.Course.NumberLecture.GetValueOrDefault(),
                        Price = (decimal)s1.Course.Price,
                        Tags = s1.Course.CourseCourseTagMappings
                        .Select(s1 => s1.CourseTag.Name).ToArray(),
                        TotalHouers = s1.Course.TotalHour.GetValueOrDefault(),
                        FullDesc = s1.Course.FullDescriptionEn,
                        Photo = s1.Course.Key.SetDownloadFileUrlByKey(storage),
                        InstractorName = s1.Course.CourseInstructorMapping
                        .Select(s2 => s2.Instructor.NameEn).ToArray(),
                        Instructors = s1.Course.CourseInstructorMapping
                        .Select(s1 => new InstructorDTO
                        {
                            Id = s1.Instructor.Id,
                            Name = s1.Instructor.NameEn,
                            JobTitle = s1.Instructor.JobTitleEn,
                            About = s1.Instructor.AboutEn,
                            Photo = s1.Instructor.Picture
                        }).ToList()

                    }).ToList()



                }).FirstOrDefaultAsync();

                if (package != null  )
                    return Ok(package);
                else
                    return NotFound("Not found courses in this package");


                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
    
    
    
    
    }
}
