using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bayer.Pegasus.Entities;

namespace Bayer.Pegasus.Data
{
    public class ProductDAL : BaseDAL
    {

        public List<Entities.Product> GetProducts(string search, List<string> brands)
        {
            List<Entities.Product> products = new List<Entities.Product>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_SEL_PRODUCT";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Search", search);

                this.CreateArrayListParameter(cmd, "@marcas", brands);


                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {

                        Bayer.Pegasus.Entities.Product product = new Bayer.Pegasus.Entities.Product();

                        product.Code = dr["COD_SAP_Produto"].ToString();
                        product.Name = dr["DS_Produto"].ToString();

                        products.Add(product);
                    }

                }

                cmd.Connection.Close();

            }




            return products;
        }

        public List<Entities.Brand> GetBrands(string search)
        {
            List<Entities.Brand> brands = new List<Entities.Brand>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_SEL_BRAND";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Search", search);



                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {

                        Bayer.Pegasus.Entities.Brand brand = new Bayer.Pegasus.Entities.Brand();

                        brand.Name = dr["Marca"].ToString();


                        brands.Add(brand);
                    }

                }

                cmd.Connection.Close();

            }




            return brands;
        }


        public List<Entities.Kpis.TopKPI> GetTopBrandsKPI(int numberProducts, Entities.ReportDateInterval reportDateInterval, SalesStructureAccess salesStructure, List<string> units, List<string> clients, string typeDataChart)
        {
            try
            {
                List<Entities.Kpis.TopKPI> kpis = new List<Entities.Kpis.TopKPI>();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    string sql = "SPS_PGS_KPI_TOP_BRANDS";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateIntParameter(cmd, "@NumeroMarcas", numberProducts);
                    CreateIntervalParameter(cmd, "@Intervalo", reportDateInterval);
                    CreateSalesStructureParameter(cmd, salesStructure);
                    CreateArrayListParameter(cmd, "@Unidades", units);

                    CreateArrayListParameter(cmd, "@Clientes", clients);
                    CreateStringParameter(cmd, "@KPI", typeDataChart);

                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            Bayer.Pegasus.Entities.Kpis.TopKPI kpi = new Bayer.Pegasus.Entities.Kpis.TopKPI();
                            kpi.Code = dr["Marca"].ToString();
                            kpi.Description = dr["Marca"].ToString();
                            kpi.Value = (long)(decimal)dr["Valor"];
                            kpi.Quantity = (long)(decimal)dr["Qtd"];
                            kpi.PercentageQuantity = Convert.ToDecimal(dr["Percentual_Qtd"]) * 100;
                            kpi.PercentageValue = Convert.ToDecimal(dr["Percentual_Valor"]) * 100;
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


        public List<Entities.Kpis.TopKPI> GetTopStockBrandsKPI(int numberBrands, Entities.ReportDateInterval reportDateInterval, SalesStructureAccess salesStructure, List<string> units)
        {
            try
            {
                List<Entities.Kpis.TopKPI> kpis = new List<Entities.Kpis.TopKPI>();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    string sql = "SPS_PGS_KPI_TOP_STOCK_BRANDS";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateIntParameter(cmd, "@NumeroMarcas", numberBrands);
                    CreateIntervalParameter(cmd, "@Intervalo", reportDateInterval);
                    CreateSalesStructureParameter(cmd, salesStructure);
                    CreateArrayListParameter(cmd, "@Unidades", units);

                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            Bayer.Pegasus.Entities.Kpis.TopKPI kpi = new Bayer.Pegasus.Entities.Kpis.TopKPI();
                            kpi.Code = dr["Marca"].ToString();
                            kpi.Description = dr["Marca"].ToString();
                            kpi.Quantity = (long)dr["Qtd"];
                            kpi.PercentageQuantity = Convert.ToDecimal(dr["Percentual_Qtd"]) * 100;
                            kpi.PercentageValue = Convert.ToDecimal(dr["Percentual_Valor"]) * 100;

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

        public List<Entities.Kpis.TopKPI> GetTopProductsKPI(int numberProducts, Entities.ReportDateInterval reportDateInterval, SalesStructureAccess salesStructure, List<string> units, List<string> clients, string typeDataChart)
        {
            try
            {
                List<Entities.Kpis.TopKPI> kpis = new List<Entities.Kpis.TopKPI>();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    string sql = "SPS_PGS_KPI_TOP_PRODUCTS";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateIntParameter(cmd, "@NumeroProdutos", numberProducts);
                    CreateIntervalParameter(cmd, "@Intervalo", reportDateInterval);
                    CreateSalesStructureParameter(cmd, salesStructure);
                    CreateArrayListParameter(cmd, "@Unidades", units);

                    CreateArrayListParameter(cmd, "@Clientes", clients);
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
                            kpi.PercentageQuantity = Convert.ToDecimal(dr["Percentual_Qtd"]) * 100;
                            kpi.PercentageValue = Convert.ToDecimal(dr["Percentual_Valor"]) * 100;
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


        public List<Entities.Kpis.TopKPI> GetTopStockProductsKPI(int numberProducts, Entities.ReportDateInterval reportDateInterval, SalesStructureAccess salesStructure, List<string> units)
        {
            try
            {
                List<Entities.Kpis.TopKPI> kpis = new List<Entities.Kpis.TopKPI>();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    string sql = "SPS_PGS_KPI_TOP_STOCK_PRODUCTS";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateIntParameter(cmd, "@NumeroProdutos", numberProducts);
                    CreateIntervalParameter(cmd, "@Intervalo", reportDateInterval);
                    CreateSalesStructureParameter(cmd, salesStructure);
                    CreateArrayListParameter(cmd, "@Unidades", units);

                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            Bayer.Pegasus.Entities.Kpis.TopKPI kpi = new Bayer.Pegasus.Entities.Kpis.TopKPI();
                            kpi.Code = dr["COD_SAP_Produto"].ToString();
                            kpi.Description = dr["DS_Produto"].ToString();
                            kpi.Quantity = (long)dr["Qtd"];
                            kpi.PercentageQuantity = Convert.ToDecimal(dr["Percentual_Qtd"]) * 100;
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
