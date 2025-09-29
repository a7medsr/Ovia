using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ovia.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly MomEntity _momDb;
        private readonly string _uploadFolder = "https://testphoto.violetdecoreg.com/media/";
        // private readonly string _uploadFolder = @"E:\training\Momentum 1\Momentumbackend\Momentum\Uploads\";

        //private readonly string _uploadUrl = "https://testphoto.violetdecoreg.com/api/upload";
        private readonly string _uploadUrl = "https://testphoto.violetdecoreg.com/media/";




        public ImageController(MomEntity momDb)
        {
            _momDb = momDb;
        }

        [HttpPost("Files")]
        public async Task<IActionResult> Files(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);
                    var response = await client.PostAsync(_uploadUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var photo = new Photo
                        {
                            Id = 0,
                            FileName = file.FileName,
                            FilePath = _uploadUrl, // Assuming you want to store the URL of the uploaded image
                            UploadDate = DateTime.Now
                        };

                        _momDb.Photo.Add(photo);
                        await _momDb.SaveChangesAsync();

                        return Ok(new { photo.Id, photo.FileName, photo.FilePath });
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
        }



        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_uploadFolder, fileName);

            var extension = Path.GetExtension(file.FileName);

            var newFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
            var path = Path.Combine("Uploads", newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var photo = new Photo
            {
                Id = 0,
                FileName = newFileName,
                FilePath = path,
                UploadDate = DateTime.Now
            };

            _momDb.Photo.Add(photo);
            await _momDb.SaveChangesAsync();

            return Ok(new { photo.Id, photo.FileName, photo.FilePath });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = await _momDb.Photo.FindAsync(id);

            if (photo == null)
                return NotFound();

            var stream = new FileStream(photo.FilePath, FileMode.Open);
            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = photo.FileName
            };
        }
    }
}



