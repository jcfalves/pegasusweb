using System;
using System.Collections.Generic;
using System.Text;
using Bayer.Pegasus.Data;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Utils;
using Newtonsoft.Json.Linq;

namespace Bayer.Pegasus.Business
{
    public class PartnerBO : BaseBO
    {
        public List<Entities.Partner> GetPartners(Bayer.Pegasus.Entities.SalesStructureAccess salesStructure, string search, bool? isHeadQuarter, string[] partnerHeadquarterCodes, string[] crmCodes)
        {

            using (var partnerDAL = new PartnerDAL())
            {
                return partnerDAL.GetPartners(salesStructure, search, isHeadQuarter, partnerHeadquarterCodes, crmCodes);
            }
        }

    }
}
