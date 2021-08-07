using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Bayer.Pegasus.Entities.SalesStructure
{

  /// <summary>
  /// EN - Sales District  PT - Diretoria de Negocio 
  /// </summary>
  [DataContract]
  public class SalesDistrictSingle {
    /// <summary>
    /// EN - Sales District Code PT - Codigo da Diretoria de Negocio 
    /// </summary>
    /// <value>EN - Sales District Code PT - Codigo da Diretoria de Negocio </value>
    [DataMember(Name="code", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "code")]
    public string Code { get; set; }

    /// <summary>
    /// EN - Full name sales district PT - Nome da diretoria de negocio 
    /// </summary>
    /// <value>EN - Full name sales district PT - Nome da diretoria de negocio </value>
    [DataMember(Name="name", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// EN - Director cwid PT - CWID do diretor 
    /// </summary>
    /// <value>EN - Director cwid PT - CWID do diretor </value>
    [DataMember(Name="directorCwid", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "directorCwid")]
    public string DirectorCwid { get; set; }

    /// <summary>
    /// EN - Director name PT - Nome do diretor 
    /// </summary>
    /// <value>EN - Director name PT - Nome do diretor </value>
    [DataMember(Name="directorName", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "directorName")]
    public string DirectorName { get; set; }

    /// <summary>
    /// EN - Coordinator cwid PT - CWID do coordenador 
    /// </summary>
    /// <value>EN - Coordinator cwid PT - CWID do coordenador </value>
    [DataMember(Name="coordinatorCwid", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "coordinatorCwid")]
    public string CoordinatorCwid { get; set; }

    /// <summary>
    /// EN - Coordinator name PT - Nome do coordenador 
    /// </summary>
    /// <value>EN - Coordinator name PT - Nome do coordenador </value>
    [DataMember(Name="coordinatorName", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "coordinatorName")]
    public string CoordinatorName { get; set; }

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
      sb.Append("class SalesDistrictSingle {\n");
      sb.Append("  Code: ").Append(Code).Append("\n");
      sb.Append("  Name: ").Append(Name).Append("\n");
      sb.Append("  DirectorCwid: ").Append(DirectorCwid).Append("\n");
      sb.Append("  DirectorName: ").Append(DirectorName).Append("\n");
      sb.Append("  CoordinatorCwid: ").Append(CoordinatorCwid).Append("\n");
      sb.Append("  CoordinatorName: ").Append(CoordinatorName).Append("\n");
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
