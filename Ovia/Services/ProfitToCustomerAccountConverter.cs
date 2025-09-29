using Hangfire;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.IO.Filters;
using Ovia.Models;
using Ovia.Services.SendEmails;
using System;
using System.Linq;
using System.Threading.Tasks; 

namespace Ovia.Services
{
    public class ProfitToCustomerAccountConverter
    {
        private readonly MomEntity _dbContext;
        private readonly IMailingServices mailingServices;

        public ProfitToCustomerAccountConverter(
            MomEntity dbContext,
            IMailingServices _mailingServices)
        {
            _dbContext = dbContext;
            this.mailingServices = _mailingServices;
        }


        public async Task ConvertProfitToCustomerAccount()
        {
            try
            {
                var distinctCustomerIds =await _dbContext.Profit
                    .Select(p => p.DistributorId)
                    .Distinct()
                    .ToListAsync();

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    foreach (var customerId in distinctCustomerIds)
                    {
                        decimal profitTotal = 0;
                        DateTime todayDate = DateTime.Today;

                        // Find the start and end dates of the current week
                        DateTime reportFromDate = todayDate
                            .AddDays(-(int)todayDate.DayOfWeek + (int)DayOfWeek.Sunday - 6);
                        DateTime reportToDate = reportFromDate.AddDays(7);

                        var profits = await _dbContext.Profit
                            .Where(p => p.DistributorId == customerId &&
                                        p.IsPaid == false
                                        &&
                                        p.ProfitDate >= reportFromDate &&
                                        p.ProfitDate <= reportToDate)
                                            
                                       .ToListAsync();
                        //var profits = await _dbContext.Profit
                        //    .Where(p => p.DistributorId == customerId &&
                        //                p.IsPaid == false &&
                        //                ( p.ProfitDate == new DateTime(2024, 3, 24)))
                        //    .ToListAsync();



                        foreach (var prof in profits)
                        {
                            profitTotal += prof.Profit1 ?? 0;
                            prof.IsPaid = true;
                            prof.PaymentDate = DateTime.Now;
                        }

                        if (profitTotal > 0)
                        {
                            var custom_info = await _dbContext
                                .CustomerAttributes
                                .Where(c => c.Id == customerId)
                                .Include(c => c.CustomerInfo)
                                .FirstOrDefaultAsync();


                            string word = NumberToWordsConverter.ConvertToWords(profitTotal);

/*
                            ////////////////////////send mail with new check////////////////////////
                            await mailingServices.SendEmailAsync(
                           custom_info.CustomerInfo.Email,
                           "Bank Check",
                           $@"
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Check</title>
    <style>
        body {{
            background-color: #f5f5f5;
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
        }}

        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }}

        .title {{
            text-align: center;
            margin-bottom: 20px;
            color: #3366cc;
        }}

        .content {{
            color: #555555;
            padding: 10px;
        }}

        .content p {{
            margin: 10px 0;
        }}

        .content strong {{
            font-weight: bold;
        }}

        .link {{
            color: #3366cc;
            text-decoration: none;
        }}

        .link:hover {{
            text-decoration: underline;
        }}

        .box {{
            border: 2px solid #3366cc;
            padding: 20px;
            border-radius: 10px;
            background-color: #f9f9f9;
        }}

        .box-title {{
            color: #3366cc;
            font-size: 18px;
            margin-bottom: 10px;
        }}

        .box-content {{
            color: #555555;
            margin-bottom: 20px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""title"">
            <h2>BANK CHECK</h2>
        </div>
        <div class=""content"">
            <div class=""box"">
                <div class=""box-title"">Dear <strong>{custom_info.CustomerInfo.NameEn}</strong>,</div>
                <div class=""box-content"">
                    <p style=""color: #008000;"">Congratulations <span style=""font-family: 'Tarbush', sans-serif; color: #008000;"">  </p>
                    <p>You received a new check from Momentum.</p>
                    <p>About the period from {reportFromDate.Date.ToShortDateString()} to {reportToDate.Date.ToShortDateString()}:</p>
                    <p>Your points: <strong>{profitTotal}</strong></p>
                    <p>Your commission: <strong>${profitTotal}</strong></p>
                    <p>Your commission: <strong>{word} dollars</strong></p>
                    <p>You can view your new check from this <a class=""link"" href=""https://www.google.com"">link</a>.</p>
                </div>
            </div>
            <p>Best regards,</p>
            <p>The Momentum Team</p>
        </div>
    </div>
</body>
</html>
",null);   */



                            var existingBalance =await _dbContext.CustomerAccountBalances
                                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                            if (existingBalance != null)
                            {
                                var lastBalance = await _dbContext.CustomerAccountBalances
                                    .Where(c => c.CustomerId == customerId)
                                    .OrderByDescending(c => c.Id)
                                    .FirstOrDefaultAsync();

                                var newBalance = new CustomerAccountBalance
                                {
                                   Id = 0,
                                    Debit = 0,
                                    CustomerId = customerId,
                                    Credit = profitTotal,
                                    Balance = (lastBalance?.Balance ?? 0) + profitTotal,
                                    TransactionDate = DateTime.Now,
                                    Description = "Profit Conversion",
                                    NsTransactionId = "Null"
                                };

                                _dbContext.CustomerAccountBalances.Add(newBalance);
                                await _dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                var newBalance = new CustomerAccountBalance
                                {
                                    Id = 0,
                                    Debit = 0,
                                    CustomerId = customerId,
                                    Credit = profitTotal,
                                    Balance = profitTotal,
                                    TransactionDate = DateTime.Now,
                                    Description = "Profit Conversion",
                                    NsTransactionId = "Null"
                                };
                                _dbContext.CustomerAccountBalances.Add(newBalance);
                                await _dbContext.SaveChangesAsync();

                            }
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting profit to customer account balance: {ex.Message}");
                throw; 
            }
        }


        public async Task UpdateUplineHistoryId()
        {
            var customerAttributes = await _dbContext.CustomerAttributes
                .Where(ca => !_dbContext.CustomerInfo.Any(ci => ci.RoleId == 1 && ci.Id == ca.CustomerInfoId))
                .OrderBy(ca => ca.Id)
                .ToListAsync();

            foreach (var customerAttribute in customerAttributes)
            {
                int distId = customerAttribute.Id;

                if (distId == 2)
                {
                    var parentChild = await _dbContext.CustomerNetwork
                        .FirstOrDefaultAsync(cn => cn.ChildId == distId);

                    if (parentChild != null)
                    {
                        string newDistributorUplineHistoryId = "/" + distId;

                        parentChild.UplineHistoryId = newDistributorUplineHistoryId;
                        await _dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    var parentChild = await _dbContext.CustomerNetwork
                        .FirstOrDefaultAsync(cn => cn.ChildId == distId && cn.ParentId != null);

                    if (parentChild != null)
                    {
                        var validatedDisUplineHistoryId = await _dbContext.CustomerNetwork
                            .Where(cn => cn.ChildId == parentChild.ParentId)
                            .Select(cn => cn.UplineHistoryId)
                            .FirstOrDefaultAsync();

                        if (validatedDisUplineHistoryId != null)
                        {
                            string newDistributorUplineHistoryId = validatedDisUplineHistoryId + "/" + distId;

                            parentChild.UplineHistoryId = newDistributorUplineHistoryId;
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var child = await _dbContext.CustomerNetwork
                            .FirstOrDefaultAsync(cn => cn.ChildId == distId);

                        if (child != null)
                        {
                            child.UplineHistoryId = null;
                            child.HandSide = null;

                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
                await Task.Delay(20);
            }

            // Add Hangfire job to update UplineHistoryId every hour
            // RecurringJob.AddOrUpdate(() => UpdateUplineHistoryId(), Cron.Hourly);

          
        }





        public async Task SendNewCheckMailToCustomr()
        {
            try
            {
                var distinctCustomerIds = await _dbContext.Profit
                    .Select(p => p.DistributorId)
                    .Distinct()
                    .ToListAsync();

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    foreach (var customerId in distinctCustomerIds)
                    {
                        decimal profitTotal = 0;
                        DateTime todayDate = DateTime.Today;

                        // Find the start and end dates of the current week
                        DateTime reportFromDate = todayDate
                            .AddDays(-(int)todayDate.DayOfWeek + (int)DayOfWeek.Sunday - 6);
                        DateTime reportToDate = reportFromDate.AddDays(6);

                        var profits = await _dbContext.Profit
                            .Where(p => p.DistributorId == customerId &&
                                        p.ProfitDate >= reportFromDate &&
                                        p.ProfitDate <= reportToDate)

                                       .ToListAsync();


                        foreach (var prof in profits)
                        {
                            profitTotal += prof.Profit1 ?? 0;
                            prof.IsPaid = true;
                            prof.PaymentDate = DateTime.Now;
                        }

                        if (profitTotal > 0)
                        {

                         




                            var custom_info = await _dbContext
                                .CustomerAttributes
                                .Where(c => c.Id == customerId)
                                .Include(c => c.CustomerInfo)
                                .FirstOrDefaultAsync();



                            if (custom_info == null || custom_info.CustomerInfo == null)
                                continue;

                            // reportToDate = reportToDate.AddDays(-1);
                            string word = NumberToWordsConverter.ConvertToWords(profitTotal);

                            
                                                        ////////////////////////send mail with new check////////////////////////
                                                        await mailingServices.SendEmailAsync(
                                                       custom_info.CustomerInfo.Email,
                                                       "Bank Check",
                                                       $@"
                            <html lang=""en"">
                            <head>
                                <meta charset=""UTF-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                <title>Check</title>
                                <style>
                                    body {{
                                        background-color: #f5f5f5;
                                        margin: 0;
                                        padding: 0;
                                        font-family: Arial, sans-serif;
                                    }}

                                    .container {{
                                        max-width: 600px;
                                        margin: 0 auto;
                                        padding: 20px;
                                        background-color: #ffffff;
                                        border-radius: 10px;
                                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                    }}

                                    .title {{
                                        text-align: center;
                                        margin-bottom: 20px;
                                        color: #3366cc;
                                    }}

                                    .content {{
                                        color: #555555;
                                        padding: 10px;
                                    }}

                                    .content p {{
                                        margin: 10px 0;
                                    }}

                                    .content strong {{
                                        font-weight: bold;
                                    }}

                                    .link {{
                                        color: #3366cc;
                                        text-decoration: none;
                                    }}

                                    .link:hover {{
                                        text-decoration: underline;
                                    }}

                                    .box {{
                                        border: 2px solid #3366cc;
                                        padding: 20px;
                                        border-radius: 10px;
                                        background-color: #f9f9f9;
                                    }}

                                    .box-title {{
                                        color: #3366cc;
                                        font-size: 18px;
                                        margin-bottom: 10px;
                                    }}

                                    .box-content {{
                                        color: #555555;
                                        margin-bottom: 20px;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class=""container"">
                                    <div class=""title"">
                                        <h2>BANK CHECK</h2>
                                    </div>
                                    <div class=""content"">
                                        <div class=""box"">
                                            <div class=""box-title"">Dear <strong>{custom_info.CustomerInfo.NameEn}</strong>,</div>
                                            <div class=""box-content"">
                                                <p style=""color: #008000;"">Congratulations <span style=""font-family: 'Tarbush', sans-serif; color: #008000;"">  </p>
                                                <p>You received a new check from Momentum.</p>
                                                <p>About the period from {reportFromDate.Date.ToShortDateString()} to {reportToDate.Date.ToShortDateString()}:</p>
                                                <p>Your points: <strong>{profitTotal}</strong></p>
                                                <p>Your commission: <strong>${profitTotal}</strong></p>
                                                <p>Your commission: <strong>{word} dollars</strong></p>
                                                <p>You can view your new check from this <a class=""link"" href=""https://momentum-net.com/admin/my-business"">link</a>.</p>
                                            </div>
                                        </div>
                                        <p>Best regards,</p>
                                        <p>The Momentum Team</p>
                                    </div>
                                </div>
                            </body>
                            </html>
                            ",null);   



                            //var existingBalance = await _dbContext.CustomerAccountBalances
                            //    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                            //if (existingBalance != null)
                            //{
                            //    var lastBalance = await _dbContext.CustomerAccountBalances
                            //        .Where(c => c.CustomerId == customerId)
                            //        .OrderByDescending(c => c.Id)
                            //        .FirstOrDefaultAsync();

                            //    var newBalance = new CustomerAccountBalance
                            //    {
                            //        Id = 0,
                            //        Debit = 0,
                            //        CustomerId = customerId,
                            //        Credit = profitTotal,
                            //        Balance = (lastBalance?.Balance ?? 0) + profitTotal,
                            //        TransactionDate = DateTime.Now,
                            //        Description = "Profit Conversion",
                            //        NsTransactionId = "Null"
                            //    };

                            //    _dbContext.CustomerAccountBalances.Add(newBalance);
                            //    await _dbContext.SaveChangesAsync();
                            //}
                            //else
                            //{
                            //    var newBalance = new CustomerAccountBalance
                            //    {
                            //        Id = 0,
                            //        Debit = 0,
                            //        CustomerId = customerId,
                            //        Credit = profitTotal,
                            //        Balance = profitTotal,
                            //        TransactionDate = DateTime.Now,
                            //        Description = "Profit Conversion",
                            //        NsTransactionId = "Null"
                            //    };
                            //    _dbContext.CustomerAccountBalances.Add(newBalance);
                            //    await _dbContext.SaveChangesAsync();

                            //}

                        
                        
                        
                        }
                    }
                    //await _dbContext.SaveChangesAsync();
                    //transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting profit to customer account balance: {ex.Message}");
                throw;
            }
        }











    }











}

