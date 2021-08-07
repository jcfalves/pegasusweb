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
    /// 
    /// </summary>
    [DataContract]
    public partial class Customer : IEquatable<Customer>
    {
        public Customer() {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Customer" /> class.
        /// </summary>
        /// <param name="Code">Customer code.</param>
        /// <param name="Name">Name.</param>
        /// <param name="TradeName">Trade name.</param>
        /// <param name="DocumentType">Document type.</param>
        /// <param name="DocumentNumber">Document number.</param>
        /// <param name="Status">Customer status.</param>
        public Customer(string Code = default(string), string Name = default(string), string TradeName = default(string), string DocumentType = default(string), string DocumentNumber = default(string), string Status = default(string))
        {
            this.Code = Code;
            this.Name = Name;
            this.TradeName = TradeName;
            this.DocumentType = DocumentType;
            this.DocumentNumber = DocumentNumber;
            this.Status = Status;

        }

        /// <summary>
        /// Customer code
        /// </summary>
        /// <value>Customer code</value>
        [DataMember(Name = "code")]
        public string Code { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        /// <value>Name</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }
        /// <summary>
        /// Trade name
        /// </summary>
        /// <value>Trade name</value>
        [DataMember(Name = "trade_name")]
        public string TradeName { get; set; }
        /// <summary>
        /// Document type
        /// </summary>
        /// <value>Document type</value>
        [DataMember(Name = "document_type")]
        public string DocumentType { get; set; }
        /// <summary>
        /// Document number
        /// </summary>
        /// <value>Document number</value>
        [DataMember(Name = "document_number")]
        public string DocumentNumber { get; set; }
        /// <summary>
        /// Customer status
        /// </summary>
        /// <value>Customer status</value>
        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Customer status
        /// </summary>
        /// <value>Customer status</value>
        [DataMember(Name = "address")]
        public string Address { get; set; }

        /// <summary>
        /// Customer status
        /// </summary>
        /// <value>Customer status</value>
        [DataMember(Name = "zipCode")]
        public string ZipCode { get; set; }
        
        [JsonProperty(PropertyName = "acquired")]
        public bool Acquired
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "loyal")]
        public bool Loyal
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "lost")]
        public bool Lost
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "reacquired")]
        public bool Reacquired
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "retained")]
        public bool Retained
        {
            get;
            set;
        }

        public Entities.City City
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
            sb.Append("class Customer {\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  TradeName: ").Append(TradeName).Append("\n");
            sb.Append("  DocumentType: ").Append(DocumentType).Append("\n");
            sb.Append("  DocumentNumber: ").Append(DocumentNumber).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
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
            return Equals((Customer)obj);
        }

        /// <summary>
        /// Returns true if Customer instances are equal
        /// </summary>
        /// <param name="other">Instance of Customer to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Customer other)
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
                    this.TradeName == other.TradeName ||
                    this.TradeName != null &&
                    this.TradeName.Equals(other.TradeName)
                ) &&
                (
                    this.DocumentType == other.DocumentType ||
                    this.DocumentType != null &&
                    this.DocumentType.Equals(other.DocumentType)
                ) &&
                (
                    this.DocumentNumber == other.DocumentNumber ||
                    this.DocumentNumber != null &&
                    this.DocumentNumber.Equals(other.DocumentNumber)
                ) &&
                (
                    this.Status == other.Status ||
                    this.Status != null &&
                    this.Status.Equals(other.Status)
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
                if (this.TradeName != null)
                    hash = hash * 59 + this.TradeName.GetHashCode();
                if (this.DocumentType != null)
                    hash = hash * 59 + this.DocumentType.GetHashCode();
                if (this.DocumentNumber != null)
                    hash = hash * 59 + this.DocumentNumber.GetHashCode();
                if (this.Status != null)
                    hash = hash * 59 + this.Status.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Customer left, Customer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Customer left, Customer right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
