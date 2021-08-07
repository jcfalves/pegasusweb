using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Data
{
    public class CFOPDAL : BaseDAL
    {

        public List<Entities.CFOP> GetCFOPs(string search)
        {
            var cfops = new List<Entities.CFOP>();

            using (var conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                var sql = "SPS_PGS_SEL_CFOP";

                var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Fl_Pegasus", 1);
                cmd.Parameters.AddWithValue("@Fl_Ativo", 1);

                if (search == "Credito")
                {
                    /*
                    cmd.Parameters.AddWithValue("@Credito", 1);
                    cmd.Parameters.AddWithValue("@Debito", 0);
                    */
                    cmd.Parameters.AddWithValue("@Fl_Operacao", 1);
                }
                else if (search == "Debito")
                {
                    /*
                    cmd.Parameters.AddWithValue("@Credito", 0);
                    cmd.Parameters.AddWithValue("@Debito", 1);
                    */
                    cmd.Parameters.AddWithValue("@Fl_Operacao", -1);
                }

                cmd.Connection.Open();
                using (var dr = GetDataReader(cmd))
                {
                    while (dr.Read())
                    {

                        var cfop = new Bayer.Pegasus.Entities.CFOP();


                        cfop = new Bayer.Pegasus.Entities.CFOP();

                        cfop.Code = dr["Cd_Cfop"].ToString();

                        cfop.Description = dr["Ds_Cfop"].ToString();

                        cfop.OperationType = (int)dr["Fl_Operacao"];

                        cfop.Debit = (int)dr["Fl_Operacao"] == -1 ? true : false;

                        cfop.Credit = (int)dr["Fl_Operacao"] == 1 ? true : false;

                        cfops.Add(cfop);
                    }

                }

                cmd.Connection.Close();

            }
            return cfops;
        }
    }
}
