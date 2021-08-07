using Bayer.Pegasus.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Bayer.Pegasus.Data
{
    public class RetroativoDAL : BaseDAL
    {

        public List<CFOPRegistration> GetAllCFOPs()
        {
            try
            {
                var retroativoList = new List<CFOPRegistration>();

                using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    var sql = "SPS_PGS_SEL_CFOP";

                    var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            CFOPRegistration retroativo = new CFOPRegistration();
                            retroativo.CfopCode = (int)(dr["Cd_Cfop"]);
                            retroativo.CfopDescription = dr["Ds_Cfop"].ToString();
                            retroativo.OperationType = (int)dr["Fl_Operacao"];
                            retroativo.FlagPegasus = Convert.ToBoolean(dr["Fl_Pegasus"]);
                            retroativo.FlagEnable = Convert.ToBoolean(dr["Fl_Ativo"]);
                            retroativoList.Add(retroativo);
                        }
                    }
                    cmd.Connection.Close();
                }
                return retroativoList;
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

        public List<Retroativo> GetListArquivosRetroativos(string status)
        {
            try
            {
                var retroativoList = new List<Retroativo>();

                using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS))
                {
                    var sql = "SPS_PGS_SEL_ARQUIVOS_RETROATIVOS";

                    var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateStringParameter(cmd, "@status", status);

                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            Retroativo retroativo = new Retroativo();
                            retroativo.idArquivoretroativo = (int)(dr["Id_Arquivo_Retroativo"]);
                            retroativo.dsNome = dr["Ds_Nome"].ToString();
                            retroativo.dsAcao = dr["Ds_Acao"].ToString();
                            retroativo.dtAcao = dr["Dt_Acao"].ToString();
                            retroativo.dsStatus = dr["status"].ToString();

                            retroativoList.Add(retroativo);
                        }
                    }
                    cmd.Connection.Close();
                }
                return retroativoList;
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

        public void SaveArquivoRetroativo(Retroativo retroativo, string user)
        {
            try
            {

                using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS))
                {
                    var sql = "SPS_PGS_UPD_ARQUIVO_RETROATIVO";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    CreateIntParameter(cmd, "@idArquivoretroativo", retroativo.idArquivoretroativo);
                    CreateIntParameter(cmd, "@idAcao", retroativo.idAcao);
                    CreateStringParameter(cmd, "@dsUsuario", user);

                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();

                    cmd.Connection.Close();
                }
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

        public CFOPRegistration GetCFOP(CFOPRegistration _retroativo)
        {
            try
            {
                CFOPRegistration retroativo = new CFOPRegistration();

                using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    var sql = "SPS_PGS_SEL_CFOP";

                    var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    CreateIntParameter(cmd, "@Cd_Cfop", _retroativo.CfopCode);

                    if (!string.IsNullOrEmpty(_retroativo.CfopDescription))
                    {
                        CreateStringParameter(cmd, "@Ds_Cfop", _retroativo.CfopDescription);
                    }
                    if (_retroativo.OperationType == -1 ||
                        _retroativo.OperationType == 0 ||
                        _retroativo.OperationType == 1)
                    {
                        CreateIntParameter(cmd, "@Fl_Operacao", _retroativo.OperationType);
                    }

                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            if ((int)(dr["Cd_Cfop"]) > 0)
                            {
                                retroativo.CfopCode = (int)(dr["Cd_Cfop"]);
                                retroativo.CfopDescription = dr["Ds_Cfop"].ToString();
                                retroativo.OperationType = (int)dr["Fl_Operacao"];
                                retroativo.FlagPegasus = Convert.ToBoolean(dr["Fl_Pegasus"]);
                                retroativo.FlagEnable = Convert.ToBoolean(dr["Fl_Ativo"]);
                            }
                        }
                    }
                    cmd.Connection.Close();
                }
                return retroativo;
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
        public CFOPRegistration GetCFOPByCode(int cfopCode)
        {
            try
            {
                CFOPRegistration retroativo = new CFOPRegistration();

                using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    var sql = "SPS_PGS_SEL_CFOP";

                    var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    CreateIntParameter(cmd, "@Cd_Cfop", cfopCode);
                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            if ((int)(dr["Cd_Cfop"]) > 0)
                            {
                                retroativo.CfopCode = (int)(dr["Cd_Cfop"]);
                                retroativo.CfopDescription = dr["Ds_Cfop"].ToString();
                                retroativo.OperationType = (int)dr["Fl_Operacao"];
                                retroativo.FlagPegasus = Convert.ToBoolean(dr["Fl_Pegasus"]);
                                retroativo.FlagEnable = Convert.ToBoolean(dr["Fl_Ativo"]);
                            }
                        }
                    }
                    cmd.Connection.Close();
                }
                return retroativo;
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

        public int SaveArquivoRetroativo(CFOPRegistration retroativo, string user)
        {
            try
            {
                int returnedId = 0;
                using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    var sql = retroativo.Acao == 'I' ? "SPS_PGS_INS_CFOP" : "SPS_PGS_UPD_CFOP";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    CreateIntParameter(cmd, "@Cd_Cfop", retroativo.CfopCode);
                    CreateStringParameter(cmd, "@Ds_Cfop", retroativo.CfopDescription);
                    CreateIntParameter(cmd, "@Fl_Operacao", retroativo.OperationType);
                    CreateBooleanParameter(cmd, "@Fl_Pegasus", retroativo.FlagPegasus);
                    CreateBooleanParameter(cmd, "@Fl_Ativo", retroativo.FlagEnable);
                    CreateStringParameter(cmd, "@Cd_Login_Usuario", user);

                    cmd.Connection.Open();

                    returnedId = cmd.ExecuteNonQuery(); 
                    cmd.Connection.Close();
                }
                return returnedId;

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
    }
}
