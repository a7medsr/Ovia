using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ovia.DTO;
using Ovia.Models;
using Ovia.Services;
using Ovia.Services.SendEmails;
using Ovia.Services.StorageFiles;
using static Ovia.Services.Content;

namespace Ovia.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]

    public class InfoController : ControllerBase
    {


        Ovia.Services.Content BusinessContent = new Ovia.Services.Content(); // Fully qualify Content




        //private Content BusinessContent = new Content();
        private int _userId { get; set; }
        public InfoController(IHttpContextAccessor httpContextAccessor)
        {

            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _userId = int.Parse(userId);
            }
        }
        [HttpGet]
        //[Authorize(Roles ="Admin")]
        [Route("api/Info/GetOfferContent")]
        public IActionResult GetOfferContent(string lang = "en")
        {
            try
            {
                var response = BusinessContent.GetOfferSection(lang);
                if (response == null)
                    return StatusCode(404, "Not Found");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet]
        [Route("api/Info/GetAboutContent")]
        public IActionResult GetAboutContent(string lang = "en")
        {
            try
            {
                var response = BusinessContent.GetAboutContent(lang);
                if (response == null)
                    return StatusCode(404, "Not Found");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpPost]
        [Route("api/Info/ContactUs")]
        public IActionResult ContactUs([FromBody] ContactUsDTO contactUsData)
        {
            try
            {
                var response = BusinessContent.ContactUs(contactUsData);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpGet]
        [Route("api/Info/GetAllInstrutors")]
        public IActionResult GetAllInstrutors(string lang = "en", int limit = 10, int page = 1)
        {
            try
            {
                var res = BusinessContent.GetAllInstructor(lang, limit, page);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
        //[HttpGet]
        //[Route("api/Info/GetInstructorDetails")]
        //public IActionResult GetInstructorDetails(string urlinstractorName,int instructorId, string lang = "en")
        //{
        //    try
        //    {
        //        var res = BusinessContent.GetInstructorDetails(urlinstractorName,instructorId, lang);

        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }

        //}

        [HttpGet]
        [Route("api/Media/GetAllMedia")]
        public IActionResult GetAllMedia(string lang = "en", string mediaType = "MEETING", int limit = 10, int page = 1)
        {
            try
            {
                var res = BusinessContent.GetAllMedia(lang, mediaType, limit, page);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
     
        [HttpGet]
        [Route("api/Media/GetMediaDetails")]
        public IActionResult GetMediaDetails(int MediaId, MediaType mediaType, string lang = "en")
        {
            try
            {
                var res = BusinessContent.GetMediaDetails(MediaId, mediaType, lang);
                if (res == null)
                    return StatusCode(204, "Not Found");
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
        [HttpGet]
        [Route("api/Info/GetAllEmployes")]
        public IActionResult GetAllEmployes(string lang = "en", int limit = 10, int page = 1)
        {
            try
            {
                var res = BusinessContent.GetAllEmploye(lang, limit, page);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
        [HttpGet]
        [Route("api/Info/GetAllTalks")]
        public IActionResult GetAllTalks(string lang = "en", int limit = 10, int page = 1)
        {
            try
            {
                var res = BusinessContent.GetAllTalk(lang, limit, page);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }

        [HttpGet]
        [Route("api/Info/getinstractoruriName")]
        public IActionResult getinstractoruriName()
        {
            try
            {
                var res = BusinessContent.getinstractoruriName();

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
        //[HttpGet]
        //[Route("api/Info/GetAllCountries")]
        //public IActionResult GetAllCountries(string lang = "en", int limit = 10, int page = 1)
        //{
        //    try
        //    {
        //        var res = BusinessContent.GetAllCountries(lang, limit, page);

        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }

        //}
    }
}
