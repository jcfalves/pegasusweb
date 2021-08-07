/*
 * Auth Services
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: V 1.0.1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Bayer.Pegasus.Entities.Auth
{

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class LevelViewModelSwagger :  IEquatable<LevelViewModelSwagger>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelViewModelSwagger" /> class.
        /// </summary>
        /// <param name="Result">Result.</param>
        /// <param name="Return">Return.</param>
        public LevelViewModelSwagger(Message Result = default(Message), LevelViewModel Return = default(LevelViewModel))
        {
            this.Result = Result;
            this.Return = Return;
            
        }

        /// <summary>
        /// Gets or Sets Result
        /// </summary>
        [DataMember(Name="result")]
        public Message Result { get; set; }
        /// <summary>
        /// Gets or Sets Return
        /// </summary>
        [DataMember(Name="return")]
        public LevelViewModel Return { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class LevelViewModelSwagger {\n");
            sb.Append("  Result: ").Append(Result).Append("\n");
            sb.Append("  Return: ").Append(Return).Append("\n");
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
            return Equals((LevelViewModelSwagger)obj);
        }

        /// <summary>
        /// Returns true if LevelViewModelSwagger instances are equal
        /// </summary>
        /// <param name="other">Instance of LevelViewModelSwagger to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(LevelViewModelSwagger other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.Result == other.Result ||
                    this.Result != null &&
                    this.Result.Equals(other.Result)
                ) && 
                (
                    this.Return == other.Return ||
                    this.Return != null &&
                    this.Return.Equals(other.Return)
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
                    if (this.Result != null)
                    hash = hash * 59 + this.Result.GetHashCode();
                    if (this.Return != null)
                    hash = hash * 59 + this.Return.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(LevelViewModelSwagger left, LevelViewModelSwagger right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LevelViewModelSwagger left, LevelViewModelSwagger right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
