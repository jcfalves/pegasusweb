using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Reflection;

namespace Bayer.Pegasus.Web.Controllers
{
    public class CFOPController : Base.LoggedBaseController
    {
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CFOPController));

        #region Private Read-Only Fields
        private readonly HttpClient _httpClient;
        private AccessTokenViewModel _accessToken;
        #endregion        

        #region Constructor

        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;


        public CFOPController(ILogger<CFOPController> logger, ITokenStore tokenStore, ILoginStore loginStore, IHttpContextAccessor accessor)
        {
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
            this._logger = logger;
            _httpClient = new HttpClient();
            _tokenStore = tokenStore;
            _loginStore = loginStore;
            this._accessor = accessor;
        }
        #endregion


        [Authorize]
        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JArray Results([FromBody]JObject data)
        {
            
            JArray filteredArray = new JArray();

            try
            {
                if (data != null)
                {
                    var search = data["search"].Value<String>();

                    if (!String.IsNullOrEmpty(search))
                    {

                        using (var cfopBO = new Bayer.Pegasus.Business.CFOPBO())
                        {

                            var items = cfopBO.GetCFOPs(search);


                            foreach (var item in items)
                            {
                                JObject jobject = new JObject();
                                jobject["code"] = item.Code;
                                jobject["description"] = item.Description;
                                jobject["debit"] = item.Debit;
                                jobject["credit"] = item.Credit;
                                filteredArray.Add(jobject);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                JObject jobject = new JObject();
                jobject["value"] = "erro";
                jobject["label"] = ex.ToString();
                filteredArray.Add(jobject);
            }


            return filteredArray;

        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public override async Task SignAsUserAsync(string user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,  user),
                    new Claim(ClaimTypes.Name,  user)
                };


                var IAMHelper = new Bayer.Pegasus.ApiClient.Helpers.IAMHelper();
                IAMHelper.WriteOnLog = Pegasus.Utils.Configuration.Instance.WriteOnLog;

                string ip = "127.0.0.1";

                if (_accessor != null)
                {
                    ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                }

                List<RoleModel> roles = new List<RoleModel>();
                try
                {
                    LoginSSOViewModel model = new LoginSSOViewModel();
                    model.AppId = Utils.Configuration.Instance.AppIdLogin;
                    model.Login = user;

                    _log4net.Debug($"model.AppId: {model.AppId}");
                    _log4net.Debug($"model.CultureName: {model.CultureName}");
                    _log4net.Debug($"model.IP: {model.IP}");
                    _log4net.Debug($"model.Login: {model.Login}");
                    _log4net.Debug($"model: {model}");

                    _log4net.Debug($"accessToken.ClientHash: {_accessToken.ClientHash}");

                    string token = await _tokenStore.FetchToken(_accessToken.ClientHash);
                    _log4net.Debug($"token: {token}");

                    var authenticationBO = new Bayer.Pegasus.Business.AuthenticationBO();
                    var output = await authenticationBO.LoginSSO(model, token, this._accessToken.ClientId);


                    _log4net.Debug($"output: {output}");

                    JToken jtoken = JToken.Parse(output);
                    var rolls = jtoken.SelectToken("return").SelectToken("roles").ToString();

                    _log4net.Debug($"rolls: {rolls}");
                    roles = JsonConvert.DeserializeObject<List<RoleModel>>(rolls.ToString());

                    _log4net.Debug($"roles: { roles }");

                    ReturnModel retmodel = new ReturnModel();
                    retmodel = JsonConvert.DeserializeObject<ReturnModel>(output.ToString());
                    var rl = _loginStore.Get(retmodel);

                    _log4net.Debug($"var rl: { rl }");
                }
                catch (Exception ex)
                {
                    _log4net.Error($"MethodName: {" model - Message: " + ex.Message + "   Trace: " + ex.StackTrace} ");
                }


                if (_logger != null && IAMHelper.WriteOnLog)
                {
                    if (roles == null)
                    {
                        _log4net.Debug($"AuthService: Role NULL");
                        _logger.LogInformation("AuthService: Role NULL");
                    }
                    else
                    {
                        _log4net.Debug($"AuthService: Role não está NULL");
                        _logger.LogInformation("AuthService: Role não está NULL");

                        foreach (var role in roles)
                        {
                            _log4net.Debug($"_logger.LogInformation(role.Name): { role.Name }");
                            _logger.LogInformation(role.Name);
                        }
                    }

                    if (IAMHelper.LogInformation != null)
                    {
                        foreach (var message in IAMHelper.LogInformation)
                        {
                            _log4net.Debug($"message: { message }");
                            _logger.LogInformation(message);
                        }
                    }

                }

                foreach (var role in roles)
                {
                    _log4net.Debug($"roleName: { role.Name }");
                    _log4net.Debug($"levelName: { role.Level.Name }");

                    var roleName = role.Name;
                    var levelName = role.Level.Name;

                    var claimRole = new Claim(ClaimTypes.Role, roleName);

                    if (levelName != null && role.Level.RestrictionCodes != null)
                    {
                        if (role.Level.RestrictionCodes.Count > 0)
                        {

                            claimRole.Properties["LevelName"] = levelName.ToUpper();
                            claimRole.Properties[levelName.ToUpper()] = String.Join(";", role.Level.RestrictionCodes.ToArray());

                        }
                    }

                    _log4net.Debug($"claimRole: { claimRole }");

                    claims.Add(claimRole);
                }

                _log4net.Debug($"claims: { claims }");

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                };

                var userPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.User = userPrincipal;

                _log4net.Debug($"HttpContext.SignInAsync CookieAuthenticationDefaults.AuthenticationScheme: { CookieAuthenticationDefaults.AuthenticationScheme }");
                _log4net.Debug($"HttpContext.SignInAsync userPrincipal: { userPrincipal }");
                _log4net.Debug($"HttpContext.SignInAsync authProperties: { authProperties }");

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal, authProperties);
            }
            catch (Exception ex)
            {
                _log4net.Error($"MethodName: {MethodBase.GetCurrentMethod().Name + "  Message: " + ex.Message + "   Trace: " + ex.StackTrace} ");
            }
        }
    }
}