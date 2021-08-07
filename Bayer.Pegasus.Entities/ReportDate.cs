using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    [DataContract(Name = "ReportDate")]
    public class ReportDate
    {
        public ReportDate()
        {

        }
        [DataMember(Name = "Year")]
        public int Year { get; set; }
        [DataMember(Name = "YearToYear")]
        public string YearToYear { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ReportDate {\n");
            sb.Append("  Year: ").Append(Year).Append("\n");
            sb.Append("  YearToYear: ").Append(YearToYear).Append("\n");
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

