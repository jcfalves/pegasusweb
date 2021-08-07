using System;
using System.Collections.Generic;
using System.Text;
using Bayer.Pegasus.Data;
using Bayer.Pegasus.Utils;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Bayer.Pegasus.Business
{
    public class HistoryBO : BaseBO
    {
        public FeedbackService ValidateHistory(JObject data)
        {

            DataValidation dataValidation = new DataValidation();

            var feedbackService = Validate(data);

            if (data["description"] != null)
            {
                dataValidation.ValidateString("AnalysisName", true, data["description"].Value<String>().Trim(), "Nome da Análise", 1, 1000);
            }

            feedbackService.Import(dataValidation.FeedBackService);

            return feedbackService;
        }

        public bool ExistsHistory(System.Security.Claims.ClaimsPrincipal user, string report, string name) {
            using (var historyDAL = new HistoryDAL())
            {
                var list =  historyDAL.ListHistory(user.Identity.Name, report);

                return list.Any(p => p.Description == name);
            }
        }

        public List<Entities.History> ListHistory(System.Security.Claims.ClaimsPrincipal user, string report) {
            using (var historyDAL = new HistoryDAL())
            {
                return historyDAL.ListHistory(user.Identity.Name, report);
            }
        }

        public Entities.History GetHistory(System.Security.Claims.ClaimsPrincipal user, long id)
        {
            using (var historyDAL = new HistoryDAL())
            {
                return historyDAL.GetHistory(user.Identity.Name, id);
            }
        }

        public bool DeleteHistory(System.Security.Claims.ClaimsPrincipal user, long id)
        {
            using (var historyDAL = new HistoryDAL())
            {
                return historyDAL.DeleteHistory(user.Identity.Name, id);
            }
        }

        public long SaveHistory(System.Security.Claims.ClaimsPrincipal user, string report, Entities.History history)
        {
            using (var historyDAL = new HistoryDAL())
            {
                return historyDAL.SaveHistory(user.Identity.Name, report, history);
            }
        }
    }
}
