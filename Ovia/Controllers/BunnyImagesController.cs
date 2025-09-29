using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ovia.Models;
using RestSharp;
using System.Net.Http.Headers;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Polly.Bulkhead;
using MailChimp.Net.Models;




namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BunnyImagesController : ControllerBase
    {
        private readonly MomEntity momDb;


        public BunnyImagesController(MomEntity _momDb)
        {
            momDb = _momDb;
        }


        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadFileAsync(IFormFile image, int customerId)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            const string BASE_HOSTNAME = "storage.bunnycdn.com";
            const string STORAGE_ZONE_NAME = "momentumfiles";  // Replace with your actual storage zone name
            string FILENAME_TO_UPLOAD = image.FileName;  // Use the uploaded file's original name
            const string ACCESS_KEY = "839da027-876d-4e56-a71837a9fff3-0bab-4ba6";  // Replace with your actual access key
            const string CONTENT_TYPE = "application/octet-stream";


          
          
            Photo existingPhoto = null;
            do
            {
                existingPhoto = momDb.Photo.FirstOrDefault(p => p.FileName == FILENAME_TO_UPLOAD);
                if (existingPhoto != null)
                {
                    FILENAME_TO_UPLOAD = GenerateNewFilename(FILENAME_TO_UPLOAD);
                }
            } while (existingPhoto != null);


            string url = $"https://{BASE_HOSTNAME}/{STORAGE_ZONE_NAME}/{FILENAME_TO_UPLOAD}";

            using (HttpClient client = new HttpClient())
            {
                // Set the access key in the header
                client.DefaultRequestHeaders.Add("AccessKey", ACCESS_KEY);

                // Read the file into a byte array
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                using (var content = new ByteArrayContent(fileBytes))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(CONTENT_TYPE);

                    // Make the PUT request to upload the file
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        momDb.Photo.Add(new Photo{
                            Id=0,
                            FileName = FILENAME_TO_UPLOAD,
                            FilePath = url,
                            CustomerId = customerId,
                            UploadDate = DateTime.Now
                        });
                        await momDb.SaveChangesAsync();



                        return Ok("File uploaded successfully.");
                    }
                    else
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, $"Failed to upload file. Response: {responseBody}");
                    }
                }
            }
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private string GenerateNewFilename(string originalFilename)
        {
            
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            return $"{Path.GetFileNameWithoutExtension(originalFilename)}_{timestamp}{Path.GetExtension(originalFilename)}";
        }







        [HttpGet("DownloadImage")]
        public async Task<IActionResult> DownloadImage(int customerId)
        {
            var photo = await momDb.Photo
                                .Where(c => c.CustomerId == customerId)
                                .OrderByDescending(p => p.UploadDate) // Assuming you want the most recently uploaded photo
                                .LastOrDefaultAsync();

            if (photo == null)
            {
                return NotFound("No photo found for the given customer ID.");
            }

            var client = new RestClient($"https://storage.bunnycdn.com/momentumfiles/{photo.FileName}");
            var request = new RestRequest();
            request.AddHeader("accept", "*/*");
            request.AddHeader("AccessKey", "839da027-876d-4e56-a71837a9fff3-0bab-4ba6");

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                // Assuming the content is an image, return it as a file result
                byte[] imageBytes = response.RawBytes;
                string contentType = "image/jpeg";  // Adjust content type as necessary
                return File(imageBytes, contentType, "image.jpg");
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"Failed to download image. Response: {response.Content}");
            }
        }





        [HttpGet("GetImageUrl")]
        public async Task<IActionResult> GetImageUrl(int customerId)
        {
            // Order by the primary key or a suitable column to ensure a deterministic sort order
            var photo = await momDb.Photo
                                  .Where(c => c.CustomerId == customerId)
                                  .OrderByDescending(p => p.UploadDate) // Assuming you want the most recently uploaded photo
                                  .FirstOrDefaultAsync();

            if (photo == null)
            {
                return NotFound("No photo found for the specified customer.");
            }

            string url = $@" https://momentum2.b-cdn.net/{photo.FileName}";
            // string url = $"https://storage.bunnycdn.com/momentumfiles/{photo.FileName}";
            // string imgUrl = photo.FilePath;

            // return Ok(new { imageUrl = imgUrl });
            return Ok(url);
        }

        



    }
}
