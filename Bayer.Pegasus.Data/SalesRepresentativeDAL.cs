using System.Collections.Generic;

namespace Bayer.Pegasus.Data
{
    public class SalesRepresentativeDAL : BaseDAL
    {
        public List<Entities.SalesRepresentative> GetSalesRepresentativesCode(string cwdid)
        {
            List<Entities.SalesRepresentative> reotrno = new List<Entities.SalesRepresentative>();
            using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                var cmd = new System.Data.SqlClient.SqlCommand("SPS_PGS_SEL_SalesRepresentatives", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CWID", cwdid);

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {
                        reotrno.Add(new Entities.SalesRepresentative
                        {
                            code = dr[0].ToString()
                        });
                    }
                }

                cmd.Connection.Close();
            }
            return reotrno;
        }
    }
}
