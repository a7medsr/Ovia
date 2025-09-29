using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.Models;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly MomEntity momDb;
        public InstructorController(MomEntity _momDb)
        {
            momDb = _momDb;
        }


        [HttpGet("EnrolledStudents")]
        public async Task<IActionResult> StudentsEnrolledInInstructorCourses(int instructorId)
        {
            var customerCount = await (from i in momDb.CourseInstructorMappings
                                       join c in momDb.CourseCustomerMapping
                                       on i.CourseId equals c.CourseId
                                       where i.InstructorId == instructorId && c.PackageSelectId != null
                                       select c.CustomerId).CountAsync();

            return Ok(customerCount);

        }



        [HttpGet("ActiveCourses")]
        public async Task<IActionResult> ActiveCourses(int instructorId)
        {
            var courseNumber = await momDb.CourseInstructorMappings
                .Where(c => c.InstructorId == instructorId).CountAsync();
            return Ok(courseNumber);

  }

        [HttpGet("TotalEarnings")]
        public async Task<IActionResult> TotalEarnings(int instructorId)
        {
            var total = await momDb.Profit
                .Where(c => c.ProcessTypeId == 20).SumAsync(c=>c.Profit1);
           
            return Ok(total);

        }


        [HttpGet("AverageRate")]
        public async Task<IActionResult> GetInstructorRatingStatistics(int instructorId)
        {
            var query = from i in momDb.CourseInstructorMappings
                        join c in momDb.CourseComment on i.CourseId equals c.CourseId
                        where i.InstructorId == instructorId
                        select c.Rate;

            var rateCount = await query.CountAsync();

            // If there are no rates, set totalRate to 0 and averageRate to null
            if (rateCount == 0)
            {
                var result = new
                {
                    RateCount = rateCount,
                    Average = 0
                };
                return Ok(result);
            }
            else
            {
                //var totalRate = await query.SumAsync();
                var averageRate = await query.AverageAsync();

                var result = new
                {
                    RateCount = rateCount,
                    Average = averageRate
                };

                return Ok(result);
            }
        }

        [HttpGet("Revenue")]
        public async Task<IActionResult> Revenue(int year, int instructorId)
        {
            try
            {
                var data = await (from prof in momDb.Profit
                                  where prof.DistributorId == instructorId &&
                                        prof.ProfitDate != null &&
                                        prof.ProfitDate.Value.Year == year &&
                                        prof.ProcessTypeId == 20
                                  group prof by prof.ProfitDate.Value.Month into g
                                  select new
                                  {
                                      Month = g.Key,
                                      Total = g.Sum(v => v.Profit1)
                                  }).ToListAsync();

                if (data == null || data.Count == 0)
                    return BadRequest("No profit data available for the specified year and Instructor");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("CoursesOverview")]
        public async Task<IActionResult> CoursesOverview(int year, int instructorId)
        {
            try
            {
                // List of all months
                var allMonths = Enumerable.Range(1, 12).Select(m => new { Month = m, TotalEnrollments = 0 }).ToList();

                // Query for enrollments
                var enrollments = await (from i in momDb.CourseInstructorMappings
                                         join c in momDb.CourseCustomerMapping
                                         on i.CourseId equals c.CourseId
                                         where i.InstructorId == instructorId
                                               && c.PackageSelectId != null
                                               && c.CreationDate.HasValue
                                               && c.CreationDate.Value.Year == year
                                         group c by c.CreationDate.Value.Month into g
                                         select new
                                         {
                                             Month = g.Key,
                                             TotalEnrollments = g.Count()
                                         }).ToListAsync();

                // Join the allMonths list with the query result
                var result = from m in allMonths
                             join e in enrollments on m.Month equals e.Month into gj
                             from sub in gj.DefaultIfEmpty()
                             select new
                             {
                                 Month = m.Month,
                                 TotalEnrollments = sub?.TotalEnrollments ?? 0
                             };

                if (result == null || !result.Any())
                    return BadRequest("No course data available for the specified year and instructor");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /*
        [HttpGet("CoursesOverview")]
        public async Task<IActionResult> CoursesOverview(int year, int instructorId)
        {
            try
            {
                var data = await (from i in momDb.CourseInstructorMappings
                                  join c in momDb.CourseCustomerMapping
                                  on i.CourseId equals c.CourseId
                                  where i.InstructorId == instructorId
                                        && c.PackageSelectId != null
                                        && c.CreationDate.HasValue
                                        && c.CreationDate.Value.Year == year
                                  group c by c.CreationDate.Value.Month into g
                                  select new
                                  {
                                      Month = g.Key,
                                      TotalEnrollments = g.Count()
                                  }).ToListAsync();





            
                if (data == null || data.Count == 0)
                    return BadRequest("No Course data available for the specified year and Instructor");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


*/








    }
}
