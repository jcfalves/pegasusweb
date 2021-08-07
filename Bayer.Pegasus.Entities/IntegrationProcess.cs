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
    /// Entity Class for table Pegasus_ODS.Integracao
    /// </summary>
    [DataContract(Name = "Integracao")]
    public class IntegrationProcess
    {
        public IntegrationProcess()
        {

        }
        [DataMember(Name = "Cd_Integracao")]
        public int Code { get; set; }

        [DataMember(Name = "Nm_Integracao")]
        public string Name { get; set; }

        [DataMember(Name = "Id_Origem_Carga")]
        public int SourceCode { get; set; }

        [DataMember(Name = "Fl_Frequencia")]
        public string IntervalType { get; set; }

        [DataMember(Name = "Vl_Intervalo_Horario")]
        public int IntervalValue { get; set; }

        [DataMember(Name = "Nm_Pacote_SSIS")]
        public string SSISPackageName { get; set; }

        [DataMember(Name = "Ds_Url_Servico")]
        public string ServiceUrl { get; set; }

        [DataMember(Name = "Ds_Parametro_Padrao")]
        public string DefaultParameter { get; set; }

        [DataMember(Name = "Fl_Execucao_Manual")]
        public string CanExecuteManually { get; set; }

        [DataMember(Name = "Fl_Fluxo")]
        public string Flow { get; set; }

        [DataMember(Name = "Nu_Ordem_Execucao")]
        public int ExecutionOrder { get; set; }

        [DataMember(Name = "Steps")]
        public List<Monitor> Steps { get; set; }

        [DataMember(Name = "NumCnpjDistr")]
        public string NumCnpjDistr { get; set; }

        [DataMember(Name = "NumNotaFiscal")]
        public string NumeroNotaFiscal { get; set; }

        [DataMember(Name = "PeriodIni")]
        public string DtPeriodIni { get; set; }

        [DataMember(Name = "PeriodEnd")]
        public string DtPeriodEnd { get; set; }

        [DataMember(Name = "Fl_Ativo")]
        public string FlagEnable { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class IntegrationProcess {\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  SourceCode: ").Append(SourceCode).Append("\n");
            sb.Append("  IntervalType: ").Append(IntervalType).Append("\n");
            sb.Append("  IntervalValue: ").Append(IntervalValue).Append("\n");
            sb.Append("  SSISPackageName: ").Append(SSISPackageName).Append("\n");
            sb.Append("  ServiceUrl: ").Append(ServiceUrl).Append("\n");
            sb.Append("  DefaultParameter: ").Append(DefaultParameter).Append("\n");
            sb.Append("  CanExecuteManually: ").Append(CanExecuteManually).Append("\n");
            sb.Append("  Flow: ").Append(Flow).Append("\n");
            sb.Append("  ExecutionOrder: ").Append(ExecutionOrder).Append("\n");
            sb.Append("  ExecutioStepsnOrder: ").Append(Steps).Append("\n");
            sb.Append("  NumCnpjDistr: ").Append(NumCnpjDistr).Append("\n");
            sb.Append("  NumeroNotaFiscal: ").Append(NumeroNotaFiscal).Append("\n");
            sb.Append("  DtPeriodIni: ").Append(DtPeriodIni).Append("\n");
            sb.Append("  DtPeriodEnd: ").Append(DtPeriodEnd).Append("\n");
            sb.Append("  FlagEnable: ").Append(FlagEnable).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
