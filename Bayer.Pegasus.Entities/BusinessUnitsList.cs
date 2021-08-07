using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bayer.Pegasus.Entities
{
    public partial class BusinessUnitsList
    {
        public BusinessUnitsList()
        {
        }

        public BusinessUnitsList(string businessUnitCode = default(string), string businessUnitName = default(string))
        {
            this.businessUnitCode = businessUnitCode;
            this.businessUnitName = businessUnitName;
        }

        [DataMember(Name = "businessUnitCode")]
        public string businessUnitCode { get; set; }

        [DataMember(Name = "businessUnitName")]
        public string businessUnitName { get; set; }

    }



    public partial class BusinessUnitsArrayList
    {
        public BusinessUnitsArrayList()
        {
        }

        public BusinessUnitsArrayList(string businessGroupCode, List<BusinessUnitsList> businessUnitsList)
        {
            this.businessGroupCode = businessGroupCode;
            this.businessUnitsList = businessUnitsList;
        }

        [DataMember(Name = "businessGroupCode")]
        public string businessGroupCode { get; set; }

        [DataMember(Name = "businessUnitsList")]
        public List<BusinessUnitsList> businessUnitsList { get; set; }
    }
}
