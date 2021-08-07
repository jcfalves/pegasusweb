using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Bayer.Pegasus.Entities.SalesStructure
{

  /// <summary>
  /// EN - Sales Structure vigency PT - Vigência da estrutura de vendas 
  /// </summary>
  [DataContract]
  public class SalesStructureSingle {
    /// <summary>
    /// EN - Structure ID PT - ID da Estrutura 
    /// </summary>
    /// <value>EN - Structure ID PT - ID da Estrutura </value>
    [DataMember(Name="id", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "id")]
    public int? Id { get; set; }

    /// <summary>
    /// EN - Country code PT - Código do país 
    /// </summary>
    /// <value>EN - Country code PT - Código do país </value>
    [DataMember(Name="countryCode", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "countryCode")]
    public string CountryCode { get; set; }

    /// <summary>
    /// EN - Sales Organization Code PT - Código da organização de venda 
    /// </summary>
    /// <value>EN - Sales Organization Code PT - Código da organização de venda </value>
    [DataMember(Name="salesOrgCode", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "salesOrgCode")]
    public string SalesOrgCode { get; set; }

    /// <summary>
    /// EN - Vigency start date PT - Data de início da vigência 
    /// </summary>
    /// <value>EN - Vigency start date PT - Data de início da vigência </value>
    [DataMember(Name="startDate", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// EN - Vigency end date PT - Data de fim da vigência 
    /// </summary>
    /// <value>EN - Vigency end date PT - Data de fim da vigência </value>
    [DataMember(Name="endDate", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// EN - Active flag PT - Indicador de ativo 
    /// </summary>
    /// <value>EN - Active flag PT - Indicador de ativo </value>
    [DataMember(Name="activeFlag", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "activeFlag")]
    public bool? ActiveFlag { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      var sb = new StringBuilder();
      sb.Append("class SalesStructureSingle {\n");
      sb.Append("  Id: ").Append(Id).Append("\n");
      sb.Append("  CountryCode: ").Append(CountryCode).Append("\n");
      sb.Append("  SalesOrgCode: ").Append(SalesOrgCode).Append("\n");
      sb.Append("  StartDate: ").Append(StartDate).Append("\n");
      sb.Append("  EndDate: ").Append(EndDate).Append("\n");
      sb.Append("  ActiveFlag: ").Append(ActiveFlag).Append("\n");
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
