using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;


namespace Bayer.Pegasus.Entities
{
    /// <summary>
    /// Entity Class for table Pegasus_ODS.Parametro
    /// </summary>
    [DataContract(Name = "Parametro")]
    public class ServiceParameter
    {
        public ServiceParameter()
        {

        }

        [DataMember(Name = "Cd_Parametro")]
        public string Code { get; set; }

        [DataMember(Name = "Nm_Parametro")]
        public string Name { get; set; }

        [DataMember(Name = "Fl_Tipo_Parametro")]
        public string Format { get; set; }

        [DataMember(Name = "Vl_Parametro")]
        public string Value { get; set; }

    }
}
