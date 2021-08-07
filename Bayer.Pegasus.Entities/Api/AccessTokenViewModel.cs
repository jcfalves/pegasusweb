using Newtonsoft.Json;
using System;
using System.Text;

namespace Bayer.Pegasus.Entities.Api
{
    public class AccessTokenViewModel
    {
        public AccessTokenViewModel()
        {

        }

        public AccessTokenViewModel(string ClientId = default(string), string ClientSecret = default(string))
        {
            this.ClientId = ClientId;
            this.ClientSecret = ClientSecret;
        }

        #region Public Properties

        [JsonProperty("clientid")]
        public string ClientId { get; set; }

        [JsonProperty("clientsecret")]
        public string ClientSecret { get; set; }

        [JsonProperty("clientHash")]
        public string ClientHash
        {
            get { return (Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + ClientSecret))); }
        }

        #endregion
    }
}
