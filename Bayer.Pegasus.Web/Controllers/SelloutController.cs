using Bayer.Pegasus.Business;
using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Web.Controllers
{

    public class SelloutController : Base.LoggedBaseController
    {
        #region Private Read-Only Fields
        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;
        private readonly HttpClient _httpClient;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RetroativoController));
        #endregion

        private AccessTokenViewModel _accessToken;

        public SelloutController(ILogger<SelloutController> logger, IHttpContextAccessor accessor,
            ILoginStore loginStore, ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
            _loginStore = loginStore;
            _httpClient = new HttpClient();
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
            this._logger = logger;
            this._accessor = accessor;
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return AuthenticationError();

            var model = GetGenericReportModel();

            ViewData["Title"] = "Sell Out";

            if (!model.SalesStructureAccess.HasSalesStructure)
            {
                return View("AccessError", model);
            }

            ReportDateBO reportDateBO = new ReportDateBO();
            List<string> listaYearMoviment = new List<string>();
            List<string> listaYearYearMoviment = new List<string>();

            var YearMoviment = reportDateBO.GetListYearMoviment().OrderByDescending(x => x.Year).ToList();
            if (YearMoviment.Count > 0)
            {
                foreach (var item in YearMoviment)
                {
                    listaYearMoviment.Add(item.Year.ToString());
                    listaYearYearMoviment.Add(item.YearToYear.ToString());
                }

                ViewData["YearMoviment"] = listaYearMoviment;
                ViewData["YearYearMoviment"] = listaYearYearMoviment;
            }

            return View(model);
        }

        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult DataChart([FromBody]JObject data)
        {

            if (data == null)
                return Json(new JArray());

            using (var selloutBO = new Bayer.Pegasus.Business.SelloutBO())
            {

                var partners = GetListFilterValues((JArray)data["Partners"]);
                var units = GetListFilterValues((JArray)data["Units"]);

                var kpis = new List<Entities.Kpis.TopKPI>();

                var intervalDataChart = data["IntervalDataChart"].Value<string>();

                var typeDataChart = data["TypeDataChart"].Value<string>();

                var feedbackService = selloutBO.ValidateTopSelloutKPI(User, data);

                if (feedbackService.Success)
                {
                    if (data["ReportDateViewModel"] != null)
                    {
                        var reportInterval = Bayer.Pegasus.Utils.DateUtils.GetIntervalDate((JObject)data["ReportDateViewModel"]);

                        kpis = selloutBO.GetTopSelloutKPI(User, partners, units, reportInterval, typeDataChart);

                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(intervalDataChart))
                        {
                            var reportInterval = Entities.ReportDateInterval.FromIntervalDataChart(intervalDataChart);

                            kpis = selloutBO.GetTopSelloutKPI(User, partners, units, reportInterval, typeDataChart);

                        }
                    }

                    return Json(kpis);
                }
                else {
                    return Json(feedbackService);
                }
               
            }
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
