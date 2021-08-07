using System.Collections.Generic;

namespace Bayer.Pegasus.Entities.Api
{
    public class SalesDistrictModel
    {
        public string name { get; set; }
        public List<SalesOfficeModel> salesOfficesList { get; set; }
        public string code { get; set; }
        public int salesStructureId { get; set; }
        public string directorName { get; set; }
        public string directorCwid { get; set; }
        public string coordinatorName { get; set; }
        public string coordinatorCwid { get; set; }
    }
}
