using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Bayer.Pegasus.Business
{

    public class ETLServiceBO : BaseBO
    {
        public enum LogType
        {
            Start = 'I', /*Registro de Início de Processamento da Fase*/
            Finish = 'F', /*Registro de Fim de Processamento da Fase*/
            Info = 'D', /*Log de Detalhamento do Processamento*/
            Error = 'E', /*Log de Erro no Processamento (Impeditivo)*/
            Alert = 'A' /*Log de Alerta no Processamento (Não Impeditivo)*/
        }
        private Dictionary<string, ServiceParameter> _parameters;
        public Dictionary<string, ServiceParameter> GetParameters()
        {
            Dictionary<string, ServiceParameter> retDic = new Dictionary<string, ServiceParameter>();
            using (var serviceDAL = new ETLServiceManagerDAL())
            {
                foreach (ServiceParameter item in serviceDAL.GetParameters())
                    retDic.Add(item.Code, item);
                return retDic;
            }
        }
        public List<ProcessItem> GetPendingProcesses()
        {
            using (var serviceDAL = new ETLServiceManagerDAL())
            {
                return serviceDAL.GetPendingProcesses().OrderBy(x => x.Id).OrderBy(x => x.ExecutionOrder).OrderBy(x => x.ExecutionType).ToList();

            }
        }
        public List<IntegrationProcess> GetIntegrationProcesses()
        {
            using (var serviceDAL = new ETLServiceManagerDAL())
            {
                return serviceDAL.GetIntegrationProcesses();

            }
        }
        public long CreateProcessItem(ProcessItem process, DateTime dateRerence, System.Security.Claims.ClaimsPrincipal user)
        {
            using (var serviceDAL = new ETLServiceManagerDAL())
            {
                return serviceDAL.CreateProcessItem(process, dateRerence, user.Identity.Name);
            }
        }

        public bool ValidateStatusProcess(int cd_Integracao)
        {
            using (var serviceDAL = new ETLServiceManagerDAL())
            {
                return serviceDAL.ValidateStatusProcess(cd_Integracao);
            }
        }

        public bool ValidateDateReference(DateTime dateReference)
        {
            using (var serviceDAL = new ETLServiceManagerDAL())
            {
                return serviceDAL.ValidateDateReference(dateReference);
            }
        }

        public void AddProcessItemLog(ProcessItemLog log)
        {
            using (var serviceDAL = new ETLServiceManagerDAL())
            {
                serviceDAL.AddProcessItemLog(log);
            }
        }

        public bool UpdateProcess(int IdProcessamento, string Fl_Situacao, DateTime? Dt_Fim_Processamento)
        {
            using (var serviceDAL = new ETLServiceManagerDAL())
            {
                return serviceDAL.UpdateProcess(IdProcessamento, Fl_Situacao, Dt_Fim_Processamento);
            }
        }
        public void AddProcessItemLog(long proccessItemId, int stage, char logType, string description)
        {
            using (var serviceDAL = new ETLServiceManagerDAL())
            {
                ProcessItemLog log = new ProcessItemLog();
                log.Created = DateTime.Now;
                log.LogType = logType.ToString();
                log.ProcessItemId = proccessItemId;
                log.StageCode = stage;
                log.Description = description;
                serviceDAL.AddProcessItemLog(log);
            }
        }
    }
}
