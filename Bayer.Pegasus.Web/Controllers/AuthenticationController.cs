using Bayer.Pegasus.ApiClient.Helpers;
using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace Bayer.Pegasus.Web.Controllers
{

    [Route("authentication")]
    public sealed class AuthenticationController : Controller
    {
        #region Private Read-Only Fields
        private readonly HttpClient _httpClient;
        private AccessTokenViewModel _accessToken;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RetroativoController));
        #endregion        

        #region Constructor

        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;

        public AuthenticationController(ITokenStore tokenStore, ILoginStore loginStore)
        {
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
            _httpClient = new HttpClient();
            _tokenStore = tokenStore;
            _loginStore = loginStore;
        }
        #endregion

        #region Controller Actions
        [AllowAnonymous]
        [HttpPost, Route("login")]
        [Consumes("application/json")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            try
            {
                _loginStore.Remove();
                _tokenStore.Remove();

                model.AppId = Utils.Configuration.Instance.AppIdLogin;

                string token = await _tokenStore.FetchToken(_accessToken.ClientHash);
                var authenticationBO = new Bayer.Pegasus.Business.AuthenticationBO();
                var output = await authenticationBO.GetLogin(model, token, this._accessToken.ClientId);

                return Json(JsonConvert.DeserializeObject(output));
            }
            catch (Exception e)
            {
                var msg = e.Message;
                //return Json("Erro: Login");
                return null;
            }
        }


        [AllowAnonymous]
        [HttpPost, Route("loginSSO")]
        [Consumes("application/json")]
        public async Task<IActionResult> LoginSSO([FromBody]LoginSSOViewModel model)
        {
            try
            {
                _log4net.Debug($"AuthenticationController.LoginSSO (Início)");

                _loginStore.Remove();
                _tokenStore.Remove();

                model.AppId = Utils.Configuration.Instance.AppIdLogin;

                _log4net.Debug($"AuthenticationController.LoginSSO - Obtendo o Token na _tokenStore.FetchToken (Início)");

                string token = await _tokenStore.FetchToken(_accessToken.ClientHash);
                _log4net.Debug($"AuthenticationController.LoginSSO - token: {token}");

                _log4net.Debug($"AuthenticationController.LoginSSO - Obtendo o Token na _tokenStore.FetchToken (Fim)");

                var authenticationBO = new Bayer.Pegasus.Business.AuthenticationBO();
                var output = await authenticationBO.LoginSSO(model, token, this._accessToken.ClientId);

                LoginInfoModel retu = JsonConvert.DeserializeObject<LoginInfoModel>(output);
                _loginStore.Get(retu.Return);

                _log4net.Debug($"AuthenticationController.LoginSSO (Fim)");

                return Json(JsonConvert.DeserializeObject(output));
            }
            catch (Exception e)
            {
                _log4net.Error($"AuthenticationController.LoginSSO - Exception: {e.Message} - InnerException: {e.InnerException} - Trace: {e.StackTrace}");
                EmailHelper.SendErrorEmail($"Erro na AuthenticationController.LoginSSO", $"Exception: {e.Message} - InnerException: {e.InnerException} - StackTrace: {e.StackTrace}");
                var msg = e.Message;
                //return Json("Erro: LoginSSO");
                return null;
            }
        }


        [AllowAnonymous]
        [HttpPost, Route("logout")]
        [Consumes("application/json")]
        public async Task<IActionResult> Logout([FromBody]LogoutViewModel model)
        {
            try
            {
                _loginStore.Remove();

                model.AppId = Utils.Configuration.Instance.AppIdLogin;

                string token = await _tokenStore.FetchToken(_accessToken.ClientHash);
                var authenticationBO = new Bayer.Pegasus.Business.AuthenticationBO();
                var output = await authenticationBO.Logout(model, token, this._accessToken.ClientId);

                return Json(JsonConvert.DeserializeObject(output));
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                //return Json("Erro: Logout");
                return null;
            }
        }
        #endregion
    }
}