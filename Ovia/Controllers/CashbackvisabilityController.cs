using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using Polly;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashbackvisabilityController : ControllerBase
    {
        private readonly MomEntity momDb;

        public CashbackvisabilityController(MomEntity _momDb)
        {
            momDb = _momDb;
        }


        [HttpPost("AddcashbackDAy")]
        [AllowAnonymous]
        public async Task<IActionResult> AddcashbackDAy([FromForm] CashbackvisablityDTO model)
        {
            DateTime f = DateTime.Now;
            DateTime timfrom = Convert.ToDateTime(model.datefrom);
            DateTime timto = Convert.ToDateTime(model.dateto);

            var adday = new VisabltyCash()
            {
                timefrom = timfrom,
                timeto = timto,
                day = model.day,
                IsActive = model.IsActive,
                Notes = model.Notes
            };

            momDb.VisabltyCash.Add(adday);
           await momDb.SaveChangesAsync();
            return Ok(adday);
        }




        [HttpDelete("deleteday")]
        [AllowAnonymous]
        public async Task<IActionResult> deleteday(int id)
        {
            var day = await momDb.VisabltyCash.FirstOrDefaultAsync(c => c.id == id);

            if (day != null)
            {
                momDb.VisabltyCash.Remove(day);
                await momDb.SaveChangesAsync();
                return Ok("Day deleted successfully");
            }
            else
            {
                return BadRequest("This day not found");
            }
        }


        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {

            var day = await momDb.VisabltyCash.ToListAsync();

            if (day == null)
            {
                return BadRequest("Not found any period");
            }
            else
                return Ok(day);
        }






    }
}
