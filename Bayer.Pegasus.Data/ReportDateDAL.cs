using Bayer.Pegasus.Entities;
using System.Collections.Generic;

namespace Bayer.Pegasus.Data
{
    public class ReportDateDAL : BaseDAL
    {
        public List<ReportDate> GetListYearMoviment()
        {
            List<ReportDate> results = new List<ReportDate>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_SEL_FILTRO_ANO_MOVIMENTO";
                var finalyear = System.DateTime.Now.Year;

                if (System.DateTime.Now.Month >= 4)
                {
                    finalyear++;
                }
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                        ReportDate reportDate = new ReportDate();
                        reportDate.Year = (int)dr[0];
                        reportDate.YearToYear = ((int)dr[0] - 1).ToString() + "/" + dr[0].ToString();
                        results.Add(reportDate);
                    }
                }

                cmd.Connection.Close();

            }
            return results;
        }
    }
}
