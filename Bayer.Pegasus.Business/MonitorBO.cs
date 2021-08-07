using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Utils;
using Newtonsoft.Json.Linq;

namespace Bayer.Pegasus.Business
{
    public class MonitorBO : BaseBO
    {
        public List<Monitor> GetSteps(Entities.IntegrationProcess integrationProcess, DateTime? PeriodIni, DateTime? PeriodEnd,
                                      string TypeExecute, string Situation)
        {
            using (var monitorDAL = new MonitorDAL())
            {
                return monitorDAL.GetSteps(integrationProcess, PeriodIni, PeriodEnd, TypeExecute, Situation);
            }
        }

        public List<LogResult> GetLogByProcess(int IdProcessamento, int CdFaseProcessamento)        
        {
            using (var monitorDAL = new MonitorDAL())
            {
                return monitorDAL.GetLogByProcess(IdProcessamento, CdFaseProcessamento);
            }
        }
    }
}
