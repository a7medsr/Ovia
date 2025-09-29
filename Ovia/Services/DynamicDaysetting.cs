using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ovia.Models;
using Ovia.DTO;

namespace Ovia.Services
{
    public class DynamicDaysetting
    {
        MomEntity Context = new MomEntity();


        public daysettingDTO addDaysetting( string lang)
        {
            var checkday = (from d in Context.Dinamicday
                           where d.IsActive == true && d.IsDeleted == false 
                            select d.dayfrom).FirstOrDefault().ToString();

            if (checkday==null)
            {
                return null;
            }

            //DateTime ddd = Convert.ToDateTime( checkday).DayOfWeek;
            DateTime Today_date = DateTime.Today;
            while (Today_date.DayOfWeek.ToString() != checkday)
            {
                Today_date = Today_date.AddDays(-1);
            }
            DateTime Report_1_Date_From= Today_date.AddDays(-7);
            var daysetting = new daysettingDTO()
            {
                Report_0_Date_From = Today_date,
                Report_0_Date_To = Today_date.AddDays(6),
                Report_0_Date_Begain = Today_date.AddDays(7),
                Report_1_Date_From = Today_date.AddDays(-7),
                Report_1_Date_To = Report_1_Date_From.AddDays(6)
           };
             

            if (lang == "ar")
            {
               

                return daysetting;
            }
            else
            {
                return daysetting;
            }
        }


        public object thisweekdirect(int distrputerid,string lang)
        {
            var datesobj = addDaysetting(lang);

            var weekdirect = (from custnet in Context.CustomerNetwork
                              where custnet.SponsorId == distrputerid &&
                              (custnet.CreationDate >= datesobj.Report_0_Date_From && custnet.CreationDate <= datesobj.Report_0_Date_To)
                              select custnet.ChildId).Count();
            if (lang == "ar")
            {
                
                return weekdirect;
            }
            else
            {
                return weekdirect;
            }
        }


        public object lastweekdirect(int distrputerid, string lang)
        {
            var datesobj = addDaysetting(lang);

            var weekdirect = (from custnet in Context.CustomerNetwork
                              where custnet.SponsorId == distrputerid &&
                              (custnet.CreationDate >= datesobj.Report_1_Date_From && custnet.CreationDate <= datesobj.Report_1_Date_To)
                              select custnet.ChildId).Count();
            if (lang == "ar")
            {

                return weekdirect;
            }
            else
            {
                return weekdirect;
            }
        }
    }
}
