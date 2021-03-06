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
    public partial class AlterPasswordModel :  IEquatable<AlterPasswordModel>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterPasswordModel" /> class.
        /// </summary>
        /// <param name="AppId">AppId.</param>
        /// <param name="Login">Login.</param>
        /// <param name="Ip">Ip.</param>
        /// <param name="CultureName">CultureName.</param>
        /// <param name="OldPassword">OldPassword.</param>
        /// <param name="NewPassword">NewPassword.</param>
        public AlterPasswordModel(string AppId = default(string), string Login = default(string), string Ip = default(string), string CultureName = default(string), string OldPassword = default(string), string NewPassword = default(string))
        {
            this.AppId = AppId;
            this.Login = Login;
            this.Ip = Ip;
            this.CultureName = CultureName;
            this.OldPassword = OldPassword;
            this.NewPassword = NewPassword;
            
        }

        /// <summary>
        /// Gets or Sets AppId
        /// </summary>
        [DataMember(Name="appId")]
        public string AppId { get; set; }
        /// <summary>
        /// Gets or Sets Login
        /// </summary>
        [DataMember(Name="login")]
        public string Login { get; set; }
        /// <summary>
        /// Gets or Sets Ip
        /// </summary>
        [DataMember(Name="ip")]
        public string Ip { get; set; }
        /// <summary>
        /// Gets or Sets CultureName
        /// </summary>
        [DataMember(Name="cultureName")]
        public string CultureName { get; set; }
        /// <summary>
        /// Gets or Sets OldPassword
        /// </summary>
        [DataMember(Name="oldPassword")]
        public string OldPassword { get; set; }
        /// <summary>
        /// Gets or Sets NewPassword
        /// </summary>
        [DataMember(Name="newPassword")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AlterPasswordModel {\n");
            sb.Append("  AppId: ").Append(AppId).Append("\n");
            sb.Append("  Login: ").Append(Login).Append("\n");
            sb.Append("  Ip: ").Append(Ip).Append("\n");
            sb.Append("  CultureName: ").Append(CultureName).Append("\n");
            sb.Append("  OldPassword: ").Append(OldPassword).Append("\n");
            sb.Append("  NewPassword: ").Append(NewPassword).Append("\n");
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
            return Equals((AlterPasswordModel)obj);
        }

        /// <summary>
        /// Returns true if AlterPasswordModel instances are equal
        /// </summary>
        /// <param name="other">Instance of AlterPasswordModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AlterPasswordModel other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.AppId == other.AppId ||
                    this.AppId != null &&
                    this.AppId.Equals(other.AppId)
                ) && 
                (
                    this.Login == other.Login ||
                    this.Login != null &&
                    this.Login.Equals(other.Login)
                ) && 
                (
                    this.Ip == other.Ip ||
                    this.Ip != null &&
                    this.Ip.Equals(other.Ip)
                ) && 
                (
                    this.CultureName == other.CultureName ||
                    this.CultureName != null &&
                    this.CultureName.Equals(other.CultureName)
                ) && 
                (
                    this.OldPassword == other.OldPassword ||
                    this.OldPassword != null &&
                    this.OldPassword.Equals(other.OldPassword)
                ) && 
                (
                    this.NewPassword == other.NewPassword ||
                    this.NewPassword != null &&
                    this.NewPassword.Equals(other.NewPassword)
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
                    if (this.AppId != null)
                    hash = hash * 59 + this.AppId.GetHashCode();
                    if (this.Login != null)
                    hash = hash * 59 + this.Login.GetHashCode();
                    if (this.Ip != null)
                    hash = hash * 59 + this.Ip.GetHashCode();
                    if (this.CultureName != null)
                    hash = hash * 59 + this.CultureName.GetHashCode();
                    if (this.OldPassword != null)
                    hash = hash * 59 + this.OldPassword.GetHashCode();
                    if (this.NewPassword != null)
                    hash = hash * 59 + this.NewPassword.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(AlterPasswordModel left, AlterPasswordModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AlterPasswordModel left, AlterPasswordModel right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
