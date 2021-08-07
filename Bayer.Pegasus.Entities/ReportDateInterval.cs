using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Bayer.Pegasus.Entities
{
    public class ReportDateInterval
    {
        public ReportDateInterval() { }

        public ReportDateInterval(DateTime startDate, DateTime endDate) {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        public ReportDateInterval(int year, bool last12Months = false) {
            this.StartDate = new DateTime(year, 1, 1, 0, 0, 0);
            this.EndDate = new DateTime(year, 12, 31, 23, 59, 59);

            if (last12Months) {
               
                EndDate = System.DateTime.Now;
                StartDate = System.DateTime.Now.AddMonths(-12);

               
            }
        }

        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public static ReportDateInterval FromIntervalDataChart(string intervalDataChart) {
            
            DateTime dtEnd = System.DateTime.Now.Date;
            DateTime dtStart = System.DateTime.MinValue;


            switch (intervalDataChart)
            {
                case "Year":
                    dtStart = new DateTime(dtEnd.Year, 1, 1);
                    break;
                case "Last12Months":
                    dtStart = dtEnd.AddMonths(-12);
                    break;
                case "Month":
                    dtStart = dtEnd.AddMonths(-1);
                    break;
                case "Week":
                    dtStart = dtEnd.AddDays(-7);
                    break;
            }

            Entities.ReportDateInterval reportDateInterval = new Entities.ReportDateInterval(dtStart, dtEnd);

            return reportDateInterval;
        }
    }
}
