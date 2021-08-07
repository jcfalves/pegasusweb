using Bayer.Pegasus.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Bayer.Pegasus.Data
{
    public class ProductPriceDAL : BaseDAL
    {
        public List<ProductPrice> ValidImportPrice(long codeProcessament)
        {
            List<ProductPrice> results = new List<ProductPrice>();

            using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_PRECOS_TRATAMENTO";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateLongParameter(cmd, "@Id_Processamento", codeProcessament);

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {

                    while (dr.Read())
                    {
                        if (!string.IsNullOrEmpty(dr["Cd_Erro"].ToString()))
                        {
                            ProductPrice productPrice = new ProductPrice();

                            productPrice.id = (int)dr["Nr_Linha"];
                            productPrice.NumberLine = (int)dr["Nr_Linha"];
                            productPrice.CoinTypes = dr["Sg_Moeda"].ToString();
                            productPrice.BoardName = dr["Nm_Diretoria"].ToString();
                            productPrice.CodeCluster = dr["Cd_Cluster"].ToString();
                            productPrice.RegionalName = dr["Nm_Regional"].ToString();
                            productPrice.TradeMark = dr["Nm_Marca"].ToString();
                            productPrice.CodeProduct = dr["Cd_Produto"].ToString();
                            productPrice.NameProduct = dr["Nm_Produto"].ToString();
                            productPrice.ValueProduct = dr["Vl_Produto"].ToString();
                            productPrice.ProcessId = (long)dr["Id_Processamento"];                            
                            productPrice.Created = (DateTime)dr["Dt_Criacao"];
                            productPrice.FL_Processing = dr["Fl_Tratamento"].ToString() == "1" ? true : false;                             
                            productPrice.ErrorCategoryId = (int)dr["Id_Categoria_Erro"];
                            productPrice.DescriptionErrorCategory = dr["Ds_Categoria_Erro"].ToString();
                            productPrice.CodeError = dr["Cd_Erro"].ToString();
                            productPrice.DescriptionError = dr["Ds_Erro"].ToString();

                            results.Add(productPrice);
                        }

                    }
                }

                cmd.Connection.Close();

            }

            return results;
        }
        public List<ProcessItem> ValidDataPrice(long codeProcessament)
        {
            List<ProcessItem> results = new List<ProcessItem>();

            using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_PROCESSAMENTO";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateLongParameter(cmd, "@Id_Processamento", codeProcessament);

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                        if (!string.IsNullOrEmpty(dr["Id_Processamento"].ToString()))
                        {
                            ProcessItem processItem = new ProcessItem();
                            processItem.Id = (long)dr["Id_Processamento"];
                            processItem.IntegrationProcessCode = (int)dr["Cd_Integracao"];
                            processItem.Started = (DateTime)dr["Dt_Inicio_Processamento"];
                            if (!(dr["Dt_Fim_Processamento"] is DBNull))
                            {
                                processItem.Finished = (DateTime)dr["Dt_Fim_Processamento"];
                            }
                            processItem.ExecutionType = dr["Fl_Tipo_Execucao"].ToString();
                            processItem.InputParameter = dr["Ds_Parametro"].ToString();
                            processItem.StatusCode = dr["Fl_Situacao"].ToString();
                            processItem.ReadRecords = !(dr["Qt_Registro_Lido"] is DBNull) ? (long)dr["Qt_Registro_Lido"] : 0;
                            processItem.WriteRecords = !(dr["Qt_Registro_Gravado"] is DBNull) ? (long)dr["Qt_Registro_Gravado"] : 0;
                            processItem.RejectRecords = !(dr["Qt_Registro_Rejeitado"] is DBNull) ? (long)dr["Qt_Registro_Rejeitado"] : 0;
                            processItem.ReferenceDate = (DateTime)dr["Dt_Referencia"];
                            processItem.Created = (DateTime)dr["Dt_Criacao"];
                            processItem.User = dr["Cd_Login_Usuario"].ToString();
                            results.Add(processItem);
                        }

                    }
                }

                cmd.Connection.Close();

            }
            return results;
        }

        public ProcessItem GetLastIntegrationProcesses(int code)
        {
            ProcessItem processItem = new ProcessItem();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_ULTIMO_PROCESSAMENTO_INTEGRACAO";
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateLongParameter(cmd, "@Cd_Integracao", code);
                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            if (!string.IsNullOrEmpty(dr["Id_Processamento"].ToString()))
                            {
                                processItem.Id = (long)dr["Id_Processamento"];
                                processItem.IntegrationProcessCode = (int)dr["Cd_Integracao"];
                                processItem.Started = (DateTime)dr["Dt_Inicio_Processamento"];
                                if (!(dr["Dt_Fim_Processamento"] is DBNull))
                                {
                                    processItem.Finished = (DateTime)dr["Dt_Fim_Processamento"];
                                }
                                processItem.ExecutionType = dr["Fl_Tipo_Execucao"].ToString();
                                processItem.InputParameter = dr["Ds_Parametro"].ToString();
                                processItem.StatusCode = dr["Fl_Situacao"].ToString();
                                processItem.ReadRecords = !(dr["Qt_Registro_Lido"] is DBNull) ? (long)dr["Qt_Registro_Lido"] : 0;
                                processItem.WriteRecords = !(dr["Qt_Registro_Gravado"] is DBNull) ? (long)dr["Qt_Registro_Gravado"] : 0;
                                processItem.RejectRecords = !(dr["Qt_Registro_Rejeitado"] is DBNull) ? (long)dr["Qt_Registro_Rejeitado"] : 0;
                                processItem.ReferenceDate = (DateTime)dr["Dt_Referencia"];
                                processItem.Created = (DateTime)dr["Dt_Criacao"];
                                processItem.User = dr["Cd_Login_Usuario"].ToString();
                            }

                        }
                    }
                }
            }
            return processItem;
        }

        public long AddImportPrice(DataTable table, long codeProcessament)
        {
            try
            {
                long insertedId = 0;

                using (var sqlCopy = new SqlBulkCopy(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString_ODS,
                                                    SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers))
                {
                    sqlCopy.DestinationTableName = table.TableName;

                    foreach (DataColumn column in table.Columns)
                    {
                        sqlCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                    }

                    sqlCopy.WriteToServer(table);

                }

                return insertedId;

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                // Set IndexOutOfRangeException to the new exception's InnerException.
                throw new System.Exception("index parameter is out of range.", ex);

                throw;
            }
            
        }

    }
}
