using Bayer.Pegasus.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Bayer.Pegasus.Data
{
    public class CFOPRegistrationDAL : BaseDAL
    {

        public List<CFOPRegistration> GetAllCFOPs()
        {
            try
            {
                var cfopsRegistration = new List<CFOPRegistration>();

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
                            var cfopRegistration = new CFOPRegistration();
                            cfopRegistration.CfopCode = (int)(dr["Cd_Cfop"]);
                            cfopRegistration.CfopDescription = dr["Ds_Cfop"].ToString();
                            cfopRegistration.OperationType = (int)dr["Fl_Operacao"];
                            cfopRegistration.FlagPegasus = Convert.ToBoolean(dr["Fl_Pegasus"]);
                            cfopRegistration.FlagEnable = Convert.ToBoolean(dr["Fl_Ativo"]);
                            cfopsRegistration.Add(cfopRegistration);
                        }
                    }
                    cmd.Connection.Close();
                }
                return cfopsRegistration;
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
        public CFOPRegistration GetCFOP(CFOPRegistration cfopRegistration)
        {
            try
            {
                CFOPRegistration cfop = new CFOPRegistration();

                using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    var sql = "SPS_PGS_SEL_CFOP";

                    var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    CreateIntParameter(cmd, "@Cd_Cfop", cfopRegistration.CfopCode);
                    
                    if (!string.IsNullOrEmpty(cfopRegistration.CfopDescription))
                    {
                        CreateStringParameter(cmd, "@Ds_Cfop", cfopRegistration.CfopDescription);
                    }
                    if (cfop.OperationType == -1 ||
                        cfop.OperationType == 0 ||
                        cfop.OperationType == 1)
                    {
                        CreateIntParameter(cmd, "@Fl_Operacao", cfopRegistration.OperationType);
                    }
                    
                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            if ((int)(dr["Cd_Cfop"]) > 0)
                            {
                                cfop.CfopCode = (int)(dr["Cd_Cfop"]);
                                cfop.CfopDescription = dr["Ds_Cfop"].ToString();
                                cfop.OperationType = (int)dr["Fl_Operacao"];
                                cfop.FlagPegasus = Convert.ToBoolean(dr["Fl_Pegasus"]);
                                cfop.FlagEnable = Convert.ToBoolean(dr["Fl_Ativo"]);
                            }
                        }
                    }
                    cmd.Connection.Close();
                }
                return cfop;
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
                CFOPRegistration cfop = new CFOPRegistration();

                using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    var sql = "SPS_PGS_SEL_CFOP";

                    var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    CreateIntParameter(cmd, "@Cd_Cfop", cfopCode);
                    /*
                    if (!string.IsNullOrEmpty(cfopRegistration.CfopDescription))
                    {
                        CreateStringParameter(cmd, "@Ds_Cfop", cfopRegistration.CfopDescription);
                    }
                    if (cfop.OperationType == -1 ||
                        cfop.OperationType == 0 ||
                        cfop.OperationType == 1)
                    {
                        CreateIntParameter(cmd, "@Fl_Operacao", cfopRegistration.OperationType);
                    }
                    */
                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            if ((int)(dr["Cd_Cfop"]) > 0)
                            {
                                cfop.CfopCode = (int)(dr["Cd_Cfop"]);
                                cfop.CfopDescription = dr["Ds_Cfop"].ToString();
                                cfop.OperationType = (int)dr["Fl_Operacao"];
                                cfop.FlagPegasus = Convert.ToBoolean(dr["Fl_Pegasus"]);
                                cfop.FlagEnable = Convert.ToBoolean(dr["Fl_Ativo"]);
                            }
                        }
                    }
                    cmd.Connection.Close();
                }
                return cfop;
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

        public int SaveCFOP(CFOPRegistration cfopRegistration, string user)
        {
            try
            {
                int returnedId = 0;
                //var cfopsRegistration = new CFOPRegistration();
                using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    var sql = cfopRegistration.Acao == 'I' ? "SPS_PGS_INS_CFOP" : "SPS_PGS_UPD_CFOP";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;                   
                    CreateIntParameter(cmd, "@Cd_Cfop", cfopRegistration.CfopCode);
                    CreateStringParameter(cmd, "@Ds_Cfop", cfopRegistration.CfopDescription);
                    CreateIntParameter(cmd, "@Fl_Operacao", cfopRegistration.OperationType);
                    CreateBooleanParameter(cmd, "@Fl_Pegasus", cfopRegistration.FlagPegasus);
                    CreateBooleanParameter(cmd, "@Fl_Ativo", cfopRegistration.FlagEnable);
                    CreateStringParameter(cmd, "@Cd_Login_Usuario", user);

                    cmd.Connection.Open();
                    //cmd.ExecuteNonQuery();
                    returnedId = cmd.ExecuteNonQuery(); //long.Parse(cmd.Parameters["@Cd_Cfop"].Value.ToString());
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
