using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.Models;

namespace Ovia.Services
{
    public class AchieveRanks
    {

        private readonly MomEntity momDb;
        public AchieveRanks(MomEntity _momDb)
        {
            momDb = _momDb;
        }





        public async Task  Achieve_Customer_Ranks()
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

                            await UpdateCustomerRank_And_AddCustomerRankHistory(customer.Id, 2, 1, 2);


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


                //return Ok("Updated Ranking");
            }
            //else
            //{
            //    return "Not found users";
            //}


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


        private async Task UpdateCustomerRank_And_AddCustomerRankHistory(int customerId, int rankId, int OldRankId, int NewRankId)
        {
            await Update_Customer_Rank(customerId, rankId);
            await Customer_Rank_History(customerId, OldRankId, NewRankId);

        }



        private async Task Update_Customer_Rank(int customerId, int rankId)
        {
            var customerRank = await momDb.CustomerAttributes.FirstOrDefaultAsync(c => c.Id == customerId);

            if (customerRank != null)
            {
                customerRank.RankId = rankId;
                await momDb.SaveChangesAsync();
            }
        }


        private async Task Customer_Rank_History(int customerId, int OldRankId, int NewRankId)
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


        private async Task<Rank> GetRankDetails(int rankId)
        {
            var rank = await momDb.Rank.FirstOrDefaultAsync(r => r.Id == rankId);
            return rank;
        }









        private async Task<CustomerAttribute> getCustomerRank(int customerId)
        {
            var customerRank = await momDb.CustomerAttributes.Where(c => c.Id == customerId)
                .Include(c => c.Rank).FirstOrDefaultAsync();
            if (customerRank != null)
                return customerRank;
            throw new Exception("Customer not found");
        }









    }
}
