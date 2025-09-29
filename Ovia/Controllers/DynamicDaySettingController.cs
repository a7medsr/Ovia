using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using Ovia.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ovia.Controllers
{
    [ApiController]
   // [Authorize]
    public class DynamicDaySettingController : ControllerBase
    {
        private readonly MomEntity momDb;

        private DynamicDaysetting daysitting = new DynamicDaysetting();
        private int _userId { get; set; }
        public DynamicDaySettingController(IHttpContextAccessor httpContextAccessor, MomEntity _momDb)
        {
            momDb = _momDb;
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _userId = int.Parse(userId);
            }
        }


        [HttpGet]
        [Route("api/DynamicDaySettingController/AddDaySetting")]
      ///  [Authorize]
        public async Task<IActionResult> AddDaysetting()
        {
            try
            {
                var response =await addDaysetting();
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
        [Route("api/DynamicDaySettingController/ThisWeekDirect")]
       // [Authorize]
        public async Task<IActionResult> ThisWeekDirect(int customerId)
        {
            try
            {

                var datesobj =await addDaysetting();

                var weekdirect = await (from custnet in momDb.CustomerNetwork
                                  where custnet.SponsorId == customerId &&
                                  (custnet.CreationDate >= datesobj.Report_0_Date_From &&
                                  custnet.CreationDate <= datesobj.Report_0_Date_To)
                                  select custnet.ChildId).CountAsync();


                if (weekdirect == null)
                    return StatusCode(404, "Not Found");
                return Ok(weekdirect);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

     
        
        [HttpGet]
        [Route("api/DynamicDaySettingController/LastWeekDirect")]
        //[Authorize]
        public async Task<IActionResult> LastWeekDirect(int customerId)
        {
            try
            {
                var datesobj =await addDaysetting();

                var weekdirect = await (from custnet in momDb.CustomerNetwork
                                  where custnet.SponsorId == customerId &&
                                  (custnet.CreationDate >= datesobj.Report_1_Date_From &&
                                  custnet.CreationDate <= datesobj.Report_1_Date_To)
                                  select custnet.ChildId).CountAsync();


                if (weekdirect == null)
                    return StatusCode(404, "Not Found");
                return Ok(weekdirect);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }








        private async Task<daysettingDTO> addDaysetting()
        {
            var checkday = await (from d in momDb.Dinamicday
                                  where d.IsActive == true && d.IsDeleted == false
                                  select d.dayfrom).FirstOrDefaultAsync();

            if (checkday == null)
            {
                return null;
            }

            string checkdayString = checkday.ToString();
            DateTime todayDate = DateTime.Today;

            while (todayDate.DayOfWeek.ToString() != checkdayString)
            {
                todayDate = todayDate.AddDays(-1);
            }

            DateTime report1DateFrom = todayDate.AddDays(-7);
            var daysetting = new daysettingDTO
            {
                Report_0_Date_From = todayDate,
                Report_0_Date_To = todayDate.AddDays(6),
                Report_0_Date_Begain = todayDate.AddDays(7),
                Report_1_Date_From = report1DateFrom,
                Report_1_Date_To = report1DateFrom.AddDays(6)
            };

            return daysetting;
        }









    }
}
