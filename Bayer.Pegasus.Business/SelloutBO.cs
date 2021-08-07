using System;
using System.Collections.Generic;
using System.Text;
using Bayer.Pegasus.Data;
using System.Threading.Tasks;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Utils;
using Newtonsoft.Json.Linq;

namespace Bayer.Pegasus.Business
{
    public class SelloutBO : BaseBO
    {
        public FeedbackService ValidatePegasusReport(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);

            var feedbackService = Validate(data);

            if (salesStructure.CanAccessMultiplePartners)
                dataValidation.ValidateArray("PegasusReport_Partners", true, (JArray)data["Partners"], "Parceiros");

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }


        public List<SelloutItem> GetPegasusReport(System.Security.Claims.ClaimsPrincipal user, List<string> partners, List<string> units, List<string> clients, List<string> brands, List<string> products, List<string> cities, Entities.ReportDateInterval reportInterval)
        {

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var selloutDAL = new SelloutDAL())
            {
                return selloutDAL.GetPegasusReport(salesStructure, units, clients, brands, products, cities, reportInterval);
            }
        }


        public FeedbackService ValidateSellOutEvolutionKPI(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);


            var feedbackService = Validate(data);

            var filter = data["Filter"].Value<bool>();

            if (filter)
            {
                if (salesStructure.CanAccessMultiplePartners)
                    dataValidation.ValidateArray("SelloutEvolution_Partners", true, (JArray)data["Partners"], "Parceiros");

            }

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }

        public List<Entities.Kpis.EvolutionKPI> GetSellOutEvolutionKPI(System.Security.Claims.ClaimsPrincipal user, List<string> partners, List<string> units, List<string> clients, List<string> brands, List<string> products, List<string> cities, Entities.ReportDateInterval reportInterval, string groupBy, string typeDataChart)
        {

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var selloutDAL = new SelloutDAL())
            {
                return selloutDAL.GetSellOutEvolutionKPI(salesStructure, units, clients, brands, products, cities, reportInterval, groupBy, typeDataChart);
            }
        }

        public FeedbackService ValidateTopSelloutKPI(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);


            var feedbackService = Validate(data);

            var filter = data["Filter"].Value<bool>();

            if (filter)
            {
                if (salesStructure.CanAccessMultiplePartners)
                    dataValidation.ValidateArray("Sellout_Partners", true, (JArray)data["Partners"], "Parceiros");

            }

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }



        public List<Entities.Kpis.TopKPI> GetTopSelloutKPI(System.Security.Claims.ClaimsPrincipal user, List<string> partners, List<string> units, Entities.ReportDateInterval reportInterval, string typeDataChart)
        {

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var selloutDAL = new SelloutDAL())
            {
                var kpis = selloutDAL.GetTopSelloutKPI(salesStructure, units, reportInterval, typeDataChart);

                Bayer.Pegasus.Entities.Kpis.TopKPI.CalculatePercentage(kpis, typeDataChart);

                return kpis;
            }
        }
    }
}
