using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bayer.Pegasus.Business
{
    public class ProductPriceBO : BaseBO
    {
        public FeedbackService ValidateHealthCheckReport(System.Security.Claims.ClaimsPrincipal user, JObject data)
        {
            DataValidation dataValidation = new DataValidation();

            var feedbackService = Validate(data);

            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);

            if (salesStructure.CanAccessMultiplePartners)
                dataValidation.ValidateArray("StockReport_Partners", true, (JArray)data["Partners"], "Parceiros");


            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;

        }

        public List<Entities.ProductPrice> ValidImportPrice(long codeProcessament)
        {

            using (var productPriceDAL = new ProductPriceDAL())
            {
                return productPriceDAL.ValidImportPrice(codeProcessament);
            }
        }

        public List<ProcessItem> ValidDataPrice(long codeProcessament)
        {

            using (var productPriceDAL = new ProductPriceDAL())
            {
                return productPriceDAL.ValidDataPrice(codeProcessament);
            }
        }
        public ProcessItem GetLastIntegrationProcesses(int code)
        {
            using (var productPriceDAL = new ProductPriceDAL())
            {
                ProcessItem processItem = new ProcessItem();
                processItem = productPriceDAL.GetLastIntegrationProcesses(code);

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

        public long AddImportPrice(DataTable dt, DateTime dateRerence, System.Security.Claims.ClaimsPrincipal user, long codeReturn)
        {
            ETLServiceBO etlServiceBO = new ETLServiceBO();
            ProcessItemLog processItemLog = new ProcessItemLog();

            processItemLog.Created = DateTime.Now;
            processItemLog.ProcessItemId = codeReturn;
            processItemLog.Description = "Fase iniciada em " + DateTime.Now.ToString("dd/MM/yyyy hh:mm") ;
            processItemLog.LogType = "I";
            processItemLog.StageCode = 1;

            etlServiceBO.AddProcessItemLog(processItemLog);

            using (var productPriceDAL = new ProductPriceDAL())
            {
                productPriceDAL.AddImportPrice(dt, codeReturn);
            }

            return codeReturn;
        }

    }
}
