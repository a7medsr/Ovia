using Ovia.DTO;
using Ovia.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.Services
{
    public  class DynamicDay
    {
        MomEntity Context = new MomEntity();

        public object addDynamicDay(DynamicdayDTO model,string lang)
        {
            if (lang=="ar")
            {
                var day = new dinamicday()
                {
                    dayfrom=model.dayfrom,
                    dayto=model.dayto,
                    IsActive=model.IsActive,
                    IsDeleted=model.IsDeleted,
                    Notes=model.Notes
                };

                Context.Dinamicday.Add(day);
                Context.SaveChanges();
                return day;
            }
            else
            {
                var day = new dinamicday()
                {
                    dayfrom = model.dayfrom,
                    dayto = model.dayto,
                    IsActive = model.IsActive,
                    IsDeleted = model.IsDeleted,
                    Notes = model.Notes
                };

                Context.Dinamicday.Add(day);
                Context.SaveChanges();
                return day;
            }
        }
    }
}
