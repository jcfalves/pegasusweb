using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Business
{
    public class HealthCheckBO : BaseBO
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

        public List<TypeErrorHealthCheck> GetTypeErrorHealthCheck()
        {
            using (var HealthCheckDAL = new HealthCheckDAL())
            {
                return HealthCheckDAL.GetTypeErrorHealthCheck();
            }
        }

        public List<ErrorHealthCheck> GetErrorHealthCheckDashboard(DateTime? DtInicio, DateTime? DtFim, int IdCategoria, List<string> Tipos)
        {
            using (var HealthCheckDAL = new HealthCheckDAL())
            {
                return HealthCheckDAL.GetErrorHealthCheckDashboard(DtInicio, DtFim, IdCategoria, Tipos);
            }
        }

        public Dictionary<TypeErrorHealthCheck, List<ErrorHealthCheck>> GetErrorHealthCheck(DateTime? DtInicio, DateTime? DtFim, int IdCategoria, List<string> Tipos)
        {
            using (var HealthCheckDAL = new HealthCheckDAL())
            {
                return HealthCheckDAL.GetErrorHealthCheck(DtInicio, DtFim, IdCategoria, Tipos);
            }
        }

        public List<TypeErrorHealthCheck> GetTypeErrorCategoryIdHC(int IdCategoria)
        {
            using (var HealthCheckDAL = new HealthCheckDAL())
            {
                return HealthCheckDAL.GetTypeErrorCategoryIdHC(IdCategoria);
            }
        }

        public List<CategoryHeathCheck> GetListCategoryHeathCheck()
        {
            using (var HealthCheckDAL = new HealthCheckDAL())
            {
                return HealthCheckDAL.GetListCategoryHeathCheck();
            }
        }
    }
}
