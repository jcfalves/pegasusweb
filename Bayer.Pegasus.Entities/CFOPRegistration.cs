using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    [DataContract(Name = "CFOPRegistration")]
    public class CFOPRegistration
    {
        public CFOPRegistration()
        {

        }
        [DataMember(Name = "Cd_Cfop")]
        public int CfopCode
        {
            get;
            set;
        }

        [DataMember(Name = "Ds_Cfop")]
        public string CfopDescription
        {
            get;
            set;
        }

        [DataMember(Name = "Fl_Operacao")]
        public int OperationType
        {
            get;
            set;
        }

        [DataMember(Name = "Fl_Pegasus")]
        public bool FlagPegasus
        {
            get;
            set;
        }

        [DataMember(Name = "Fl_Ativo")]
        public bool FlagEnable
        {
            get;
            set;
        }

        [DataMember(Name = "Acao")]
        public char Acao
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CFOPRegistration {\n");
            sb.Append("  CfopCode: ").Append(CfopCode).Append("\n");
            sb.Append("  CfopDescription: ").Append(CfopDescription).Append("\n");
            sb.Append("  OperationType: ").Append(OperationType).Append("\n");
            sb.Append("  FlagPegasus: ").Append(FlagPegasus).Append("\n");
            sb.Append("  FlagEnable: ").Append(FlagEnable).Append("\n");
            sb.Append("  Acao: ").Append(Acao).Append("\n");
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
