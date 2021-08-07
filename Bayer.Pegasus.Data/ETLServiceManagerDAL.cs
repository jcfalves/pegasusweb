using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Bayer.Pegasus.Data
{
    public class ETLServiceManagerDAL : BaseDAL
    {
        public List<Entities.ServiceParameter> GetParameters()
        {
            List<Entities.ServiceParameter> serviceParameters = new List<Entities.ServiceParameter>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_PARAMETRO";
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {

                        Entities.ServiceParameter parm = new Entities.ServiceParameter();
                        parm.Code = dr["Cd_Parametro"].ToString();
                        parm.Name = dr["Nm_Parametro"].ToString();
                        parm.Format = dr["Fl_Tipo_Parametro"].ToString();
                        parm.Value = dr["Vl_Parametro"].ToString();
                        serviceParameters.Add(parm);
                    }
                }
                cmd.Connection.Close();
            }
            return serviceParameters;
        }
        public List<Entities.ProcessItem> GetPendingProcesses()
        {
            List<Entities.ProcessItem> pendingProcess = new List<Entities.ProcessItem>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_PROCESSAMENTO_PENDENTE";
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {

                            Entities.ProcessItem item = new Entities.ProcessItem();
                            item.Id = dr.GetInt64(dr.GetOrdinal("Id_Processamento"));
                            item.IntegrationProcessCode = dr.GetInt32(dr.GetOrdinal("Cd_Integracao"));
                            if (!dr.IsDBNull(dr.GetOrdinal("Dt_Inicio_Processamento")))
                                item.Started = dr.GetDateTime(dr.GetOrdinal("Dt_Inicio_Processamento"));
                            if (!dr.IsDBNull(dr.GetOrdinal("Dt_Fim_Processamento")))
                                item.Started = dr.GetDateTime(dr.GetOrdinal("Dt_Fim_Processamento"));
                            item.ExecutionType = dr.GetString(dr.GetOrdinal("Fl_Tipo_Execucao"));
                            item.InputParameter = dr.GetString(dr.GetOrdinal("Ds_Parametro"));
                            item.StatusCode = dr.GetString(dr.GetOrdinal("Fl_Situacao"));
                            if (!dr.IsDBNull(dr.GetOrdinal("Qt_Registro_Lido")))
                                item.ReadRecords = dr.GetInt64(dr.GetOrdinal("Qt_Registro_Lido"));
                            if (!dr.IsDBNull(dr.GetOrdinal("Qt_Registro_Gravado")))
                                item.WriteRecords = dr.GetInt64(dr.GetOrdinal("Qt_Registro_Gravado"));
                            if (!dr.IsDBNull(dr.GetOrdinal("Qt_Registro_Rejeitado")))
                                item.RejectRecords = dr.GetInt64(dr.GetOrdinal("Qt_Registro_Rejeitado"));

                            item.ExecutionOrder = dr.GetInt32(dr.GetOrdinal("Nu_Ordem_Execucao"));
                            pendingProcess.Add(item);
                        }
                    }
                }
            }
            return pendingProcess;
        }

        public List<Entities.IntegrationProcess> GetIntegrationProcesses()
        {
            try
            {
                List<Entities.IntegrationProcess> integrationProcesses = new List<Entities.IntegrationProcess>();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS))
                {
                    string sql = "SPS_PGS_SEL_INTEGRACAO";
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Connection.Open();
                        using (var dr = GetDataReader(cmd))
                        {
                            while (dr.Read())
                            {

                                Entities.IntegrationProcess item = new Entities.IntegrationProcess();
                                item.Code = dr.GetInt32(dr.GetOrdinal("Cd_Integracao"));
                                item.Name = dr.GetString(dr.GetOrdinal("Nm_Integracao"));
                                item.SourceCode = dr.GetInt32(dr.GetOrdinal("Id_Origem_Carga"));
                                item.IntervalType = dr.GetString(dr.GetOrdinal("Fl_Frequencia"));
                                if (!dr.IsDBNull(dr.GetOrdinal("Vl_Intervalo_Horario")))
                                    item.IntervalValue = dr.GetInt32(dr.GetOrdinal("Vl_Intervalo_Horario"));
                                if (!dr.IsDBNull(dr.GetOrdinal("Nm_Pacote_SSIS")))
                                    item.SSISPackageName = dr.GetString(dr.GetOrdinal("Nm_Pacote_SSIS"));
                                if (!dr.IsDBNull(dr.GetOrdinal("Ds_Url_Servico")))
                                    item.ServiceUrl = dr.GetString(dr.GetOrdinal("Ds_Url_Servico"));
                                if (!dr.IsDBNull(dr.GetOrdinal("Ds_Parametro_Padrao")))
                                    item.DefaultParameter = dr.GetString(dr.GetOrdinal("Ds_Parametro_Padrao"));
                                item.CanExecuteManually = dr.GetString(dr.GetOrdinal("Fl_Execucao_Manual"));
                                item.Flow = dr.GetString(dr.GetOrdinal("Fl_Fluxo"));
                                item.ExecutionOrder = dr.GetInt32(dr.GetOrdinal("Nu_Ordem_Execucao"));
                                item.FlagEnable = dr.GetString(dr.GetOrdinal("Fl_Ativo"));
                                integrationProcesses.Add(item);
                            }
                        }
                    }
                }
                return integrationProcesses;

            }
            catch (SqlException sqlEx)
            {
                throw sqlEx;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public void AddProcessItemLog(Entities.ProcessItemLog log)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_INS_LOGPROCESSAMENTO";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateLongParameter(cmd, "@Id_Processamento", log.ProcessItemId);
                CreateDateTimeParameter(cmd, "@Dt_Inclusao", log.Created);
                CreateStringParameter(cmd, "@Ds_Log_Processamento", log.Description);
                CreateIntParameter(cmd, "@Cd_Fase_Processamento", log.StageCode);
                CreateStringParameter(cmd, "@Fl_Tipo_Log", log.LogType);

                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

            }
        }

        public bool UpdateProcess(int IdProcessamento, string Fl_Situacao, DateTime? Dt_Fim_Processamento)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_UPD_PROCESSAMENTO";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateLongParameter(cmd, "@Id_Processamento", IdProcessamento);
                CreateStringParameter(cmd, "@Fl_Situacao", Fl_Situacao);                
                CreateDateTimeParameter(cmd, "@Dt_Fim_Processamento", Dt_Fim_Processamento);

                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

            }

            return true;
        }

        public long CreateProcessItem(Entities.ProcessItem process, DateTime dateRerence, string user)
        {
            long returnedId = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_INS_PROCESSAMENTO";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter id = new SqlParameter();
                id.ParameterName = "@Id_Processamento";
                id.DbType = System.Data.DbType.Int64;
                id.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(id);
                CreateStringParameter(cmd, "@Cd_Login_Usuario", user);
                CreateDateTimeParameter(cmd, "@Dt_Inicio_Processamento", DateTime.Now);
                CreateStringParameter(cmd, "@Fl_Tipo_Execucao", process.ExecutionType);
                CreateIntParameter(cmd, "@Cd_Integracao", process.IntegrationProcessCode);
                CreateStringParameter(cmd, "@Ds_Parametro", process.InputParameter);
                CreateStringParameter(cmd, "@Fl_Situacao", process.StatusCode);
                CreateDateTimeParameter(cmd, "@Dt_Referencia", dateRerence);

                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                returnedId = long.Parse(cmd.Parameters["@Id_Processamento"].Value.ToString());
                cmd.Connection.Close();

            }
            return returnedId;
        }

        public bool ValidateStatusProcess(int cd_Integracao)
        {
            long returnedId = 0;
            using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_SITUACAO_INTEGRACAO";

                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection.Open();

                    CreateIntParameter(cmd, "@Cd_Integracao", cd_Integracao);
                    
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            returnedId = dr.GetInt32(dr.GetOrdinal("Fl_Situacao"));
                        }
                    }
                }
            }

            if (returnedId == 0)
                return false;
            else
                return true;
        }

        public bool ValidateDateReference(DateTime dateReference)
        {
            long returnedId = 0;
            using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_ULTIMA_EXEC_INTEGRACAO";

                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection.Open();

                    CreateIntParameter(cmd, "@Cd_Integracao", 4);
                    CreateDateTimeParameter(cmd, "@Dt_Referencia", dateReference);


                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {

                            returnedId = dr.GetInt32(dr.GetOrdinal("Fl_Existe"));
                        }
                    }
                }
            }

            if (returnedId == 0)
                return false;
            else
                return true;
        }
    }
}
