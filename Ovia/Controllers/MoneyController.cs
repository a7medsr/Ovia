using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ovia.Services;
using MailChimp.Net.Models;
using Polly;
using Ovia.Models;

namespace Ovia.Controllers
{
    
    [ApiController]
    //[Authorize]
    public class MoneyController : ControllerBase
    {
        private readonly MomEntity momDb;


        private int _userId { get; set; }
        public MoneyController(IHttpContextAccessor httpContextAccessor, MomEntity _momDb)
        {
            momDb =_momDb;
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _userId = int.Parse(userId);
            }
        }

        

        //[Authorize]
        [HttpGet]
        [Route("api/MonyController/Total_BV_Right")]
        public IActionResult Total_BV_Right(int customerId)
        {
            var pointcount = (from pnt in momDb.Points

                              where pnt.CustomerId == customerId
                              && pnt.Side == "Right"
                              select pnt.PointCount).Sum();


            if (pointcount == 0) 
                return BadRequest("Not found");
            
            return Ok(pointcount);
        }




       // [Authorize]
        [HttpGet]
        [Route("api/MonyController/Total_BV_Left")]
        public IActionResult Total_BV_Left(int customerId)
        {

            // var res = monydeal.Total_BV_Left(lang, _userId);

            var pointcount = (from pnt in momDb.Points

                              where pnt.CustomerId == customerId
                             && pnt.Side == "Left"
                              select pnt.PointCount).Sum();


            if (pointcount == 0)
                return BadRequest("Not found");
            
            return Ok(pointcount);
        }
        

        //[Authorize]
        [HttpGet]
        [Route("api/MonyController/UNTotal_BV_Right")]
        public IActionResult UNTotal_BV_Right(int customerId)
        {

            //var res = monydeal.UNTotal_BV_Right(lang, _userId);
            var pointcount = (from pnt in momDb.Points

                              where pnt.CustomerId == customerId
                              && (pnt.ProcessTypeId == 3 || pnt.ProcessTypeId == 4) 
                              && pnt.Side == "Right" && pnt.IsCalculated == false
                              select pnt.PointCount).Sum();

            if (pointcount == 0)
                return BadRequest();
            
            return Ok(pointcount);
        }

        

        //[Authorize]
        [HttpGet]
        [Route("api/MonyController/UNTotal_BV_Left")]
        public IActionResult UNTotal_BV_Left(int customerId)
        {
            var pointcount = (from pnt in momDb.Points

                              where pnt.CustomerId == customerId
                              && (pnt.ProcessTypeId == 3 || pnt.ProcessTypeId == 4) && pnt.Side == "Left" && pnt.IsCalculated == false
                              select pnt.PointCount).Sum();


            if (pointcount == 0)
            {
                return BadRequest("Not found");
            }
            return Ok(pointcount);
        }
        
        //[Authorize]
        [HttpGet]
        [Route("api/MonyController/Details_Right")]
        public IActionResult Details_Right(int customerId)
        {



            var pointcount = (from custattrp in momDb.CustomerAttributes
                              join custinfo in momDb.CustomerInfo
                             on custattrp.CustomerInfoId equals custinfo.Id


                              join point in momDb.Points
                              on custattrp.Id equals point.CustomerId


                              join prcesstyp in momDb.ProcessType
                              on point.ProcessTypeId equals prcesstyp.Id

                              join pointprocess in momDb.PointProcess
                              on point.PointProcessId equals pointprocess.Id

                              orderby point.PointDate descending

                              where prcesstyp.Id == 3
                              && point.Side == "Right" && custattrp.Id == customerId
                              select new
                              {

                                  point.CustomerId,
                                  point.PointProcessId,
                                  pointprocess.ForCustomerId,
                                  custinfo.NameEn,
                                  point.PointCount,
                                  point.Side,
                                  point.ProcessTypeId,
                                  prcesstyp.Process,
                                  point.PointDate,
                                  point.CalculationDate,
                                  point.IsCalculated,
                                  point.IsFlashed,
                                  point.FlashDate

                              }).ToList();




            if (pointcount.Count == 0)
              return BadRequest("Not found");
            
            return Ok(pointcount);
        }

        //Details_Left

        //[Authorize]
        [HttpGet]
        [Route("api/MonyController/Details_Left")]
        public IActionResult Details_Left(int customerId)
        {
            var pointcount = (from custattrp in momDb.CustomerAttributes
                              join custinfo in momDb.CustomerInfo
                              on custattrp.CustomerInfoId equals custinfo.Id


                              join point in momDb.Points
                              on custattrp.Id equals point.CustomerId


                              join prcesstyp in momDb.ProcessType
                              on point.ProcessTypeId equals prcesstyp.Id

                              join pointprocess in momDb.PointProcess
                              on point.PointProcessId equals pointprocess.Id

                              orderby point.PointDate descending

                              where prcesstyp.Id == 3
                              && point.Side == "Left" && custattrp.Id == customerId
                              select new
                              {

                                  point.CustomerId,
                                  point.PointProcessId,
                                  pointprocess.ForCustomerId,
                                  custinfo.NameEn,
                                  point.PointCount,
                                  point.Side,
                                  point.ProcessTypeId,
                                  prcesstyp.Process,
                                  point.PointDate,
                                  point.CalculationDate,
                                  point.IsCalculated,
                                  point.IsFlashed,
                                  point.FlashDate

                              }).ToList();


             


            if (pointcount.Count == 0)
                return BadRequest();
            
            return Ok(pointcount);
        }

        //[Authorize]
        [HttpGet]
        [Route("api/MonyController/getTOtalPouns")]
        public IActionResult getTOtalPouns(int customerId)
        {

            var totalbouns = (from custattrep in momDb.CustomerAttributes
                              join custdisc in momDb.CustomerDiscounts
                              on custattrep.Id equals customerId

                              where custattrep.CustomerInfoId == customerId
                              select (custdisc.Credit)).Sum();

            var bouns = (from profit in momDb.Profit
                         join custattrep in momDb.CustomerAttributes
                         on profit.DistributorId equals custattrep.Id
                         where custattrep.CustomerInfoId == customerId
                         select (profit.Profit1)).Sum();

            if (totalbouns + bouns == 0)
                return BadRequest();
            
            return Ok(totalbouns + bouns);
        }

       // [Authorize]
        [HttpGet]
        [Route("api/MonyController/getTOtaldirect")]
        public IActionResult getTOtaldirect(int sponcerid)
        {


            var totaldirect = (from custnetw in momDb.CustomerNetwork
                               where custnetw.SponsorId == sponcerid
                               select custnetw.ChildId == null ? 0 : custnetw.ChildId).Count();

            if (totaldirect == 0)
            {
                return BadRequest();
            }
            return Ok(totaldirect);
        }

        //[Authorize]
        [HttpGet]
        [Route("api/MonyController/getAllDiscount")]
        public IActionResult getAllDiscount(int custid)
        {

            var totaldiscount = (from custdisc in momDb.CustomerDiscounts
                                 join custattrp in momDb.CustomerAttributes
                                 on custdisc.CustomerId equals custattrp.Id
                                 where custattrp.CustomerInfoId == custid
                                 select custdisc.Credit == null ? 0 : custdisc.Credit).Sum();
            if (totaldiscount == 0)
                return BadRequest();
            
            return Ok(totaldiscount);
        }
    }
}
