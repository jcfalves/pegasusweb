using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Bayer.Pegasus.Entities;

namespace Bayer.Pegasus.Data
{
    public class AcceraReportDAL : BaseDAL
    {
        public List<Entities.AcceraReportItem> GetReport(SalesStructureAccess salesStructure)
        {
            List<Entities.AcceraReportItem> items = new List<Entities.AcceraReportItem>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(Bayer.Pegasus.Utils.Configuration.Instance.ConnectionString))
            {
                string sql = "SPS_PGS_SEL_ACCERA";


                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                CreateSalesStructureParameter(cmd, salesStructure);

                cmd.Connection.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var acceraItem = new Entities.AcceraReportItem();
                        acceraItem.CodePartner = dr["CrmCode"].ToString();
                        acceraItem.CNPJPartner = dr["CNPJ_Parceiro"].ToString();
                        acceraItem.PartnerName = dr["Nome_Parceiro"].ToString();
                        acceraItem.Criticality = dr["Criticidade"].ToString();
                        acceraItem.Action = dr["Acao"].ToString();
                        acceraItem.Responsible = dr["Responsavel"].ToString();
                        acceraItem.LastInteraction = DateTime.Parse(dr["Dt_Ult_Interacao"].ToString());


                        if(!Convert.IsDBNull(dr["Dt_Ult_Posicao_Estoque"]))
                            acceraItem.LastStockPosition = DateTime.Parse(dr["Dt_Ult_Posicao_Estoque"].ToString());

                        acceraItem.DaysPosition = new Dictionary<int, string>();

                        for (int i = 1; i < 32; i++)
                        {
                            String vField = (i < 10) ? "Dias_v0" + i : "Dias_v" + i;
                            String valueField = (String.IsNullOrEmpty(dr[vField].ToString())) ? "OK" : dr[vField].ToString();
                            
                            acceraItem.DaysPosition.Add(i, valueField);
                        }

                        items.Add(acceraItem);
                    }

                }

                cmd.Connection.Close();

            }

            return items;
        }
    }
}
