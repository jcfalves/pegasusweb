using Newtonsoft.Json;


namespace Bayer.Pegasus.Entities.Api
{
    public class LoginInfoModel
    {
        [JsonProperty("result")]
        public InfoModel Result { get; set; }

        [JsonProperty("return")]
        public ReturnModel Return { get; set; }
    }
}
