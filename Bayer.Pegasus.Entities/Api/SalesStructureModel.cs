using System.Collections.Generic;

namespace Bayer.Pegasus.Entities.Api
{
    public class SalesStructureModel
    {
        public string startDate { get; set; }
        public int id { get; set; }
        public string countryCode { get; set; }
        public string salesOrgCode { get; set; }
        public bool activeFlag { get; set; }
        public List<SalesDistrictModel> salesDistrictsList { get; set; }
    }
}
