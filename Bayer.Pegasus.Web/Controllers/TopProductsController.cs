﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Bayer.Pegasus.Business;
using CacheStrategy.Stores;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Bayer.Pegasus.Entities.Api;
using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json;

namespace Bayer.Pegasus.Web.Controllers
{

    public class TopProductsController : Base.LoggedBaseController
    {

        #region Private Read-Only Fields
        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;
        private readonly HttpClient _httpClient;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RetroativoController));
        #endregion

        private AccessTokenViewModel _accessToken;

        public TopProductsController(ILogger<TopProductsController> logger, IHttpContextAccessor accessor,
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
            model.ShowLabelCFOP = true;

            ViewData["Title"] = "Top Produtos em Sell Out";

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

            using (var productBO = new Bayer.Pegasus.Business.ProductBO())
            {

                var feedbackService = productBO.ValidateTopKPI(User, data);



                if (feedbackService.Success)
                {
                    ViewData["FilterApl"] = "Parceiro:";
                    var partners = GetListFilterValues((JArray)data["Partners"]);
                    var units = GetListFilterValues((JArray)data["Units"]);
                    var clients = GetListFilterValues((JArray)data["Clients"]);

                    var numberProducts = data["NumberProducts"].Value<int>();
                    var year = data["Year"].Value<string>();
                    var typeDataChart = data["TypeDataChart"].Value<string>();

                    var intervalDataChart = data["IntervalDataChart"].Value<string>();

                    var groupBy = data["GroupBy"].Value<string>();

                    Boolean groupByBrands = false;

                    if (!String.IsNullOrEmpty(groupBy))
                    {
                        groupByBrands = true;
                    }

                    List<Entities.Kpis.TopKPI> kpis = new List<Entities.Kpis.TopKPI>();

                    if (!string.IsNullOrEmpty(year))
                    {
                        kpis = productBO.GetTopKPI(User, numberProducts, int.Parse(year), partners, units, clients, typeDataChart, groupByBrands);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(intervalDataChart))
                        {
                            Entities.ReportDateInterval reportDateInterval = null;

                            if (intervalDataChart == "Custom")
                            {
                                reportDateInterval = Bayer.Pegasus.Utils.DateUtils.GetIntervalDate((JObject)data["ReportDateViewModel"]);
                            }
                            else
                            {
                                reportDateInterval = Entities.ReportDateInterval.FromIntervalDataChart(intervalDataChart);
                            }


                            kpis = productBO.GetTopKPI(User, numberProducts, reportDateInterval, partners, units, clients, typeDataChart, groupByBrands);

                        }
                    }

                    return Json(kpis);
                }
                else
                {
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
