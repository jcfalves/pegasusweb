using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bayer.Pegasus.Entities;


namespace Bayer.Pegasus.Data
{
    public class StockDAL : BaseDAL
    {
        public List<Entities.Stock> GetStockReport(SalesStructureAccess salesStructure, List<string> units, List<string> brands, List<string> products, ReportDateInterval reportInterval)
        {
            List<Stock> results = new List<Stock>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_SEL_STOCK";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateSalesStructureParameter(cmd, salesStructure);
                CreateArrayListParameter(cmd, "@Unidades", units);
                CreateArrayListParameter(cmd, "@Marcas", brands);
                CreateArrayListParameter(cmd, "@Produtos", products);
                CreateIntervalParameter(cmd, "@Intervalo", reportInterval);


                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {

                    while (dr.Read()) {
                       
                        Entities.Stock stock = new Stock();
                        stock.Partner = new Partner();
                        stock.Partner.Code = dr["CRM_Code"].ToString();
                        stock.Partner.ERPCode = dr["CD_SAP_Matriz"].ToString(); 
                        stock.Partner.Cnpj = dr["CNPJ_Matriz_Parceiro"].ToString();
                        stock.Partner.CompanyName = dr["NM_Matriz_Parceiro"].ToString();
                        stock.Partner.TradeName = dr["NM_Fantasia_Matriz_Parceiro"].ToString();


                        stock.Unit = new Partner();
                        stock.Unit.Code = dr["CRM_Code_Filial"].ToString();
                        stock.Unit.ERPCode = dr["CD_SAP_Filial"].ToString();
                        stock.Unit.Cnpj = dr["CNPJ_Filial_Parceiro"].ToString();
                        stock.Unit.CompanyName = dr["NM_Filial_Parceiro"].ToString();
                        stock.Unit.TradeName = dr["NM_Fantasia_Filial_Parceiro"].ToString();

                        stock.Product = new Product();
                        stock.Product.Code = dr["CD_SAP_PRODUTO"].ToString();
                        stock.Product.Name = dr["DS_Produto"].ToString();

                        stock.StockDate = (DateTime)dr["Data"];

                        stock.Quantities = new Dictionary<string, decimal>();

                        stock.Quantities["H"] = (decimal)dr["Qtd_H"];
                        stock.Quantities["AG"] = (decimal)dr["Qtd_AG"];
                        stock.Quantities["TB"] = (decimal)dr["Qtd_TB"];
                        stock.Quantities["VF"] = (decimal)dr["Qtd_VF"];
                        stock.Quantities["T"] = (decimal)dr["Qtd_T"];

                        stock.UpdateDate = (DateTime)dr["DataCarga"];

                        results.Add(stock);
                    }
                }

                cmd.Connection.Close();

            }

            return results;

        }

        public List<Entities.Kpis.EvolutionKPI> GetStockEvolutionKPI(SalesStructureAccess salesStructure, List<string> units, List<string> brands, List<string> products, Entities.ReportDateInterval reportInterval, string groupBy)
        {
            List<Entities.Kpis.EvolutionKPI> kpis = new List<Entities.Kpis.EvolutionKPI>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_KPI_EVOLUTION_STOCK";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateSalesStructureParameter(cmd, salesStructure);
                CreateArrayListParameter(cmd, "@Unidades", units);
                CreateArrayListParameter(cmd, "@Marcas", brands);
                CreateArrayListParameter(cmd, "@Produtos", products);
                CreateIntervalParameter(cmd, "@Intervalo", reportInterval);
                CreateStringParameter(cmd, "@AgrupadoPor", groupBy);


                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {

                    kpis = GetEvolutionKPIsFromDataReader(dr, groupBy, "Quantity");
                }
                
                cmd.Connection.Close();

            }

            return kpis;
        }

        public StockTransitResult SaveStockTransit(long codeProcessament)
        {
            StockTransitResult stockTransitResult = new StockTransitResult();

            using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_GRAVA_ESTOQUE_TRANSITO";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateLongParameter(cmd, "@Id_Processamento", codeProcessament);

                cmd.Connection.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        stockTransitResult.Fl_Situacao = dr["Fl_Situacao"].ToString();
                        stockTransitResult.Dt_Inicio_Processamento = dr["Dt_Inicio_Processamento"].ToString();
                        stockTransitResult.Dt_Fim_Processamento = dr["Dt_Fim_Processamento"].ToString();
                        stockTransitResult.Qt_Registro_Lido = dr["Qt_Registro_Lido"].ToString();
                        stockTransitResult.Qt_Registro_Rejeitado = dr["Qt_Registro_Rejeitado"].ToString();
                        stockTransitResult.Qt_Registro_Gravado = dr["Qt_Registro_Gravado"].ToString();
                    }

                    dr.Close();
                }

                conn.Close();

                return stockTransitResult;

            }
        }

        public long AddStockTransit(DataTable table, long codeProcessament)
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

        public List<StockTransit> ValidStockTransit(long codeProcessament)
        {
            List<StockTransit> results = new List<StockTransit>();

            using (SqlConnection conn = new SqlConnection(Utils.Configuration.Instance.ConnectionString_ODS))
            {
                string sql = "SPS_PGS_SEL_ESTOQUE_TRANSITO_TRATAMENTO";

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
                            StockTransit stockTransit = new StockTransit();

                            stockTransit.id = (int)dr["Nr_Linha"];
                            stockTransit.NumberLine = (int)dr["Nr_Linha"];
                            stockTransit.NF = dr["Nr_Nota_Fiscal"].ToString();
                            stockTransit.CD = dr["Nm_Centro_Distribuicao"].ToString();
                            stockTransit.CDSAP = dr["Cd_Sap"].ToString();
                            stockTransit.NameRecipient = dr["Nm_Destinatario"].ToString();
                            stockTransit.CNPJRecipient = dr["Cnpj_Cpf_Destinatario"].ToString();
                            stockTransit.CityRecipient = dr["Nm_Cidade_Destinatario"].ToString();
                            stockTransit.UF = dr["Sg_UF_Destinatario"].ToString();
                            stockTransit.ValueNF = dr["Vl_Nota_Fiscal"].ToString();
                            stockTransit.WeightNF = dr["Vl_Peso"].ToString();
                            stockTransit.ShippingCompany = dr["Nm_Transportadora"].ToString();
                            stockTransit.DeliveryDate = dr["Dt_Entrega"].ToString();
                            stockTransit.StatusDelivery = dr["St_Entrega"].ToString();
                            stockTransit.ComplementaryInformation = dr["Ds_Informacao_Complementar"].ToString();
                            stockTransit.Id_Processamento = dr["Id_Processamento"].ToString();
                            stockTransit.Dt_Criacao = dr["Dt_Criacao"].ToString();
                            stockTransit.Fl_Tratamento = dr["Fl_Tratamento"].ToString();
                            stockTransit.Cd_Erro = dr["Cd_Erro"].ToString();
                            stockTransit.Ds_Erro = dr["Ds_Erro"].ToString();

                            results.Add(stockTransit);
                        }
                        
                    }
                }
                
                cmd.Connection.Close();

            }

            return results;
        }

        public List<ProcessItem> ValidDataTransit(long codeProcessament)
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
    }
}
