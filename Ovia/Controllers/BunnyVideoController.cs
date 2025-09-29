using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BunnyVideoController : ControllerBase
    {
        private readonly MomEntity _momDb;
        private readonly string accessKey;

        public BunnyVideoController(MomEntity momDb)
        {


            _momDb = momDb;
             accessKey = "6ccd07fd-9027-4fab-abbd6e59f3b0-4a55-4070";
        }
        //video id =>  b24b3df6-cba9-4291-85dd-16ddc69f0159
        //[HttpGet]
        //[Route("GetVideo")]


        [HttpGet]
        [Route("GetCourseVideos")]
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

                var videoList = new List<BunnyVideoDTO>();

                foreach (var lecture in course.CourseDetails)
                {
                    var video = new BunnyVideoDTO();

                    if (lecture.IsActive == true)
                    {
                        var res = await GetVideo(lecture.OnlineId);
                        if (res != null && res is OkObjectResult okResult)
                        {
                            //var videoDetails = okResult.Value as VideoDTO;
                            //video.LectureName = lecture.NameEn;
                            //video.IsActive = (bool)lecture.IsActive;
                            //video.VideoDetails = videoDetails;

                            //videoList.Add(video);
                            if (okResult.Value != null)
                            {
                                // Deserialize the response content to VideoDTO
                                var videoDetails = JsonConvert.DeserializeObject<VideoDTO>(okResult.Value.ToString());
                                video.LectureName = lecture.NameEn;
                                video.IsActive = (bool)lecture.IsActive;
                                video.VideoDetails = videoDetails;

                                videoList.Add(video);
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










        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<IActionResult> GetVideo(string videoId)
        {
            var client = new RestClient($"https://video.bunnycdn.com/library/234264/videos/{videoId}");
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("AccessKey", accessKey);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            else
            {
                return StatusCode(500);
            }
        }






        [HttpPost]
        [Route("AddVideo")]
        public async Task<IActionResult> UploadVideo(IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var client = new RestClient("https://video.bunnycdn.com/library/234264/videos");
            var request = new RestRequest(Method.POST);
            request.AddHeader("AccessKey", "6ccd07fd-9027-4fab-abbd6e59f3b0-4a55-4070");

            // Add file data to request
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await videoFile.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Specify the parameter name for the file in the request
            request.AddFileBytes("file", fileBytes, videoFile.FileName, "video/mp4");

            // Construct the JSON body with other metadata
            var body = new
            {
                title = "Lecture 1",
                thumbnailTime = DateTimeOffset.Parse("2024-04-24").ToUnixTimeSeconds() // Convert the date to Unix timestamp
            };
            request.AddJsonBody(body);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            else
            {
                return BadRequest(response.Content);
            }
        }
    





    }
}
