using System;
using System.Collections.Generic;
using System.Text;
using Bayer.Pegasus.Data;

namespace Bayer.Pegasus.Business
{
    public class AcceraReportBO : BaseBO
    {

        public List<Entities.AcceraReportItem> GetReport(System.Security.Claims.ClaimsPrincipal user, List<string> partners)
        {
            var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(user);
            if (salesStructure.CanAccessMultiplePartners)
            {
                salesStructure.Partners = partners;
            }

            using (var acceraDAL = new AcceraReportDAL())
            {
                return acceraDAL.GetReport(salesStructure);
            }
        }
    }
}
