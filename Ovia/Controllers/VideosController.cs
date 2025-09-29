using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using Ovia.Services;
using RestSharp;

namespace Ovia.Controllers
{
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly MomEntity _momDb;

        public VideosController(MomEntity momDb)
        {
            _momDb = momDb;
        }

        [HttpGet]
        [Route("api/Videos/GetVideoOTPById")]
        public IActionResult GetVideoOTPById(string videoId)
        {
            try
            {
                var res = GetVideo(videoId);
                if (res == null)
                    return StatusCode(204, "Not Found");

                return Ok(res); // Return the OTPDTO 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet]
        [Route("api/Videos/GetCourseVideos")]
        public async Task<IActionResult> GetCourseVideos(int courseId)
        {
            try
            {
                var course = await _momDb.Courses
                    .Where(c => c.Id == courseId)
                    .Include(c => c.CourseDetails)
                    .FirstOrDefaultAsync();

                if (course == null)
                    return NotFound("Course not found");

                if (course.CourseDetails == null || !course.CourseDetails.Any())
                    return NotFound("No videos found in this course");

                var videoList = new List<OTPDTO>();
                foreach (var lecture in course.CourseDetails)
                {
                  if(lecture.IsActive == true)
                    {
                        var res = GetVideoOTPById(lecture.OnlineId);
                        if (res != null && res is OkObjectResult okResult)
                        {
                            var otpDto = okResult.Value as OTPDTO;
                            otpDto.LectureName = lecture.NameEn;
                            if (otpDto != null)
                            {
                                videoList.Add(otpDto);

                            }
                        }
                    }
                }

                return Ok(videoList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        //[HttpGet]
        //[Route("api/Videos/GetCourseVideos")]
        //public async Task<IActionResult> GetCourseVideosAsync(int courseId)
        //{
        //    try
        //    {
        //        var course = await _momDb.Courses
        //            .Where(c => c.Id == courseId)
        //            .Include(c => c.CourseDetails)
        //            .FirstOrDefaultAsync();

        //        if (course == null)
        //            return NotFound("Course not found");

        //        if (course.CourseDetails == null || !course.CourseDetails.Any())
        //            return NotFound("No videos found in this course");

        //        var videoList = new List<OTPDTO>();
        //        foreach (var videoId in course.CourseDetails)
        //        {
        //            var res = GetVideoOTPById(videoId.OnlineId);
        //            if (res != null)
        //            {
        //                videoList.Add((OTPDTO)res);
        //            }
        //        }

        //        return Ok(videoList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}



        [ApiExplorerSettings(IgnoreApi = true)]
        private OTPDTO GetVideo(string videoId)
        {
            RestClient client = new RestClient("https://dev.vdocipher.com/api/videos/" + videoId + "/otp");
            client.AddDefaultHeader("Authorization", "Apisecret CAmWqPxOGIy4bkhgj7akgDVcd2cq9PK8zJlZzmrlN8JEC4FaaJwNXxYcB5xqQe20");

            var res = new OTPDTO();
            res = CallCustomerAPI.CallOutAPIWithAuth(client, new OTPDTO(), 0).Result;

            if (string.IsNullOrEmpty(res.otp) || string.IsNullOrEmpty(res.otp))
                return null;
            return res;
        }




    }
}
