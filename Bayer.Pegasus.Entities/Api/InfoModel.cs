using Newtonsoft.Json;


namespace Bayer.Pegasus.Entities.Api
{
    public class InfoModel
    {
        [JsonProperty("code")]
        public int? Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("culture")]
        public string Culture { get; set; }

        [JsonProperty("clientid")]
        public string ClientId { get; set; }

        [JsonProperty("accesstoken")]
        public string AccessToken { get; set; }
    }
}
