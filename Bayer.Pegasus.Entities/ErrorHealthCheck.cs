using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    /// <summary>
    /// Entity Class for table Pegasus_ODS.Parametro
    /// </summary>
    [DataContract(Name = "Erro")]
    public class ErrorHealthCheck
    {

        public ErrorHealthCheck()
        {

        }

        [DataMember(Name = "Cd_Tipo_Registro")]
        public string CodeTypeRegister { get; set; }

        [DataMember(Name = "Id_Categoria_Erro")]
        public string ErrorCategoryId { get; set; }

        [DataMember(Name = "Cd_Erro")]
        public string Code { get; set; }

        [DataMember(Name = "Ds_Erro")]
        public string Description { get; set; }

        [DataMember(Name = "Ds_Registro")]
        public string ColumnsValues { get; set; }

        [DataMember(Name = "Qt_Ocorrencia")]
        public long Total_Occurrences { get; set; }

        [DataMember(Name = "Pc_Ocorrencia_Total")]
        public double Percent { get; set; }
    }
}
