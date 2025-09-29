using Ovia.DTO;
using Ovia.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Ovia.Services.StorageFiles
{
    public class Bank
    {
        MomEntity Context = new MomEntity();

        //public object gettotalpmony( string lang,int customerId)
        //{
        //    if (lang=="ar")
        //    {
        //        var palance = (from ca in Context.CustomerAccountBalanceSingUps
        //                       where ca.CustomerId == customerId
        //                       orderby ca.Id descending
        //                       select ca.Balance).Take(1);

        //        return palance;
        //    }
        //    else
        //    {
        //        var palance = (from ca in Context.CustomerAccountBalanceSingUps
        //                       where ca.CustomerId == customerId
        //                       orderby ca.Id descending
        //                       select ca.Balance).Take(1);

        //        return palance;
        //    }

        //}

        ////public object getpalance(string lang, int userId)
        ////{
        ////    if (lang == "ar")
        ////    {
        ////        var palance = (from ca in Context.CustomerAccountBalances
        ////                       where ca.CustomerId == userId
        ////                       orderby ca.Id descending
        ////                       select  ca.Balance ).Take(1).FirstOrDefault();
        ////       // decimal c = Convert.ToDecimal(palance);
        ////        return palance;
        ////    }
        ////    else
        ////    {
        ////        var palance = (from ca in Context.CustomerAccountBalances
        ////                       where ca.CustomerId == userId
        ////                       orderby ca.Id descending
        ////                       select ca.Balance).Take(1).FirstOrDefault();

        ////       // decimal c = Convert.ToDecimal(palance);
        ////        return palance;
        ////    }

        ////}

        //public object TotalDiscounts(string lang, int customerId)
        //{
        //    if (lang == "ar")
        //    {
        //        var palance = (from ca in Context.CustomerDiscounts
        //                       where ca.CustomerId == customerId
        //                       orderby ca.Id descending
        //                       select ca.Balance==null? 0:ca.Balance).Take(1);

        //        return palance;
        //    }
        //    else
        //    {
        //        var palance = (from ca in Context.CustomerDiscounts
        //                       where ca.CustomerId == customerId
        //                       orderby ca.Id descending
        //                       select ca.Balance == null ? 0 : ca.Balance).Take(1);

        //        return palance;
        //    }

        //}



        //public IQueryable<DirectcomitionDto> Directcomission(string lang, int customerId)
        //{

        //    if (lang == "ar")
        //    {
        //        var test = (from n in Context.CustomerAttributes where n.CustomerInfoId == customerId select n.Id).ToList();
        //       var dirtco = (from custinfo in Context.CustomerInfo

        //                 join custatrpute in Context.CustomerAttributes
        //                 on custinfo.Id equals custatrpute.CustomerInfoId

        //                 join custnetwork in Context.CustomerNetwork
        //                 on custatrpute.Id equals custnetwork.ChildId

        //                 join pointp in Context.PointProcess
        //                 on custatrpute.Id equals pointp.ForCustomerId

        //                 join proft in Context.Profit
        //                 on pointp.Id equals proft.PointProcessId
        //                 orderby proft.PointProcessId
        //                 where test.Contains((int)custnetwork.SponsorId) 
        //                 && (pointp.ProcessTypeId == 1 || pointp.ProcessTypeId == 5)
        //                 select new DirectcomitionDto
        //                 {
        //                     NameEn = custinfo.NameAr,
        //                     mobile = custinfo.Mobile,
        //                     ProcessTypeId = pointp.ProcessTypeId,
        //                     CreationDate = custatrpute.CreationDate,
        //                     profit = proft.Profit1
        //                 });


        //        //dssfsdvdsdsfds
        //        return dirtco;
        //    }
        //    else
        //    {
        //        var test =(from n in Context.CustomerAttributes where n.CustomerInfoId == customerId select n.Id).ToList();
        //      var  dirtc = (from custinfo in Context.CustomerInfo

        //                 join custatrpute in Context.CustomerAttributes
        //                 on custinfo.Id equals custatrpute.CustomerInfoId

        //                 join custnetwork in Context.CustomerNetwork
        //                 on custatrpute.Id equals custnetwork.ChildId

        //                 join pointp in Context.PointProcess
        //                 on custatrpute.Id equals pointp.ForCustomerId

        //                 join proft in Context.Profit
        //                 on pointp.Id equals proft.PointProcessId
        //                 orderby proft.PointProcessId
        //                 where test.Contains((int)custnetwork.SponsorId) 
        //                 && (pointp.ProcessTypeId == 1 || pointp.ProcessTypeId == 5)
        //                 select new DirectcomitionDto
        //                 {
        //                     NameEn = custinfo.NameEn,
        //                     mobile = custinfo.Mobile,
        //                     ProcessTypeId = pointp.ProcessTypeId,
        //                     CreationDate = custatrpute.CreationDate,
        //                     profit = proft.Profit1
        //                 });




        //        return dirtc;
        //    }

        //}


        //public object Bonus(string lang, int customerId)
        //{
        //        //IQueryable<DirectcomitionDto> dirtc;
        //        if (lang == "ar")
        //        {
        //            var bones = (from profit in Context.Profit

        //                     join prostyp in Context.ProcessType
        //                     on profit.ProcessTypeId equals prostyp.Id
        //                     orderby profit.Id
        //                     join custattrput in Context.CustomerAttributes
        //                     on profit.DistributorId equals custattrput.Id

        //                     join custinfo in Context.CustomerInfo
        //                     on custattrput.CustomerInfoId equals custinfo.Id



        //                     where profit.ProcessTypeId 
        //                     >=1&& profit.ProcessTypeId <= 2 &&custattrput.CustomerInfoId==customerId
        //                     select new 
        //                     {
        //                        custinfo.NameEn,
        //                        profit.Id,
        //                        profit.DistributorId,
        //                        profit.Profit1,
        //                        profit.ProfitDate,
        //                        profit.IsPaid,
        //                        profit.PaymentDate,
        //                        profit.ProcessTypeId,
        //                        prostyp.Process,
        //                        custattrput.ReferId
        //                     });


        //            return bones;
        //        }
        //        else
        //        {


        //                var bones = (from profit in Context.Profit

        //                             join prostyp in Context.ProcessType
        //                             on profit.ProcessTypeId equals prostyp.Id
        //                             orderby profit.Id
        //                             join custattrput in Context.CustomerAttributes
        //                             on profit.DistributorId equals custattrput.Id

        //                             join custinfo in Context.CustomerInfo
        //                             on custattrput.CustomerInfoId equals custinfo.Id


        //                             where profit.ProcessTypeId 
        //                             >= 1 && profit.ProcessTypeId <= 2 && custattrput.CustomerInfoId == customerId
        //                             select new
        //                             {
        //                                 custinfo.NameEn,
        //                                 profit.Id,
        //                                 profit.DistributorId,
        //                                 profit.Profit1,
        //                                 profit.ProfitDate,
        //                                 profit.IsPaid,
        //                                 profit.PaymentDate,
        //                                 profit.ProcessTypeId,
        //                                 prostyp.Process,
        //                                 custattrput.ReferId
        //                             });


        //                return bones;
        //            }
        //    }



        // public object TRANSFERP_MONEY(string lang, string ReferId,int userId)
        //     { 

        //       if (lang == "ar")
        //       {
        //        var TRANSFERP = (from custattrput in Context.CustomerAttributes

        //                         join custinfo in Context.CustomerInfo
        //                         on custattrput.CustomerInfoId equals custinfo.Id


        //                         where Context.CustomerAttributes.Where(x => x.ReferId == ReferId).FirstOrDefault().Id != userId && custattrput.ReferId == ReferId

        //                         select new
        //                         {
        //                             custattrput.Id,
        //                             custinfo.NameAr,
        //                             custattrput.ReferId,
        //                             custattrput.IsActive,
        //                             custattrput.Renewal

        //                         });


        //        return TRANSFERP;
        //       }
        //      else
        //      {
        //        //custattrput.ReferId == ReferId
        //        //Context.CustomerAttributes.Where(x=>x.ReferId==ReferId).FirstOrDefault().Id== userId
        //        //from v in Context.CustomerAttributes where v.ReferId  ==ReferId select v.Id !=custattrput.Id && custattrput.ReferId==ReferId
        //        var TRANSFERP = (from custattrput in Context.CustomerAttributes

        //                         join custinfo in Context.CustomerInfo
        //                         on custattrput.CustomerInfoId equals custinfo.Id


        //                         where Context.CustomerAttributes.Where(x => x.ReferId == ReferId).FirstOrDefault().Id != userId && custattrput.ReferId==ReferId

        //                         select new
        //                         {
        //                             custattrput.Id,
        //                             custinfo.NameEn,
        //                             custattrput.ReferId,
        //                             custattrput.IsActive,
        //                             custattrput.Renewal

        //                         });


        //        return TRANSFERP;
        //    }
        //}






        //public object TRANSFERP_MONEYOpreation(string lang, int userId, decimal amount,string referid)
        //{

        //    if (lang == "ar")
        //    {
        //        var checkrefeid = TRANSFERP_MONEY("en", referid, userId);

        //        var useridtransferto = Context.CustomerAttributes.Where(x => x.ReferId == referid).FirstOrDefault().Id;

        //        if (checkrefeid==null)
        //        {
        //            return false;
        //        }



        //        // var checkbalance = getpalance("ar", userId);
        //        //decimal checkbalance = Context.Customer_Account_Balance_Sing_Up.Where(x => x.Id == userId ).Select(x=>x.Balance).LastOrDefault();
        //        var palance = (from ca in Context.CustomerAccountBalances
        //                       where ca.CustomerId == userId
        //                       orderby ca.Id descending
        //                       select ca.Balance).Take(1).FirstOrDefault();

        //        if (palance < amount)
        //        {
        //            return false;
        //        }

        //        var custnameto = (from n in Context.CustomerInfo
        //                          join v in Context.CustomerAttributes
        //                          on n.Id equals v.CustomerInfoId
        //                          where n.Id == userId
        //                          select n.NameAr).FirstOrDefault();


        //        //Context.CustomerInfo.Where(x => x.Id == userId).FirstOrDefault().NameAr;
        //        var custbalancetrancferfrom = new CustomerAccountBalanceSingUp()
        //        {
        //            CustomerId = userId,
        //            Debit = amount,
        //            Credit = 0,
        //            TransactionDate = DateTime.Now,
        //            Balance = palance - amount,
        //            Description = "to" + custnameto

        //        };

        //        Context.CustomerAccountBalanceSingUps.Add(custbalancetrancferfrom);
        //        Context.SaveChanges();

        //        var custnamefrom = (from n in Context.CustomerInfo
        //                            join v in Context.CustomerAttributes
        //                            on n.Id equals v.CustomerInfoId
        //                            where n.Id == useridtransferto
        //                            select n.NameAr).FirstOrDefault();

        //        var custbalancetrancferto = new CustomerAccountBalanceSingUp()
        //        {
        //            CustomerId = useridtransferto,
        //            Debit = 0,
        //            Credit = amount,
        //            TransactionDate = DateTime.Now,
        //            Balance = palance + amount,
        //            Description = "from" + custnamefrom

        //        };
        //        Context.CustomerAccountBalanceSingUps.Add(custbalancetrancferto);
        //        Context.SaveChanges();
        //        return new { custbalancetrancferfrom, custbalancetrancferto};
        //    }
        //    else
        //    {
        //        var checkrefeid = TRANSFERP_MONEY("en", referid, userId);

        //        var useridtransferto = Context.CustomerAttributes.Where(x => x.ReferId == referid).FirstOrDefault().Id;

        //        if (checkrefeid.ToString()=="")
        //        {
        //            return false;
        //        }



        //        // var checkbalance = getpalance("ar", userId);
        //        //decimal checkbalance = Context.Customer_Account_Balance_Sing_Up.Where(x => x.Id == userId ).Select(x=>x.Balance).LastOrDefault();
        //        var palance = (from ca in Context.CustomerAccountBalances
        //                       where ca.CustomerId == userId 
        //                       orderby ca.Id descending
        //                       select ca.Balance).Take(1).FirstOrDefault();

        //        if (palance<amount)
        //        {
        //            return false;
        //        }

        //        var custnameto = (from n in Context.CustomerInfo
        //                         join v in Context.CustomerAttributes
        //                         on n.Id equals v.CustomerInfoId
        //                         where n.Id == userId
        //                         select n.NameEn).FirstOrDefault();


        //            //Context.CustomerInfo.Where(x => x.Id == userId).FirstOrDefault().NameAr;
        //        var custbalancetrancferfrom = new CustomerAccountBalanceSingUp()
        //        {
        //            CustomerId = userId,
        //            Debit = amount,
        //            Credit = 0,
        //            TransactionDate = DateTime.Now,
        //            Balance = palance - amount,
        //            Description ="to"+ custnameto

        //        };

        //        Context.CustomerAccountBalanceSingUps.Add(custbalancetrancferfrom);
        //        Context.SaveChanges();

        //        var custnamefrom = (from n in Context.CustomerInfo
        //                         join v in Context.CustomerAttributes
        //                         on n.Id  equals v.CustomerInfoId
        //                         where  n.Id ==  useridtransferto
        //                         select n.NameEn).FirstOrDefault();

        //        var custbalancetrancferto = new CustomerAccountBalanceSingUp()
        //        {
        //            CustomerId = useridtransferto,
        //            Debit = 0,
        //            Credit = amount,
        //            TransactionDate = DateTime.Now,
        //            Balance =palance+amount,
        //            Description ="from  "+ custnamefrom

        //        };
        //        Context.CustomerAccountBalanceSingUps.Add(custbalancetrancferto);
        //        Context.SaveChanges();
        //        return new { custbalancetrancferfrom, custbalancetrancferto };
        //    }
        //}


        ////public object cashingpointBack(string lang , RequestcashinDTO model)
        ////{
        ////    int time =DateTime.Now.Hour;

        ////    // double time1 = Convert.ToDateTime(time).Hour;

        ////    if (time >= 12 && time <= 24)
        ////    {
        ////        time = time - 12;
        ////    }
        ////    var totalpalance = (from ca in Context.CustomerAccountBalances
        ////                        where ca.CustomerId == model.UserId
        ////                        orderby ca.Id descending
        ////                        select ca.Balance).Take(1).FirstOrDefault();

        ////    List<string> days = (from d in Context.VisabltyCash
        ////                         where d.IsActive == true 
        ////                         &&( time <=d.timefrom.Hour &&time <=d.timeto.Hour)
        ////                         select d.day).ToList();

        ////    if (!days.Contains(DateTime.Now.DayOfWeek.ToString()))
        ////    {
        ////        return false;
        ////    }


        ////    var checkpaymentway = (from rcards in Context.RequestsCards
        ////                          join rtyp in Context.RequestType
        ////                          on rcards.RequestTypeId equals rtyp.Id

        ////                          join rbank in Context.RequestsBanks
        ////                          on rcards.BankId equals rbank.Id

        ////                          where rcards.CustomerId == model.UserId && rcards.IsUsed == true
        ////                          select new
        ////                          {
        ////                              rtyp.Id,
        ////                              rtyp.Name,
        ////                              rcards.CustomerId,
        ////                              rcards.IsUsed,
        ////                              rbank.BankName
        ////                          }).ToList();
        ////    if (!checkpaymentway.Any())
        ////    {
        ////        return "add payment way";
        ////    }

        ////    if (lang=="en")
        ////    {



        ////        var addcash = new RequestCash()
        ////        {
        ////            CustomerId=model.UserId,
        ////            RequestDate=DateTime.Now,
        ////            RequestedAmount= totalpalance,
        ////            RequestTypeId=model.RequestTypeId,
        ////            IsPaid=false,
        ////            UnPaid=false
        ////        };
        ////        Context.RequestCash.Add(addcash);
        ////        Context.SaveChanges();

        ////        var custbalancetrancferto = new CustomerAccountBalanceSingUp()
        ////        {
        ////            CustomerId = model.UserId,
        ////            Debit = 0,
        ////            Credit = totalpalance,
        ////            TransactionDate = DateTime.Now,
        ////            Balance =totalpalance- totalpalance,
        ////            Description = "from  cashing point Back" 

        ////        };
        ////        Context.CustomerAccountBalanceSingUps.Add(custbalancetrancferto);
        ////        Context.SaveChanges();
        ////        return new { addcash, custbalancetrancferto };
        ////    }
        ////    else
        ////    {
        ////        var addcash = new RequestCash()
        ////        {
        ////            CustomerId = model.UserId,
        ////            RequestDate = DateTime.Now,
        ////            RequestedAmount = Convert.ToDecimal(totalpalance),
        ////            RequestTypeId = model.RequestTypeId,
        ////            IsPaid = false,
        ////            UnPaid = false
        ////        };
        ////        Context.RequestCash.Add(addcash);
        ////        Context.SaveChanges();

        ////        var custbalancetrancferto = new CustomerAccountBalanceSingUp()
        ////        {
        ////            CustomerId = model.UserId,
        ////            Debit = 0,
        ////            Credit = totalpalance,
        ////            TransactionDate = DateTime.Now,
        ////            Balance =totalpalance - totalpalance,
        ////            Description = "from  cashing point Back"

        ////        };
        ////        Context.CustomerAccountBalanceSingUps.Add(custbalancetrancferto);
        ////        Context.SaveChanges();
        ////        return new { addcash, custbalancetrancferto };
        ////    }
        ////}


        //public object Transfer_In(string lang,int customerAttrId)
        //{
        //    if (lang == "ar")
        //    {
        //        var total = (from ca in Context.CustomerAccountBalanceSingUps
        //                     where ca.CustomerId == customerAttrId
        //                            && ca.Debit == 0 && ca.Credit > 0
        //                            orderby ca.Id descending
        //                            select ca).ToList();
        //        return total;
        //    }
        //    else
        //    {
        //        var total = (from ca in Context.CustomerAccountBalanceSingUps
        //                     where ca.CustomerId == customerAttrId
        //                            && ca.Debit == 0 && ca.Credit > 0
        //                            orderby ca.Id descending
        //                            select ca).ToList();
        //        return total;
        //    }
        //}
        //public object Transfer_Out(string lang, int customerAttrId)
        //{
        //    if (lang == "ar")
        //    {
        //        var total = (from ca in Context.CustomerAccountBalanceSingUps
        //                     where ca.CustomerId == customerAttrId
        //                     && ca.Credit == 0 && ca.Debit > 0
        //                     && !(ca.Description.Contains("New Membership Ref ID"))
        //                     orderby ca.Id descending
        //                     select ca).ToList();
        //        return total;
        //    }
        //    else
        //    {
        //        var total = (from ca in Context.CustomerAccountBalanceSingUps
        //                     where ca.CustomerId == customerAttrId
        //                     && ca.Credit == 0 && ca.Debit > 0
        //                     && !(ca.Description.Contains("New Membership Ref ID"))
        //                     orderby ca.Id descending
        //                     select ca).ToList();
        //        return total;
        //    }
        //}
        //public object Sells_Report(string lang, int customerAttrId)
        //{
        //    if (lang == "ar")
        //    {
        //        var total = (from ca in Context.CustomerAccountBalanceSingUps
        //                     where ca.CustomerId == customerAttrId
        //                     && ca.Credit == 0 && ca.Debit > 0
        //                     && ca.Description.Contains("sell")
        //                     orderby ca.Id descending
        //                     select ca).ToList();
        //        return total;
        //    }
        //    else
        //    {
        //        var total = (from ca in Context.CustomerAccountBalanceSingUps
        //                     where ca.CustomerId == customerAttrId
        //                     && ca.Credit == 0 && ca.Debit > 0
        //                     && ca.Description.Contains("sell")
        //                     orderby ca.Id descending
        //                     select ca).ToList();
        //        return total;
        //    }
        //}
        ////public object Get_The_Balance(string lang, int customerAttrId)
        ////{
        ////    if (lang == "ar")
        ////    {
        ////        var total = (from ca in Context.CustomerAccountBalances
        ////                     where ca.CustomerId == customerAttrId
        ////                     && !(ca.Description.Contains("Transfer"))
        ////                     orderby ca.Id descending
        ////                     select ca).ToList();
        ////        return total;
        ////    }
        ////    else
        ////    {
        ////        var total = (from ca in Context.CustomerAccountBalances
        ////                     where ca.CustomerId == customerAttrId
        ////                     && !(ca.Description.Contains("Transfer"))
        ////                     orderby ca.Id descending
        ////                     select ca).ToList();
        ////        return total;
        ////    }
        ////}

        //public object Customer_Account_Plan_B_Bonus(string lang, int customerAttrId)
        //{
        //    if (lang == "ar")
        //    {
        //        var total = (from ca in Context.CustomerAccountMomentumBonus
        //                     where ca.Used == false && ca.CustomerId == customerAttrId

        //                     select ca.Credit).Sum();
        //        return total;
        //    }
        //    else
        //    {
        //        var total = (from ca in Context.CustomerAccountMomentumBonus
        //                     where ca.Used == false && ca.CustomerId==customerAttrId

        //                     select  ca.Credit).Sum();
        //        return total;
        //    }
        //}


        //public object Customer_Account_Plan_B_BonusAll(string lang, int customerAttrId)
        //{
        //    if (lang == "ar")
        //    {
        //        var total = (from ca in Context.CustomerAccountMomentumBonus
        //                     where  ca.CustomerId == customerAttrId

        //                     select ca.Credit).Sum();
        //        return total;
        //    }
        //    else
        //    {
        //        var total = (from ca in Context.CustomerAccountMomentumBonus
        //                     where  ca.CustomerId == customerAttrId

        //                     select ca.Credit).Sum();
        //        return total;
        //    }
        //}

    }
}
