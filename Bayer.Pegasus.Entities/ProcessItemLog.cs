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
    /// Entity Class for table Pegasus_ODS.LogProcessamento
    /// </summary>
    [DataContract(Name = "LogProcessamento")]
    public class ProcessItemLog
    {
        public ProcessItemLog()
        {

        }
        [DataMember(Name = "Id_Log_Processamento")]
        public Int64 Id { get; set; }

        [DataMember(Name = "Id_Processamento")]
        public Int64 ProcessItemId { get; set; }

        [DataMember(Name = "Dt_Inclusao")]
        public DateTime Created { get; set; }

        [DataMember(Name = "Ds_Log_Processamento")]
        public string Description { get; set; }

        [DataMember(Name = "Cd_Fase_Processamento")]
        public int StageCode { get; set; }

        [DataMember(Name = "Fl_Tipo_Log")]
        public string LogType { get; set; }

    }
}
