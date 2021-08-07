using System;
using System.Collections.Generic;
using System.Text;
using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Utils;
using Newtonsoft.Json.Linq;

namespace Bayer.Pegasus.Business
{
    public class ClientBO : BaseBO
    {

        public FeedbackService ValidateStatusClientsKpis(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);


            var feedbackService = Validate(data);

            if (salesStructure.CanAccessMultiplePartners)
                dataValidation.ValidateArray("ClientsReport_Partners", true, (JArray)data["Partners"], "Parceiros");
            
            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }


        public FeedbackService ValidateTopClientsKPI(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);


            var feedbackService = Validate(data);

            if (salesStructure.CanAccessMultiplePartners)
                dataValidation.ValidateArray("TopClients_Partners", true, (JArray)data["Partners"], "Parceiros");

            dataValidation.ValidateInteger("NumberClients", true, data["NumberClients"].Value<string>(), "Parceiros");
            dataValidation.ValidateInteger("Year", true, data["Year"], "Ano");

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }

        


        public List<Entities.Kpis.ClientLocationKPI> GetStatusClientsKpisByLocation(System.Security.Claims.ClaimsPrincipal user, List<string> partners) {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var clientDAL = new ClientDAL())
            {
                return clientDAL.GetStatusClientsKpisByLocation(salesStructure);
            }
        }


        public List<Entities.Customer> GetCustomers(System.Security.Claims.ClaimsPrincipal user, List<string> partners, string search)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var clientDAL = new ClientDAL())
            {
                return clientDAL.GetCustomers(salesStructure, search);
            }
        }

        public List<Entities.Customer> GetCustomersStatus(System.Security.Claims.ClaimsPrincipal user, List<string> partners)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var clientDAL = new ClientDAL())
            {
                return clientDAL.GetCustomersStatus(salesStructure);
            }
        }

        public List<Entities.Kpis.TopKPI> GetTopClientsKPI(System.Security.Claims.ClaimsPrincipal user, int numberClients, int year, List<string> partners, List<string> units, List<string> brands, List<string> products, string typeDataChart)
        {

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var clientDAL = new ClientDAL())
            {
                var kpis = clientDAL.GetTopClientsKPI(numberClients, year, salesStructure, units, brands, products, typeDataChart);

                Bayer.Pegasus.Entities.Kpis.TopKPI.CalculatePercentage(kpis, typeDataChart);

                return kpis;
            }
        }
    }
}
