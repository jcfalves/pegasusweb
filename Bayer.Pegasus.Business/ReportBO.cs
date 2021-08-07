using System;
using System.Collections.Generic;
using System.Text;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Data;

namespace Bayer.Pegasus.Business
{
    public class ReportBO : BaseBO
    {
        public Entities.Report GetReport(string identifier)
        {
            using (var reportDAL = new ReportDAL())
            {
                return reportDAL.GetReport(identifier);
            }
        }

        public long SaveReport(System.Security.Claims.ClaimsPrincipal user, Entities.Report report)
        {
            using (var reportDAL = new ReportDAL())
            {

                report.Identifier = Bayer.Pegasus.Utils.Base64.Base64Encode(System.Guid.NewGuid().ToString());
                report.Created = System.DateTime.Now;

                return reportDAL.SaveReport(user.Identity.Name, report);
            }
        }

    }
}
