using Microsoft.AspNetCore.Mvc;
using Ovia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ovia.Services;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfitController : ControllerBase
    {
        private readonly MomEntity dbContext;

        public ProfitController(MomEntity dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfitReport(int memberId)
        {
            try
            {
                // Get the earliest profit date for the member
                DateTime? earliestProfitDate = await dbContext.Profit
                    .Where(p => p.DistributorId == memberId)
                    .MinAsync(p => p.ProfitDate);

                if (earliestProfitDate.HasValue)
                {
                    // Initialize a list to store all profit data for each period
                    var profitList = new List<object>();

                    // Start from the earliest profit date and iterate forwards
                    DateTime currentDate = GetStartOfWeek(earliestProfitDate.Value);

                    // Loop through each week starting from the earliest profit date
                    while (currentDate <= DateTime.Now)
                    {
                        // Calculate the start and end dates of the current week
                        DateTime weekStartDate = currentDate;
                        DateTime weekEndDate = GetEndOfWeek(currentDate);

                        // Retrieve profit data for the current week
                        var renewalData = dbContext.Profit
                            .Where(p => p.DistributorId == memberId &&
                                        p.ProfitDate >= weekStartDate.Date &&
                                        p.ProfitDate <= weekEndDate.Date)
                            .GroupBy(p => p.ProcessTypeId)
                            .Select(g => new
                            {
                                ProcessTypeId = g.Key,
                                TotalProfit = g.Sum(p => p.Profit1)
                            })
                            .ToList();

                        // Calculate bonus, sponsor, and total for the week
                        decimal bonus = renewalData
                            .FirstOrDefault(p => p.ProcessTypeId == 2)?.TotalProfit ?? 0;
                        decimal sponsor = renewalData
                            .Where(p => p.ProcessTypeId == 1 || p.ProcessTypeId == 5)
                            .Sum(p => p.TotalProfit ?? 0);
                        decimal total = renewalData.Sum(p => p.TotalProfit ?? 0); // Handling nullable decimal

                        var info = await dbContext.CustomerAttributes
                            .Where(c => c.Id == memberId)
                            .Include(c => c.CustomerInfo)
                            .FirstOrDefaultAsync();

                        // Add profit data for the current week to the list
                        if (total != 0)
                        {
                            profitList.Add(new
                            {
                                Week_Date_From = weekStartDate.ToString("dddd dd/MM/yyyy"),
                                Week_Date_To = weekEndDate.ToString("dddd dd/MM/yyyy"),
                                Checks = bonus,
                                Direct = sponsor,
                                Total = total,
                                TotalInWords = NumberToWordsConverter.ConvertToWords(total),
                                Name = info.CustomerInfo.NameEn
                            });
                        }
                        // Move to the next week
                        currentDate = weekEndDate.AddDays(1);
                    }

                    // Return the list of profit data for all periods
                    return Ok(profitList);
                }

                return NotFound(); // No profit data found
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //[HttpGet("total_profit")]
        //public async Task<int> totalprofit(int usertid)
        //{
        //    int profit = (int)dbContext.Profit.Where(x => x.DistributorId == usertid).Sum(x => x.Profit1);
        //    return profit;
        //    // return Ok(profit.ToString());
        //}

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Friday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        private DateTime GetEndOfWeek(DateTime date)
        {
            return GetStartOfWeek(date).AddDays(6);
        }
    }
}
