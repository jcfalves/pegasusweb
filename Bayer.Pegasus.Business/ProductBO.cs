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
    public class ProductBO : BaseBO
    {

        public List<Entities.Brand> GetBrands(string search)
        {
            using (var productDAL = new ProductDAL())
            {
                return productDAL.GetBrands(search);
            }
        }

        public List<Entities.Product> GetProducts(string search, List<string> brands)
        {
            using (var productDAL = new ProductDAL())
            {
                return productDAL.GetProducts(search, brands);
            }
        }

        public FeedbackService ValidateTopKPI(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);


            DataValidation dataValidation = new DataValidation();

            var feedbackService = Validate(data);

            var filter = data["Filter"].Value<bool>();

            if (filter)
            {

                if (salesStructure.CanAccessMultiplePartners)
                    dataValidation.ValidateArray("TopProducts_Partners", true, (JArray)data["Partners"], "Parceiros");

                dataValidation.ValidateInteger("NumberProducts", true, data["NumberProducts"].Value<String>(), "Número de Produtos");
                dataValidation.ValidateInteger("Year", true, data["Year"], "Ano");
            }

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }

        public List<Entities.Kpis.TopKPI> GetTopKPI(System.Security.Claims.ClaimsPrincipal user, int numberProducts, Entities.ReportDateInterval reportDateInterval, List<string> partners, List<string> units, List<string> clients, string typeDataChart, bool groupByBrands = false)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var productDAL = new ProductDAL())
            {
                List<Entities.Kpis.TopKPI> kpis = new List<Entities.Kpis.TopKPI>();

                if (groupByBrands)
                {
                    kpis = productDAL.GetTopBrandsKPI(numberProducts, reportDateInterval, salesStructure, units, clients, typeDataChart);

                }
                else
                {
                    kpis = productDAL.GetTopProductsKPI(numberProducts, reportDateInterval, salesStructure, units, clients, typeDataChart);
                }

                Bayer.Pegasus.Entities.Kpis.TopKPI.CalculatePercentage(kpis, typeDataChart);

                return kpis;
            }
        }

        public List<Entities.Kpis.TopKPI> GetTopKPI(System.Security.Claims.ClaimsPrincipal user, int numberProducts, int year, List<string> partners, List<string> units, List<string> clients, string typeDataChart, bool groupByBrands = false)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            List<Entities.Kpis.TopKPI> kpis = new List<Entities.Kpis.TopKPI>();

            using (var productDAL = new ProductDAL())
            {
                if (groupByBrands)
                    kpis = productDAL.GetTopBrandsKPI(numberProducts, new Entities.ReportDateInterval(year), salesStructure, units, clients, typeDataChart);
                else
                    kpis = productDAL.GetTopProductsKPI(numberProducts, new Entities.ReportDateInterval(year), salesStructure, units, clients, typeDataChart);
            }



            Bayer.Pegasus.Entities.Kpis.TopKPI.CalculatePercentage(kpis, typeDataChart);

            return kpis;
        }


        public FeedbackService ValidateTopStockKPI(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);

            DataValidation dataValidation = new DataValidation();

            var feedbackService = Validate(data);

            var filter = data["Filter"].Value<bool>();

            if (filter)
            {

                if (salesStructure.CanAccessMultiplePartners)
                    dataValidation.ValidateArray("TopStock_Partners", true, (JArray)data["Partners"], "Parceiros");

                dataValidation.ValidateInteger("NumberProducts", true, data["NumberProducts"].Value<String>(), "Número de Produtos");


                dataValidation.ValidateInteger("Year", true, data["Year"], "Ano");
            }

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }
        public List<Entities.Kpis.TopKPI> GetTopStockKPI(System.Security.Claims.ClaimsPrincipal user, int numberProducts, Entities.ReportDateInterval reportDateInterval, List<string> partners, List<string> units, bool groupByBrands = false)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var productDAL = new ProductDAL())
            {
                var kpis = new List<Bayer.Pegasus.Entities.Kpis.TopKPI>();

                if (groupByBrands)
                {
                    kpis = productDAL.GetTopStockBrandsKPI(numberProducts, reportDateInterval, salesStructure, units);
                }
                else
                {
                    kpis = productDAL.GetTopStockProductsKPI(numberProducts, reportDateInterval, salesStructure, units);
                }

                Bayer.Pegasus.Entities.Kpis.TopKPI.CalculatePercentageByQuantity(kpis);

                return kpis;
            }
        }

        public List<Entities.Kpis.TopKPI> GetTopStockKPI(System.Security.Claims.ClaimsPrincipal user, int numberProducts, int year, List<string> partners, List<string> units, bool groupByBrands = false)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var productDAL = new ProductDAL())
            {
                var kpis = new List<Bayer.Pegasus.Entities.Kpis.TopKPI>();


                if (groupByBrands)
                {
                    kpis = productDAL.GetTopStockBrandsKPI(numberProducts, new Entities.ReportDateInterval(year), salesStructure, units);
                }
                else
                {
                    kpis = productDAL.GetTopStockProductsKPI(numberProducts, new Entities.ReportDateInterval(year), salesStructure, units);
                }


                Bayer.Pegasus.Entities.Kpis.TopKPI.CalculatePercentageByQuantity(kpis);

                return kpis;
            }
        }
    }
}
