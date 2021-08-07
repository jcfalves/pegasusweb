using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Utils;
using Newtonsoft.Json.Linq;

namespace Bayer.Pegasus.Business
{
    public class StockBO : BaseBO
    {

        public FeedbackService ValidateStockReport(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var feedbackService = Validate(data);

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);

            if (salesStructure.CanAccessMultiplePartners)
                dataValidation.ValidateArray("StockReport_Partners", true, (JArray)data["Partners"], "Parceiros");


            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }

        public List<Entities.Stock> GetStockReport(System.Security.Claims.ClaimsPrincipal user, List<string> partners, List<string> units, List<string> brands, List<string> products, ReportDateInterval reportDate)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var stockDAL = new StockDAL())
            {
                return stockDAL.GetStockReport(salesStructure, units, brands, products, reportDate);
            }
        }

        public FeedbackService ValidateStockEvolutionKPI(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var feedbackService = Validate(data);

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);


            var filter = data["Filter"].Value<bool>();

            if (filter)
            {

                if (salesStructure.CanAccessMultiplePartners)
                    dataValidation.ValidateArray("StockEvolution_Partners", true, (JArray)data["Partners"], "Parceiros");

            }

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }

        public List<Entities.Kpis.EvolutionKPI> GetStockEvolutionKPI(System.Security.Claims.ClaimsPrincipal user, List<string> partners, List<string> units, List<string> brands, List<string> products, Entities.ReportDateInterval reportInterval, string groupBy)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var stockDAL = new StockDAL())
            {
                return stockDAL.GetStockEvolutionKPI(salesStructure, units, brands, products, reportInterval, groupBy);
            }

        }

        public StockTransitResult SaveStockTransit(long codeProcessament)
        {

            using (var stockDAL = new StockDAL())
            {
                return stockDAL.SaveStockTransit(codeProcessament);
            }
        }

        public long AddStockTransit(DataTable dt, DateTime dateRerence, System.Security.Claims.ClaimsPrincipal user, long codeReturn)
        {
            ETLServiceBO etlServiceBO = new ETLServiceBO();
            ProcessItemLog processItemLog = new ProcessItemLog();

            processItemLog.Created = DateTime.Now;
            processItemLog.ProcessItemId = codeReturn;
            processItemLog.Description = "Fase iniciada em " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");
            processItemLog.LogType = "I";
            processItemLog.StageCode = 1;                     

            etlServiceBO.AddProcessItemLog(processItemLog);

            using (var stockDAL = new StockDAL())
            {
                stockDAL.AddStockTransit(dt, codeReturn);
            }

            return codeReturn;
        }

        public List<Entities.StockTransit> ValidStockTransit(long codeProcessament)
        {

            using (var stockDAL = new StockDAL())
            {
                return stockDAL.ValidStockTransit(codeProcessament);
            }
        }

        public List<ProcessItem> ValidDataTransit(long codeProcessament)
        {

            using (var stockDAL = new StockDAL())
            {
                return stockDAL.ValidDataTransit(codeProcessament);
            }
        }

        public ProcessItem GetLastIntegrationProcesses(int code)
        {
            using (var stockDAL = new StockDAL())
            {
                ProcessItem processItem = new ProcessItem();
                processItem = stockDAL.GetLastIntegrationProcesses(code);
                                
                switch (processItem.StatusCode)
                {
                    case "E":
                        processItem.StatusCode = "Finalizado com Erro";
                        break;
                    case "A":
                        processItem.StatusCode = "Finalizado com Alertas";
                        break;
                    case "S":
                        processItem.StatusCode = "Finalizado com Sucesso";
                        break;
                    case "P":
                        processItem.StatusCode = "Pendente";
                        break;
                    case "I":
                        processItem.StatusCode = "Iniciado";
                        break;  
                    default:
                        Console.WriteLine("Default case");
                        break;
                }

                return processItem;
            }

        }
    }
}

