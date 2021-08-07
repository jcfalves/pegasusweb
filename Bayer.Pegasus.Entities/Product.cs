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
    /// Product
    /// </summary>
    [DataContract]
    public partial class Product : IEquatable<Product>
    {

        public Product() {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product" /> class.
        /// </summary>
        /// <param name="Code">Code Product.</param>
        /// <param name="Name">Full name product.</param>
        /// <param name="businessUnitCode">Full name product.</param>
        public Product(string Code = default(string), string Name = default(string), string businessUnitCode = default(string))
        {
            this.Code = Code;
            this.Name = Name;
            this.businessUnitCode = businessUnitCode;

        }

        /// <summary>
        /// Code Product
        /// </summary>
        /// <value>Code Product</value>
        [DataMember(Name = "code")]
        public string Code { get; set; }
        /// <summary>
        /// Full name product
        /// </summary>
        /// <value>Full name product</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }


        [DataMember(Name = "businessUnitCode")]
        public string businessUnitCode { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Product {\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  businessUnitCode: ").Append(Name).Append("\n");
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
            return Equals((Product)obj);
        }

        /// <summary>
        /// Returns true if Product instances are equal
        /// </summary>
        /// <param name="other">Instance of Product to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Product other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    this.Code == other.Code ||
                    this.Code != null &&
                    this.Code.Equals(other.Code)
                ) &&
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) &&
                (
                    this.businessUnitCode == other.businessUnitCode ||
                    this.businessUnitCode != null &&
                    this.businessUnitCode.Equals(other.businessUnitCode)
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
                if (this.Code != null)
                    hash = hash * 59 + this.Code.GetHashCode();
                if (this.Name != null)
                    hash = hash * 59 + this.Name.GetHashCode();
                if (this.businessUnitCode != null)
                    hash = hash * 59 + this.businessUnitCode.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Product left, Product right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Product left, Product right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
