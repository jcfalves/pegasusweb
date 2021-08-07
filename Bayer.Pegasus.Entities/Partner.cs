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
    /// Partner (Distribuidor)
    /// </summary>
    [DataContract]
    public partial class Partner 
    {

        /// <summary>
        /// EN - Partner ID on ERP PT - Identificador do Parceiro 
        /// </summary>
        /// <value>EN - Partner ID on ERP PT - Identificador do Parceiro </value>
        [DataMember(Name = "Code", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "Code")]
        public string Code {
            get {
                if (CrmCode != null) {
                    return CrmCode.Replace("-", "");
                }
                return CrmCode;
            }
            set { CrmCode = value; }
        }


        /// <summary>
        /// EN - Partner ID on CRM PT - Identificador do Parceiro no CRM 
        /// </summary>
        /// <value>EN - Partner ID on CRM PT - Identificador do Parceiro no CRM </value>
        [DataMember(Name = "crmCode", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "crmCode")]
        public string CrmCode { get; set; }

        /// <summary>
        /// EN - Partner ID on ERP PT - Identificador do Parceiro 
        /// </summary>
        /// <value>EN - Partner ID on ERP PT - Identificador do Parceiro </value>
        [DataMember(Name = "ERPCode", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "ERPCode")]
        public string ERPCode { get; set; }

        /// <summary>
        /// EN - CNPJ PT - CNPJ 
        /// </summary>
        /// <value>EN - CNPJ PT - CNPJ </value>
        [DataMember(Name = "cnpj", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "cnpj")]
        public string Cnpj { get; set; }

        /// <summary>
        /// EN - Company name PT - Razao Social 
        /// </summary>
        /// <value>EN - Company name PT - Razao Social </value>
        [DataMember(Name = "companyName", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "companyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// EN - Trade Name PT - Nome Fantasia 
        /// </summary>
        /// <value>EN - Trade Name PT - Nome Fantasia </value>
        [DataMember(Name = "tradeName", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "tradeName")]
        public string TradeName { get; set; }

        /// <summary>
        /// EN - Trade register number PT - Codigo da Inscrição Estadual 
        /// </summary>
        /// <value>EN - Trade register number PT - Codigo da Inscrição Estadual </value>
        [DataMember(Name = "inscricaoEstadualNumber", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "inscricaoEstadualNumber")]
        public string InscricaoEstadualNumber { get; set; }

        /// <summary>
        /// EN - Status PT - Status 
        /// </summary>
        /// <value>EN - Status PT - Status </value>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// EN - Price group  PT - Grupo de preco 
        /// </summary>
        /// <value>EN - Price group  PT - Grupo de preco </value>
        [DataMember(Name = "priceGroupCode", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "priceGroupCode")]
        public string PriceGroupCode { get; set; }

        /// <summary>
        /// EN - Partner type  PT - Tipo de Parceiro  
        /// </summary>
        /// <value>EN - Partner type  PT - Tipo de Parceiro  </value>
        [DataMember(Name = "type", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// EN - Partner Group code PT - Codigo do grupo de parceiro 
        /// </summary>
        /// <value>EN - Partner Group code PT - Codigo do grupo de parceiro </value>
        [DataMember(Name = "partnerGroupCode", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "partnerGroupCode")]
        public string PartnerGroupCode { get; set; }

        /// <summary>
        /// EN - Headquarter PT - Matriz 
        /// </summary>
        /// <value>EN - Headquarter PT - Matriz </value>
        [DataMember(Name = "isHeadquarter", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "isHeadquarter")]
        public bool? IsHeadquarter { get; set; }


        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PartnerSingle {\n");
            sb.Append("  CrmCode: ").Append(CrmCode).Append("\n");
            sb.Append("  ERPCode: ").Append(ERPCode).Append("\n");
            sb.Append("  Cnpj: ").Append(Cnpj).Append("\n");
            sb.Append("  CompanyName: ").Append(CompanyName).Append("\n");
            sb.Append("  TradeName: ").Append(TradeName).Append("\n");
            sb.Append("  InscricaoEstadualNumber: ").Append(InscricaoEstadualNumber).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  PriceGroupCode: ").Append(PriceGroupCode).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  PartnerGroupCode: ").Append(PartnerGroupCode).Append("\n");
            sb.Append("  IsHeadquarter: ").Append(IsHeadquarter).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Get the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
