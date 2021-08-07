using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Bayer.Pegasus.Entities.SalesStructure
{

  /// <summary>
  /// EN - Sales Office PT - Regional 
  /// </summary>
  [DataContract]
  public class SalesOfficeSingle {
    /// <summary>
    /// EN - Code sales office PT - Codigo da Regional 
    /// </summary>
    /// <value>EN - Code sales office PT - Codigo da Regional </value>
    [DataMember(Name="code", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "code")]
    public string Code { get; set; }

    /// <summary>
    /// EN - Full name sales office PT - Nome da Regional 
    /// </summary>
    /// <value>EN - Full name sales office PT - Nome da Regional </value>
    [DataMember(Name="name", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// EN - Code of sales district PT - Codigo da diretoria de negocios 
    /// </summary>
    /// <value>EN - Code of sales district PT - Codigo da diretoria de negocios </value>
    [DataMember(Name="salesDistrictCode", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "salesDistrictCode")]
    public string SalesDistrictCode { get; set; }

    /// <summary>
    /// EN - Manager cwid PT - CWID do gerente 
    /// </summary>
    /// <value>EN - Manager cwid PT - CWID do gerente </value>
    [DataMember(Name="managerCwid", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "managerCwid")]
    public string ManagerCwid { get; set; }

    /// <summary>
    /// EN - Manager name PT - Nome do gerente 
    /// </summary>
    /// <value>EN - Manager name PT - Nome do gerente </value>
    [DataMember(Name="managerName", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "managerName")]
    public string ManagerName { get; set; }

    /// <summary>
    /// EN - Structure ID PT - ID da Estrutura 
    /// </summary>
    /// <value>EN - Structure ID PT - ID da Estrutura </value>
    [DataMember(Name="salesStructureId", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "salesStructureId")]
    public int? SalesStructureId { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      var sb = new StringBuilder();
      sb.Append("class SalesOfficeSingle {\n");
      sb.Append("  Code: ").Append(Code).Append("\n");
      sb.Append("  Name: ").Append(Name).Append("\n");
      sb.Append("  SalesDistrictCode: ").Append(SalesDistrictCode).Append("\n");
      sb.Append("  ManagerCwid: ").Append(ManagerCwid).Append("\n");
      sb.Append("  ManagerName: ").Append(ManagerName).Append("\n");
      sb.Append("  SalesStructureId: ").Append(SalesStructureId).Append("\n");
      sb.Append("}\n");
      return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson() {
      return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

}
}
