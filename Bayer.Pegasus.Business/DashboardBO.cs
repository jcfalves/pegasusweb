using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Linq;
using Bayer.Pegasus.Utils;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Business
{
    public class DashboardBO : BaseBO
    {
        public JObject GetDashboard(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            JObject dashboard = new JObject();

            var intervalDataChart = data["IntervalDataChart"].Value<string>();
            var typeDataChart = data["TypeDataChart"].Value<string>();

            var reportDateInterval = Entities.ReportDateInterval.FromIntervalDataChart(intervalDataChart);

            dashboard["ticks"] = Bayer.Pegasus.Utils.DateUtils.GetJArrayTicks(reportDateInterval);


            using (var stockBO = new Bayer.Pegasus.Business.StockBO())
            {
                var stockEvolutionKPI = stockBO.GetStockEvolutionKPI(user, null, null, null, null, reportDateInterval, null);


                dashboard["stockEvolution"] = JsonUtils.GetEvolutionKPIsAsJArray(stockEvolutionKPI);

            }

            using (var productBO = new Bayer.Pegasus.Business.ProductBO())
            {
                var topStockProducts = productBO.GetTopStockKPI(user, 5, reportDateInterval, null, null);

                dashboard["topStockProducts"] = JsonUtils.GetTopKPIsAsJArray(topStockProducts);

            }

            using (var selloutBO = new Bayer.Pegasus.Business.SelloutBO())
            {

                var evolutionKPIs = selloutBO.GetSellOutEvolutionKPI(user, null, null, null, null, null, null, reportDateInterval, null, typeDataChart);
                var topKPIs = selloutBO.GetTopSelloutKPI(user, null, null, reportDateInterval, typeDataChart);

                dashboard["selloutEvolution"] = JsonUtils.GetEvolutionKPIsAsJArray(evolutionKPIs);
                dashboard["sellout"] = JsonUtils.GetTopKPIsAsJArray(topKPIs);

            }


            dashboard["selloutStock"] = GetSelloutStockKPI(user, reportDateInterval, null, null, null, null);



            return dashboard;

        }




        public FeedbackService ValidateDashboardFilter(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);

            var feedbackService = Validate(data);

            if (salesStructure.CanAccessMultiplePartners)
                dataValidation.ValidateArray("Dashboard_Partners", true, (JArray)data["Partners"], "Parceiros");



            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;
        }


        public FeedbackService ValidateSelloutStockKPI(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);


            var feedbackService = Validate(data);

            var filter = data["Filter"].Value<bool>();

            if (filter)
            {
                if (salesStructure.CanAccessMultiplePartners)
                    dataValidation.ValidateArray("SelloutStock_Partners", true, (JArray)data["Partners"], "Parceiros");

                dataValidation.ValidateInteger("Year", true, data["Year"], "Ano");
            }

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;
        }

        public JObject GetSelloutStockKPI(System.Security.Claims.ClaimsPrincipal user, Entities.ReportDateInterval reportInterval, List<string> partners, List<string> units, List<string> brands, List<string> products)
        {
            JArray stock = new JArray();

            JArray sellout = new JArray();

            using (var stockBO = new Bayer.Pegasus.Business.StockBO())
            {

                stock = stockBO.GetStockEvolutionKPI(user, partners, units, brands, products, reportInterval, null).First().ToJArray();

            }

            using (var selloutBO = new Bayer.Pegasus.Business.SelloutBO())
            {

                sellout = (selloutBO.GetSellOutEvolutionKPI(user, partners, units, null, brands, products, null, reportInterval, null, "Quantity")).First().ToJArray();

            }

            JObject sellOutStock = new JObject();

            sellOutStock["stock"] = stock;
            sellOutStock["sellout"] = sellout;



            return sellOutStock;
        }


        public JObject GetSelloutStockKPI(System.Security.Claims.ClaimsPrincipal user, int year, bool last12Months, List<string> partners, List<string> units, List<string> brands, List<string> products)
        {


            Entities.ReportDateInterval reportInterval = new Entities.ReportDateInterval();



            if (last12Months)
            {

                reportInterval.StartDate = System.DateTime.Now.AddMonths(-12);
                reportInterval.EndDate = System.DateTime.Now;
            }
            else
            {
                reportInterval.StartDate = new DateTime(year, 1, 1);
                reportInterval.EndDate = new DateTime(year, 12, 31, 23, 59, 59);
            }

            return GetSelloutStockKPI(user, reportInterval, partners, units, brands, products);

        }
    }
}
