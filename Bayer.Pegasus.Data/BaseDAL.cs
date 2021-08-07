using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Bayer.Pegasus.Entities;

namespace Bayer.Pegasus.Data
{
    public class BaseDAL : IDisposable
    {
        public List<Entities.Kpis.EvolutionKPI> GetEvolutionKPIsFromDataReader(SqlDataReader dr, string groupBy, string typeDataChart) {

            List<Entities.Kpis.EvolutionKPI> kpis = new List<Entities.Kpis.EvolutionKPI>();

            if (String.IsNullOrEmpty(groupBy))
            {
                var kpi = new Bayer.Pegasus.Entities.Kpis.EvolutionKPI();
                
                while (dr.Read())
                {
                    var ordem = (int)dr["Ordem"];
                   
                    decimal valor = 0;

                    if (typeDataChart == "Quantity")
                    {
                        object o = dr["Qtd"];

                        if (o is Int64) {
                            valor = (decimal)(long)dr["Qtd"];
                        }
                        else {
                            valor = (decimal)dr["Qtd"];
                        }

                    }
                    else {
                        valor = (decimal)dr["Valor"];
                    }

                    var item = new Tuple<int, decimal>(ordem, valor);

                    kpi.KPIData.Add(item);


                }
                
                kpis.Add(kpi);

            }
            else
            {
                List<Tuple<string, int, decimal>> rawData = new List<Tuple<string, int, decimal>>();

                while (dr.Read())
                {
                    var grupo = (string)dr["grupo"];
                    var ordem = (int)dr["Ordem"];

                    decimal valor = 0;

                    if (typeDataChart == "Quantity")
                    {
                        object o = dr["Qtd"];

                        if (o is Int64)
                        {
                            valor = (decimal)(long)dr["Qtd"];
                        }
                        else
                        {
                            valor = (decimal)dr["Qtd"];
                        }
                    }
                    else
                    {
                        valor = (decimal)dr["Valor"];
                    }

                    Tuple<string, int, decimal> rawDataItem = new Tuple<string, int, decimal>(grupo, ordem, valor);
                    rawData.Add(rawDataItem);

                }

                kpis = Entities.Kpis.EvolutionKPI.FromRawData(rawData);

            }

            return kpis;
        }
       
        public SqlDataReader GetDataReader(SqlCommand cmd) {
            var cmdSettings = new System.Data.SqlClient.SqlCommand("SET ARITHABORT ON", cmd.Connection);
            cmdSettings.ExecuteNonQuery();
            cmd.CommandTimeout = 180;
            return cmd.ExecuteReader();
        }


        public void CreateBooleanParameter(SqlCommand cmd, string parameterName, bool? value)
        {

            if (value.HasValue) {
                SqlParameter p = new SqlParameter(parameterName, SqlDbType.Bit);
                p.Value = value.Value;


                cmd.Parameters.Add(p);
            }
            
        }

        public void CreateIntParameter(SqlCommand cmd, string parameterName, int value)
        {
            SqlParameter p = new SqlParameter(parameterName, SqlDbType.Int);
            p.Value = value;

            cmd.Parameters.Add(p);


        }

        public void CreateLongParameter(SqlCommand cmd, string parameterName, long value)
        {
            SqlParameter p = new SqlParameter(parameterName, SqlDbType.BigInt);
            p.Value = value;

            cmd.Parameters.Add(p);


        }

        public void CreateVarBinaryParameter(SqlCommand cmd, string parameterName, byte[] value)
        {
            SqlParameter p = new SqlParameter(parameterName, SqlDbType.VarBinary);

            if (value == null)
            {
                p.Value = DBNull.Value;

            }
            else
            {
                p.Value = value;

            }

            cmd.Parameters.Add(p);
        }

        public void CreateStringParameter(SqlCommand cmd, string parameterName, string value)
        {
            

            SqlParameter p = new SqlParameter(parameterName, SqlDbType.VarChar);

            if (value == null)
            {
                p.Value = DBNull.Value;

            }
            else
            {
                p.Value = value;

            }

            cmd.Parameters.Add(p);
        }

        public void CreateDateTimeParameter(SqlCommand cmd, string parameterName, DateTime? value)
        {
            SqlParameter p = new SqlParameter(parameterName, SqlDbType.DateTime);

            if (value == null)
            {
                p.Value = DBNull.Value;

            }
            else
            {
                p.Value = value;

            }

            cmd.Parameters.Add(p);
        }


        public void CreateIntervalParameter(SqlCommand cmd, string parameterName, Entities.ReportDateInterval interval) {
            SqlParameter pStart = new SqlParameter(parameterName + "Inicio", SqlDbType.Date);

            if (interval == null || interval.StartDate == null) {
                pStart.Value = DBNull.Value;
            }
            else {
                pStart.Value = interval.StartDate;
            }
            
            cmd.Parameters.Add(pStart);

            SqlParameter pEnd = new SqlParameter(parameterName + "Fim", SqlDbType.Date);
          
            if (interval == null || interval.EndDate == null)
            {
                pEnd.Value = DBNull.Value;
            }
            else
            {
                pEnd.Value = interval.EndDate;
            }


            cmd.Parameters.Add(pEnd);
        }

        public void CreateSalesStructureParameter(SqlCommand cmd, SalesStructureAccess salesStructure) {
            List<string> empty = new List<string>();

            if (salesStructure.IsSalesDistrict)
            {
                CreateArrayListParameter(cmd, "@Diretorias", salesStructure.SalesDistrict);
            }
            else {
                CreateArrayListParameter(cmd, "@Diretorias", empty);
            }

            if (salesStructure.IsSalesOffice)
            {
                CreateArrayListParameter(cmd, "@Regionais", salesStructure.SalesOffice);
            }
            else
            {
                CreateArrayListParameter(cmd, "@Regionais", empty);
            }

            if (salesStructure.IsSalesRepresentative)
            {
                CreateArrayListParameter(cmd, "@Rtvs", salesStructure.SalesRepresentative);
            }
            else
            {
                CreateArrayListParameter(cmd, "@Rtvs", empty);
            }

            CreateArrayListParameter(cmd, "@Parceiros", salesStructure.Partners);



        }

        public void CreateArrayListParameter(SqlCommand cmd, string parameterName, List<string> values) {

            DataTable tvp = new DataTable();

            tvp.Columns.Add("Item");

            if (values != null) {
                foreach (var value in values)
                {
                    tvp.Rows.Add(value);
                }
            }
           

            SqlParameter tvparam = new SqlParameter();
            tvparam.ParameterName = parameterName;
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.Value = tvp;
            

            cmd.Parameters.Add(tvparam);
            
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            
        }

    }
}
