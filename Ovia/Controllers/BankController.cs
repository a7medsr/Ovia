using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;

using Ovia.Models;
using Ovia.Services.StorageFiles;
using Polly;

namespace Ovia.Controllers
{
    //   [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class BankController : ControllerBase
    {
        private Bank bank = new Bank();
        private readonly MomEntity momDb;

        private int _userId { get; set; }
        public BankController(IHttpContextAccessor httpContextAccessor, MomEntity _momDb)
        {

            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _userId = int.Parse(userId);
            }

            this.momDb = _momDb; 
        }

     

        // GET: api/CustomerInfo
        #region Transfer money from user to user
        [HttpPost]
        [Route("api/BankController/TransferMoney")]
        public async Task<IActionResult> TransferMoney(TransferDTO  dto)
        {
            var RecieverData = await Customer_Info(dto.RecieverBackOfficeId);
            var SenderData = await  Customer_Info(dto.SenderBackOfficeId);

            if (RecieverData == null)
                return BadRequest("Sender not found");

            if (SenderData == null)
                return BadRequest("Receiver not found");

            var senderBalance = await momDb.NsStartingBalances
                .Where(u => u.NsId == SenderData.Id).
                OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync(); ;

            if (dto.Amount < 15)
                return BadRequest("Amount must be equal to or more than $15");

            if (senderBalance.Remain < dto.Amount)
                return BadRequest("Your balance is less than the amount");


            senderBalance.paid += dto.Amount;
            senderBalance.Remain -= dto.Amount;
            await momDb.SaveChangesAsync();

            // Generate  NsTransactionId serial
            string TransactionId = GenerateTransactionId(SenderData.Id);

            var PaymentHistory = new NsPaymentHistory
            {
                Id=0,
                NS_ID = SenderData.Id,
                Amount = dto.Amount,
                Transfered_To_ID = RecieverData.Id,
                Transaction_ID = TransactionId,
                Transaction_Date = DateTime.Now,
                Notes = $@"Transfer money to {dto.RecieverBackOfficeId}  with transaction serial {TransactionId}"
            };
            momDb.NsPaymentHistory.Add(PaymentHistory);
            await momDb.SaveChangesAsync();


            var balance =await Receiver_SignupBalance(RecieverData.Id);

           
            var RecieversignupBalance = new CustomerAccountBalanceSingUp
            {
                Id = 0,
                CustomerId = RecieverData.Id,
                Debit = 0,
                Credit = dto.Amount,
                Balance = balance != null ? balance.Balance + dto.Amount : dto.Amount,
                TransactionDate = DateTime.Now,
                NsTransactionId = TransactionId,
                Description= $@" Transfer from {dto.SenderBackOfficeId} with transaction serial {TransactionId}"
            };
            momDb.CustomerAccountBalanceSingUps.Add(RecieversignupBalance);
            
            

            await momDb.SaveChangesAsync();
            return Ok("Money transferred successfully");

        }


        //get User Data
        private async Task<CustomerAttribute> Customer_Info(string BackOfficeId)
        {
            var Info = await momDb.CustomerAttributes.FirstOrDefaultAsync(c => c.ReferId == BackOfficeId);
            return Info; // Since the return type is Task<CustomerAttributes>, you can't return BadRequest here
        }
        private async Task<CustomerAccountBalanceSingUp> Receiver_SignupBalance(int customerAttributeId)
        {
            var balance = await momDb.CustomerAccountBalanceSingUps
                .Where(c => c.CustomerId == customerAttributeId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            return balance;
        }
        // Generate transaction ID
        // Generate transaction ID
        private string GenerateTransactionId(int senderId)
        {
            Random rand = new Random();
            return $"{senderId}{rand.Next(1000, 10000)}";
        }

        #endregion

        //request cash money
        [HttpGet]
        [Route("api/BankController/RequestCash")]
        public async Task<IActionResult> RequestCash(int balanceId)
        {
            var customerAccountBalance = await momDb.CustomerAccountBalances
                                                   .FirstOrDefaultAsync(c => c.Id == balanceId);

            if (customerAccountBalance.Balance != 0)
            {
                var existingCard = await momDb.RequestsCards
                   .FirstOrDefaultAsync(c =>
                   c.CustomerId == customerAccountBalance.CustomerId
                     && c.IsApproved == true);
                   
                if(existingCard == null) {
                    return BadRequest("You cant request cash before add payment method");
                }


                var cash = new RequestCash
                {
                    Id = 0,
                    CustomerId = customerAccountBalance.CustomerId,
                    CreatedBy = customerAccountBalance.CustomerId,
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    IsPaid = false,
                    PaidBy = null,
                    PaidDate = null,
                    RequestDate = DateTime.Now,
                    RequestedAmount = customerAccountBalance.Balance,

                };
                momDb.RequestCash.Add(cash);
                await momDb.SaveChangesAsync();

                var customer_balance = new CustomerAccountBalance
                {
                    Id = 0,
                    CustomerId = customerAccountBalance.CustomerId,
                    Debit = customerAccountBalance.Balance,
                    TransactionDate = DateTime.Now,
                    Description = "Request caching balance",
                    Credit = 0,
                    Balance = 0,
                     

                };
                momDb.CustomerAccountBalances.Add(customer_balance);
                await momDb.SaveChangesAsync();


                return Ok(cash);

            }
            else
                return NotFound("Dont found any money in your balance");
        }




     //   [Authorize]
        [HttpGet]
        [Route("api/BankController/GetBalance")]
        public async Task<IActionResult> GetLatestBalanceAsync(int userId)
        {
            try
            {
                var latestBalance = await momDb.CustomerAccountBalances
       .Where(c => c.CustomerId == userId)
       .OrderByDescending(c => c.Id) // Assuming Id is the primary key or represents the order of records
       .FirstOrDefaultAsync();
                if (latestBalance == null)
                    return BadRequest("You dont have any balance");
             
                return Ok(latestBalance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        [HttpGet]
        [Route("api/BankController/GetBalanceHistory")]
        public async Task<IActionResult> GetBalanceHistory(int CustomerAttributeId)
        {
            var balances = await momDb.CustomerAccountBalances
                .Where(c => c.CustomerId == CustomerAttributeId)
                .OrderByDescending(c => c.Id) // Assuming Id is the primary key or represents the order of records
                .ToArrayAsync();

            if (balances.Length > 0)
            {
                return Ok(balances);
            }
            else
            {
                return NotFound("No balance history found.");
            }
        }



        [HttpGet]
        [Route("api/BankController/GetSignupAccountBalance")]
        public async Task<IActionResult> GetSignupAccountBalance(int userId)
        {
            try
            {
                var latestBalance = await momDb.CustomerAccountBalanceSingUps
       .Where(c => c.CustomerId == userId)
       .OrderByDescending(c => c.Id) // Assuming Id is the primary key or represents the order of records
       .FirstOrDefaultAsync();


                return Ok(latestBalance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

       
        
        
        
        
        
        // [Authorize]
        [HttpGet]
        [Route("api/BankController/GetTOTALPMONEY")]
        public IActionResult GetTOTALPMONEY(int customerId)
        {
            try
            {

                var balance = (from ca in momDb.CustomerAccountBalanceSingUps
                               where ca.CustomerId == customerId
                               orderby ca.Id descending
                               select ca.Balance).Take(1);


                if (balance == null)
                    return StatusCode(204, "Not Fond");
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


       // [Authorize]
        [HttpGet]
        [Route("api/BankController/TotalDiscounts")]
        public IActionResult TotalDiscounts(int customerId)
        {
            try
            {
                var res = (from ca in momDb.CustomerDiscounts
                               where ca.CustomerId == customerId
                           orderby ca.Id descending
                               select ca.Balance == null ? 0 : ca.Balance).Take(1);

                if (res == null)
                    return StatusCode(204, "Not Fond");
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }



       // [Authorize]
        [HttpGet]
        [Route("api/BankController/Directcomission")]
        public IActionResult Directcomission(int customerId)
        {
            try
            {
                var test = (from n in momDb.CustomerAttributes where n.CustomerInfoId == customerId select n.Id).ToList();
                var dirtc = (from custinfo in momDb.CustomerInfo

                             join custatrpute in momDb.CustomerAttributes
                             on custinfo.Id equals custatrpute.CustomerInfoId

                             join custnetwork in momDb.CustomerNetwork
                             on custatrpute.Id equals custnetwork.ChildId

                             join pointp in momDb.PointProcess
                             on custatrpute.Id equals pointp.ForCustomerId

                             join proft in momDb.Profit
                             on pointp.Id equals proft.PointProcessId
                             orderby proft.PointProcessId
                             where test.Contains((int)custnetwork.SponsorId)
                             && (pointp.ProcessTypeId == 1 || pointp.ProcessTypeId == 5)
                             select new DirectcomitionDto
                             {
                                 NameEn = custinfo.NameEn,
                                 mobile = custinfo.Mobile,
                                 ProcessTypeId = pointp.ProcessTypeId,
                                 CreationDate = custatrpute.CreationDate,
                                 profit = proft.Profit1
                             });




                
                if (dirtc == null)
                    return StatusCode(204, "Not Fond");
                return Ok(dirtc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


       // [Authorize]
        [HttpGet]
        [Route("api/BankController/Bonus")]
        public IActionResult Bonus(int customerId)
        {
            try
            {

                var bones = (from profit in momDb.Profit

                             join prostyp in momDb.ProcessType
                             on profit.ProcessTypeId equals prostyp.Id
                             orderby profit.Id
                             join custattrput in momDb.CustomerAttributes
                             on profit.DistributorId equals custattrput.Id

                             join custinfo in momDb.CustomerInfo
                             on custattrput.CustomerInfoId equals custinfo.Id


                             where profit.ProcessTypeId
                             >= 1 && profit.ProcessTypeId <= 2 && custattrput.CustomerInfoId == customerId
                             select new
                             {
                                 custinfo.NameEn,
                                 profit.Id,
                                 profit.DistributorId,
                                 profit.Profit1,
                                 profit.ProfitDate,
                                 profit.IsPaid,
                                 profit.PaymentDate,
                                 profit.ProcessTypeId,
                                 prostyp.Process,
                                 custattrput.ReferId
                             });


                 if (bones == null)
                    return StatusCode(204, "Not Fond");
                return Ok(bones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


       // [Authorize]
        [HttpGet]
        [Route("api/BankController/CheckTRANSFERP_MONEY")]
        public IActionResult CheckTRANSFERP_MONEY(string ReferId, int userId)
        {
            try
            {
                //from v in Context.CustomerAttributes where v.ReferId  ==ReferId select v.Id !=custattrput.Id && custattrput.ReferId==ReferId
                var TRANSFERP = (from custattrput in momDb.CustomerAttributes

                                 join custinfo in momDb.CustomerInfo
                                 on custattrput.CustomerInfoId equals custinfo.Id


                                 where momDb.CustomerAttributes.Where(x => x.ReferId == ReferId).FirstOrDefault().Id != userId && custattrput.ReferId == ReferId

                                 select new
                                 {
                                     custattrput.Id,
                                     custinfo.NameEn,
                                     custattrput.ReferId,
                                     custattrput.IsActive,
                                     custattrput.Renewal

                                 });
                if (TRANSFERP == null)
                    return StatusCode(204, "Not Fond");
                return Ok(TRANSFERP);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

      //  [Authorize]
        [HttpPost]
        [Route("api/BankController/TRANSFERP_MONEYOpreation")]
        public IActionResult TRANSFERP_MONEYOpreation(string referid, decimal amount, int userId)
        {
            try
            {
                var checkrefeid = TRANSFERP_MONEY("en", referid, userId);

                var useridtransferto = momDb.CustomerAttributes.Where(x => x.ReferId == referid).FirstOrDefault().Id;

                if (checkrefeid.ToString() == "")
                {
                    return BadRequest("check refer id is empty");
                }




                var palance = (from ca in momDb.CustomerAccountBalances
                               where ca.CustomerId == userId
                               orderby ca.Id descending
                               select ca.Balance).Take(1).FirstOrDefault();

                if (palance < amount)
                {
                    return BadRequest("Insufficient balance");
                }

                var custnameto = (from n in momDb.CustomerInfo
                                  join v in momDb.CustomerAttributes
                                  on n.Id equals v.CustomerInfoId
                                  where n.Id == userId
                                  select n.NameEn).FirstOrDefault();


                //Context.CustomerInfo.Where(x => x.Id == userId).FirstOrDefault().NameAr;
                var custbalancetrancferfrom = new CustomerAccountBalanceSingUp()
                {
                    CustomerId = userId,
                    Debit = amount,
                    Credit = 0,
                    TransactionDate = DateTime.Now,
                    Balance = palance - amount,
                    Description = "to" + custnameto

                };

                momDb.CustomerAccountBalanceSingUps.Add(custbalancetrancferfrom);
                momDb.SaveChangesAsync();

                var custnamefrom = (from n in momDb.CustomerInfo
                                    join v in momDb.CustomerAttributes
                                    on n.Id equals v.CustomerInfoId
                                    where n.Id == useridtransferto
                                    select n.NameEn).FirstOrDefault();

                var custbalancetrancferto = new CustomerAccountBalanceSingUp()
                {
                    CustomerId = useridtransferto,
                    Debit = 0,
                    Credit = amount,
                    TransactionDate = DateTime.Now,
                    Balance = palance + amount,
                    Description = "from  " + custnamefrom

                };
                momDb.CustomerAccountBalanceSingUps.Add(custbalancetrancferto);
                momDb.SaveChangesAsync();
                return Ok(new { custbalancetrancferfrom, custbalancetrancferto });





            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }






        private object TRANSFERP_MONEY(string lang, string ReferId, int userId)
        {

            if (lang == "ar")
            {
                var TRANSFERP = (from custattrput in momDb.CustomerAttributes

                                 join custinfo in momDb.CustomerInfo
                                 on custattrput.CustomerInfoId equals custinfo.Id


                                 where momDb.CustomerAttributes.Where(x => x.ReferId == ReferId).FirstOrDefault().Id != userId && custattrput.ReferId == ReferId

                                 select new
                                 {
                                     custattrput.Id,
                                     custinfo.NameAr,
                                     custattrput.ReferId,
                                     custattrput.IsActive,
                                     custattrput.Renewal

                                 });


                return TRANSFERP;
            }
            else
            {
                //custattrput.ReferId == ReferId
                //Context.CustomerAttributes.Where(x=>x.ReferId==ReferId).FirstOrDefault().Id== userId
                //from v in Context.CustomerAttributes where v.ReferId  ==ReferId select v.Id !=custattrput.Id && custattrput.ReferId==ReferId
                var TRANSFERP = (from custattrput in momDb.CustomerAttributes

                                 join custinfo in momDb.CustomerInfo
                                 on custattrput.CustomerInfoId equals custinfo.Id


                                 where momDb.CustomerAttributes.Where(x => x.ReferId == ReferId).FirstOrDefault().Id != userId && custattrput.ReferId == ReferId

                                 select new
                                 {
                                     custattrput.Id,
                                     custinfo.NameEn,
                                     custattrput.ReferId,
                                     custattrput.IsActive,
                                     custattrput.Renewal

                                 });


                return TRANSFERP;
            }
        }





        // [Authorize]
        [HttpPost]
       [Route("api/BankController/cashingpointBack")]
        public async Task<IActionResult> cashingpointBack([FromForm] RequestcashinDTO model, string lang)
        {
            try
            {
                int time = DateTime.Now.Hour;

                if (time >= 12 && time <= 24)
                {
                    time = time - 12;
                }

                var totalBalance = (from ca in momDb.CustomerAccountBalances
                                    where ca.CustomerId == model.UserId
                                    orderby ca.Id descending
                                    select ca.Balance).Take(1).FirstOrDefault();

                List<string> days = (from d in momDb.VisabltyCash
                                     where d.IsActive == true
                                     && (time <= d.timefrom.Hour && time <= d.timeto.Hour)
                                     select d.day).ToList();

                if (!days.Contains(DateTime.Now.DayOfWeek.ToString()))
                {
                    return StatusCode(400, "Invalid request: Cashing point not available at this time.");
                }

                var checkPaymentWay = (from rcards in momDb.RequestsCards
                                       join rtyp in momDb.RequestType on rcards.RequestTypeId equals rtyp.Id
                                       join rbank in momDb.RequestsBanks on rcards.BankId equals rbank.Id
                                       where rcards.CustomerId == model.UserId && rcards.IsUsed == true
                                       select new
                                       {
                                           rtyp.Id,
                                           rtyp.Name,
                                           rcards.CustomerId,
                                           rcards.IsUsed,
                                           rbank.BankName
                                       }).ToList();

                if (!checkPaymentWay.Any())
                {
                    return BadRequest("Please add a payment method.");
                }

                var addCash = new RequestCash()
                {
                    CustomerId = model.UserId,
                    RequestDate = DateTime.Now,
                    RequestedAmount = totalBalance,
                    RequestTypeId = model.RequestTypeId,
                    IsPaid = false,
                    UnPaid = false
                };
                momDb.RequestCash.Add(addCash);
                await momDb.SaveChangesAsync();

                var customerBalanceTransferTo = new CustomerAccountBalanceSingUp()
                {
                    CustomerId = model.UserId,
                    Debit = 0,
                    Credit = totalBalance,
                    TransactionDate = DateTime.Now,
                    Balance = totalBalance - totalBalance,
                    Description = "from  cashing point Back"
                };
                momDb.CustomerAccountBalanceSingUps.Add(customerBalanceTransferTo);
                await momDb.SaveChangesAsync();

                return Ok(new { addCash, customerBalanceTransferTo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




        //[Authorize]
        [HttpGet]
        [Route("api/BankController/TransferIn")]
        public IActionResult TransferIn(int customerAttrId)
        {
            try
            {
                var total = (from ca in momDb.CustomerAccountBalanceSingUps
                             where ca.CustomerId == customerAttrId
                                    && ca.Debit == 0 && ca.Credit > 0
                             orderby ca.Id descending
                             select ca).ToList();

                if (total == null)
                    return StatusCode(204, "Not Fond");
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
       
        //[Authorize]
        [HttpGet]
        [Route("api/BankController/TransferOut")]
        public IActionResult TransferOut(int customerAttrId)
        {
            try
            {
                var total = (from ca in momDb.CustomerAccountBalanceSingUps
                             where ca.CustomerId == customerAttrId
                             && ca.Credit == 0 && ca.Debit > 0
                             && !(ca.Description.Contains("New Membership Ref ID"))
                             orderby ca.Id descending
                             select ca).ToList(); 
                if (total == null)
                    return StatusCode(204, "Not Fond");
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
  
        
        //  [Authorize]
        [HttpGet]
        [Route("api/BankController/SellsReport")]
        public IActionResult SellsReport(int customerAttrId)
        {
            try
            {
                var total = (from ca in momDb.CustomerAccountBalanceSingUps
                             where ca.CustomerId == customerAttrId
                             && ca.Credit == 0 && ca.Debit > 0
                             && ca.Description.Contains("sell")
                             orderby ca.Id descending
                             select ca).ToList();

                if (total == null)
                    return StatusCode(204, "Not Fond");
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
     
        //  [Authorize]
        [HttpGet]
        [Route("api/BankController/GetTheBalance")]
        public async Task<IActionResult> GetTheBalance(int customerAttrId)
        {
            try
            {
                

                var Total = await momDb.CustomerAccountBalances
                    .Where(c => c.CustomerId == customerAttrId && c.Description.Contains("Transfer"))
                     .OrderByDescending(c => c.Id)
                     .ToListAsync();



                
                return Ok(Total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


       // [Authorize]
        [HttpGet]
        [Route("api/BankController/Customer_Account_Plan_B_Bonus")]
        public IActionResult Customer_Account_Plan_B_Bonus(int customerAttrId)
        {
            try
            {
                var total = (from ca in momDb.CustomerAccountMomentumBonus
                             where ca.Used == false && ca.CustomerId == customerAttrId

                             select ca.Credit).Sum();

                if (total == null)
                    return StatusCode(204, "Not Fond");
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }



        //[Authorize]
        [HttpGet]
        [Route("api/BankController/Customer_Account_Plan_B_BonusAll")]
        public IActionResult Customer_Account_Plan_B_BonusAll(int customerAttrId)
        {
            try
            {
                var total = (from ca in momDb.CustomerAccountMomentumBonus
                             where ca.CustomerId == customerAttrId

                             select ca.Credit).Sum();
                if (total == null)
                    return StatusCode(204, "Not Fond");
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        //[Authorize]
        //[HttpGet]
        //[Route("api/CustomerInfo/GetCustomerNetwork")]
        //public IActionResult GetCustomerNetwork(int ParentId)
        //{
        //    try
        //    {
        //        if (ParentId == 0)
        //        {
        //            ParentId = _userId;
        //        }
        //        var res = BusinessCustomerInfo.GetCustomerNetwork(ParentId);
        //        if (res == null)
        //            return StatusCode(404, "Not Found");
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("api/CustomerInfo/SetVideoAttendance")]
        //public IActionResult SetVideoAttendance( string VideoId)
        //{
        //    try
        //    {
        //        var res = BusinessCustomerInfo.SetVideoAttendance(_userId, VideoId);
        //        if (res == false)
        //            return StatusCode(303, "Attended before");
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}

        //// GET: api/CustomerInfo/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/CustomerInfo
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/CustomerInfo/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
