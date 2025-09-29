using MailChimp.Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;

using Ovia.Models;
using Ovia.Services.SendEmails;
using Ovia.Services.StorageFiles;
using RestSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using Ovia.Services;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using DocumentFormat.OpenXml.Vml;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Twilio.TwiML.Voice;
using System.Threading.Tasks;


namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly MomEntity momDb;
        private readonly IStorageService storage;
        private readonly IMailingServices mailing;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FilesServices _filesServices;

        public AdminController(
            MomEntity _momDb,
            IStorageService _storage,
            IMailingServices _mailing,
            IHttpClientFactory httpClientFactory,
            FilesServices filesServices)
        {
            this.momDb = _momDb;
            this.storage = _storage;
            this.mailing = _mailing;
            this._httpClientFactory = httpClientFactory;
            _filesServices = filesServices;

        }

        [HttpGet("GetUplines")]
        public async Task<IActionResult> GetUplines(int id)
        {
            var result = await momDb.CustomerNetwork
                     .Where(cn => cn.UplineHistoryId.Contains($"/{id}/"))
                     .CountAsync();
            return Ok(result);

        }


        [HttpGet("AchieveRanks")]
        public async Task<IActionResult> AchieveRanks()
        {

            var customers = await momDb.CustomerAttributes
                           .Select(c => new { Id = c.Id, RankId = c.RankId })
                           .OrderByDescending(c => c.Id)
                           .ToListAsync();
            if (customers.Count != 0)
            {

                foreach (var customer in customers)
                {
                    DateTime todayDate = DateTime.Today;

                    // Calculate the start and end dates for the current week (assuming week starts on Sunday)
                    DateTime reportFromDate_1 = todayDate
                                   .AddDays(-(int)todayDate.DayOfWeek + (int)DayOfWeek.Sunday - 6);
                    DateTime reportToDate_1 = reportFromDate_1.AddDays(7);

                    // Calculate the start and end dates for the previous week
                    DateTime reportFromDate_2 = reportFromDate_1.AddDays(-7);
                    DateTime reportToDate_2 = reportFromDate_2.AddDays(6);

                    var points = await momDb.Points
                 .Where(c => c.CustomerId == customer.Id && c.PointDate >= reportFromDate_2 && c.PointDate <= reportToDate_1)
                 .ToListAsync();


                    var leftPointsSum = points.Where(s => s.Side == "Left").Sum(s => s.PointCount);
                    var rightPointsSum = points.Where(s => s.Side == "Right").Sum(s => s.PointCount);

                    //get distributer numbers
                    var distributersNumber = await (from cn in momDb.CustomerNetwork
                                                    join ca in momDb.CustomerAttributes on cn.ChildId equals ca.Id
                                                    join ci in momDb.CustomerInfo on ca.CustomerInfoId equals ci.Id
                                                    where cn.SponsorId == customer.Id && ci.RoleId == 3 && ca.Renewal == true
                                                    select cn.ChildId).SumAsync();

                    if (customer.RankId == 1)
                    {
                        var rank = await GetRankDetails(2);
                        //Executive
                        if (distributersNumber >= rank.SponsorFor &&
                            leftPointsSum >= rank.CvOn2Sides &&
                            rightPointsSum >= rank.CvOn2Sides)
                        
                            await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id , 2,1,2);
                          
                        
                    }

                    if (customer.RankId == 2)
                    {
                        var rank = await GetRankDetails(3);

                        //Manager
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                            await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 3, 2, 3);



                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 1, 2, 1);

                       

                    }
                    if (customer.RankId == 3)
                    {
                        var rank = await GetRankDetails(4);

                        //Silver Manager
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                        
                            await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 4, 3, 4);




                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 2, 3, 2);

                      

                    }

                    if (customer.RankId == 4)
                    {
                        var rank = await GetRankDetails(5);
                        //Gold Manager 
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                            await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 5, 4, 5);
                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 3, 4, 3);
                    }

                    if (customer.RankId == 5)
                    {
                        var rank = await GetRankDetails(6);

                        //Platinum Manager  
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)          
                            await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 6, 5, 6);
                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 4, 5, 4);
                    }

                    if (customer.RankId == 6)
                    {
                        var rank = await GetRankDetails(7);

                        //Elite Manager  
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                            await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 7, 6, 7);

                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 5, 6, 5);

                    }

                    if (customer.RankId == 7)
                    {
                        var rank = await GetRankDetails(8);

                        //Director   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                            await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 8, 7, 8);


                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 6, 7, 6);

                    }


                    if (customer.RankId == 8)
                    {
                        var rank = await GetRankDetails(9);

                        //Sapphire Director   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                            await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 9, 8, 9);

                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 7, 8, 7);

                    }

                    if (customer.RankId == 9)
                    {
                        var rank = await GetRankDetails(10);

                        //Ruby Director   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                        {
                            //should 4 distributer achieve gold rank



                            //the date that customer reach to rank id = 9          
                            DateTime creationDate = await momDb.RankHistory.Where(c => c.DistributerId == customer.Id && c.NewRankId == 9).Select(c => c.CreationDate).FirstOrDefaultAsync();
                            //get the distributers for  user
                            var sumChildId = await GetDistributersNumberThatAchieveRank(5, creationDate, customer.Id);
                            if (sumChildId >= 4)
                            {
                                await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 10, 9, 10);

                            }

                        }
                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 8, 9, 10);

                    }

                    if (customer.RankId == 10)
                    {
                        var rank = await GetRankDetails(11);

                        //Emerald Director   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                        {

                            //the date that customer reach to rank id = 9          
                            DateTime creationDate = await momDb.RankHistory.Where(c => c.DistributerId == customer.Id && c.NewRankId == 10).Select(c => c.CreationDate).FirstOrDefaultAsync();
                            //get the distributers for  user
                            var sumChildId = await GetDistributersNumberThatAchieveRank(5, creationDate, customer.Id);
                            // 6 distributer is gold
                            if (sumChildId >= 6)
                            {
                                await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 11, 10, 11);
                            }
                        }
                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 9, 10, 9);

                    }



                    if (customer.RankId == 11)
                    {
                        var rank = await GetRankDetails(12);

                        //Black Dimaond   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                        {

                            //the date that customer reach to rank id = 9          
                            DateTime creationDate = await momDb.RankHistory.Where(c => c.DistributerId == customer.Id && c.NewRankId == 11).Select(c => c.CreationDate).FirstOrDefaultAsync();
                            //get the distributers for  user
                            var sumChildId = await GetDistributersNumberThatAchieveRank(8, creationDate, customer.Id);
                            if (sumChildId >= 4)  // 4 distributer Achieved --->> the Director Manager rank
                            {
                                await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 12, 11, 12);
                            }
                        }
                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 10, 11, 10);

                    }



                    if (customer.RankId == 12)
                    {
                        var rank = await GetRankDetails(13);

                        //Presedential Diamond   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                        {


                            //the date that customer reach to rank id = 9          
                            DateTime creationDate = await momDb.RankHistory.Where(c => c.DistributerId == customer.Id && c.NewRankId == 12).Select(c => c.CreationDate).FirstOrDefaultAsync();
                            //get the distributers for  user
                            var sumChildId = await GetDistributersNumberThatAchieveRank(9, creationDate, customer.Id);
                            if (sumChildId >= 6)  //  6 distributer achieve the  Sapphire Director rank
                            {
                                await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 13, 12, 13);       
                            }
                        }
                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 11, 12, 11);
                    }

                    if (customer.RankId == 13)
                    {
                        var rank = await GetRankDetails(14);

                        //Ambassador  Diamond   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                        {

                            //the date that customer reach to rank id = 9          
                            DateTime creationDate = await momDb.RankHistory.Where(c => c.DistributerId == customer.Id && c.NewRankId == 13).Select(c => c.CreationDate).FirstOrDefaultAsync();
                            //get the distributers for  user
                            var sumChildId = await GetDistributersNumberThatAchieveRank(10, creationDate, customer.Id);
                            if (sumChildId >= 6)  //  6 distributer achieve the  Ruby Director rank
                            {
                                await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 14, 13, 14);

                              
                            }
                        }
                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 12, 13, 12);


                    }

                    if (customer.RankId == 14)
                    {
                        var rank = await GetRankDetails(15);

                        //Royal  Diamond   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                        {

                            //the date that customer reach to rank id = 9          
                            DateTime creationDate = await momDb.RankHistory.Where(c => c.DistributerId == customer.Id && c.NewRankId == 14).Select(c => c.CreationDate).FirstOrDefaultAsync();
                            //get the distributers for  user
                            var sumChildId = await GetDistributersNumberThatAchieveRank(2, creationDate, customer.Id);
                            if (sumChildId >= 6)  //  6 distributer achieve the  blue  diamond rank 
                            {
                                await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 15, 14, 15);
                            }
                        }
                        //else
                      
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 13, 14, 13);


                    }


                    if (customer.RankId == 15)
                    {
                        var rank = await GetRankDetails(16);

                        //Crown  Diamond   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                        {
                            //the date that customer reach to rank id = 9          
                            DateTime creationDate = await momDb.RankHistory.Where(c => c.DistributerId == customer.Id && c.NewRankId == 15).Select(c => c.CreationDate).FirstOrDefaultAsync();
                            //get the distributers for  user
                            var sumChildId = await GetDistributersNumberThatAchieveRank(12, creationDate, customer.Id);
                            if (sumChildId >= 6)         //  6 distributer achieve the  black  diamond rank 
                            {
                                await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 16, 15, 16);

                               
                            }
                        }
                        //else
                        await Update_Customer_Rank(customer.Id, 14);
                        await Customer_Rank_History(customer.Id, 15, 14);

                    }
                    //if it in the last Rank  ==> check if achieve the same rank
                    if (customer.RankId == 16)
                    {
                        var rank = await GetRankDetails(16);

                        //Crown  Diamond   
                        if (distributersNumber >= rank.SponsorFor && leftPointsSum >= rank.CvOn2Sides && rightPointsSum >= rank.CvOn2Sides)
                        {
                            //the date that customer reach to rank id = 9          
                            DateTime creationDate = await momDb.RankHistory.Where(c => c.DistributerId == customer.Id && c.NewRankId == 15).Select(c => c.CreationDate).FirstOrDefaultAsync();
                            //get the distributers for  user
                            var sumChildId = await GetDistributersNumberThatAchieveRank(12, creationDate, customer.Id);
                            if (sumChildId >= 6)         //  6 distributer achieve the  black  diamond rank 
                            {
                                await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 16, 16, 16);

                               
                            }
                        }
                        //else
                        await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 15, 16, 15);


                    }

                }


                return Ok("Updated Ranking");
            }
            else
            {
                return NotFound("Not found users");
            }
        

    }













































        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<int> GetDistributersNumberThatAchieveRank(int RankId, DateTime CreationDate, int CustomerId)
        {
            var sumChildId = await (from cn in momDb.CustomerNetwork
                                    join rh in momDb.RankHistory on cn.ChildId equals rh.DistributerId
                                    where rh.NewRankId >= RankId // gold Rank
                                    && rh.CreationDate.Date >= CreationDate
                                    && cn.UplineHistoryId.Contains($"/{CustomerId}/")
                                    select cn.ChildId).SumAsync();

            return (int)sumChildId;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private async System.Threading.Tasks.Task UpdateCustomerRank_And_AddCustomerRankHistory(int customerId, int rankId, int OldRankId, int NewRankId)
        {
            await Update_Customer_Rank(customerId,rankId);
            await Customer_Rank_History(customerId,  OldRankId,  NewRankId);

        }



        [ApiExplorerSettings(IgnoreApi = true)]
        private async System.Threading.Tasks.Task Update_Customer_Rank(int customerId, int rankId)
        {
            var customerRank = await momDb.CustomerAttributes.FirstOrDefaultAsync(c => c.Id == customerId);

            if (customerRank != null)
            {
                customerRank.RankId = rankId;
                await momDb.SaveChangesAsync();
            }
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private async System.Threading.Tasks.Task Customer_Rank_History(int customerId, int OldRankId, int NewRankId)
        {
            momDb.RankHistory.Add(new RankHistory
            {
                Id = 0,
                DistributerId = customerId,
                OldRankId = OldRankId,
                NewRankId = NewRankId,
                CreationDate = DateTime.Now
            });
            await momDb.SaveChangesAsync();
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<Models.Rank> GetRankDetails(int rankId)
        {
            var rank = await momDb.Rank.FirstOrDefaultAsync(r => r.Id == rankId);
            return rank;
        }







    

        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<CustomerAttribute> getCustomerRank(int customerId)
        {
            var customerRank = await momDb.CustomerAttributes.Where(c => c.Id == customerId)
                .Include(c => c.Rank).FirstOrDefaultAsync();
            if (customerRank != null)
                return customerRank;
            throw new Exception("Customer not found");
        }



     
        [HttpGet("GetAllRequestsCash")]
        public async Task<IActionResult> GetAllRequestsCash()
        {
            var requests = await momDb.RequestCash
                .Where(c => c.IsPaid == false)
                .ToListAsync();

            if (requests.Count == 0)
                return NotFound("Not found any request cash");
            else
            {
                var RequestList = new List<GetRequestCash>();
                foreach( var req in requests)
                {

                    var binance = momDb.RequestsCards.FirstOrDefault(c => c.CustomerId == req.CustomerId);
                    var binanceId = binance != null ? binance.CardId : null;




                    var CustomerData =await momDb.CustomerAttributes
                        .Where(c => c.Id == req.CustomerId)
                        .Include(c => c.CustomerInfo)
                        .FirstOrDefaultAsync();

                    var requ = new GetRequestCash
                    {
                        Id =req.Id,
                        CustomerId = req.CustomerId,
                        CustomerEmail = CustomerData.CustomerInfo.Email,
                        CustomerName = CustomerData.CustomerInfo.NameEn,
                        ReferId = CustomerData.ReferId,
                        BinanceId = binanceId,
                        RequestedAmount = req.RequestedAmount,
                        RequestDate = req.RequestDate,
                        RequestTypeId = req.RequestTypeId,
                        IsPaid = req.IsPaid,
                        PaidBy = req.PaidBy,
                        PaidDate = req.PaidDate,
                       UnPaid = req.UnPaid,
                       UnPaidDate = req.UnPaidDate

                    };

                    RequestList.Add(requ);
                }
                return Ok(RequestList);

            }

        }

       

        [HttpPut("SendCash")]
        public async Task<IActionResult> SendCash(int requestId, int adminId)
        {
            var request = await momDb.RequestCash
                .FirstOrDefaultAsync(c => c.Id == requestId);

            
            if (request == null)
                return NotFound("Not found any request cash");
            else
            {
                request.IsPaid = true;
                request.PaidDate = DateTime.Now;
                request.PaidBy = adminId;
               
                await momDb.SaveChangesAsync();




                //         var customerBalance = await momDb.CustomerAccountBalances
                //.Where(c => c.CustomerId == request.CustomerId)
                //.OrderByDescending(c => c.Id) // Assuming Id is the primary key or a field indicating the order
                //.FirstOrDefaultAsync();


                //         customerBalance.Debit = request.RequestedAmount;
                //         customerBalance.Balance -= request.RequestedAmount;
                //         customerBalance.Description = "Transfer from momentum";

                //         await momDb.SaveChangesAsync();

                var Customer_Info = await momDb.CustomerAttributes
                      .Where(c => c.Id == request.CustomerId)
                      .Include(c => c.CustomerInfo)
                      .FirstOrDefaultAsync();

                decimal commission = request.RequestedAmount ?? 0; // Use a default value if the commission is null
                DateTime paidDate = request.PaidDate ?? DateTime.MinValue; // Use a default value if the paid date is null

                await mailing.sendEmailToCustomerWhenAdminConvertHisMoney(
                    Customer_Info.CustomerInfo.Email,
                    Customer_Info.CustomerInfo.NameEn,
                    commission,
                    paidDate
                );

                return Ok("Paid Successfully");


            }

        }





        #region Package Type
        [HttpPost("AddPackageType")]
        public async Task<IActionResult> AddPackageType(string packageType)
        {
            if (packageType == null)
            {
                return Ok(ErrorEventArgs.Empty);
            }

            else
            {
                var existingPackage = await momDb.PackagesTypes
                    .Where(p => p.Name == packageType)
                    .FirstOrDefaultAsync();
                if (existingPackage != null)
                {

                    return Ok("this package type is founded");
                }
                var package = new PackagesTypes
                {
                    Id = 0,
                    Name = packageType,
                    IsActive = true,
                    IsDeleted = false,
                };


                momDb.PackagesTypes.Add(package);
                await momDb.SaveChangesAsync();


                return Ok(package);
            }

        }




        [HttpGet("GetAllPackageTypes")]
        public async Task<IActionResult> GetAllPackageTypes( )
        {
            var types = await momDb.PackagesTypes.ToListAsync();

            if (types.Count == 0)
            {
                return Ok("Not found any package types");
            }
            else
                return Ok(types);
        }


        [HttpPut("UpdatePackageTypes")]
        public async Task<IActionResult> UpdatePackageTypes(updatePackageTypeDTO dto)
        {
            var existingPackage = await momDb.PackagesTypes
                .Where(p => p.Id == dto.id)
                .FirstOrDefaultAsync(); 
            if(existingPackage == null)
            {
                return Ok("This package not found");
            }

            else
            {
                if(dto.name == null)
                {
                    return Ok(ErrorEventArgs.Empty);
                }
                else
                {
                    existingPackage.Name = dto.name;
                    await momDb.SaveChangesAsync();

                    return Ok(existingPackage);
                }


            }





        }




        [HttpDelete("DeletePackageTypes")]
        public async Task<IActionResult> DeletePackageTypes(int PackageTypeId)
        {
            var exisitingType = await momDb.PackagesTypes
               .Where(p => p.Id == PackageTypeId)
               .FirstOrDefaultAsync();

            if(exisitingType == null) {
                return Ok("This package type not found");
            }
            else
            {
                momDb.PackagesTypes.Remove(exisitingType);
                await momDb.SaveChangesAsync();

                var packages = await momDb.Packages
                    .Where(p => p.PackageTypeId == PackageTypeId)
                    .ToListAsync();

                if(packages.Count != 0)
                {
                    momDb.RemoveRange(packages);
                    await momDb.SaveChangesAsync();

                }

                return Ok("This package type and its packages deleted successfully!");


            }
        }

        #endregion


        #region Package
        [HttpPost("AddPackage")]
        public async Task<IActionResult> AddPackage([FromForm]AddBackageDTO dto)
        {
            //// Check if DTO is null or invalid
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var existingType = await momDb.PackagesTypes
                .FirstOrDefaultAsync(p => p.Id == dto.PackageTypeId);

            if (existingType == null)
            {
                return NotFound("This package type was not found.");
            }

            var existingPackage = await momDb.Packages
                .FirstOrDefaultAsync(p => p.Name == dto.Name);

            if (existingPackage != null)
            {
                return Conflict("This package already exists.");
            }

            var package = new Package
            {
                PackageTypeId = dto.PackageTypeId,
                Name = dto.Name,
                ShortDescription = dto.ShortDescription,
                FullDescription = dto.FullDescription,
                Price = dto.Price,
                ShowOnHomePage = false,
                Published = false,
                SponsorDistributorToDistributor = dto.SponsorDistributorToDistributor,
                BusinessValue = dto.BusinessValue
            };

            momDb.Packages.Add(package);
            await momDb.SaveChangesAsync();

            if (dto.File != null)
            {
                var uploadedFile = await storage.Upload(dto.File);

                var file = new PackageImage
                {
                    PackageId = package.Id,
                    FileName = uploadedFile.FileName,
                    Key = uploadedFile.Key,
                    Extension = uploadedFile.Extension,
                    FileSize = uploadedFile.FileSize
                };

                momDb.PackageImages.Add(file);
                await momDb.SaveChangesAsync();
                return Ok(package);

            }
            else
                return Ok(package);

        }




        [HttpGet("GetAllPackages")]
        public async Task<IActionResult> GetAllPackages()
        {
            var packages = await momDb.Packages.Where(x=> x.Published == true).ToListAsync();

            if (packages.Count == 0)
            {
                return Ok("Not found any packages");
            }
            else {

                List<GetPackagesDTO> AllPackages = new List<GetPackagesDTO>();

               foreach(var p in packages)
                {
                    var image = await momDb.PackageImages
                                        .Where(i => i.PackageId == p.Id)
                                        .FirstOrDefaultAsync();

                    string url = "";

                    if (image != null) {
                      url = image?.Key?.SetDownloadFileUrlByKey(storage);
                    }

                    var pack = new GetPackagesDTO
                    {
                        Id= p.Id,
                        Name = p.Name,
                        ShortDescription = p.ShortDescription,
                        FullDescription = p.FullDescription,
                        ShowOnHomePage = p.ShowOnHomePage,
                        Key = image?.Key,
                        Url = url,
                        Price = p.Price,
                        OldPrice = p.OldPrice,
                        BusinessValue = p.BusinessValue,
                        DisplayOrder = p.DisplayOrder,
                        MembershipID = p.MembershipID,
                        Published = p.Published,
                        PackageTypeId = p.PackageTypeId,
                        SponsorCustomerToCustomer = p.SponsorCustomerToCustomer,
                        SponsorDistributorToCustomer = p.SponsorDistributorToCustomer,
                        SponsorDistributorToDistributor = p.SponsorDistributorToDistributor,
                        Summit_Coins = p.Summit_Coins,
                        Summit_Cost = p.Summit_Cost,
                        TeamId = p.TeamId,
                        SponsorTeam = p.SponsorTeam

                    };

                    AllPackages.Add(pack);
                }
                return Ok(AllPackages);
            }


        }



        [HttpPut("UpdateImagePackage")]
        public async Task<IActionResult> UpdateImagePackage(int packageId, IFormFile img)
        {
            var package = await momDb.Packages.FirstOrDefaultAsync(s => s.Id == packageId);
            if(package == null)
            {
                return BadRequest("This package not found");
            }


            var existingImage = await momDb.PackageImages
                .FirstOrDefaultAsync(i => i.PackageId == packageId);

            if(img == null)
            {
                return Ok("Invalid Image");
            }
            else
            {
                var uploaded = await storage.Upload(img);

                //updated in current image
                if (existingImage != null)
                {

                    existingImage.Key = uploaded.Key;
                    existingImage.Extension = uploaded.Extension;
                    existingImage.FileName = uploaded.FileName;
                    existingImage.FileSize = uploaded.FileSize;

                    await momDb.SaveChangesAsync();
                    return Ok("Updated Successfully");

                }
                //upload new image
                else
                {
                    var packageImage = new PackageImage
                    {
                        Id = 0,
                        PackageId = packageId,
                        Key = uploaded.Key,
                        FileName = uploaded.FileName,
                        FileSize = uploaded.FileSize,
                        Extension = uploaded.Extension
                    };


                     momDb.PackageImages.Add(packageImage);
                    await momDb.SaveChangesAsync();

                    return Ok("Added Successfully");

                }

            }



        }

        //show package in home
        [HttpPut("PackageShowInHomePage")]
        public async Task<IActionResult> PackageShowInHomePage(int packageId)
        {
            var package = await momDb.Packages
                .FirstOrDefaultAsync(p => p.Id == packageId);

            if(package != null)
            {
                package.ShowOnHomePage = !package.ShowOnHomePage;
                await momDb.SaveChangesAsync();
                return Ok("Updated Successfully");
            }
            else
                return Ok("This Package Not Found");

        }

        //package published
        [HttpPut("PackagePublished")]
        public async Task<IActionResult> PackagePublished(int packageId)
        {
            var package = await momDb.Packages
                .FirstOrDefaultAsync(p => p.Id == packageId);

            if (package != null)
            {
                package.Published = !package.Published;
                await momDb.SaveChangesAsync();
                return Ok("Updated Successfully");
            }
            else
                return Ok("This Package Not Found");

        }




        #endregion



        #region add user to ns starting balance
        [HttpPost("AddNsStartingBalanceToUser")]
        public async Task<IActionResult> AddBalanceToUser(AddNsStartBalanceDTO dto)
        {
            var existingUserBalance = await momDb.NsStartingBalances.
                Where(c=> c.NsId == dto.NsId)
    .OrderByDescending(c => c.Id) 
    .FirstOrDefaultAsync(c => c.NsId == dto.NsId);


            if (existingUserBalance != null)
            {
                existingUserBalance.DailyStartingBalance += dto.DailyStartingBalance;
                existingUserBalance.Remain += dto.DailyStartingBalance;
                existingUserBalance.Type = dto.Type;
                await momDb.SaveChangesAsync();
                return Ok("Updated Successfully");
            }
            else
            {
                var nsUser = new NsStartingBalance
                {
                    Id = 0,
                    NsId = dto.NsId,
                    Type = dto.Type,
                    paid = 0,
                    DailyStartingBalance = dto.DailyStartingBalance,
                    Remain = dto.DailyStartingBalance,
                    IsActive = true
                };

                momDb.NsStartingBalances.Add(nsUser);
                await momDb.SaveChangesAsync();

            }




            var transactionid = GenerateTransactionId(dto.AdminId);

            var PaymentHistory = new NsPaymentHistory
            {
                Id = 0,
                NS_ID =dto.AdminId ,
                Amount = dto.DailyStartingBalance,
                Transfered_To_ID = dto.NsId,
                Transaction_ID = transactionid,
                Transaction_Date = DateTime.Now,
                Notes = $@"Add money by Admin with transaction serial {transactionid}"
            };
            momDb.NsPaymentHistory.Add(PaymentHistory);
            await momDb.SaveChangesAsync();



            return Ok("balance added successfully ");
        }



        // Generate transaction ID
        private string GenerateTransactionId(int senderId)
        {
            Random rand = new Random();
            return $"{senderId}{rand.Next(1000, 10000)}";
        }


        #endregion



        #region Add Point Equation
        [HttpPost("AddPointEquation")]
        public async Task<IActionResult> AddPointEquation(AddPointEquationDTO dto)
        {
            

            var point = new PointEquation
            {   Id=0,
                Points = dto.Points,
                BalanceEG = dto.BalanceEG,
                BalanceUSD = dto.BalanceUSD,
                Description = dto.Description
            };
            momDb.pointEquations.Add(point);
            await momDb.SaveChangesAsync();

            return Ok("Added Successfully");

        }
        #endregion

        #region Courses
        //add course
        [HttpPost("AddCourse")]
        public async Task<IActionResult> AddCourse([FromForm] AddCourseDTO dto)
        {
            try
            {
                // Check if the provided CourseTypeId exists
                var coursetype = await momDb.CourseType.FirstOrDefaultAsync(c => c.Id == dto.CourseTypeId);
                if (coursetype == null)
                {
                    return NotFound("Course type not found. Unable to add course.");
                }

                var key = "";
                if (dto.Img != null)
                {
                    var uploaded = await storage.Upload(dto.Img);
                    key = uploaded.Key;
                }

                // Create a new Course instance
                var course = new Course
                {
                    NameEn = dto.NameEn,
                    ShortDescriptionEn = dto.ShortDescriptionEn,
                    FullDescriptionEn = dto.FullDescriptionEn,
                    CourseTypeId = dto.CourseTypeId,
                    TotalHour = dto.TotalHour,
                    NumberLecture = dto.NumberLecture,
                    Price = dto.Price,
                    Key = key,
                    IsActive = false
                };

                // Add the course to the database
                momDb.Courses.Add(course);

                // Save changes to the database
                await momDb.SaveChangesAsync();

                // Return the newly created course
                return Ok("Course added successfully");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // You can customize this part based on your logging mechanism
                Console.WriteLine($"An error occurred while adding a new course: {ex.Message}");

                // Return a generic error message
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        //Get Package Courses
        [HttpGet("GetPackageCourses")]
        public async Task<IActionResult> GetPackageCourses(int packageId)
        {
            var courses = await momDb.Courses
                .Where(c=>c.CourseTypeId == packageId)
                .ToListAsync();

            if(courses.Count != 0)
            {
                List<GetCoursesDTO> coursesList = new List<GetCoursesDTO>();    
                foreach(var course in courses)
                {
                    string url = null;
                    if( course.Key.Length != 0)
                    {
                        url =course.Key.SetDownloadFileUrlByKey(storage);
                    }

                    var co = new GetCoursesDTO
                    {
                        Id=course.Id,
                        NameEn = course.NameEn,
                        ShortDescriptionEn = course.ShortDescriptionEn,
                        FullDescriptionEn = course.FullDescriptionEn,
                        Price = course.Price,
                        IsActive = course.IsActive,
                        CourseTypeId = course.CourseTypeId,
                        TotalHour = course.TotalHour,
                        URL = url,
                        NumberLecture = course.NumberLecture
                    };
                    coursesList.Add(co);
                }
                return Ok(coursesList);
            }

            return NotFound("not found courses");
        }

        //update Course Data
        [HttpPut("UpdateCourse")]
        public async Task<IActionResult> UpdateCourse([FromForm]UpdateCourseDTO dto)
        {
            var existCourse = await momDb.Courses
                .FirstOrDefaultAsync(c => c.Id == dto.CourseId);

            if(existCourse != null)
            {
                var key = existCourse.Key;
                if (dto.Img != null)
                {
                    var uploaded = await storage.Upload(dto.Img);
                    key = uploaded.Key;
                }

                existCourse.Key = key;
                existCourse.NameEn = dto.NameEn == null ? existCourse.NameEn : dto.NameEn;
                existCourse.ShortDescriptionEn = dto.ShortDescriptionEn == null ? existCourse.ShortDescriptionEn : dto.ShortDescriptionEn;
                existCourse.FullDescriptionEn = dto.FullDescriptionEn == null ? existCourse.FullDescriptionEn : dto.FullDescriptionEn; 
                existCourse.Price = dto.Price == null ? existCourse.Price : dto.Price;
                existCourse.NumberLecture = dto.NumberLecture == null ? existCourse.NumberLecture : dto.NumberLecture;

                await momDb.SaveChangesAsync();
                return Ok(existCourse);
            }

            return NotFound("Course not found!!!!");

        }



        [HttpDelete("DeleteCourse")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var existCourse = await momDb.Courses
              .FirstOrDefaultAsync(c => c.Id == courseId);

            if(existCourse != null)
            {
                ///remove from  Courses
                momDb.Courses.Remove(existCourse);
                await momDb.SaveChangesAsync();

                ///remove from Instructor mapping
                var courseInstructor = await momDb.CourseInstructorMappings
                     .FirstOrDefaultAsync(c=> c.CourseId == courseId);
               
                if(courseInstructor != null)
                    momDb.CourseInstructorMappings.Remove(courseInstructor);
              
                ///remove from customer mapping
                var courseCustomer = await momDb.CourseCustomerMapping
                   .FirstOrDefaultAsync(c => c.CourseId == courseId);

                if (courseInstructor != null)
                    momDb.CourseCustomerMapping.Remove(courseCustomer);


                await momDb.SaveChangesAsync();
                return Ok("Deleted Successfully");
            }
            return NotFound("Not found");
        }

        #endregion



        // Add Instructor
        [HttpPost("AddInstructor")]
        public async Task<IActionResult>  AddInstructor([FromForm] AddInstructorDTO dto)
        {
            var inst = await momDb.CustomerAttributes
                .Where(c => c.ReferId == dto.InstructorID)
                .Include(c => c.CustomerInfo)
                .FirstOrDefaultAsync();
                

            if (inst != null)
            {
                var uploadImage = await storage.Upload(dto.Image);
                   
                var instructor = new Instructor
                {
                    Id = 0,
                    InsInfoID = inst.CustomerInfoId,
                    NameEn = inst.CustomerInfo.NameEn,
                    JobTitleEn = dto.JobTitleEn,
                    Email = inst.CustomerInfo.Email,
                    Address = dto.Address,
                    Mobile = inst.CustomerInfo.Mobile,
                    AboutEn = dto.AboutEn,
                    Picture = uploadImage.Key
                };

                momDb.Instructors.Add(instructor);
                await momDb.SaveChangesAsync();

                //to be instructore role
                inst.CustomerInfo.RoleId = 4;
                await momDb.SaveChangesAsync();


                return Ok(instructor);

            }
            else
                return NotFound("this customer not found");
        }
        // assign course to instructor
        [HttpPost("AssignCourseToInstructor")]
        public async Task<IActionResult> AssignCourseToInstructor([FromBody] assignCourseToInstructorDTO dto)
        {
            var instructor = await momDb.Instructors.FirstOrDefaultAsync(i => i.Id == dto.InstructorId);
            var course = await momDb.Courses.FirstOrDefaultAsync( c => c.Id == dto.CourseId);

            var courseAssignToinstructor = await momDb.CourseInstructorMappings
                .FirstOrDefaultAsync(x => x.CourseId == dto.CourseId);

            if (course == null)
                return NotFound("Course not found");
            if (instructor == null)
                return NotFound("Instructor not found");
            if (courseAssignToinstructor != null && courseAssignToinstructor.Instructor != null)
            {
                return BadRequest("Course assign instructor already");
            }
                var assign = new CourseInstructorMapping
                {
                    Id = 0,
                    CourseId = dto.CourseId,
                    InstructorId = dto.InstructorId,
                    DisplayOrder = 0
                };
                momDb.CourseInstructorMappings.Add(assign);
                await momDb.SaveChangesAsync();
                return Ok(assign);
            

        }
        // Re assign course to instructor
        [HttpPut("Re_assignCourseToInstructor")]
        public async Task<IActionResult>Re_ssignCourseToInstructor([FromBody] assignCourseToInstructorDTO dto)
        {
            var instructor = await momDb.Instructors.FirstOrDefaultAsync(i => i.Id == dto.InstructorId);
            var course = await momDb.Courses.FirstOrDefaultAsync(c => c.Id == dto.CourseId);

            var courseAssignToinstructor = await momDb.CourseInstructorMappings
                .FirstOrDefaultAsync(x => x.CourseId == dto.CourseId);

            if (course == null)
                return NotFound("Course not found");
            if (instructor == null)
                return NotFound("Instructor not found");
            if (courseAssignToinstructor == null)
            {
                return BadRequest("Course assign to instructor not found");
            }

            courseAssignToinstructor.InstructorId = dto.InstructorId; 
            await momDb.SaveChangesAsync();
            return Ok("Updated Successfully");



        }
        // Delete  course from Assign  instructor
        [HttpDelete("DeletecoursefromAssigninstructor")]
        public async Task<IActionResult> DeletecoursefromAssigninstructor(int mappingId)
        {
            var instructorsAndcourses = await momDb
                .CourseInstructorMappings
                .FirstOrDefaultAsync(i => i.Id == mappingId);

            if (instructorsAndcourses == null)
                return NotFound("Instructor And Course Not Found not found in mapping");

            momDb.CourseInstructorMappings.Remove(instructorsAndcourses);
            await momDb.SaveChangesAsync();
            return Ok("Un Assigned Successfully");




        }
        //Get Courses And Them Instructors
        [HttpGet("GetCoursesAndThemInstructors")]
        public async Task<IActionResult> GetCoursesAndThemInstructors()
        {
            var instructorsAndcourses = await momDb.CourseInstructorMappings
                .Include(cim => cim.Instructor)
                .Include(cim => cim.Course)
                .ToListAsync();

           if (instructorsAndcourses == null )
                return Ok("Not found Any Courses Assign To Instructors");

            //var InsCourMapp = new List<CoursesAndInstructorsDTO>();

            //foreach (var i in instructorsAndcourses)
            //{
            //    var course_and_instructors = new CoursesAndInstructorsDTO
            //    {
            //        Id = i.Id,
            //        InstructorId = i.InstructorId,
            //        InstructorName = i.Instructor?.NameEn,
            //        JobTitle = i.Instructor?.JobTitleEn,
            //        CourseId = i.CourseId,
            //        CourseName = i.Course?.NameEn
            //    };
            //    InsCourMapp.Add(course_and_instructors);
           // }

           // return Ok(InsCourMapp);

            return Ok(instructorsAndcourses);   
        }





        [HttpGet("GetInstructorCourses")]
        public async Task<IActionResult> GetInstructorCourses(int InstructorId)
        {
            var Instructor_Courses = await momDb.CourseInstructorMappings
                .Where(c => c.InstructorId == InstructorId)
                .Include(c => c.Course)
                .Select(c => new { c.Course.Id, c.Course.NameEn })
                .ToListAsync();

            if (Instructor_Courses.Count != 0)
                return Ok(Instructor_Courses);

            return NotFound("Not found courses for this instructor");
        }


        [HttpGet("GetCustomerPackagesReport")]
        public async Task<IActionResult> GetCustomerPackagesReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Ensure proper date range (assuming you want to include both startDate and endDate)
                endDate = endDate.AddDays(1);

                var packages = await momDb.CustomerPackageSelect
                    .Where(p => p.CreationDate >= startDate && p.CreationDate < endDate)
                    .Join(momDb.CustomerNetwork,
                        package => package.CustomerId,
                        network => network.ChildId,
                        (package, network) => new { Package = package, Network = network })
                    .Join(momDb.CustomerAttributes,
                        temp => temp.Network.ChildId,
                        attribute => attribute.Id,
                        (temp, attribute) => new { temp.Package, temp.Network, Attribute = attribute })
                    .Join(momDb.CustomerInfo,
                        temp => temp.Attribute.CustomerInfoId,
                        info => info.Id,
                        (temp, info) => new { temp.Package, temp.Network, temp.Attribute, Info = info })
                    .Join(momDb.CustomerAttributes,
                        temp => temp.Network.SponsorId,
                        sponsor => sponsor.Id,
                        (temp, sponsor) => new
                        {
                            temp.Package.Id,
                            temp.Network.ChildId,
                            temp.Attribute.ReferId,
                            temp.Attribute.CustomerInfoId,
                            temp.Info.NameEn,
                            temp.Info.Mobile,
                            temp.Info.whatsappmobile,
                            temp.Network.SponsorId,
                            SponsorMobile = sponsor.CustomerInfo.Mobile,
                            SponsorWhatsappMobile = sponsor.CustomerInfo.whatsappmobile,
                            SponsorNameEn = sponsor.CustomerInfo.NameEn,
                            temp.Package.Cost
                        })
                    .ToListAsync();

                return Ok(packages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddHomePhoto")]
        public async Task<IActionResult> AddHomePhoto([FromForm] AddHomePhotoDTO dto)
        {
            try
            {
                if (dto.Image == null && dto.Image.Length == 0)
                    return BadRequest("File is required.");

         


                var uploadedImg = await storage.Upload(dto.Image);
                var photo = new HomePhotos
                {
                    Description = dto.Description,
                    Key = uploadedImg.Key,
                    Extension = uploadedImg.Extension,
                    Size = uploadedImg.FileSize,
                    FileName = uploadedImg.FileName,
                    ShowInHome = true,
                    IsActive = true,
                    IsDeleted = false
                };

                momDb.HomePhotos.Add(photo);
                await momDb.SaveChangesAsync();

                return Ok(photo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to upload photo: {ex.Message}");
            }
        }



        [HttpGet("GetHomePhoto")]
        public async Task<IActionResult> GetHomePhoto(string description)
        {
            var photos = await momDb.HomePhotos
                .Where(c => c.Description == description)
                .ToListAsync();


            if(photos.Count != 0)
            {
                var photoList = new List<GetPhotoDTO>();
                foreach (var photo in photos)
                {
                    var url = photo.Key?.SetDownloadFileUrlByKey(storage);
                    var photoDTO = new GetPhotoDTO
                    {
                        Url = url,
                        Name = photo.FileName,
                        Description = photo.Description
                    };
                    photoList.Add(photoDTO);
                }
                return Ok(photoList);
            }
            else
            {
                return BadRequest("Not found any photos");
            }
        }

        [HttpGet("SellesPackageReport")]
        public async Task<IActionResult> SellesPackageReport(DateTimeOffset from, DateTimeOffset to)
        {
            if (from == DateTimeOffset.MinValue && to == DateTimeOffset.MinValue)
                return BadRequest("Enter valid data");

            if (to != null && from > to)
                return BadRequest("Invalid date range: 'from' date should be before 'to' date.");


            var customerPackageSelect = await (from customerPackage in momDb.CustomerPackageSelect
                                       join custattribute in momDb.CustomerAttributes
                                           on customerPackage.CustomerId equals custattribute.Id
                                       join customer in momDb.CustomerInfo
                                           on custattribute.CustomerInfoId equals customer.Id
                                       join package in momDb.Packages
                                           on customerPackage.PackagesId equals package.Id
                                       select new
                                       {
                                           customer.NameEn,
                                           custattribute.ReferId,
                                           package.Name,
                                           package.Price,
                                           customerPackage.InvoiceSerial,
                                           customerPackage.CreationDate
                                       }).ToListAsync();

            
            var report = customerPackageSelect.Where(c => c.CreationDate >= from && c.CreationDate <= to.AddDays(1)).ToList();
            if (report.Count == 0)
                return NotFound("Not found any selles in this date"); 
            return Ok(report);
            





        }



        [HttpGet("SellesPackageReportAsA_PDF")]
        public async Task<IActionResult> SellesPackageReportAsA_PDF(DateTimeOffset? from, DateTimeOffset? to)
        {
            if (from == DateTimeOffset.MinValue && to == DateTimeOffset.MinValue)
                return BadRequest("Enter valid data");

            if (to != null && from > to)
                return BadRequest("Invalid date range: 'from' date should be before 'to' date.");
           
                var customerPackageSelect = await (from customerPackage in momDb.CustomerPackageSelect
                                           join custattribute in momDb.CustomerAttributes
                                               on customerPackage.CustomerId equals custattribute.Id
                                           join customer in momDb.CustomerInfo
                                               on custattribute.CustomerInfoId equals customer.Id
                                           join package in momDb.Packages
                                               on customerPackage.PackagesId equals package.Id
                                           select new
                                           {
                                               customer.NameEn,
                                               custattribute.ReferId,
                                               package.Name,
                                               package.Price,
                                               customerPackage.InvoiceSerial,
                                               customerPackage.CreationDate
                                           }).ToListAsync();

            
                var report = customerPackageSelect.Where(c => c.CreationDate >= from && c.CreationDate <= to!.Value.AddDays(1)).ToList();
                if(report.Count == 0)
                      return NotFound("Not found any selles in this date");


            // Generate the report PDF using the PdfService
            byte[] pdfBytes = _filesServices.GeneratePdfReport(report);

                // Return the PDF as a file attachment
                return File(pdfBytes, "application/pdf", "SellesPackageReport.pdf");


             }





        [HttpGet("SellesPackageReport_AsExcelSheet")]
        public async Task<IActionResult> SellesPackageReport_AsExcelSheet(DateTimeOffset? from, DateTimeOffset? to)
        {
            if (from == DateTimeOffset.MinValue && to == DateTimeOffset.MinValue)
                return BadRequest("Enter valid data");

            if (to != null && from > to)
                return BadRequest("Invalid date range: 'from' date should be before 'to' date.");

            var customerPackageSelect = await (from customerPackage in momDb.CustomerPackageSelect
                                               join custattribute in momDb.CustomerAttributes
                                               on customerPackage.CustomerId equals custattribute.Id
                                               join customer in momDb.CustomerInfo
                                               on custattribute.CustomerInfoId equals customer.Id
                                               join package in momDb.Packages
                                               on customerPackage.PackagesId equals package.Id
                                               select new
                                               {
                                                   customer.NameEn,
                                                   custattribute.ReferId,
                                                   package.Name,
                                                   package.Price,
                                                   customerPackage.InvoiceSerial,
                                                   customerPackage.CreationDate
                                               }).ToListAsync();

          
                customerPackageSelect = customerPackageSelect.Where(c => c.CreationDate >= from && c.CreationDate <= to!.Value.AddDays(1)).ToList();
                if (customerPackageSelect.Count == 0)
                       return NotFound("Not found any selles in this date");


            // Create a new Excel package
            using (var excelPackage = new ExcelPackage())
            {
                // Add a new worksheet
                var worksheet = excelPackage.Workbook.Worksheets.Add("SellesPackageReport");

                // Set the column headers
                worksheet.Cells[1, 1].Value = "Customer Name";
                worksheet.Cells[1, 2].Value = "Refer ID";
                worksheet.Cells[1, 3].Value = "Package Name";
                worksheet.Cells[1, 4].Value = "Price";
                worksheet.Cells[1, 5].Value = "Invoice Serial";
                worksheet.Cells[1, 6].Value = "Creation Date";

                // Fill the data rows
                for (int i = 0; i < customerPackageSelect.Count; i++)
                {
                    var item = customerPackageSelect[i];
                    worksheet.Cells[i + 2, 1].Value = item.NameEn;
                    worksheet.Cells[i + 2, 2].Value = item.ReferId;
                    worksheet.Cells[i + 2, 3].Value = item.Name;
                    worksheet.Cells[i + 2, 4].Value = item.Price;
                    worksheet.Cells[i + 2, 5].Value = item.InvoiceSerial;
                    worksheet.Cells[i + 2, 6].Value = item.CreationDate;
                }

                // Autofit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Convert the Excel package to a byte array
                byte[] excelBytes = excelPackage.GetAsByteArray();

                // Return the Excel file as a file attachment
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SellesPackageReport.xlsx");
            }

        }





        [HttpPost("RequestCashForCustomerByAdmin")]
        public async Task<IActionResult> RequestCustomerCashByAdmin(string backOfficeId, int adminId)
        {
            var custom_Attribute = await momDb.CustomerAttributes
                .FirstOrDefaultAsync(c => c.ReferId == backOfficeId);


            var customerAccountBalance = await momDb.CustomerAccountBalances
    .Where(c => c.CustomerId == custom_Attribute.Id)
    .OrderByDescending(c => c.Id) 
    .FirstOrDefaultAsync();

            if(customerAccountBalance != null)
            {
                if (customerAccountBalance.Balance != 0)
                {
                    var cash = new RequestCash
                    {
                        Id = 0,
                        CustomerId = customerAccountBalance.CustomerId,
                        CreatedBy = adminId,
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
                        Description = "Request caching balance by admin",
                        Credit = 0,
                        Balance = 0,
                    };
                    momDb.CustomerAccountBalances.Add(customer_balance);
                    await momDb.SaveChangesAsync();


                    return Ok($@"You requested cash for back office id {backOfficeId}  successfully");

                }
                else
                    return NotFound("Dont found any money in your balance");


            }
            else
                return NotFound("Dont found any balance for this user");


        }




        [HttpPost("RequestCashForAllCustomersByAdmin")]
        public async Task<IActionResult> RequestCashForAllCustomersByAdmin(int adminId)
        {
            using (var transaction = momDb.Database.BeginTransaction())
            {
                try
                {
                    //var customersBalance = await momDb.CustomerAccountBalances
                    //    .Where(c => c.Balance != 0)
                    //    .ToListAsync();

                    DateTime startDate = new DateTime(2024, 4, 1);
                    DateTime endDate = new DateTime(2024, 4, 2);

                    var customersBalance = await momDb.CustomerAccountBalances
                        .Where(c => c.Balance != 0 && c.TransactionDate >= startDate && c.TransactionDate < endDate.AddDays(1))
                        .GroupBy(c => c.CustomerId)
                        .Select(g => g.OrderByDescending(c => c.TransactionDate).FirstOrDefault())
                        .ToListAsync();

                    return Ok(customersBalance);

                    //if (customersBalance.Count != 0)
                    //{
                    //    foreach (var balance in customersBalance)
                    //    {
                    //        var cash = new RequestCash
                    //        {
                    //            Id = 0,
                    //            CustomerId = balance.CustomerId,
                    //            CreatedBy = adminId,
                    //            CreationDate = DateTime.Now,
                    //            IsActive = true,
                    //            IsDeleted = false,
                    //            IsPaid = false,
                    //            PaidBy = null,
                    //            PaidDate = null,
                    //            RequestDate = DateTime.Now,
                    //            RequestedAmount = balance.Balance
                    //        };
                    //        momDb.RequestCash.Add(cash);

                    //        var customerBalanceTransaction = new CustomerAccountBalance
                    //        {
                    //            Id = 0,
                    //            CustomerId = balance.CustomerId,
                    //            Debit = balance.Balance,
                    //            TransactionDate = DateTime.Now,
                    //            Description = "Request cash  by admin",
                    //            Credit = 0,
                    //            Balance = 0
                    //        };
                    //        momDb.CustomerAccountBalances.Add(customerBalanceTransaction);
                    //    }
                    //    await momDb.SaveChangesAsync();
                    //     transaction.Commit();
                    //    return Ok("Cash requested successfully for all  customers.");
                    //}
                    //else
                    //{
                    //    return NotFound("Not found balnances for customers.");
                    //}
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }









        #region Token Prices or Point values
        [HttpPost("AddTokenPrices")]
        public async Task<IActionResult> AddTokenPrices(decimal points)
        {


            var point = new PointValue
            {
                Id = 0,
                Value = points,
                IsActive = true
            };
            momDb.PointValue.Add(point);
            await momDb.SaveChangesAsync();

            return Ok("Added Successfully");

        }


        [HttpGet("GetTokenPrices")]
        public async Task<IActionResult> GetTokenPrices(decimal points)
        {
            var activePoints = await momDb.PointValue.Where(c => c.IsActive == true).ToListAsync();
            if (activePoints.Count != 0)
                return Ok(activePoints);
            return NotFound("not found any points");

        }


        #endregion




    }
}
