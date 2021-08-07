using Bayer.Pegasus.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Bayer.Pegasus.Data
{
    public class HealthCheckDAL : BaseDAL
    {
        public List<TypeErrorHealthCheck> GetTypeErrorHealthCheck()
        {
            try
            {
                TypeErrorHealthCheck typeErrorHealthCheck;
                List<TypeErrorHealthCheck> results = new List<TypeErrorHealthCheck>();

                using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
                {
                    string sql = "SPS_PGS_SEL_TIPO_ERRO";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {

                        while (dr.Read())
                        {
                            typeErrorHealthCheck = new TypeErrorHealthCheck();
                            typeErrorHealthCheck.Code = dr["Cd_Erro"].ToString();
                            typeErrorHealthCheck.Description = dr["Ds_Erro"].ToString();
                            typeErrorHealthCheck.ErrorCategoryId = int.Parse(dr["Id_Categoria_Erro"].ToString());

                            results.Add(typeErrorHealthCheck);
                        }
                    }
                    cmd.Connection.Close();
                }
                return results;

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

        public Dictionary<TypeErrorHealthCheck, List<ErrorHealthCheck>> GetErrorHealthCheck(DateTime? DtInicio, DateTime? DtFim, int IdCategoria, List<string> Tipos)
        {
            try
            {
                Dictionary<TypeErrorHealthCheck, List<ErrorHealthCheck>> dict = new Dictionary<TypeErrorHealthCheck, List<ErrorHealthCheck>>();

                TypeErrorHealthCheck typeErrorHealthCheck;
                ErrorHealthCheck errorHealthCheck;
                List<TypeErrorHealthCheck> typeErrorsHealthCheck = new List<TypeErrorHealthCheck>();
                List<ErrorHealthCheck> errorsHealthCheck = new List<ErrorHealthCheck>();

                using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
                {
                    string sql = "SPS_PGS_SEL_OCORRENCIAS_HEALTHCHECK";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateIntParameter(cmd, "@Id_Categoria_Erro", IdCategoria);
                    CreateDateTimeParameter(cmd, "@Dt_Fim_Periodo", DtFim);
                    CreateDateTimeParameter(cmd, "@Dt_Inicio_Periodo", DtInicio);
                    CreateArrayListParameter(cmd, "@Tipos", Tipos);


                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {

                        while (dr.Read())
                        {
                            typeErrorHealthCheck = new TypeErrorHealthCheck();
                            typeErrorHealthCheck.CodeTypeRegister = dr["Cd_Tipo_Registro"].ToString();
                            typeErrorHealthCheck.DescriptionTyeRegister = dr["Ds_Tipo_Registro"].ToString();
                            typeErrorHealthCheck.Columns = dr["Ds_Nomes_Colunas"].ToString();

                            typeErrorsHealthCheck.Add(typeErrorHealthCheck);
                        }

                        dr.NextResult();

                        while (dr.Read())
                        {
                            errorHealthCheck = new ErrorHealthCheck();
                            errorHealthCheck.CodeTypeRegister = dr["Cd_Tipo_Registro"].ToString();
                            errorHealthCheck.ErrorCategoryId = dr["Id_Categoria_Erro"].ToString();
                            errorHealthCheck.Code = dr["Cd_Erro"].ToString();
                            errorHealthCheck.Description = dr["Ds_Erro"].ToString();
                            errorHealthCheck.ColumnsValues = dr["Ds_Registro"].ToString();

                            errorsHealthCheck.Add(errorHealthCheck);
                        }
                    }
                    cmd.Connection.Close();
                }

                foreach (TypeErrorHealthCheck tpErrorCheck in typeErrorsHealthCheck)
                {
                    dict[tpErrorCheck] = new List<ErrorHealthCheck>();

                    foreach (ErrorHealthCheck errorCheck in errorsHealthCheck)
                    {
                        if (tpErrorCheck.CodeTypeRegister == errorCheck.CodeTypeRegister)
                        {
                            dict[tpErrorCheck].Add(errorCheck);
                        }
                    }
                }

                return dict;

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

        public List<ErrorHealthCheck> GetErrorHealthCheckDashboard(DateTime? DtInicio, DateTime? DtFim, int IdCategoria, List<string> Tipos)
        {
            ErrorHealthCheck errorHealthCheck;
            List<ErrorHealthCheck> results = new List<ErrorHealthCheck>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
                {
                    string sql = "SPS_PGS_SEL_DASHBOARD_HEALTHCHECK";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateIntParameter(cmd, "@Id_Categoria_Erro", IdCategoria);
                    CreateDateTimeParameter(cmd, "@Dt_Fim_Periodo", DtFim);
                    CreateDateTimeParameter(cmd, "@Dt_Inicio_Periodo", DtInicio);
                    CreateArrayListParameter(cmd, "@Tipos", Tipos);

                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {

                        while (dr.Read())
                        {
                            errorHealthCheck = new ErrorHealthCheck();
                            errorHealthCheck.ErrorCategoryId = dr["Id_Categoria_Erro"].ToString();
                            errorHealthCheck.Code = dr["Cd_Erro"].ToString();
                            errorHealthCheck.Description = dr["Ds_Erro"].ToString();
                            errorHealthCheck.Total_Occurrences = (long)dr["Qt_Ocorrencia"];
                            errorHealthCheck.Percent = Math.Round((double)dr["Pc_Ocorrencia_Total"]);

                            results.Add(errorHealthCheck);
                        }
                    }
                    cmd.Connection.Close();
                }
                return results;
            }
            catch  (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
            
        }

        public List<TypeErrorHealthCheck> GetTypeErrorCategoryIdHC(int IdCategoria)
        {
            try
            {
                TypeErrorHealthCheck typeErrorHealthCheck;
                List<TypeErrorHealthCheck> results = new List<TypeErrorHealthCheck>();

                using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
                {
                    string sql = "SPS_PGS_SEL_TIPO_ERRO";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    CreateIntParameter(cmd, "@Id_Categoria_Erro", IdCategoria);

                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {

                        while (dr.Read())
                        {
                            typeErrorHealthCheck = new TypeErrorHealthCheck();
                            typeErrorHealthCheck.Code = dr["Cd_Erro"].ToString();
                            typeErrorHealthCheck.Description = dr["Ds_Erro"].ToString();
                            typeErrorHealthCheck.ErrorCategoryId = int.Parse(dr["Id_Categoria_Erro"].ToString());
                            typeErrorHealthCheck.Impediment = dr["FL_Impeditivo"].ToString();

                            results.Add(typeErrorHealthCheck);
                        }
                    }
                    cmd.Connection.Close();
                }
                return results;
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

        public List<CategoryHeathCheck> GetListCategoryHeathCheck()
        {
            try
            {
                CategoryHeathCheck categoryHeathCheck;
                List<CategoryHeathCheck> results = new List<CategoryHeathCheck>();

                using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
                {
                    string sql = "SPS_PGS_SEL_CATEGORIA_ERRO";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {

                        while (dr.Read())
                        {
                            categoryHeathCheck = new CategoryHeathCheck();
                            categoryHeathCheck.CategoryId = int.Parse(dr["Id_Categoria_Erro"].ToString());
                            categoryHeathCheck.Description = dr["Ds_Categoria_Erro"].ToString();
                            results.Add(categoryHeathCheck);
                        }
                    }
                    cmd.Connection.Close();
                }
                return results;
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
