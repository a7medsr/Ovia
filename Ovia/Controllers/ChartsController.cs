using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.Models;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly MomEntity momDb;

        public ChartsController(MomEntity _momDb)
        {
            momDb = _momDb; 
        }


        [HttpGet]
        [Route("GetLeftCv_RightCV")]
        public async Task<IActionResult> GetLeftCv_RightCV(int CustomerId)
        {
            var points = await momDb.Points
                                    .Where(c => c.CustomerId == CustomerId && c.IsCalculated == false)
                                    .ToListAsync();

            var leftCv = points
                             .Where(s => s.Side == "Left")
                             .Sum(c => c.PointCount);

            var rightCv = points
                              .Where(s => s.Side == "Right")
                              .Sum(c => c.PointCount);

            var totalProfit = await momDb.Profit
                                         .Where(c => c.DistributorId == CustomerId && c.ProcessTypeId == 2)
                                         .SumAsync(c => c.Profit1);



            return Ok(new
            {
                LeftCv = leftCv,
                RightCv = rightCv,
                TotalProfit = totalProfit
            });
        }


        //   [Authorize]
        //[HttpPost]
        [HttpGet("ProfitChart")]
        public async Task<IActionResult> ProfitChart(int year, int custid)
        {
            try
            {
                var data = await (from profit in momDb.Profit
                                  join custatr in momDb.CustomerAttributes
                                  on profit.DistributorId equals custatr.Id
                                  where profit.ProfitDate != null && profit.ProfitDate.Value.Year == year && custatr.Id == custid
                                  group profit by profit.ProfitDate.Value.Month into g
                                  select new { month = g.Key, total = g.Sum(v => v.Profit1) }).ToListAsync();

                if (data == null || data.Count == 0)
                    return BadRequest("No profit data available for the specified year and customer");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // [Authorize]
        //  [HttpPost]
        [HttpGet("SellesChart")]
        public async Task<IActionResult> SellesChart(int year, int userId)
        {
            try
            {
                var data = await (from custnet in momDb.CustomerNetwork
                                  join custatr in momDb.CustomerAttributes
                                  on custnet.ChildId equals custatr.Id
                                  where custnet.CreationDate != null && custnet.CreationDate.Value.Year == year &&
                                  custnet.UplineHistoryId.Contains(userId.ToString()) && custatr.IsActive == true
                                  group custnet by custnet.CreationDate.Value.Month into g
                                  select new { month = g.Key, total = g.Count() }).ToListAsync();

                if (data == null || data.Count == 0)
                    return StatusCode(303, "No selles data available for the specified year and user");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }
}
