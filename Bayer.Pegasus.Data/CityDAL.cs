using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Data
{
    public class CityDAL : BaseDAL
    {

        public List<Entities.City> GetCities(string search)
        {
            List<Entities.City> cities = new List<Entities.City>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_SEL_CIDADE";

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                
                cmd.Parameters.AddWithValue("@NumCidades", 30);
                cmd.Parameters.AddWithValue("@Search", search);

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {

                        Bayer.Pegasus.Entities.City city = new Bayer.Pegasus.Entities.City();


                        city = new Bayer.Pegasus.Entities.City();

                        city.CityName = dr["CityName"].ToString();

                        city.CodeIbge = dr["CodeIbge"].ToString();

                        city.StateAcronym = dr["StateAcronym"].ToString();

                        cities.Add(city);
                    }

                }

                cmd.Connection.Close();

            }




            return cities;
        }
    }
}
