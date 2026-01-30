using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using Ovia.Services.SendEmails;
using Ovia.Services.SendWhatsApp360Live;
using Ovia.Services.StorageFiles;
using System.Text;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerInfoController : ControllerBase
    {
        private readonly IStorageService storage;
        private readonly MomEntity momDb;
        private readonly Whats360Client _whats360Client;
        private readonly IMailingServices mailingService;

        public CustomerInfoController(
            IStorageService _storage,
            MomEntity _momDb,
            Whats360Client whats360Client,
            IMailingServices _mailingService
            )
        {
            storage = _storage;
            momDb = _momDb;
            _whats360Client = whats360Client;
            mailingService = _mailingService;

        }



        //[HttpGet("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetCustomerInfo(int id)
        //{
        //    // Implementation to retrieve customer information from the database
        //    // You can replace this with your actual logic
        //    var customer = await momDb.CustomerInfo.FirstOrDefaultAsync(c => c.Id == id);
        //    // If customer not found, return 404
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(customer);
        //}








        [HttpGet("GetRequestType")]
        public async Task<IActionResult> GetRequestType()
        {
            var types = await momDb.RequestType.ToListAsync();
            if (types.Count == 0)
                return NotFound("Not found any types");
            else
                return Ok(types); // Returning OK with the list of types
        }


        [HttpGet("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            try
            {
                CustomerDTO customerInfo = new CustomerDTO();

                // Retrieve data synchronously
                var customerAttribute = await momDb.CustomerAttributes.FirstOrDefaultAsync(c => c.Id == userId);
                if (customerAttribute != null)
                {
                    customerInfo.Id = customerAttribute.Id;
                    customerInfo.Email = customerAttribute.CustomerInfo.Email;
                    customerInfo.Mobile = customerAttribute.CustomerInfo.Mobile;
                    customerInfo.Name = customerAttribute.CustomerInfo.NameEn;
                    customerInfo.Picture = customerAttribute.CustomerInfo.Picture;
                    customerInfo.NoOfCourssesEnrol = customerAttribute.CourseCustomerMapping.Count;

                    var governate = await momDb.Governorate.FirstOrDefaultAsync(x => x.Id == customerAttribute.CustomerInfo.GovId);
                    if (governate != null)
                    {
                        customerInfo.governateA = governate.NameAr;
                        customerInfo.governateE = governate.NameEn;
                    }

                    var team = await momDb.Teams.FirstOrDefaultAsync(x => x.Id == customerAttribute.CustomerInfo.TeamId);
                    if (team != null)
                    {
                        customerInfo.teamname = team.Name;
                    }

                    customerInfo.username = customerAttribute.CustomerInfo.Username;

                    return Ok(customerInfo);
                }
                else
                {
                    return StatusCode(204, "Not Found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }



        //  [HttpPut("{id}")]
        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(long userId, int custId, [FromBody] CustomerDTO customerInfo)
        {
            try
            {
                var custdata = await momDb.CustomerInfo
                    .Where(x => x.Id == custId)
                    .FirstOrDefaultAsync();

                var cinfo = new CustomerRequestsData()
                {
                    CustomerId = custdata.Id,
                    OldName = custdata.NameAr,
                    NewName = customerInfo.Name,
                    OldMobile = custdata.Mobile,
                    NewMobile = customerInfo.Mobile,
                    OldEmail = custdata.Email,
                    NewEmail = customerInfo.Email,
                    RequestDate = DateTime.Now,
                    IsConfirmed = false,


                };
                momDb.CustomerRequestsData.Add(cinfo);
                await momDb.SaveChangesAsync();
                //custdata.CustomerInfo.NameAr = customerInfo.Name;
                custdata.Mobile = customerInfo.Mobile;
                custdata.GovId = (await momDb.Governorate
    .FirstOrDefaultAsync(x => x.NameEn == customerInfo.governateA))?.Id ?? 0;
                custdata.Picture = customerInfo.Picture;
                custdata.Username = customerInfo.username;
                await momDb.SaveChangesAsync();

                return Ok("Updates");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        //[HttpGet("GetCustomerNetwork")]
        //public async Task<IActionResult> GetCustomerNetwork(int ParentId)
        //{
        //    try
        //    {
        //        List<NetworkDTO> network = new List<NetworkDTO>();

        //        // Recursive function to get children
        //        void GetChildren(int parentId)
        //        {
        //            var children = momDb.CustomerNetwork.Where(c => c.Parent.Id == parentId)
        //                                                .Select(s => new NetworkDTO
        //                                                {
        //                                                    ChildId = (int)s.ChildId,
        //                                                    HandSide = s.HandSide,
        //                                                    ChildName = s.Child.CustomerInfo.NameEn,
        //                                                    ChildEmail = s.Child.CustomerInfo.Email,
        //                                                    referrid = s.Child.ReferId,
        //                                                    rank = s.Child.Rank.Rank1,
        //                                                    mobile = s.Child.CustomerInfo.Mobile
        //                                                })
        //                                                .ToList();

        //            foreach (var child in children)
        //            {
        //                network.Add(child);
        //                GetChildren(child.ChildId); // Recursively call to get children's children
        //            }
        //        }

        //        GetChildren(ParentId);

        //        if (network.Count == 0)
        //            return NotFound("Don't have anyone in the network");
        //        else
        //            return Ok(network);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}


        //[HttpGet("GetCustomerNetwork")]
        //public async Task<IActionResult> GetCustomerNetwork(int ParentId)
        //{
        //    try
        //    {
        //        List<NetworkDTO> network = new List<NetworkDTO>();

        //        // Recursive function to get children
        //        void GetChildren(int parentId, NetworkDTO parentDto)
        //        {
        //            var children = momDb.CustomerNetwork.Where(c => c.Parent.Id == parentId)
        //                                                .Select(s => new NetworkDTO
        //                                                {
        //                                                    ChildId = (int)s.ChildId,
        //                                                    HandSide = s.HandSide,
        //                                                    ChildName = s.Child.CustomerInfo.NameEn,
        //                                                    ChildEmail = s.Child.CustomerInfo.Email,
        //                                                    referrid = s.Child.ReferId,
        //                                                    rank = s.Child.Rank.Rank1,
        //                                                    mobile = s.Child.CustomerInfo.Mobile,
        //                                                    Children = new List<NetworkDTO>() // Initialize Children list
        //                                                })
        //                                                .ToList();

        //            foreach (var child in children)
        //            {
        //                parentDto.Children.Add(child); // Add child to parent's Children list
        //                GetChildren(child.ChildId, child); // Recursively call to get children's children
        //            }
        //        }

        //        // Create a duparent DTO to hold root-level children
        //        NetworkDTO rootDto = new NetworkDTO();
        //        GetChildren(ParentId, rootDto);

        //        if (rootDto.Children.Count == 0)
        //            return NotFound("Don't have anyone in the network");
        //        else
        //            return Ok(rootDto.Children);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}



        [HttpPost("SetVideoAttendance")]
        public async Task<IActionResult> SetVideoAttendance(int userId, string videoId)
        {
            try
            {
                var CourseDetails = await momDb.CourseDetails
                    .Where(c => c.OnlineId == videoId).FirstOrDefaultAsync();

                OnlineAttendance attendance = await momDb.OnlineAttendance
                    .Where(c => c.CustomerId == userId && c.CourseDetailsId == CourseDetails.Id)
                    .FirstOrDefaultAsync();

                if (attendance != null)
                {
                    return StatusCode(303, "Attended before"); // Assuming this is the correct response code and message
                }

                attendance = new OnlineAttendance()
                {
                    CourseDetailsId = CourseDetails.Id,
                    CustomerId = userId
                };

                momDb.OnlineAttendance.Add(attendance);
                await momDb.SaveChangesAsync();

                return Ok("Attendance recorded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        //  [Authorize]
        // [HttpPost]
        [HttpPost("Aprovecustinfo")]
        public async Task<IActionResult> Aprovecustinfo(int custId, bool isconfermed)
        {
            try
            {

                List<CustomerRequestsData> data = (from x in momDb.CustomerRequestsData
                                                   where x.CustomerId == custId && x.IsConfirmed == false
                                                   //&& x.IsDeleted == false
                                                   select x).ToList();


                foreach (CustomerRequestsData item in data)
                {

                    item.IsConfirmed = isconfermed;
                }
                if (data == null)
                    return StatusCode(303, "Attended before");
                return Ok(data);






            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        //   [Authorize]
        //  [HttpGet]
        [HttpGet("GetCommunityLine")]
        public async Task<IActionResult> GetCommunityLine(int userAttributeid)
        {
            try
            {
                var data = await momDb.DistributorsCommunityPersonal
                                    .Where(x => x.DistributorsId == userAttributeid)
                                    .Select(x => x.CommunityNo)
                                    .FirstOrDefaultAsync();

                if (data == null)
                    return StatusCode(404, "Not Found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        //[Authorize]
        //[HttpGet]
        [HttpGet("GetPersonalLine")]
        public async Task<IActionResult> GetPersonalLine(int userAttributeid)
        {
            try
            {
                var data = await momDb.DistributorsCommunityPersonal
                                       .Where(x => x.DistributorsId == userAttributeid)
                                       .Select(x => x.PersonalNo)
                                       .FirstOrDefaultAsync();

                if (data == null)
                    return StatusCode(404, "Not Found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }




       

        // [Authorize]
        // [HttpGet]
        [HttpGet("getcashmethod")]
        public IActionResult getcashmethod(int customerAttributeId)
        {
            try
            {
                //var custinfoid = momDb.CustomerAttributes
                //    .Where(x => x.Id == customerAttributeId)
                //    .FirstOrDefault()?.CustomerInfoId;


                var data = (from rqtyp in momDb.RequestType
                            where rqtyp.IsActive == true && (rqtyp.Id) != (from rqtypid in momDb.RequestsCards
                                                                           where rqtypid.CustomerId == customerAttributeId
                                                                           select rqtypid.RequestTypeId).FirstOrDefault()
                            select new { rqtyp.Id, rqtyp.Name  }).ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        //  [Authorize]
        //[HttpGet]
        [HttpGet("getcashmethodforuser")]
        public IActionResult getcashmethodforuser(int idinfo)
        {
            try
            {
                var custinfoid = momDb.CustomerAttributes.Where(x => x.Id == idinfo)
                    .FirstOrDefault().CustomerInfoId;


                var data = (from reqcard in momDb.RequestsCards
                            join reqtyp in momDb.RequestType
                            on reqcard.RequestTypeId equals reqtyp.Id

                            join reqbank in momDb.RequestsBanks
                            on reqcard.BankId equals reqbank.Id

                            where reqcard.CustomerId == custinfoid && reqcard.IsApproved == true

                            select new { reqcard.Id, reqtyp.Name, reqbank.BankName, reqcard.CardId, reqcard.IsUsed }).ToList();
                if (data == null)
                    return StatusCode(303, "Attended before");
                return Ok(new { data });






            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        ///  [Authorize]
        //    [HttpGet]
        [HttpGet("getcashmethodforuser_unused")]
        public async Task<IActionResult> getcashmethodforuser_unused(int customerId)
        {
            try
            {
                var customerInfoId = await momDb.CustomerAttributes
                    .Where(x => x.Id == customerId)
                    .Select(x => x.CustomerInfoId)
                    .FirstOrDefaultAsync();

                if (customerInfoId == null)
                    return StatusCode(404, "Customer not found");

                var data = await (from reqcard in momDb.RequestsCards
                                  join reqtyp in momDb.RequestType on reqcard.RequestTypeId equals reqtyp.Id
                                  join reqbank in momDb.RequestsBanks on reqcard.BankId equals reqbank.Id
                                  where reqcard.CustomerId == customerInfoId && reqcard.IsApproved == false
                                  select new { reqcard.Id, reqtyp.Name, reqbank.BankName, reqcard.CardId, reqcard.IsUsed })
                                  .ToListAsync();

                if (data.Count == 0)
                    return StatusCode(204, "No unused cash methods found for the customer");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //generate otp
        private string GenerateOtp()
        {
            Random random = new Random();
            int otpLength = 6; // You can adjust the length of the OTP as needed
            StringBuilder otpBuilder = new StringBuilder();

            for (int i = 0; i < otpLength; i++)
            {
                otpBuilder.Append(random.Next(0, 9)); // Generate a random digit
            }

            return otpBuilder.ToString();
        }





        [HttpPost("addnewcashbackmethodAndNationdId")]
        public async Task<IActionResult> addnewcashbackmethodAndNationdId
            ([FromForm] newcashbackmethodDTO model)
        {
            try
            {
                var userInfo = await momDb.CustomerAttributes
           .Where(u => u.Id == model.userid)
           .Include(u => u.CustomerInfo)
           .FirstOrDefaultAsync();

                if (userInfo != null)
                {

                    var existingCard = await momDb.RequestsCards.FirstOrDefaultAsync(c => c.CustomerId == model.userid);
                    if (existingCard != null)
                        return BadRequest("You added binance before");


                    string otp = GenerateOtp();
                    var addtorequestcard = new RequestsCards()
                    {
                        Id = 0,
                        RequestTypeId = model.methodid,
                        BankId = 3,
                        CustomerId = model.userid,
                        CardId = model.cardNumber,
                        IsUsed = false,
                        IsApproved = false,
                        ResquestDate = DateTime.Now,
                        AccNo = "null",
                        Otp = otp,
                        OtpCreateDate = DateTime.Now,
                        OtpActive = false
                    };

                    momDb.RequestsCards.Add(addtorequestcard);
                    await momDb.SaveChangesAsync();

                    var keyfront = "";
                    var keyback = "";

                    if (model.nationalIdfront != null)
                    {
                        var uploadedFront = await storage.Upload(model.nationalIdfront);
                        keyfront = uploadedFront.Key;
                    }
                    if (model.nationalIdback != null)
                    {
                        var uploadedBack = await storage.Upload(model.nationalIdback);
                        keyback = uploadedBack.Key;
                    }

                    var addtocustattachment = new CustomerAttachments()
                    {
                        Id = 0,
                        CustomerId = model.userid,
                        FrontKey = keyfront,
                        BackKey = keyback,
                        EntryDate = DateTime.Now
                    };

                    momDb.CustomerAttachments.Add(addtocustattachment);
                    await momDb.SaveChangesAsync();

                    if (userInfo.CustomerInfo != null)
                    {
                        mailingService.SendOTP(userInfo?.CustomerInfo?.Email, userInfo?.CustomerInfo?.NameEn, addtorequestcard.Otp);

                        return Ok("OTP sent successfully");
                    }
                    else
                    {
                        return BadRequest("Customer information is incomplete.");
                    }



                    return Ok(new { addtorequestcard, addtocustattachment });

                }


                else
                {
                    return BadRequest("Customer information not found.");

                }
                //if (userInfo != null)
                //{
                //    if (userInfo.CustomerInfo != null)
                //    {
                //        string mail = userInfo.CustomerInfo.Email;
                //        string name = userInfo.CustomerInfo.NameEn;
                //        await mailingService.SendOTP("momentumcompany1@gmail.com", name, otp);
                //        return Ok("OTP sent successfully");
                //    }
                //    else
                //    {
                //        return BadRequest("Customer information is incomplete: CustomerInfo is null.");
                //    }
                //}
                //else
                //{
                //    return BadRequest("Customer information not found.");
                //}

                //                if (customInfo.CustomerInfo.whatsappmobile != null)
                //                {
                //                    var dto = new SendWhatsDTO
                //                    {
                //                        recipientPhoneNumber = customInfo.CustomerInfo.whatsappmobile,
                //                        message = $@"
                //Dear {customInfo.CustomerInfo.NameEn}
                //Thank you for adding  a cash request  method. 
                //Please confirm your OTP before 10 minutes.

                //OTP: {addtorequestcard.Otp}
                //Best regards,
                //The Momentum Team
                //"
                //                    };

                //                    await _whats360Client.SendMessage(dto);

                //                    return Ok("Added request card successfully");
                //                }
                //                else
                //                {
                //                    return BadRequest("User's WhatsApp mobile number is not available");
                //                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }




        [HttpPut("ConfirmationOtp")]
        public async Task<IActionResult> ConfirmationOtp
           (int userId, string otp)
        {
            var user = await momDb.RequestsCards
                .Where(c => c.CustomerId == userId )
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            if(user != null)
            {
                if(user.Otp == otp)
                {
                    if (DateTime.Now - user.OtpCreateDate <= TimeSpan.FromMinutes(10))
                    {
                        // OTP is valid
                        user.OtpActive =true;
                        user.IsApproved = true;
                        user.IsActive = true;

                        await momDb.SaveChangesAsync();
                        return Ok("You Activated Successfully");
                    }
                    else
                    {
                        // OTP has expired
                        return BadRequest("OTP has expired. Please request a new OTP.");
                    }
                }
                else
                {
                    return BadRequest("Incorrect OTP.");
                }
            }
            else
            {
                return BadRequest("User not exist");
            }

        }


        [HttpGet("RequestNewOtp")]
        public async Task<IActionResult> RequestNewOtp(int userId)
        {
            var user = await momDb.RequestsCards
                .Where(c => c.CustomerId == userId && c.OtpActive == false)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                if(user.OtpActive == true)
                {
                    return BadRequest("You actived your card before");
                }

                string otp = GenerateOtp();

                user.Otp = otp;
                user.OtpCreateDate = DateTime.Now;
                //////////////
                await momDb.SaveChangesAsync();

                var customInfo = await momDb.CustomerAttributes
                    .Where(u => u.Id == userId)
                    .Include(u => u.CustomerInfo)
                    .FirstOrDefaultAsync();


               await mailingService.SendOTP(customInfo.CustomerInfo.Email
                    , customInfo.CustomerInfo.NameEn,user.Otp);
                                  return Ok("Resend OTP successfully");


                //                if (customInfo.CustomerInfo.whatsappmobile != null)
                //                {
                //                    var dto = new SendWhatsDTO
                //                    {
                //                        recipientPhoneNumber = customInfo.CustomerInfo.whatsappmobile,
                //                        message = $@"
                //Dear {customInfo.CustomerInfo.NameEn}
                //Thank you for requesting a new OTP. 
                //Please confirm your OTP before 10 minutes.

                //OTP: {user.Otp}
                //Best regards,
                //The Momentum Team
                //"
                //                    };

                //                    await _whats360Client.SendMessage(dto);

                //                    return Ok("Resend OTP successfully");
                //                }
                //                else
                //                {
                //                    return BadRequest("User's WhatsApp mobile number is not available");
                //                }
            }
            else
            {
                return BadRequest("User does not exist");
            }
        }



        [HttpGet("GetBinanceAccount")]
        public async Task<IActionResult> GetBinanceAccount(int customerId)
        {
            var existingCashMethod = await momDb.RequestsCards.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsApproved == true);
            if (existingCashMethod != null)
            {
                var existingNationalImgs = await momDb.CustomerAttachments
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (existingNationalImgs != null)
                {
                    var Binance = new GetBinanceAccountDTO
                    {
                        CustomerId = customerId,
                        BinanceNumber = existingCashMethod.CardId,
                        FrontIdImageUrl = existingNationalImgs.FrontKey != null ? existingNationalImgs.FrontKey.SetDownloadFileUrlByKey(storage) : "",
                        BackIdImageUrl = existingNationalImgs.BackKey != null ? existingNationalImgs.BackKey.SetDownloadFileUrlByKey(storage) : "",

                    };

                    return Ok(Binance);

                }
                return NotFound("Not found national id image");

            }
            return NotFound("You dont have binance account");


        }

        [HttpPut("EditBinanceAccount")]
        public async Task<IActionResult> EditBinanceAccount(int customerId, string accountNumber)
        {
            var existingCashMethod = await momDb.RequestsCards
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsApproved == true);
            if(existingCashMethod != null)
            {
                existingCashMethod.CardId = accountNumber;
                await momDb.SaveChangesAsync();
                return Ok("Updated successfully");
            }
            return NotFound("Binance account not found");

        }

        // [Authorize]
        //  [HttpPut]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string newPassword, int customerAttributeId)
        {
            try
            {
                var customerData = await momDb.CustomerInfo
                    .Include(f => f.CustomerAttributes)
                    .FirstOrDefaultAsync(x => x.CustomerAttributes.Any(ca => ca.Id == customerAttributeId));

                if (customerData != null)
                {
                    customerData.Password = newPassword;
                    customerData.RequestPasswordResetDate = DateTime.Now;
                    await momDb.SaveChangesAsync();

                    return Ok("Password changed successfully");
                }
                else
                {
                    return NotFound("Customer not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




        //     [Authorize]
        // [HttpGet]
        [HttpGet("CustomerInfo/cachmetodApprove")]
        public async Task<IActionResult> cachmetodApprove(int rowid, int userid)
        {
            try
            {
                var custinfoid = await momDb.CustomerAttributes
                    .Where(x => x.Id == userid)
                    .Select(s => s.CustomerInfoId)
                    .FirstOrDefaultAsync();

                var custid = await momDb.RequestsCards
                    .Where(x => x.Id == rowid)
                    .Select(c => c.CustomerId)
                    .FirstOrDefaultAsync();

                var updte = (from rqcard in momDb.RequestsCards
                             where rqcard.Id == rowid
                             select rqcard.IsUsed == true).ToList();

                var updte2 = (from rqcard in momDb.RequestsCards
                              where rqcard.Id != rowid && rqcard.CustomerId == custid
                              select rqcard.IsUsed == false).ToList();


                var data = (from reqcard in momDb.RequestsCards
                            join reqtyp in momDb.RequestType
                            on reqcard.RequestTypeId equals reqtyp.Id

                            join reqbank in momDb.RequestsBanks
                            on reqcard.BankId equals reqbank.Id

                            where reqcard.CustomerId == custinfoid && reqcard.IsApproved == false

                            select new { reqcard.Id, reqtyp.Name, reqbank.BankName, reqcard.CardId, reqcard.IsUsed }).ToList();






                if (data == null)
                    return StatusCode(303, "Attended before");
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }



        ////get child
        //public async Task<CustomerNetwork> GetChildId(int userId, string handSide)
        //{
        //    try
        //    {
        //        var data = await momDb.CustomerNetwork.FirstOrDefaultAsync(o => o.ParentId == userId && o.HandSide == handSide);
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions appropriately, for example, logging the error
        //        Console.WriteLine($"An error occurred while retrieving child ID: {ex.Message}");
        //        throw; // Rethrow the exception to propagate it up the call stack
        //    }
        //}


        //  [Authorize]
        //[HttpGet]
        [HttpGet("GetCustomerNetworkLeft")]
        public async Task<IActionResult> GetCustomerNetworkLeft(int userAttributeId)
        {
            try
            {
               var childNetwork = await momDb.CustomerNetwork
                    .FirstOrDefaultAsync(o => o.ParentId == userAttributeId && o.HandSide == "Left");


               // var childNetwork = await GetChildId(userAttributeId, "Left");
                if (childNetwork != null)
                {
                    var childId = childNetwork.ChildId;
                    var data = await (from CASpon in momDb.CustomerAttributes
                                      from CiSpon in momDb.CustomerInfo.Where(x => CASpon.CustomerInfoId == x.Id)
                                      from RSpon in momDb.Rank.Where(o => CASpon.RankId == o.Id)
                                      from Rparent in momDb.Rank
                                      from CIParent in momDb.CustomerInfo
                                      from CAParent in momDb.CustomerAttributes.Where(p => CIParent.Id == p.CustomerInfoId && Rparent.Id == p.RankId)
                                      from CAchild in momDb.CustomerAttributes
                                      from Cichild in momDb.CustomerInfo.Where(u => CAchild.CustomerInfoId == u.Id)
                                      from cn in momDb.CustomerNetwork.Where(f => CAchild.Id == f.ChildId)
                                      from Rchild in momDb.Rank.Where(d => CAchild.RankId == d.Id && CAParent.Id == cn.ParentId && CASpon.Id == cn.SponsorId)
                                      where cn.UplineHistoryId.Contains("/" + childId + "/")
                                      orderby CAchild.Id descending
                                      select new
                                      {
                                          ChildId = cn.ChildId,
                                          Childname = Cichild.NameEn,
                                          Childmobil = Cichild.Mobile,
                                          Childemail = Cichild.Email,
                                          ChildReff = CAchild.ReferId,
                                          Childcreationdate = CAchild.CreationDate,
                                          ChildEligible = CAchild.IsEligible,
                                          RankId = CAchild.RankId,
                                          Renewal = CAchild.Renewal,
                                          Rank1 = Rchild.Rank1,
                                          ParentId = cn.ParentId,
                                          ParentName = CIParent.NameEn,
                                          Parentmobile = CIParent.Mobile,
                                          ParentEmail = CIParent.Email,
                                          ParentReff = CAParent.ReferId,
                                          Parentcreation = CAParent.CreationDate,
                                          Parenteligible = CAParent.IsEligible,
                                          Parentrank = CAParent.RankId,
                                          Parentrenwal = CAParent.Renewal,
                                          ParentRankk = Rparent.Rank1,
                                          SponsorId = cn.SponsorId,
                                          SponsorName = CiSpon.NameEn,
                                          Sponsormobil = CiSpon.Mobile,
                                          Sponsoremail = CiSpon.Email,
                                          Sponsorreff = CASpon.ReferId,
                                          Sponsorcretion = CASpon.CreationDate,
                                          Sponsorelig = CASpon.IsEligible,
                                          Sponsorrankk = CASpon.RankId,
                                          Sponsorren = CASpon.Renewal,
                                          Sponsorrank = RSpon.Rank1
                                      }).ToListAsync();

                    return Ok(data);
                }

                return StatusCode(404, "Not Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        //  [Authorize]
        // [HttpGet]
        [HttpGet("GetCustomerNetworkRight")]
        public async Task<IActionResult> GetCustomerNetworkRight(int userAttributeId)
        {
            try
            {

                var childNetwork = await momDb.CustomerNetwork
                   .FirstOrDefaultAsync(o => o.ParentId == userAttributeId && o.HandSide == "Right");

                //var childNetwork = await GetChildId(userAttributeId, "Right");
                if (childNetwork != null)
                {
                    var childId = childNetwork.ChildId;
                    var data = await (from CASpon in momDb.CustomerAttributes
                                      from CiSpon in momDb.CustomerInfo.Where(x => CASpon.CustomerInfoId == x.Id)
                                      from RSpon in momDb.Rank.Where(o => CASpon.RankId == o.Id)
                                      from Rparent in momDb.Rank
                                      from CIParent in momDb.CustomerInfo
                                      from CAParent in momDb.CustomerAttributes.Where(p => CIParent.Id == p.CustomerInfoId && Rparent.Id == p.RankId)
                                      from CAchild in momDb.CustomerAttributes
                                      from Cichild in momDb.CustomerInfo.Where(u => CAchild.CustomerInfoId == u.Id)
                                      from cn in momDb.CustomerNetwork.Where(f => CAchild.Id == f.ChildId)
                                      from Rchild in momDb.Rank.Where(d => CAchild.RankId == d.Id && CAParent.Id == cn.ParentId && CASpon.Id == cn.SponsorId)
                                      where cn.UplineHistoryId.Contains("/" + childId + "/")
                                      orderby CAchild.Id descending
                                      select new
                                      {
                                          ChildId = cn.ChildId,
                                          Childname = Cichild.NameEn,
                                          Childmobil = Cichild.Mobile,
                                          Childemail = Cichild.Email,
                                          ChildReff = CAchild.ReferId,
                                          Childcreationdate = CAchild.CreationDate,
                                          ChildEligible = CAchild.IsEligible,
                                          RankId = CAchild.RankId,
                                          Renewal = CAchild.Renewal,
                                          Rank1 = Rchild.Rank1,
                                          ParentId = cn.ParentId,
                                          ParentName = CIParent.NameEn,
                                          Parentmobile = CIParent.Mobile,
                                          ParentEmail = CIParent.Email,
                                          ParentReff = CAParent.ReferId,
                                          Parentcreation = CAParent.CreationDate,
                                          Parenteligible = CAParent.IsEligible,
                                          Parentrank = CAParent.RankId,
                                          Parentrenwal = CAParent.Renewal,
                                          ParentRankk = Rparent.Rank1,
                                          SponsorId = cn.SponsorId,
                                          SponsorName = CiSpon.NameEn,
                                          Sponsormobil = CiSpon.Mobile,
                                          Sponsoremail = CiSpon.Email,
                                          Sponsorreff = CASpon.ReferId,
                                          Sponsorcretion = CASpon.CreationDate,
                                          Sponsorelig = CASpon.IsEligible,
                                          Sponsorrankk = CASpon.RankId,
                                          Sponsorren = CASpon.Renewal,
                                          Sponsorrank = RSpon.Rank1
                                      }).ToListAsync();

                    return Ok(data);
                }

                return StatusCode(404, "Not Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
















        //public object GetData(int userAttributeId)
        //{
        //    var data = (from c in momDb.CustomerAttributes
        //                join r in momDb.CustomerInfo on c.CustomerInfoId equals r.Id
        //                where c.Id == userAttributeId
        //                select new
        //                {
        //                    ReferId = c.ReferId,
        //                    Id = c.Id,
        //                    NameEn = r.NameEn
        //                }).FirstOrDefault();
        //    return data;
        //}
        //public int GetChildIdByParent(int parentId, string handside)
        //{
        //    var data = momDb.CustomerNetwork.FirstOrDefault(o => o.ParentId == parentId && o.HandSide == handside);
        //    if (data == null)
        //    {
        //        return parentId;
        //    }

        //    return GetChildIdByParent((int)data.ChildId, handside);

        //}

        ////// [Authorize]
        //////[HttpGet]
        //[HttpGet("GetPersonalName")]
        //public async Task<IActionResult> GetPersonalName(int userAttributeId)
        //{
        //    try
        //    {
        //        int finalchildId = default(int);
        //        object getPersonalNameData = null;
        //        var sponserData = await (from x in momDb.CustomerAttributes
        //                                 join c in momDb.CustomerNetwork on x.Id equals c.ChildId into net
        //                                 from r in net.DefaultIfEmpty()
        //                                 join t in momDb.CustomerInfo on x.CustomerInfoId equals t.Id
        //                                 where x.Id == userAttributeId
        //                                 select new
        //                                 {
        //                                     Id = x.Id,
        //                                     SponsorId = r.SponsorId,
        //                                     IsActive = x.IsActive,
        //                                     NameEN = t.NameEn,
        //                                     HandSide = r.HandSide
        //                                 }).FirstOrDefaultAsync();
        //        if (sponserData != null)
        //        {
        //            var handSide = sponserData.HandSide;
        //            if (handSide == "Right")
        //            {
        //                finalchildId = GetChildIdByParent(sponserData.Id, "Left");
        //            }
        //            else if (handSide == "Left")
        //            {
        //                finalchildId = GetChildIdByParent(sponserData.Id, "Right");
        //            }
        //            getPersonalNameData = GetData(finalchildId);
        //        }

        //        return getPersonalNameData == null ? StatusCode(404, "Not Found") : Ok(getPersonalNameData);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}








        /////   [Authorize]
        //// [HttpGet]
        //[HttpGet("GetPersonalNameByReferId")]
        //public async Task<IActionResult> GetPersonalNameByReferId(string ReferId)
        //{
        //    try
        //    {
        //        var userAttribute = await (from e in momDb.CustomerAttributes
        //                                   where e.ReferId == ReferId
        //                                   select e).FirstOrDefaultAsync();

        //        if (userAttribute != null)
        //        {
        //            var data = await GetPersonalName(userAttribute.Id);
        //            return data == null ? StatusCode(404, "Not Found") : Ok(data);
        //        }
        //        else
        //        {
        //            return StatusCode(404, "Not Found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}


        ////  [Authorize]
        ////   [HttpGet]
        //[HttpGet("GetCommunityName")]
        //public async Task<IActionResult> GetCommunityName(int userAttributeId)
        //{
        //    try
        //    {
        //        int finalchildId = default(int);
        //        object getCommunityNameData = null;

        //        var sponserData = await (from x in momDb.CustomerAttributes
        //                                 join c in momDb.CustomerNetwork on x.Id equals c.ChildId into net
        //                                 from r in net.DefaultIfEmpty()
        //                                 join t in momDb.CustomerInfo on x.CustomerInfoId equals t.Id
        //                                 where x.Id == userAttributeId
        //                                 select new
        //                                 {
        //                                     Id = x.Id,
        //                                     SponsorId = r.SponsorId,
        //                                     IsActive = x.IsActive,
        //                                     NameEN = t.NameEn,
        //                                     HandSide = r.HandSide
        //                                 }).FirstOrDefaultAsync();

        //        if (sponserData != null)
        //        {
        //            var handSide = sponserData.HandSide;
        //            if (handSide == "Right")
        //            {
        //                finalchildId = GetChildIdByParent(sponserData.Id, "Right");
        //            }
        //            else if (handSide == "Left")
        //            {
        //                finalchildId = GetChildIdByParent(sponserData.Id, "Left");
        //            }
        //            getCommunityNameData = GetData(finalchildId);
        //        }

        //        return getCommunityNameData == null ? StatusCode(404, "Not Found") : Ok(getCommunityNameData);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}




        ////  [Authorize]
        ////   [HttpGet]
        //[HttpGet("GetCommunityNameByReferId")]
        //public async Task<IActionResult> GetCommunityNameByReferId(string referId)
        //{
        //    try
        //    {
        //        var userAttributeId = await (from e in momDb.CustomerAttributes
        //                                     where e.ReferId == referId
        //                                     select e.Id).FirstOrDefaultAsync();
        //        var data = GetCommunityName(userAttributeId);

        //        return data == null ? StatusCode(404, "Not Found") : Ok(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}





        [HttpGet("EnrollerTree")]
        public async Task<IActionResult> EnrollerTree(string sponsorId)
        {
            int sponsor = await momDb.CustomerAttributes
                    .Where(a => a.ReferId == sponsorId).Select(s => s.Id)
                    .FirstOrDefaultAsync();

            int roleId = await momDb.Roles
.Where(c => c.Name == "InActive")
.Select(c => c.Id)
.FirstOrDefaultAsync();



            if (sponsor != 0)
            {

                var NetworkList = await momDb.CustomerNetwork
                                   .Where(a => a.SponsorId == sponsor)
                                   .Include(c => c.Child)
                                   .ThenInclude(c => c.CustomerInfo)
                                   .ToListAsync();



                if (NetworkList.Count != 0)
                {
                    List<CustomerAttributeInfo> customerAtteributeIds = new List<CustomerAttributeInfo>();
                    List<CustomerDataInHoldTankDTO> CustomersData = new List<CustomerDataInHoldTankDTO>();

                    foreach (var i in NetworkList)
                    {

                        var customerData = new CustomerDataInHoldTankDTO
                        {
                            customerAttributeId = i.Child?.Id,
                            Name = i.Child.CustomerInfo.NameEn,
                            Email = i.Child.CustomerInfo?.Email,
                            BackOfficeId = i.Child?.ReferId,
                            HasParent = i.ParentId != null ? true : false,
                            Status = i.Child.CustomerInfo.RoleId == roleId ? "Not registered yet" : "Active"
                        };

                        CustomersData.Add(customerData);



                    }




                    return Ok(CustomersData);
                }
                else
                {
                    // Return an empty list if NetworkList is empty
                    return NotFound("Dont have a network");
                }  
            }
            else
            {
                // Return an empty list if sponsor is null
                return NotFound("Dont have a network");
            }
        }


        [HttpGet("GetCustomerNetwork")]
        public async Task<IActionResult> GetCustomerNetwork(int ParentId)
        {
            try
            {
                List<NetworkDTO> network;

                network = momDb.CustomerNetwork.Where(c => c.Parent.Id == ParentId)
                    .Select(s => new NetworkDTO
                    {
                        ChildId = (int)s.ChildId,
                        HandSide = s.HandSide,
                        ChildName = s.Child.CustomerInfo.NameEn,
                        ChildEmail = s.Child.CustomerInfo.Email,
                        referrid = s.Child.ReferId,
                        rank = s.Child.Rank.Rank1,
                        mobile = s.Child.CustomerInfo.Mobile,
                    }).ToList();

                if (network.Count == 0)
                    return NotFound("don't have anyone in his network");
                else
                    return Ok(network);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("EnrollerTree111111")]
        public async Task<IActionResult> EnrollerTree111111(string sponsorId)
        {
            int sponsor = await momDb.CustomerAttributes
                                .Where(a => a.ReferId == sponsorId)
                                .Select(s => s.Id)
                                .FirstOrDefaultAsync();

            int roleId = await momDb.Roles
                                .Where(c => c.Name == "InActive")
                                .Select(c => c.Id)
                                .FirstOrDefaultAsync();

            if (sponsor != 0)
            {
                List<Customer_DataIn_Hold_Tank_DTO> customersData = new List<Customer_DataIn_Hold_Tank_DTO>();

                async Task<List<Customer_DataIn_Hold_Tank_DTO>> GetChildren(int sponsorId)
                {
                    var networkList = await momDb.CustomerNetwork
                                            .Where(a => a.SponsorId == sponsorId)
                                            .Include(c => c.Child)
                                            .ThenInclude(c => c.CustomerInfo)
                                            .ToListAsync();

                    List<Customer_DataIn_Hold_Tank_DTO> childrenData = new List<Customer_DataIn_Hold_Tank_DTO>();

                    foreach (var i in networkList)
                    {
                        var customerData = new Customer_DataIn_Hold_Tank_DTO
                        {
                            customerAttributeId = i.Child?.Id,
                            Name = i.Child.CustomerInfo.NameEn,
                            Email = i.Child.CustomerInfo?.Email,
                            BackOfficeId = i.Child?.ReferId,
                            HasParent = true,
                            Status = i.Child.CustomerInfo.RoleId == roleId ? "Not registered yet" : "Active"
                        };

                        var grandchildren = await GetChildren(i.Child.Id);
                        if (grandchildren.Any())
                        {
                            customerData.Children = grandchildren;
                        }

                        childrenData.Add(customerData);
                    }

                    return childrenData;
                }

                var sponsorData = await GetChildren(sponsor);

                return Ok(new OkObjectResult(sponsorData));
            }
            else
            {
                // Return an empty list if sponsor is null
                return Ok(new List<CustomerDataInHoldTankDTO>());
            }
        }

    }
}
