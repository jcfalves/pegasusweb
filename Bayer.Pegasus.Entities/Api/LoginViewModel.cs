using Newtonsoft.Json;


namespace Bayer.Pegasus.Entities.Api
{
    public class LoginViewModel : LoginSSOViewModel
    {
        #region Public Properties

        [JsonProperty("password")]
        public string Password { get; set; }

        #endregion

    }
}
