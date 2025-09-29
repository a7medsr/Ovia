using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;


namespace Ovia.Services
{
    public class BV
    {
        private readonly MomEntity momDb;

        public BV(MomEntity _momDb)
        {
            momDb = _momDb;  
        }


        //int Dist_ID;
        //int Major_Point = 300;
        //decimal Total_Points_L = 300;
        //decimal Total_Points_R = 300;
        //int Total_Points_Right;
        //int Total_Points_Left;
        //int PointProcessid;
        //int ForCustomerId;
        //int value;
        //int Process_Type_ID;
        //DateTime CreationDate;


        public async Task Distribute_BV_Comission()
        {
            try
            {
                //var pointProcesses = await momDb.PointProcess
                //    .Where(pp => pp.ProcessTypeId == 3 &&
                //    !(momDb.Points.Any(p => p.PointProcessId == pp.Id
                //    && p.ProcessTypeId == 3)) && pp.Value > 0)
                //    .OrderBy(pp => pp.Id)
                //    .ToListAsync();


                var pointProcesses = await momDb.PointProcess
        .Where(pp => pp.ProcessTypeId == 3 && pp.Value > 0)
        .ToListAsync();

                var pointsId = await momDb.Points
                    .Where(c => c.ProcessTypeId == 3)
                    .Select(c => c.PointProcessId)
                    .Distinct()
                    .ToListAsync();

                //get point process not found in points
                var filterPointProcess = pointProcesses
                    .Where(p => !pointsId.Contains(p.Id))
                    .ToList();



                foreach (var pointProcess in filterPointProcess)
                {
                    
                    int currentCustomerId = (int)pointProcess.ForCustomerId;
                    int businessValue = pointProcess.Id;
                    int pointCount = (int)pointProcess.Value;
                    DateTime creationDate = (DateTime)pointProcess.CreationDate;
                    while (currentCustomerId != null)
                    {
                        var customerNetwork = await momDb.CustomerNetwork
                            .FirstOrDefaultAsync(cn => cn.ChildId == currentCustomerId);


                        if (customerNetwork == null)
                        {
                            break; // Move to the next element in the foreach loop
                        }



                        if (customerNetwork != null)
                        {
                            //int parentId = (int)customerNetwork.ParentId;
                            int? parentId = customerNetwork.ParentId;
                            if (!parentId.HasValue)
                            {
                                // Move to the next element in the foreach loop
                                break; 
                            }



                            string handSide = customerNetwork.HandSide;

                            var customerAttributes = await momDb.CustomerAttributes
                                .Where(ca => ca.Id == parentId)
                                .Include(ca => ca.CustomerInfo)
                                .FirstOrDefaultAsync();

                            if (customerAttributes != null)
                            {
                                bool renewal = (bool)customerAttributes.Renewal;
                                int customerRoleId = (int)customerAttributes.CustomerInfo.RoleId;

                                if (renewal == true && customerRoleId == 3)
                                {
                                    await AddToPoints(new AddToPointDTO { parentId = (int)parentId, businessValue = businessValue, pointCount = pointCount, handSide = handSide, ProcessTypeId = 3, creationDate = creationDate, IsCalculated = false, IsFlashed = false });

                                }
                                if (renewal == false) // && customerRoleId == 3
                                {
                                    await AddToPointDelay(new AddToPointDTO { parentId = (int)parentId, businessValue = businessValue, pointCount = pointCount, handSide = handSide, ProcessTypeId = 3, creationDate = creationDate, IsCalculated = true, IsFlashed = true });
                                }
                            }

                            currentCustomerId = (int)parentId;
                        }
                        else
                        {
                            // Handle the case where no parent is found for the current customer ID
                            break;
                        }

                        // Introduce a delay to prevent excessive looping
                        Thread.Sleep(20);
                    }





                }



            }


            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in ProcessPointProcesses: " + ex.Message);
                throw;
            }
        }



       

        private async Task AddToPoints(AddToPointDTO dto)
        {
            var newPoint = new Points
            {
                CustomerId = dto.parentId,
                PointProcessId = dto.businessValue,
                PointCount = dto.pointCount,
                Side = dto.handSide,
                ProcessTypeId = dto.ProcessTypeId,   //3
                PointDate = dto.creationDate,
                IsCalculated = dto.IsCalculated,  //false
                IsFlashed = dto.IsFlashed                   //false
            };
            momDb.Points.Add(newPoint);
            await momDb.SaveChangesAsync();
        }

        private async Task AddToPointDelay(AddToPointDTO dto)
        {
            var newPointDelayed = new PointsDelay
            {
                CustomerId = dto.parentId,
                PointProcessId = (int)dto.businessValue,
                PointCount = dto.pointCount,
                Side = dto.handSide,
                ProcessTypeId = dto.ProcessTypeId,   //3
                PointDate = dto.creationDate,
                IsCalculated = dto.IsCalculated,  // true
                IsFlashed = dto.IsFlashed,                 //true
                FlashDate = dto.creationDate
            };
            momDb.PointsDelay.Add(newPointDelayed);
            await momDb.SaveChangesAsync();
        }





        public async Task Distribute_Comission_LeFt_and_Right_Points()
        {
            var points = await momDb.Points
                .Where(c => c.IsFlashed == false && c.IsCalculated == false)
                .ToListAsync(); // Materialize the grouped data into a list

            var poitsGrouped = points.AsEnumerable() // Forces client-side evaluation
                .GroupBy(c => c.CustomerId).ToList();

            foreach (var point_1 in poitsGrouped)
            {
                var leftPointsSum = point_1.Where(s => s.Side == "Left").Sum(s => s.PointCount);
                var rightPointsSum = point_1.Where(s => s.Side == "Right").Sum(s => s.PointCount);
                var customerId = point_1.Key; // Retrieve the CustomerId from the grouping key

                if (leftPointsSum == 0 || rightPointsSum == 0)
                    continue;

                while (leftPointsSum >= 450 && rightPointsSum >= 450)
                {
                    // Get customer rank
                    var Customer_Rank = await getCustomerRank((int)customerId);

                    // Get points that conversion to profit from point in this week
                    DateTime todayDate = DateTime.Today;
                    DateTime reportFromDate = todayDate.AddDays(-(int)todayDate.DayOfWeek + (int)DayOfWeek.Sunday - 6);
                    DateTime reportToDate = reportFromDate.AddDays(7);

                    var Customer_Profit = await momDb.Profit
                        .Where(p => p.DistributorId == customerId &&
                                    p.IsPaid == false && p.ProcessTypeId == 2 &&
                                    p.ProfitDate >= reportFromDate &&
                                    p.ProfitDate <= reportToDate).ToListAsync();

                    var getComissionInThisWeek = Customer_Profit.Sum(p => p.Profit1);

                    decimal HisComission = 450 * Customer_Rank.Rank.CheckPercentage;

                    if (Customer_Rank.Rank.MaxOut - getComissionInThisWeek >= HisComission)
                        HisComission = 450 * Customer_Rank.Rank.CheckPercentage;
                    if (Customer_Rank.Rank.MaxOut - getComissionInThisWeek < HisComission)
                        HisComission = (decimal)(Customer_Rank.Rank.MaxOut - getComissionInThisWeek);
                    if (Customer_Rank.Rank.MaxOut - getComissionInThisWeek == 0)
                        break;

                    if (leftPointsSum - 450 > 0)
                    {
                        await AddToPoints(new AddToPointDTO
                        {
                            parentId = (int)customerId,
                            businessValue = null,
                            pointCount = (int)(leftPointsSum - 450),
                            handSide = "Left",
                            ProcessTypeId = 4,
                            creationDate = DateTime.Now,
                            IsCalculated = false,
                            IsFlashed = false
                        });
                    }
                    if (rightPointsSum - 450 > 0)
                    {
                        await AddToPoints(new AddToPointDTO
                        {
                            parentId = (int)customerId,
                            businessValue = null,
                            pointCount = (int)(rightPointsSum - 450),
                            handSide = "Right",
                            ProcessTypeId = 4,
                            creationDate = DateTime.Now,
                            IsCalculated = false,
                            IsFlashed = false
                        });
                    }

                    var profit = new Profit
                    {
                        Id = 0,
                        DistributorId = customerId,
                        Profit1 = HisComission,  // percentage standard for rank type
                        ProfitDate = DateTime.Now,
                        IsPaid = false,
                        ProcessTypeId = 2
                    };
                    momDb.Profit.Add(profit);
                    await momDb.SaveChangesAsync();

                    /////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\ Matching Bonus   ////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                    var currentchild = customerId;
                    var Matching_Bonus = await momDb.Generation.Where(c => c.Note == "Matching Bonus").ToListAsync();


                    var Process_types = await momDb.ProcessType.Where(c => c.Process.Contains("Match Bonus")).OrderBy(c => c.Id).ToListAsync();
                    int processTypeIndex = 0;

                 
                    foreach(var bonus in Matching_Bonus)
                    {
                        var SponsorId = await momDb.CustomerNetwork.Where(c => c.ChildId == currentchild).Select(c => c.SponsorId).FirstOrDefaultAsync();
                        var Customer_Renwall = await momDb.CustomerAttributes.Where(c => c.Id == SponsorId).Include(c => c.CustomerInfo).FirstOrDefaultAsync();
                        if (SponsorId == null) break;
                        if (Customer_Renwall.Renewal != true || Customer_Renwall.CustomerInfo.RoleId != 3) continue;
                        if (Customer_Renwall.RankId < bonus.RankId)
                        {
                         //   currentchild = SponsorId;
                            continue;
                        }
                        if (Customer_Renwall.RankId >= bonus.RankId)
                        {
                            var profit1 = new Profit
                            {
                                Id = 0,
                                DistributorId = SponsorId,
                                Profit1 = HisComission * bonus.GenerationComission,  // percentage standard for rank type
                                ProfitDate = DateTime.Now,
                                IsPaid = false,
                                ProcessTypeId = Process_types[processTypeIndex].Id
                            };
                            momDb.Profit.Add(profit1);
                            await momDb.SaveChangesAsync();

                        }

                      
                        currentchild = SponsorId;
                        processTypeIndex = (processTypeIndex + 1) % Process_types.Count; //the next index

                    }
                    /////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\ Matching Bonus   ////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                    foreach (var point in point_1)
                    {
                        point.IsCalculated = true;
                        point.IsFlashed = true;
                    }
                    momDb.UpdateRange(point_1);
                    await momDb.SaveChangesAsync();

                    // Recalculate points for the next iteration
                    leftPointsSum = point_1.Where(s => s.Side == "Left").Sum(s => s.PointCount);
                    rightPointsSum = point_1.Where(s => s.Side == "Right").Sum(s => s.PointCount);
                }
            }
        }








        //public async Task Distribute_Comission_LeFt_and_Right_Points()
        //{
        //    var points = await momDb.Points
        //.Where(c => c.IsFlashed == false && c.IsCalculated == false)
        //.ToListAsync(); // Materialize the grouped data into a list


        //    var poitsGrouped = points.AsEnumerable() // Forces client-side evaluation
        //      .GroupBy(c => c.CustomerId).ToList();




        //    foreach (var point_1 in poitsGrouped)
        //    {

        //        var leftPointsSum = point_1.Where(s => s.Side == "Left").Sum(s => s.PointCount);
        //        var rightPointsSum = point_1.Where(s => s.Side == "Right").Sum(s => s.PointCount);

        //        var customerId = point_1.Key; // Retrieve the CustomerId from the grouping key


        //        if (leftPointsSum == 0 || rightPointsSum == 0)
        //            continue;

        //        if (leftPointsSum >= 450 && rightPointsSum >= 450)
        //        // if (leftPointsSum >= 450 && rightPointsSum >= 450)
        //        {
        //            //get customer rank
        //            var Customer_Rank = await getCustomerRank((int)customerId);

        //            //get points that conversion to profit from point in this week
        //           DateTime todayDate = DateTime.Today;
        //           // string dateString = "19-05-2024";
        //            //DateTime todayDate = DateTime.ParseExact(dateString, "dd-MM-yyyy", null);
        //            // Find the start and end dates of the current week
        //            DateTime reportFromDate = todayDate
        //                .AddDays(-(int)todayDate.DayOfWeek + (int)DayOfWeek.Sunday - 6);
        //            DateTime reportToDate = reportFromDate.AddDays(7);

        //            var Customer_Profit = await momDb.Profit
        //                .Where(p => p.DistributorId == customerId &&
        //                            p.IsPaid == false && p.ProcessTypeId == 2
        //                            &&
        //                            p.ProfitDate >= reportFromDate &&
        //                            p.ProfitDate <= reportToDate).ToListAsync();


        //            var getComissionInThisWeek = Customer_Profit.Sum(p => p.Profit1);


        //            decimal HisComission = 450 * Customer_Rank.Rank.CheckPercentage;

        //            if (Customer_Rank.Rank.MaxOut - getComissionInThisWeek >= HisComission)
        //                HisComission = 450 * Customer_Rank.Rank.CheckPercentage;
        //            if (Customer_Rank.Rank.MaxOut - getComissionInThisWeek < HisComission)
        //                HisComission = (decimal)(Customer_Rank.Rank.MaxOut - getComissionInThisWeek);
        //            if (Customer_Rank.Rank.MaxOut - getComissionInThisWeek == 0)
        //                continue;





        //            if (leftPointsSum - 450 > 0)
        //            {

        //                await AddToPoints(new AddToPointDTO { parentId = (int)customerId, businessValue = null, pointCount = (int)(leftPointsSum - 450), handSide = "Left", ProcessTypeId = 4, creationDate = DateTime.Now, IsCalculated = false, IsFlashed = false });

        //            }
        //            if (rightPointsSum - 450 > 0)
        //            {

        //                await AddToPoints(new AddToPointDTO { parentId = (int)customerId, businessValue = null, pointCount = (int)(rightPointsSum - 450), handSide = "Right", ProcessTypeId = 4, creationDate = DateTime.Now, IsCalculated = false, IsFlashed = false });

        //            }
        //            var profit = new Profit
        //            {
        //                Id = 0,
        //                DistributorId = customerId,
        //                Profit1 = HisComission,  //percentage standard for rank type
        //                ProfitDate = DateTime.Now,
        //                IsPaid = false,
        //                ProcessTypeId = 2
        //            };
        //            momDb.Profit.Add(profit);
        //            await momDb.SaveChangesAsync();


        //            // point_1.Where(p => p.IsCalculated == false).ExecuteUpdate(s => s.SetProperty(
        //            //    p => p.IsCalculated, True) &&
        //            //   s.SetProperty(
        //            // p => p.IsFlashed, True));

        //            // Update all elements in leftPoints
        //            foreach (var point in point_1)
        //            {
        //                //new syntax to update in db entity
        //                //var Update_Point = new Points {Id = point.Id, IsCalculated = true, IsFlashed=true };
        //                //momDb.Update(Update_Point);


        //                //await momDb.SaveChangesAsync();
        //                point.IsCalculated = true;
        //                point.IsFlashed = true;
        //            }
        //            momDb.UpdateRange(point_1);   
        //            await momDb.SaveChangesAsync();



        //        }



        //    }




        //}


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
