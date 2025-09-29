using Ovia.DTO;
using Ovia.Models;
using Ovia.Services.SendEmails;
using Ovia.Services.StorageFiles;


namespace Ovia.Services
    { 
    public class Content
    {

        MomEntity Context = new MomEntity();
        StorageService storage;
        MailingServices mailingService;
        //public Content(MomEntity _momDb, IStorageService _storage, IMailingServices _mailingService)
        //{
        //    storage = _storage;
        //    Context = _momDb;
        //    mailingService = _mailingService;
        //}



        public List<InstructorDTO> GetAllInstructor( string lang, int limit, int page)
        {
            List<InstructorDTO> instructor;

            switch (lang.ToLower())
            {
                case "ar":

                    instructor = Context.Instructors.Where(c => c.IsDeleted != true && c.IsActive == true)
                        .Skip(limit * (page - 1)).Take(limit)
                        .OrderByDescending(o => o.DisplayOrder)
                        .Select(s => new InstructorDTO
                    {
                        Name = s.NameAr,
                        JobTitle = s.JobTitleAr,
                        Id = s.Id,
                        About= s.AboutAr,
                        Photo= s.Picture,
                        NumberOfCourses= s.CourseInstructorMapping.Count,
                        uriinstractorname=s.uriinstractorname,
                        GuId = s.GuId,
                        }).ToList();

                    return instructor;
                default:
                    instructor = Context.Instructors.Where(c => c.IsDeleted == false && c.IsActive == true)
                       //.OrderBy(x => Guid.NewGuid()).Take(limit)
                       .OrderByDescending(o => o.DisplayOrder)
                        //.Skip(limit * (page - 1))
                        //.OrderBy(o => o.DisplayOrder)

                        .Select(s => new InstructorDTO
                        {
                        Name = s.NameEn,
                        JobTitle = s.JobTitleEn,
                        Id = s.Id,
                        About = s.AboutEn,
                        Photo = s.Picture,
                            NumberOfCourses = s.CourseInstructorMapping.Count,
                            uriinstractorname = s.uriinstractorname,
                        GuId = s.GuId,

                        }).ToList();

                    return instructor;
            }

        }

        //public InstructorDTO GetInstructorDetails(string urlinstractorName, int instructorId, string lang)
        //{
        //    InstructorDTO instructor;

        //     if (urlinstractorName != null)
        //        {
        //            urlinstractorName = urlinstractorName.ToLower().Trim();
        //            if (urlinstractorName.Contains(" "))
        //            {
        //                urlinstractorName = urlinstractorName.Replace(" ", "_");
        //            }
        //            instructorId = Context.Instructors.Where(c => c.uriinstractorname
        //            .Contains(urlinstractorName) && c.IsDeleted != true && c.IsActive == true)
        //            .FirstOrDefault().Id;
        //        }
        //        switch (lang.ToLower())
        //        {
        //            case "ar":
        //                instructor = Context.Instructors.Where(c => c.IsDeleted == false && c.IsActive == true && c.Id == instructorId).Select(s => new InstructorDTO
        //                {
        //                    Name = s.NameAr,
        //                    JobTitle = s.JobTitleAr,
        //                    Id = s.Id,
        //                    About = s.AboutAr,
        //                    Photo = s.Picture,
        //                    NumberOfCourses = s.CourseInstructorMapping.Count(),
        //                    NumberOfTalks = s.Talks.Count(),
        //                    GuId=s.GuId,
        //                    Talks = s.Talks.Count,
        //                    Students = s.CourseInstructorMapping.SelectMany(ss => ss.Course.CourseCustomerMapping).Count(),

        //                    NumperOfViews = (from coursINSmap in Context.CourseInstructorMappings
        //                                     join coursdeals in Context.CourseDetails
        //                                     on coursINSmap.CourseId equals coursdeals.CourseId
        //                                     join onlinattend in Context.OnlineAttendance
        //                                     on coursdeals.Id equals onlinattend.CourseDetailsId
        //                                     where coursINSmap.InstructorId == s.Id
        //                                     select onlinattend.CustomerId).Distinct().Count(),
        //                    metatag = Context.MetaTags.Where(x => x.publicId == s.Id&&x.Notes=="Instractor").Select(v => new MetatagDTO
        //                    {
        //                        NameAR = v.NameAR,
        //                        DiscriptionAR = v.DiscriptionAR,
        //                        TitleAR = v.TitleAR,
        //                        Type = v.Type,
        //                        Header = v.Header,
        //                        KeywordsAR = v.KeywordsAR,
        //                        Url = v.Url
        //                    }).FirstOrDefault(),
        //                    uriinstractorname = s.uriinstractorname
        //                }).FirstOrDefault();
                   
        //                if (instructor != null)

        //                    instructor.Courses = Context.CourseInstructorMappings.Where(c => c.InstructorId == instructorId)
        //                          .Select(s => new CourseDTO()
        //                          {
        //                              CourseId = s.CourseId.GetValueOrDefault(),
        //                              Name = s.Course.NameEn,
        //                              Price = (decimal)s.Course.Price,
        //                              OldPrice= (decimal)s.Course.OldPrice,
        //                              Desc = s.Course.ShortDescriptionAr,
        //                              Photo = s.Course.Key.SetDownloadFileUrlByKey(storage),
        //                              NoOfVideos = (int)s.Course.NumberLecture,
        //                              Language = s.Course.Language.Name,
        //                              GuId = s.Course.GuId,
        //                              Rate = s.Course.CourseCustomerMapping
        //                              .SelectMany(u => u.CourseComments.Select(ss => ss.Rate))
        //                              .Average().ToString(),
        //                              levelname=s.Course.Level.LevelEn,
        //                              uricouresname=s.Course.uricouresname,
        //                              numofuserRate = Context.CourseComment .Where(c => c.CourseId == s.Id).Count(),
        //                              DurationBySecond = s.Course.DurationBySecond.GetValueOrDefault()
        //                          }).ToList();
        //                return instructor;
        //            default:
        //                instructor = Context.Instructors.Where(c => c.IsDeleted == false && c.IsActive == true && c.Id == instructorId).Select(s => new InstructorDTO
        //                {
        //                    Name = s.NameEn,
        //                    JobTitle = s.JobTitleEn,
        //                    Id = s.Id,
        //                    About = s.AboutEn,
        //                    Photo = s.Picture,
        //                    NumberOfCourses = s.CourseInstructorMapping.Count(),
        //                    NumberOfTalks = s.Talks.Count(),
        //                    GuId = s.GuId,
        //                    Talks = s.Talks.Count,
        //                    Students = s.CourseInstructorMapping.SelectMany(ss => ss.Course.CourseCustomerMapping).Count(),
        //                    NumperOfViews = (from coursINSmap in Context.CourseInstructorMappings
        //                                     join coursdeals in Context.CourseDetails
        //                                     on coursINSmap.CourseId equals coursdeals.CourseId
        //                                     join onlinattend in Context.OnlineAttendance
        //                                     on coursdeals.Id equals onlinattend.CourseDetailsId
        //                                     where coursINSmap.InstructorId == s.Id
        //                                     select onlinattend.CustomerId).Count(),
        //                    metatag = Context.MetaTags.Where(x => x.publicId == s.Id && x.Notes == "Instractor").Select(v => new MetatagDTO
        //                    {
        //                        NameEN = v.NameEN,
        //                        DiscriptionEN = v.DiscriptionEN,
        //                        TitleEN = v.TitleEN,
        //                        Type = v.Type,
        //                        Header = v.Header,
        //                        KeywordsEN = v.KeywordsEN,
        //                        Url = v.Url
        //                    }).FirstOrDefault(),
        //                    uriinstractorname=s.uriinstractorname
        //                }).FirstOrDefault();
        //                if (instructor != null)
        //                    instructor.Courses = Context.CourseInstructorMappings.Where(c => c.InstructorId == instructorId)
        //                       .Select(s => new CourseDTO()
        //                       {
        //                           CourseId = s.CourseId.GetValueOrDefault(),
        //                           Name = s.Course.NameEn,
        //                           Price = (decimal)s.Course.Price,
        //                           OldPrice = (decimal)s.Course.OldPrice,
        //                           Desc = s.Course.ShortDescriptionEn,
        //                           Photo = s.Course.Key.SetDownloadFileUrlByKey(storage),
        //                           NoOfVideos = (int)s.Course.NumberLecture,
        //                           Language = s.Course.Language.Name,
        //                           GuId = s.Course.GuId,
        //                           Rate = s.Course.CourseCustomerMapping.SelectMany(u => u.CourseComments.Select(ss => ss.Rate)).Average().ToString(),
        //                           levelname =lang=="en"? s.Course.Level.LevelEn: s.Course.Level.LevelEn,
        //                           uricouresname = s.Course.uricouresname,
        //                           numofuserRate = Context.CourseComment.Where(c => c.CourseId == s.Id).Count(),
        //                           DurationBySecond = s.Course.DurationBySecond.GetValueOrDefault()
        //                       }).ToList();
        //                return instructor;
        //        }
           
        //}

        public ContentDTO GetOfferSection(string lang)
        {
            ContentDTO Section;
            switch (lang.ToLower())
            {
                case "ar":

                    Section = Context.Content
                        .Where(c => c.Section.Trim() == "Offer").Select(s => new ContentDTO
                    {
                        description = s.TextAr,
                    }).FirstOrDefault();
                    return Section;
                    default:
                    Section = Context.Content
                        .Where(c => c.Section == "Offer").Select(s => new ContentDTO
                    {
                        description = s.TextEn,
                    }).FirstOrDefault();
                    return Section;
            }
        }

        public AboutContentDTO GetAboutContent(string lang)
        {
            AboutContentDTO aboutContent;
            switch (lang.ToLower())
            {
                case "ar":

                    aboutContent = Context.AboutContent.Select(s => new AboutContentDTO
                    {
                        ShortDescription = s.ShortDescriptionAr,
                        FullDescription = s.FullDescriptionAr,
                        Goal = s.GoalAr,
                        Vision = s.VisionAr,
                        Mission = s.MissionAr,
                        Value = s.ValueAr,
                        Video = s.Video
                    }).FirstOrDefault();
                    return aboutContent;
                default:
                    aboutContent = Context.AboutContent.Select(s => new AboutContentDTO
                    {
                        ShortDescription = s.ShortDescriptionEn,
                        FullDescription = s.FullDescriptionEn,
                        Goal = s.GoalEn,
                        Vision = s.VisionEn,
                        Mission = s.MissionEn,
                        Value = s.ValueEn,
                        Video = s.Video
                    }).FirstOrDefault();
                    return aboutContent;
            }
        }


        public async Task<bool> ContactUs(ContactUsDTO data)
        {
            Context.ContactUs.Add(new ContactUs
            {
                Email = data.Email,
                FullName = data.FullName,
                Phone = data.Phone,
                Message = data.Message,
            });

            Context.SaveChanges();

            string message = @"Welcome " + data.FullName.Trim().Split(" ")[0] + "\n Thanks for contacting PlanB\n" +
                "This automatic reply is just to let you know that we received your message and we’ll get back to you with a response as quickly as possible as We can.\n" +
                "Best Regards,\n";

            await mailingService.SendEmailSupportTicket(data.Email, data.EventName,
                data.FullName, data.Email, data.Phone, data.Message);

            await mailingService.SendEmailAsync("info@plan-b-eg.com", "Support Ticket", message, null);

            return true;
        }



        public enum MediaType
        {
            MEETING = 0,
            EVENT = 1,
            MYS = 2,
            MS= 3,
            POC = 4
        }
        public List<MediaTypeDTO> GetAllMedia(string lang, string type, int limit, int page)
        {
            List<MediaTypeDTO> lists;
            switch (lang.ToLower())
            {
                case "ar":

                    lists = Context.Media.Where(c => c.IsDeleted == false && c.IsActive == true && c.MediaType == type )
                      .Skip(limit * (page - 1)).Take(limit)
                      .Select(s => new MediaTypeDTO
                    {
                        Id = s.Id,
                        Address = s.Address,
                        Description = s.DescriptionAr,
                        Date = (DateTime)s.CreationDate,
                        Mobile = s.Mobile,
                        Name = s.NameAr,
                        Picture = s.Picture,
                        }).ToList();

                    return lists;
                default:
                    lists = Context.Media.Where(c => c.IsDeleted == false && c.IsActive == true && c.MediaType == type)
                      .Skip(limit * (page - 1)).Take(limit)
                        .Select(s => new MediaTypeDTO
                    {
                        Id = s.Id,
                        Address = s.Address,
                        Description = s.DescriptionEn,
                        Date = (DateTime)s.CreationDate,
                        Mobile = s.Mobile,
                        Name = s.NameEn,
                        Picture = s.Picture,

                    }).ToList();
            return lists;
            }
        }
       
        public MediaTypeDTO GetMediaDetails(int MediaId, MediaType mediaType, string lang)
        {
            MediaTypeDTO MediaData;
            switch (lang.ToLower())
            {
                case "ar":

                    MediaData = Context.Media.Where(c => c.Id == MediaId).Select(s => new MediaTypeDTO
                    {
                        Id = s.Id,
                        Address = s.Address,
                        Description = s.DescriptionAr,
                        Mobile = s.Mobile,
                        Name = s.NameAr,
                        Picture = s.Picture,
                    }).
                    FirstOrDefault();
                    if (MediaData != null)
                        MediaData.Pictures = Context.PictureMapping.Where(c => c.EntityType == mediaType.ToString() && c.EntityId == MediaId)
                    .Select(s => new PictureDTO() { PictureId = s.PictureId.GetValueOrDefault(), URL = s.Picture.Url }).ToList();
                    //if (MediaData != null)
                        //    MediaData.Videos = Context.VideoMapping.Where(c => c.EntityType == type && c.EntityId == MediaId)
                        //        .Select(s => new MediaDTO() { VedioId = s.VideoId, URL = s.Video.Url }).ToList();

                        return MediaData;
                default:
                    MediaData = Context.Media.Where(c => c.Id == MediaId).Select(s => new MediaTypeDTO
                    {
                        Id = s.Id,
                        Address = s.Address,
                        Description = s.DescriptionEn,
                        Mobile = s.Mobile,
                        Name = s.NameEn,
                        Picture = s.Picture,
                    }).FirstOrDefault();
                    if (MediaData != null)
                        MediaData.Pictures = Context.PictureMapping.Where(c => c.EntityType == mediaType.ToString() && c.EntityId == MediaId)
                            .Select(s => new PictureDTO() { PictureId = s.PictureId.GetValueOrDefault(), URL = s.Picture.Url }).ToList();
                    return MediaData;
            }

        }
        public List<EmployeDTO> GetAllEmploye(string lang, int limit, int page)
        {
            List<EmployeDTO> Employes;
            switch (lang.ToLower())
            {
                case "ar":

                    Employes = Context.Employe
                        .Skip(limit * (page - 1)).Take(limit)
                        .Select(s => new EmployeDTO
                        {
                            Name = s.NameAr,
                            JobTitle = s.JobTitleAr,
                            Id = s.Id,
                            Photo = s.Picture

                        }).ToList();

                    return Employes;
                default:
                    Employes = Context.Employe
                        .Skip(limit * (page - 1)).Take(limit)
                        .Select(s => new EmployeDTO
                        {
                            Name = s.NameEn,
                            JobTitle = s.JobTitleEn,
                            Id = s.Id,
                            Photo = s.Picture
                        }).ToList();

                    return Employes;
            }

        }
        public List<TalkDTO> GetAllTalk(string lang, int limit, int page)
        {
            List<TalkDTO> talks;
            switch (lang.ToLower())
            {
                case "ar":

                    talks = Context.Talks.Where(c => c.Published == true)
                        .Skip(limit * (page - 1)).Take(limit)
                        .Select(s => new TalkDTO
                        {
                           Id = s.Id,
                           //Name = s.NameAr,
                           //Description = s.DescriptionAr,
                           Picture = s.Picture,
                           Video = s.Video,
                           //Intractor = s.Instructor.NameAr,

                        }).ToList();

                    return talks;
                default:
                    talks = Context.Talks.Where(c => c.Published == true)
                        .Skip(limit * (page - 1)).Take(limit)
                        .Select(s => new TalkDTO
                        {
                            Id = s.Id,
                            //Name = s.NameEn,
                            //Description = s.DescriptionEn,
                            Picture = s.Picture,
                            Video = s.Video,
                           // Intractor = s.Instructor.NameEn,

                        }).ToList();

                    return talks;
            }

        }

        public List<Countries> GetAllCountries(string lang, int limit, int page)
        {
            List<Countries> countries;
            switch (lang.ToLower())
            {
                case "ar":
                    countries = Context.Country
                        .Skip(limit * (page - 1)).Take(limit)
                        .Select(s => new Countries
                        {
                            dial_code = s.dial_code,
                            Id = s.Id,
                            name = s.name
                        }).ToList();

                    return countries;
                default:
                    countries = countries = Context.Country
                        .Skip(limit * (page - 1)).Take(limit)
                        .Select(s => new Countries
                        {
                            dial_code = s.dial_code,
                            Id = s.Id,
                            name = s.name

                        }).ToList();

                    return countries;
            }

        }

        public List<string> getinstractoruriName()
        {
            var instractor = Context.Instructors.Where(x=>x.IsActive == true && x.IsDeleted == false).Select(x => x.uriinstractorname).ToList();

            return instractor;
        }
    }
}
