using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bayer.Pegasus.Entities;

namespace Bayer.Pegasus.Data
{
    public class SelloutDAL : BaseDAL
    {
        public List<SelloutItem> GetPegasusReport(SalesStructureAccess salesStructure, List<string> units, List<string> clients, List<string> brands, List<string> products, List<string> cities, Entities.ReportDateInterval reportInterval)
        {
            List<SelloutItem> results = new List<SelloutItem>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_SEL_MOVIMENTO";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateSalesStructureParameter(cmd, salesStructure);
                CreateArrayListParameter(cmd, "@Unidades", units);
                CreateArrayListParameter(cmd, "@Clientes", clients);
                CreateArrayListParameter(cmd, "@Marcas", brands);
                CreateArrayListParameter(cmd, "@Produtos", products);
                CreateArrayListParameter(cmd, "@Cidades", cities);
                CreateIntervalParameter(cmd, "@Intervalo", reportInterval);


                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                        Entities.SelloutItem selloutItem = new SelloutItem();
                        selloutItem.Partner = new Partner();
                        selloutItem.Partner.Code = dr["CRM_Code"].ToString();
                        selloutItem.Partner.ERPCode = dr["CD_SAP_Matriz"].ToString();
                        selloutItem.Partner.Cnpj = dr["CNPJ_Matriz"].ToString();
                        selloutItem.Partner.CompanyName = dr["NM_Matriz"].ToString();
                        selloutItem.Partner.TradeName = dr["NM_Matriz_Fantasia"].ToString();

                        selloutItem.Unit = new Partner();
                        selloutItem.Unit.Code = dr["CRM_Code_Filial"].ToString();
                        selloutItem.Unit.ERPCode = dr["CD_SAP_Filial"].ToString();
                        selloutItem.Unit.Cnpj = dr["CNPJ_Filial"].ToString();
                        selloutItem.Unit.CompanyName = dr["NM_Filial"].ToString();
                        selloutItem.Unit.TradeName = dr["NM_Filial_Fantasia"].ToString();

                        selloutItem.FiscalCode = dr["Nr_Nota_Fiscal"].ToString();
                        selloutItem.FiscalIssuing = dr["Emissor_Nota"].ToString();
                        selloutItem.FiscalIssuingCnpj = dr["CPF_CNPJ_Operacao"].ToString();
                        selloutItem.FiscalDate = (DateTime)dr["Dt_Emissao_Nota_Fiscal"];

                        selloutItem.CFOP = new CFOP();
                        selloutItem.CFOP.Code = dr["Cd_Cfop"].ToString();
                        selloutItem.CFOP.Description = dr["Ds_Cfop"].ToString();


                        selloutItem.Transaction = (string)dr["Ds_Transacao"];

                        selloutItem.Customer = new Customer();
                        selloutItem.Customer.Code = (string)dr["Cd_Cnpj_Cpf_Consumidor"];
                        selloutItem.Customer.Name = (string)dr["Nm_Cliente"];

                        selloutItem.City = new City();
                        selloutItem.City.CityName = (string)dr["Ds_Municipio"];
                        selloutItem.City.StateAcronym = (string)dr["Ds_UF"];

                        selloutItem.Product = new Product();
                        selloutItem.Product.Code = dr["CD_SAP_PRODUTO"].ToString();
                        selloutItem.Product.Name = dr["DS_Produto"].ToString();

                        selloutItem.Quantity = Math.Abs((decimal)dr["Qtd"]);
                        selloutItem.Values = new Dictionary<string, decimal>();

                        selloutItem.Values["Unit"] = Math.Abs((decimal)dr["Vl_Unit"]);
                        selloutItem.Values["Total"] = Math.Abs((decimal)dr["Vl"]);

                        selloutItem.UpdateDate = (DateTime)dr["DataCarga"];

                        results.Add(selloutItem);
                    }

                }

                cmd.Connection.Close();

            }


            return results;
        }

        public List<Entities.Kpis.EvolutionKPI> GetSellOutEvolutionKPI(SalesStructureAccess salesStructure, List<string> units, List<string> clients, List<string> brands, List<string> products, List<string> cities, Entities.ReportDateInterval reportInterval, string groupBy, string typeDataChart)
        {
            try
            {
                List<Entities.Kpis.EvolutionKPI> kpis = new List<Entities.Kpis.EvolutionKPI>();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    string sql = "SPS_PGS_KPI_EVOLUTION_SELLOUT";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateSalesStructureParameter(cmd, salesStructure);
                    CreateArrayListParameter(cmd, "@Unidades", units);
                    CreateArrayListParameter(cmd, "@Clientes", clients);
                    CreateArrayListParameter(cmd, "@Marcas", brands);
                    CreateArrayListParameter(cmd, "@Produtos", products);
                    CreateArrayListParameter(cmd, "@Cidades", cities);
                    CreateIntervalParameter(cmd, "@Intervalo", reportInterval);
                    CreateStringParameter(cmd, "@AgrupadoPor", groupBy);


                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        kpis = GetEvolutionKPIsFromDataReader(dr, groupBy, typeDataChart);

                    }

                    cmd.Connection.Close();

                }

                return kpis;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<Entities.Kpis.TopKPI> GetTopSelloutKPI(SalesStructureAccess salesStructure, List<string> units, Entities.ReportDateInterval reportInterval, string typeDataChart)
        {
            try
            {
                List<Entities.Kpis.TopKPI> kpis = new List<Entities.Kpis.TopKPI>();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    string sql = "SPS_PGS_KPI_TOP_SELLOUT";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;


                    CreateSalesStructureParameter(cmd, salesStructure);
                    CreateArrayListParameter(cmd, "@Unidades", units);
                    CreateIntervalParameter(cmd, "@Intervalo", reportInterval);
                    CreateStringParameter(cmd, "@KPI", typeDataChart);


                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            Bayer.Pegasus.Entities.Kpis.TopKPI kpi = new Bayer.Pegasus.Entities.Kpis.TopKPI();
                            kpi.Code = dr["CD_SAP_Produto"].ToString();
                            kpi.Description = dr["DS_Produto"].ToString();
                            kpi.Value = (long)(decimal)dr["Valor"];
                            kpi.Quantity = (long)(decimal)dr["Qtd"];
                            kpis.Add(kpi);
                        }
                    }

                    cmd.Connection.Close();

                }
                return kpis;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
