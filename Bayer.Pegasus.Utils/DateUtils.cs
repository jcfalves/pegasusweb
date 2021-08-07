using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Bayer.Pegasus.Entities;
using System.Globalization;

namespace Bayer.Pegasus.Utils
{
    public class DateUtils
    {
        public static string GetMonthName(int month)
        {
            string[] months = new string[] { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

            return months[month];


        }

        public static JArray GetJArrayTicks(Entities.ReportDateInterval reportInterval, bool hasIntervalByDay = false)
        {
            var startDate = new DateTime(System.DateTime.Now.Year, 1, 1);
            var endDate = System.DateTime.Now;


            if (reportInterval.StartDate.HasValue)
            {
                startDate = reportInterval.StartDate.Value;
            }


            if (reportInterval.EndDate.HasValue)
            {
                endDate = reportInterval.EndDate.Value;
            }

            var dateSpan = DateTimeSpan.CompareDates(endDate, startDate);

            if (dateSpan.Months == 0 && dateSpan.Years == 0 && hasIntervalByDay)
            {
                return GetJArrayDays(reportInterval);
            }
            else {
                return GetJArrayMonths(reportInterval);
                
            }
           

        }


        public static JArray GetJArrayDays(Entities.ReportDateInterval reportInterval)
        {
            JArray ticks = new JArray();

            var startDate = new DateTime(System.DateTime.Now.Year, 1, 1);
            var endDate = System.DateTime.Now;

            

            if (reportInterval.StartDate.HasValue)
            {
                startDate = reportInterval.StartDate.Value;
            }


            if (reportInterval.EndDate.HasValue)
            {
                endDate = reportInterval.EndDate.Value;
            }

            var dateSpan = DateTimeSpan.CompareDates(endDate, startDate);

            for (var i = 0; i <= dateSpan.Days; i++)
            {
                JArray day = new JArray();

                var order = i + 1;

                var dayValue = startDate.AddDays(i).Day;
               
                day.Add(order);
                day.Add(dayValue);

                ticks.Add(day);
            }

            return ticks;

        }


        public static JArray GetJArrayMonths(Entities.ReportDateInterval reportInterval)
        {

            JArray ticks = new JArray();

            var startDate = new DateTime(System.DateTime.Now.Year, 1, 1);
            var yearValue = (System.DateTime.Now.Year -1)-2000;
            if (reportInterval.StartDate.HasValue) {
                startDate = reportInterval.StartDate.Value;
                yearValue = reportInterval.StartDate.Value.Year - 2000;                
            }

            for (var i = 0; i < 12; i++)
            {
                JArray month = new JArray();

                var monthValue = startDate.AddMonths(i).Month;                
                
                var order = i + 1;

                month.Add(order);
                month.Add(GetShortMonthName(monthValue)+"/"+ yearValue.ToString());

                ticks.Add(month);
                if (monthValue == 12)
                    yearValue += 1;
            }

            return ticks;

        }

        public static string GetShortMonthName(int month)
        {
            string[] months = new string[] { "", "JAN", "FEV", "MAR", "ABR", "MAI", "JUN", "JUL", "AGO", "SET", "OUT", "NOV", "DEZ" };

            return months[month];


        }
        

        public static ReportDateInterval GetIntervalDate(JObject interval)
        {
            if (interval == null)
                return null;

            ReportDateInterval dateFilter = new ReportDateInterval();

            switch (interval["TypeDate"].Value<String>()) {
                //Ano
                case "y":
                    dateFilter.StartDate = new DateTime(interval["YearDate"].Value<int>(), 1, 1);
                    dateFilter.EndDate = new DateTime(interval["YearDate"].Value<int>(), 12, 31);
                    break;
                //Safra
                case "c":
                    dateFilter.StartDate = new DateTime(interval["CropDate"].Value<int>() - 1, 4, 1);
                    dateFilter.EndDate = dateFilter.StartDate.Value.AddYears(1).AddDays(-1);
                    break;
                //Trimestre
                case "q":
                    dateFilter.StartDate = new DateTime(DateTime.Now.Year, (interval["QuarterDate"].Value<int>() - 1) * 3 + 1, 1);
                    dateFilter.EndDate = dateFilter.StartDate.Value.AddMonths(3).AddDays(-1);
                    break;
                //Bimestre
                case "b":
                    dateFilter.StartDate = new DateTime(DateTime.Now.Year, (interval["BiMonthDate"].Value<int>() - 1) * 2 + 1, 1);
                    dateFilter.EndDate = dateFilter.StartDate.Value.AddMonths(2).AddDays(-1);
                    break;
                //Mês
                case "m":
                    String[] splitDate = interval["MonthDate"].ToString().Split('/');
                    dateFilter.StartDate = new DateTime(int.Parse(splitDate[1]), int.Parse(splitDate[0]), 1);
                    dateFilter.EndDate = dateFilter.StartDate.Value.AddMonths(1).AddDays(-1);
                    break;
                //Semana
                case "w":
                    CultureInfo cul = CultureInfo.CurrentCulture;
                    int weekYear = cul.Calendar.GetWeekOfYear(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                    weekYear = weekYear + (interval["WeekDate"].Value<int>() - 1);
                    dateFilter.StartDate = FirstDateOfWeekISO8601(DateTime.Now.Year, weekYear);
                    dateFilter.EndDate = dateFilter.StartDate.Value.AddDays(6);
                    break;
                //Últimos
                case "l":
                    dateFilter.EndDate = DateTime.Now;
                    switch (interval["LastType"].Value<String>())
                    {
                        case "y":
                            dateFilter.StartDate = dateFilter.EndDate.Value.AddYears(interval["LastDate"].Value<int>() * -1);
                            break;
                        case "m":
                            dateFilter.StartDate = dateFilter.EndDate.Value.AddMonths(interval["LastDate"].Value<int>() * -1);
                            break;
                        case "d":
                            dateFilter.StartDate = dateFilter.EndDate.Value.AddDays(interval["LastDate"].Value<int>() * -1);
                            break;
                    }
                    break;
                //Personalizado
                case "custom":

                    var startDate = interval["StartDate"].Value<String>();
                    var endDate = interval["EndDate"].Value<String>();

                    if (!String.IsNullOrEmpty(startDate)) {
                        dateFilter.StartDate = DateTime.ParseExact(startDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                    }

                    if (!String.IsNullOrEmpty(endDate)) {
                        dateFilter.EndDate = DateTime.ParseExact(endDate, "d/M/yyyy", CultureInfo.InvariantCulture);

                    }
                    break;
            }

            /*Confirma que será contabilizado desde o primeiro segundo do dia*/
            if (dateFilter.StartDate.HasValue)
            {
                dateFilter.StartDate = dateFilter.StartDate.Value.AddHours(-dateFilter.StartDate.Value.Hour);
                dateFilter.StartDate = dateFilter.StartDate.Value.AddMinutes(-dateFilter.StartDate.Value.Minute);
                dateFilter.StartDate = dateFilter.StartDate.Value.AddSeconds(-dateFilter.StartDate.Value.Second);
            }

            /*Confirma que será contabilizado até o último segundo do dia*/
            if(dateFilter.EndDate.HasValue)
                dateFilter.EndDate = dateFilter.EndDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            return dateFilter;
        }


        public static int GetNumberOfBimonths(DateTime dt) {
            var bimonth = System.DateTime.Now.Month / 2;

            return bimonth + System.DateTime.Now.Month % 2;
        }

        public static int GetNumberOfQuarters(DateTime dt)
        {
            var quarter = System.DateTime.Now.Month / 3;

            if (quarter == 0)
            {
                quarter = 1;
            }

            return quarter;
        }

        public static Dictionary<int, int> GetMonthsAccera()
        {
            int countDays = 31;
            DateTime dateRef = DateTime.Now.AddDays(-1);
            Dictionary<int, int> months = new Dictionary<int, int>();
            months.Add(dateRef.Day, dateRef.Month);

            countDays -= dateRef.Day;

            while(countDays > 0)
            {
                dateRef = dateRef.AddDays(dateRef.Day * -1);
                int daysOnMonth = (countDays < dateRef.Day) ? countDays : dateRef.Day;
                months.Add(daysOnMonth, dateRef.Month);
                countDays -= dateRef.Day;
            }

            return months;
        }

        public static  int GetNumberOfWeeks(DateTime dt) {
            DateTime today = dt;
            //extract the month
            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
            //days of week starts by default as Sunday = 0
            int firstDayOfMonth = (int)firstOfMonth.DayOfWeek;
            int weeksInMonth = ((int)(firstDayOfMonth + daysInMonth) / 7);

            return weeksInMonth;
        }

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-4);
        }

        public static JArray GetJArrayMonthsYear(Entities.ReportDateInterval reportInterval)
        {

            JArray ticks = new JArray();

            var startDate = new DateTime(System.DateTime.Now.Year, 1, 1);

            if (reportInterval.StartDate.HasValue)
            {
                startDate = reportInterval.StartDate.Value;
            }

            for (var i = 0; i < 12; i++)
            {
                JArray month = new JArray();

                var monthValue = startDate.AddMonths(i).Month;
                var yearsValue = startDate.ToString("yyyy");

                var order = i + 1;

                month.Add(order);
                month.Add(GetShortMonthName(monthValue) + "/" + yearsValue);

                ticks.Add(month);
            }

            return ticks;

        }
    }
}
