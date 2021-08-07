using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bayer.Pegasus.Data;
using System.IO;
using Bayer.Pegasus.Utils;
using Newtonsoft.Json.Linq;

namespace Bayer.Pegasus.Business
{
    public class BaseBO : IDisposable
    {

        public FeedbackService Validate(JObject data)
        {

            DataValidation dataValidation = new DataValidation();

            var feedbackService = Validate(data, dataValidation);

            
            return feedbackService;
        }

        public FeedbackService Validate(JObject data, DataValidation dataValidation)
        {

            FeedbackService feedbackService = new FeedbackService();
            
            if (data == null)
                return feedbackService;


            if (data["ReportDateViewModel"] != null)
            {
                feedbackService.Import(ValidateReportDateInterval((JObject)data["ReportDateViewModel"]));
            }

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;
        }


        protected FeedbackService ValidateReportDateInterval(JObject interval)
        {

            var dataValidation = new DataValidation();

            switch (interval["TypeDate"].Value<String>())
            {
                case "y":
                    dataValidation.ValidateInteger("YearDate", true, interval["YearDate"].Value<String>(), "Ano");
                    break;
                case "q":
                    dataValidation.ValidateInteger("QuarterDate", true, interval["QuarterDate"].Value<String>(), "Trimestre");
                    break;
                case "b":
                    dataValidation.ValidateInteger("BiMonthDate", true, interval["BiMonthDate"].Value<String>(), "Bimestre");
                    break;
                case "m":
                    String[] splitDate = interval["MonthDate"].ToString().Split('/');
                    dataValidation.ValidateInteger("MonthDate", true, splitDate[0], "Mês");
                    dataValidation.ValidateInteger("MonthDate", true, splitDate[1], "Mês");
                    break;
                case "w":
                    dataValidation.ValidateInteger("WeekDate", true, interval["WeekDate"].Value<String>(), "Semana");
                    break;
                case "custom":
                    if (String.IsNullOrEmpty(interval["StartDate"].Value<String>()) && String.IsNullOrEmpty(interval["EndDate"].Value<String>()))
                    {
                        dataValidation.FeedBackService.AddCustomError("Intervalo deve ser informado.");
                        dataValidation.FeedBackService.Fields.Add("StartDate");
                        dataValidation.FeedBackService.Fields.Add("EndDate");
                    }
                    else
                    {
                        dataValidation.ValidateDate("StartDate", true, interval["StartDate"].Value<String>(), "Data de Início");
                        dataValidation.ValidateDate("EndDate", true, interval["EndDate"].Value<String>(), "Data de Término");
                    }
                    break;
                case "l":
                    if (interval["LastDate"] != null && !String.IsNullOrEmpty(interval["LastDate"].Value<String>()))
                    {
                        dataValidation.ValidateInteger("LastDate", true, interval["LastDate"].Value<String>(), "Últimos");
                    }
                    else
                    {
                        dataValidation.FeedBackService.AddCustomError("O campo Últimos deve ser preenchido.");
                        dataValidation.FeedBackService.Fields.Add("LastDate");
                    }
                    break;
            }



            return dataValidation.FeedBackService;
        }

        public void Dispose()
        {
           GC.SuppressFinalize(this);
        }
    }
}
