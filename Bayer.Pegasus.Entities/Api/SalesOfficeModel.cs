using System.Collections.Generic;

namespace Bayer.Pegasus.Entities.Api
{
    public class SalesOfficeModel
    {
        public string name { get; set; }
        public string code { get; set; }
        public int salesStructureId { get; set; }
        public string salesDistrictCode { get; set; }
        public List<SalesRepModel> salesRepsList { get; set; }
        public string managerCwid { get; set; }
        public string managerName { get; set; }
    }
}
