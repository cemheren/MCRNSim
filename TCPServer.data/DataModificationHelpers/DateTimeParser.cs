using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCPServer.Data.DataModificationHelpers
{
    public static class DateTimeParser
    {
        public static DateTimeOffset? ParseDate(string time, string date) 
        {
            int hour, min, day, month, year;
            var now = DateTimeOffset.Now;
            if (string.IsNullOrWhiteSpace(time) && string.IsNullOrWhiteSpace(date))
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(time))
            {
                var HM = time.Split(':');
                hour = int.Parse(HM.First()) % 24;
                min = int.Parse(HM.Last()) % 60;
            }
            else
            {
                hour = 0;
                min = 1;
            }

            if (!string.IsNullOrWhiteSpace(date))
            {
                var dmy = date.Split('.');
                if (dmy.Count() == 2)
                {
                    day = int.Parse(dmy[0]);
                    month = int.Parse(dmy[1]);
                    year = now.Year;
                }
                if (dmy.Count() == 3)
                {
                    day = int.Parse(dmy[0]);
                    month = int.Parse(dmy[1]);
                    year = int.Parse(dmy[2]);
                }
                else
                {
                    day = now.Day + 1;
                    month = now.Month;
                    year = now.Year;
                }
            }
            else
            {
                if (hour > now.Hour)
                {
                    day = now.Day;
                }
                else
                {
                    if (min > now.Minute)
                    {
                        day = now.Day;
                    }
                    else
                    {
                        day = now.Day + 1;
                    }
                }
                month = now.Month;
                year = now.Year;
            }

            if (month == 0)
            {
                month++;
            }
            if (day == 0)
            {
                day++;
            }
            if (year < 30)
            {
                year = 2000 + year;
            }
            if (year < 100 && year > 24)
            {
                year = 1900 + year;
            }

            return new DateTimeOffset(year, month, day, hour, min, 0, new TimeSpan()).AddHours(-2);
        }
    }


}
