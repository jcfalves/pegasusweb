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
    /// City
    /// </summary>
    [DataContract]
    public partial class City : IEquatable<City>
    {
        public City() {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="City" /> class.
        /// </summary>
        /// <param name="CodeIbge">City IBGE Code.</param>
        /// <param name="CityName">City name.</param>
        /// <param name="StateAcronym">State acronym.</param>
        public City(string CodeIbge = default(string), string CityName = default(string), string StateAcronym = default(string))
        {
            this.CodeIbge = CodeIbge;
            this.CityName = CityName;
            this.StateAcronym = StateAcronym;

        }

        /// <summary>
        /// City IBGE Code
        /// </summary>
        /// <value>City IBGE Code</value>
        [DataMember(Name = "code_ibge")]
        public string CodeIbge { get; set; }
        /// <summary>
        /// City name
        /// </summary>
        /// <value>City name</value>
        [DataMember(Name = "city_name")]
        public string CityName { get; set; }
        /// <summary>
        /// State acronym
        /// </summary>
        /// <value>State acronym</value>
        [DataMember(Name = "state_acronym")]
        public string StateAcronym { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class City {\n");
            sb.Append("  CodeIbge: ").Append(CodeIbge).Append("\n");
            sb.Append("  CityName: ").Append(CityName).Append("\n");
            sb.Append("  StateAcronym: ").Append(StateAcronym).Append("\n");
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

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((City)obj);
        }

        /// <summary>
        /// Returns true if City instances are equal
        /// </summary>
        /// <param name="other">Instance of City to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(City other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    this.CodeIbge == other.CodeIbge ||
                    this.CodeIbge != null &&
                    this.CodeIbge.Equals(other.CodeIbge)
                ) &&
                (
                    this.CityName == other.CityName ||
                    this.CityName != null &&
                    this.CityName.Equals(other.CityName)
                ) &&
                (
                    this.StateAcronym == other.StateAcronym ||
                    this.StateAcronym != null &&
                    this.StateAcronym.Equals(other.StateAcronym)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                if (this.CodeIbge != null)
                    hash = hash * 59 + this.CodeIbge.GetHashCode();
                if (this.CityName != null)
                    hash = hash * 59 + this.CityName.GetHashCode();
                if (this.StateAcronym != null)
                    hash = hash * 59 + this.StateAcronym.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(City left, City right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(City left, City right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
