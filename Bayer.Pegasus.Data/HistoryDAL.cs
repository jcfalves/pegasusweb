using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Data
{
    public class HistoryDAL : BaseDAL
    {
        public List<Entities.History> ListHistory(string user, string report)
        {
            var history = new List<Entities.History>();

            using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                var sql = "SPS_PGS_SEL_HISTORICO";

                var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateStringParameter(cmd, "@Cd_AnaliseTipo", report);
                CreateStringParameter(cmd, "@CWID", user);

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                        var item = new Entities.History();
                        item.Id = (long)dr["Id_AnaliseHistorico"];
                        item.Description = dr["Ds_Analise"].ToString();
                        item.Json = dr["Json"].ToString();
                        item.Created = (DateTime)dr["Dt_Criacao"];
                        item.Description = dr["Ds_Analise"].ToString();

                        history.Add(item);
                    }
                }
            }

            return history;
        }

        public Entities.History GetHistory(string user, long id)
        {
            var history = new Entities.History();

            using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                var sql = "SPS_PGS_GET_HISTORICO";

                var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateLongParameter(cmd, "@Id", id);
                CreateStringParameter(cmd, "@CWID", user);

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                        history.Id = (long)dr["Id_AnaliseHistorico"];
                        history.Description = dr["Ds_Analise"].ToString();
                        history.Json = dr["Json"].ToString();
                        history.Created = (DateTime)dr["Dt_Criacao"];
                        history.Description = dr["Ds_Analise"].ToString();
                    }
                }
            }

            return history;
        }


        public Boolean DeleteHistory(string user, long id)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                var sql = "SPS_PGS_EXC_HISTORICO";

                var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateLongParameter(cmd, "@Id", id);
                CreateStringParameter(cmd, "@CWID", user);

                cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                conn.Close();

                return true;

            }
        }

        public long SaveHistory(string user, string report, Entities.History history)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                long insertedId = 0;
                var sql = "SPS_PGS_INS_HISTORICO";

                var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                CreateStringParameter(cmd, "@Ds_Analise", history.Description);
                CreateStringParameter(cmd, "@Json", history.Json);
                CreateStringParameter(cmd, "@Cd_AnaliseTipo", report);
                CreateStringParameter(cmd, "@CWID", user);

                cmd.Connection.Open();

                using (var rdr = cmd.ExecuteReader())
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
