using Newtonsoft.Json;


namespace Bayer.Pegasus.Entities.Api
{
    public class LoginSSOViewModel
    {
        public LoginSSOViewModel()
        {

        }

        public LoginSSOViewModel(string AppId = default(string))
        {
            this.AppId = AppId;
        }

        #region Private Fields

        private string _ip;
        private string _culture;

        #endregion

        #region Public Properties

        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("ip")]
        public string IP
        {
            get { return (string.IsNullOrEmpty(_ip) ? "::1" : _ip); }
            set { _ip = value; }
        }

        [JsonProperty("cultureName")]
        public string CultureName
        {
            get { return (string.IsNullOrEmpty(_culture) ? "en-us" : _culture); }
            set { _culture = value; }
        }

        #endregion
    }
}
