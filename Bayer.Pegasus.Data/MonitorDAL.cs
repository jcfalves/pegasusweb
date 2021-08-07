using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bayer.Pegasus.Entities;

namespace Bayer.Pegasus.Data
{
    public class MonitorDAL : BaseDAL
    {
        public List<Monitor> GetSteps(Entities.IntegrationProcess integrationProcess, DateTime? PeriodIni, DateTime? PeriodEnd,
                                      string TypeExecute, string Situation)
        {            
            List<Monitor> results = new List<Monitor>();

            using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_MONITORAMENTO";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                if(PeriodIni != null)
                {
                    CreateDateTimeParameter(cmd, "@Dt_Inicio_Periodo", PeriodIni.Value);
                    CreateDateTimeParameter(cmd, "@Dt_Fim_Periodo", PeriodEnd.Value);
                    CreateStringParameter(cmd, "@Fl_Tipo_Execucao", TypeExecute);
                    CreateStringParameter(cmd, "@Fl_Situacao", Situation);
                }
                
                CreateIntParameter(cmd, "@Cd_Integracao", integrationProcess.Code);

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {

                    while (dr.Read())
                    {
                        Monitor monitor = new Monitor();
                        monitor.Id_Processamento = dr["Id_Processamento"].ToString();
                        monitor.Cd_Integracao = dr["Cd_Integracao"].ToString();
                        monitor.Nm_Integracao = dr["Nm_Integracao"].ToString();
                        monitor.Fl_Fluxo = dr["Fl_Fluxo"].ToString();
                        monitor.CAPTACAO = dr["CAPTACAO"].ToString();
                        monitor.VALIDACAO = dr["VALIDACAO"].ToString();
                        monitor.ODS = dr["ODS"].ToString();
                        monitor.DW = dr["DW"].ToString();
                        monitor.DMI = dr["DMI"].ToString();
                        monitor.PREPARACAO = dr["PREPARACAO"].ToString();
                        monitor.ENVIO = dr["ENVIO"].ToString();

                        results.Add(monitor);
                    }
                }

                cmd.Connection.Close();

            }

            return results;

        }

        public List<LogResult> GetLogByProcess(int IdProcessamento, int CdFaseProcessamento)
        {
            List<LogResult> logResult = new List<LogResult>();
                                    
            using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_LOGPROCESSAMENTO";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateIntParameter(cmd, "@IdProcessamento", IdProcessamento);
                CreateIntParameter(cmd, "@CdFaseProcessamento", CdFaseProcessamento);

                cmd.Connection.Open();

                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                        LogResult results = new LogResult();
                        results.Id_Log_Processamento = int.Parse(dr["Id_Log_Processamento"].ToString());
                        results.Id_Processamento = int.Parse( dr["Id_Processamento"].ToString());
                        results.Cd_Fase_Processamento = int.Parse(dr["Cd_Fase_Processamento"].ToString());
                        results.Dt_Inclusao = DateTime.Parse(dr["Dt_Inclusao"].ToString());
                        results.Ds_Log_Processamento = dr["Ds_Log_Processamento"].ToString();
                        results.Fl_Tipo_Log = dr["Fl_Tipo_Log"].ToString();

                        logResult.Add(results);
                    }
                }

                cmd.Connection.Close();

            }

            return logResult;

        }

    }
}
