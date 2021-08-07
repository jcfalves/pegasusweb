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
    /// Entity Class for table Pegasus_ODS.Processamento
    /// </summary>
    [DataContract(Name = "Processamento")]
    public class ProcessItem
    {
        public ProcessItem()
        {

        }
        [DataMember(Name = "Id_Processamento")]
        public Int64 Id { get; set; }

        [DataMember(Name = "Cd_Integracao")]
        public int IntegrationProcessCode { get; set; }

        [DataMember(Name = "Dt_Inicio_Processamento")]
        public DateTime? Started { get; set; }

        [DataMember(Name = "Dt_Fim_Processamento")]
        public DateTime? Finished { get; set; }


        [DataMember(Name = "Dt_Fim_Processamento_Grid")]
        public string FinishedGrid { get; set; }


        [DataMember(Name = "Fl_Tipo_Execucao")]
        public string ExecutionType { get; set; }

        [DataMember(Name = "Ds_Parametro")]
        public string InputParameter { get; set; }

        [DataMember(Name = "Fl_Situacao")]
        public string StatusCode { get; set; }

        [DataMember(Name = "Qt_Registro_Lido")]
        public Int64 ReadRecords { get; set; }

        [DataMember(Name = "Qt_Registro_Gravado")]
        public Int64 WriteRecords { get; set; }

        [DataMember(Name = "Qt_Registro_Rejeitado")]
        public Int64 RejectRecords { get; set; }

        [DataMember(Name = "Dt_Referencia")]
        public DateTime? ReferenceDate { get; set; }

        [DataMember(Name = "Dt_Criacao")]
        public DateTime Created { get; set; }

        [DataMember(Name = "Cd_Login_Usuario")]
        public string User { get; set; }
        [DataMember(Name = "Nu_Ordem_Execucao")]
        public int ExecutionOrder { get; set; }
        public override string ToString()
        {
            return base.ToString();
        }
        public object DataObject { get; set; }
    }
}
