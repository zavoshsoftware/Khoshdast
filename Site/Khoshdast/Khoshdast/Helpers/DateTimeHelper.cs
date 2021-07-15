using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime PostPersianDate(string currentDate)
        {
            var valueResult = currentDate;
            var parts = valueResult.Split('/'); //ex. 1391/1/19
            if (parts.Length != 3) return DateTime.Now;
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);
            return new DateTime(year, month, day, new PersianCalendar());
        }
        public static DateTime PostPersianDateForBinder(string currentDate)
        {
            var valueResult = currentDate;
            var parts = valueResult.Split('/'); //ex. 1391/1/19
            if (parts.Length != 3) return DateTime.Now;
            int year = int.Parse(parts[2]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[0]);
            return new DateTime(year, month, day, new PersianCalendar());
        }
    }
}