using Newtonsoft.Json;

namespace Bayer.Pegasus.Entities.Api
{
    public sealed class TokenViewModel
    {
        #region Public Properties

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        #endregion
    }
}
