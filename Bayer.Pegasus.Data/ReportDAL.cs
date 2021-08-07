using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Bayer.Pegasus.Data
{
    public class ReportDAL : BaseDAL
    {

        public Entities.Report GetReport(string identifier)
        {
            Entities.Report report = new Entities.Report();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_GET_RELATORIO";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateStringParameter(cmd, "@Identificador", identifier);

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                        report.Id = (long)dr["Id_Relatorio"];
                        report.SerializedContent = (byte[])dr["Conteudo"];
                        report.Json = dr["Json"].ToString();
                        report.Created = (DateTime)dr["Dt_Criacao"];
                        report.Identifier =  dr["Identificador"].ToString();
                    }
                }
            }

            return report;
        }

        public long SaveReport(string user, Entities.Report report)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                long insertedId = 0;
                string sql = "SPS_PGS_INS_RELATORIO";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateStringParameter(cmd, "@Identificador", report.Identifier);
                CreateStringParameter(cmd, "@Json", report.Json);
                CreateVarBinaryParameter(cmd, "@Conteudo", report.SerializedContent);
                CreateStringParameter(cmd, "@CWID", user);

                cmd.Connection.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        insertedId = rdr.GetInt64(0);
                    }

                    rdr.Close();
                }

                conn.Close();

                return insertedId;

            }
        }
    }
}
