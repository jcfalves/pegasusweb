using Newtonsoft.Json;


namespace Bayer.Pegasus.Entities.Api
{
    public class SalesStructureViewModel
    {
        #region Public Properties

        [JsonProperty("withpartners")]
        public bool? withPartners { get; set; }
        [JsonProperty("saledistrictcode")]
        public string salesDistrictCode { get; set; }
        [JsonProperty("salesofficecode")]
        public string salesOfficeCode { get; set; }
        [JsonProperty("salesrepcode")]
        public string salesRepCode { get; set; }
        [JsonProperty("referencedate")]
        public string referenceDate { get; set; }
        [JsonProperty("repMainfunctionflag")]
        public string repMainFunctionFlag { get; set; }
        [JsonProperty("countrycode")]
        public string countryCode { get; set; }
        [JsonProperty("salesorgcode")]
        public string salesOrgCode { get; set; }
        [JsonProperty("level")]
        public string level { get; set; }
        [JsonProperty("restrictioncodes")]
        public string restrictionCodes { get; set; }
        #endregion
    }
}
