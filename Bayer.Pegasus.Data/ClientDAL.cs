using System;
using System.Collections.Generic;
using System.Text;
using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;

namespace Bayer.Pegasus.Data
{
    public class ClientDAL : BaseDAL
    {

        public List<Entities.Customer> GetCustomers(SalesStructureAccess salesStructure, string search)
        {
            List<Entities.Customer> customers = new List<Entities.Customer>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_SEL_CLIENT";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                CreateSalesStructureParameter(cmd, salesStructure);

                cmd.Parameters.AddWithValue("@NumClientes", 30);
                cmd.Parameters.AddWithValue("@Search", search);

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {

                        Bayer.Pegasus.Entities.Customer customer = new Bayer.Pegasus.Entities.Customer();
                        customer.DocumentNumber = dr["CPF_CNPJ_Operacao"].ToString();
                        customer.Name = dr["Nm_Cliente"].ToString();
                        customer.TradeName = dr["Nm_Fantasia"].ToString();
                        customer.ZipCode = dr["DS_CEP"].ToString();
                        customer.Address = dr["Ds_Endereco"].ToString();
                        customer.Code = dr["CPF_CNPJ_Operacao"].ToString();

                        customer.City = new City();

                        customer.City.CityName = dr["Ds_Municipio"].ToString();

                        customer.City.StateAcronym = dr["Ds_UF"].ToString();

                        customers.Add(customer);
                    }

                }

                cmd.Connection.Close();

            }




            return customers;
        }


        public List<Entities.Customer> GetCustomersStatus(SalesStructureAccess salesStructure)
        {
            List<Entities.Customer> customers = new List<Entities.Customer>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_SEL_CLIENT_LOCATION";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                CreateSalesStructureParameter(cmd, salesStructure);
                
                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                       
                        Bayer.Pegasus.Entities.Customer customer = new Bayer.Pegasus.Entities.Customer();
                        customer.DocumentNumber = dr["CPF_CNPJ_Operacao"].ToString();
                        customer.Name = dr["Nm_Cliente"].ToString();
                        customer.TradeName = dr["Nm_Fantasia"].ToString();
                        customer.ZipCode = dr["DS_CEP"].ToString();
                        customer.Address = dr["Ds_Endereco"].ToString();
                        customer.Acquired = (bool)dr["Adquirido"];
                        customer.Lost = (bool)dr["Perdido"];
                        customer.Reacquired = (bool)dr["Readquirido"];
                        customer.Retained = (bool)dr["Retido"];
                        customer.Loyal = (bool)dr["Leal"];

                        customer.City = new City();

                        customer.City.CityName = dr["Ds_Municipio"].ToString();

                        customer.City.StateAcronym = dr["Ds_UF"].ToString();

                        customers.Add(customer);
                    }

                }

                cmd.Connection.Close();

            }




            return customers;
        }

        public List<Entities.Kpis.ClientLocationKPI> GetStatusClientsKpisByLocation(SalesStructureAccess salesStructure)
        {
            List<Entities.Kpis.ClientLocationKPI> kpis = new List<Entities.Kpis.ClientLocationKPI>();
            
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_KPI_STATUS_CLIENT_LOCATION";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                CreateSalesStructureParameter(cmd, salesStructure);


                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                        Bayer.Pegasus.Entities.Kpis.ClientLocationKPI kpi = new Bayer.Pegasus.Entities.Kpis.ClientLocationKPI();
                        kpi.IBGECityCode = dr["Cd_IBGE_Municipio"].ToString();
                        kpi.Name = dr["Nm_Cidade"].ToString();
                        kpi.UF = dr["DS_UF"].ToString();
                        kpi.Latitude = decimal.Parse(dr["Lat"].ToString().Replace(".", ","));
                        kpi.Longitude = decimal.Parse(dr["Lng"].ToString().Replace(".", ","));
                        kpi.Acquired = (int)dr["Qt_Adquirido"];
                        kpi.Lost = (int)dr["Qt_Perdido"];
                        kpi.Reacquired = (int)dr["Qt_Readquirido"];
                        kpi.Retained = (int)dr["Qt_Retido"];
                        kpi.Loyal = (int)dr["Qt_Leal"];

                        kpis.Add(kpi);
                    }

                }

                cmd.Connection.Close();

            }

          


            return kpis;
        }
        
        public List<Entities.Kpis.TopKPI> GetTopClientsKPI(int numberClients, int year, SalesStructureAccess salesStructure, List<string> units, List<string> brands, List<string> products, string typeDataChart)
        {
            try
            {
                List<Entities.Kpis.TopKPI> kpis = new List<Entities.Kpis.TopKPI>();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
                {
                    string sql = "SPS_PGS_KPI_TOP_CLIENTS";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    CreateIntParameter(cmd, "@NumeroClientes", numberClients);
                    CreateIntParameter(cmd, "@Ano", year);
                    CreateSalesStructureParameter(cmd, salesStructure);
                    CreateArrayListParameter(cmd, "@Unidades", units);
                    CreateArrayListParameter(cmd, "@Marcas", brands);
                    CreateArrayListParameter(cmd, "@Produtos", products);
                    CreateStringParameter(cmd, "@KPI", typeDataChart);

                    cmd.Connection.Open();
                    using (var dr = GetDataReader(cmd))
                    {
                        while (dr.Read())
                        {
                            Bayer.Pegasus.Entities.Kpis.TopKPI kpi = new Bayer.Pegasus.Entities.Kpis.TopKPI();
                            kpi.Code = dr["CPF_CNPJ_Operacao"].ToString();
                            kpi.Description = dr["Nm_Cliente"].ToString();
                            kpi.Value = (long)((decimal)dr["Valor"]);
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
    }
}
