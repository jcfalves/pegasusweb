using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    /// <summary>
    /// Entity Class for table Pegasus_ODS.Parametro
    /// </summary>
    [DataContract(Name = "Tipo_Erro")]   
    public class TypeErrorHealthCheck
    {
        public TypeErrorHealthCheck()
        {

        }

        [DataMember(Name = "Cd_Erro")]
        public string Code { get; set; }

        [DataMember(Name = "Cd_Tipo_Registro")]
        public string CodeTypeRegister { get; set; }
        
        [DataMember(Name = "Ds_Erro")]
        public string Description { get; set; }

        [DataMember(Name = "Ds_Tipo_Registro")]
        public string DescriptionTyeRegister { get; set; }

        [DataMember(Name = "Ds_Nomes_Colunas")]
        public string Columns { get; set; }

        [DataMember(Name = "Id_Categoria_Erro")]
        public int ErrorCategoryId { get; set; }

        [DataMember(Name = "DateIni")]
        public string DateIni { get; set; }

        [DataMember(Name = "DateEnd")]
        public string DateEnd { get; set; }

        [DataMember(Name = "CheckedTypes")]
        public string CheckedTypes { get; set; }

        [DataMember(Name = "FL_Impeditivo")]
        public string Impediment { get; set; }

        [DataMember(Name = "Email")]
        public string Email { get; set; }
    }
}
