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
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Reflection;

namespace Bayer.Pegasus.Web.Controllers
{

    public class ClientsReportController : Base.LoggedBaseController
    {

        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ClientsReportController));

        #region Private Read-Only Fields
        private readonly HttpClient _httpClient;
        private AccessTokenViewModel _accessToken;
        #endregion        

        #region Constructor

        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;


        public ClientsReportController(ILogger<ClientsReportController> logger, ITokenStore tokenStore, ILoginStore loginStore, IHttpContextAccessor accessor)
        {
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
            this._logger = logger;
            _httpClient = new HttpClient();
            _tokenStore = tokenStore;
            _loginStore = loginStore;
            this._accessor = accessor;
        }
        #endregion


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return AuthenticationError();

            var model = GetGenericReportModel();

            ViewData["Title"] = "Retenção / Perda / Aquisição de Clientes";

            if (!model.SalesStructureAccess.HasSalesStructure)
            {
                return View("AccessError", model);
            }

            if (model.SalesStructureAccess.IsPartner)
            {
                model.ShowHistory = false;
            }

            return View(model);

        }
      

        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult DataMap([FromBody]JObject data)
        {

            if (data == null)
                return Json(new JArray());

            var partners = GetListFilterValues((JArray)data["Partners"]);

            using (var clientBO = new Bayer.Pegasus.Business.ClientBO())
            {
                var feedbackService = clientBO.ValidateStatusClientsKpis(User, data);

                if (feedbackService.Success)
                {
                    var kpis = clientBO.GetStatusClientsKpisByLocation(User, partners);

                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    nfi.NumberDecimalDigits = 0;
                    nfi.NumberGroupSeparator = ".";

                    Dictionary<String, String> totals = new Dictionary<string, string>();

                    totals.Add("TotalLost", kpis.Sum(item => item.Lost).ToString("N", nfi));
                    totals.Add("TotalLoyal", kpis.Sum(item => item.Loyal).ToString("N", nfi));
                    totals.Add("TotalRetained", kpis.Sum(item => item.Retained).ToString("N", nfi));
                    totals.Add("TotalAcquired", kpis.Sum(item => item.Acquired).ToString("N", nfi));
                    totals.Add("TotalReacquired", kpis.Sum(item => item.Reacquired).ToString("N", nfi));

                    return Json(new { LocationData = kpis, TotalData = totals });
                }
                else {
                    return Json(feedbackService);
                }
            }
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public FileResult Excel(string json)
        {

            if (String.IsNullOrEmpty(json))
                return null;

            JObject data = JObject.Parse(json);

            var partners = GetListFilterValues((JArray)data["Partners"]);


            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Clientes");
                

                worksheet.View.ShowGridLines = false;

                worksheet.Cells["A1:H1"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double;

                worksheet.Cells["A1:H1"].Style.Font.Bold = true;

                worksheet.Cells["A1"].Value = "CPF/CNPJ";
                worksheet.Cells["B1"].Value = "Nome";
                worksheet.Cells["C1"].Value = "Nome Fantasia";
                worksheet.Cells["D1"].Value = "CEP";
                worksheet.Cells["E1"].Value = "Endereço";
                worksheet.Cells["F1"].Value = "UF";
                worksheet.Cells["G1"].Value = "Município";
                worksheet.Cells["H1"].Value = "Status";


                var excelLine = 1;

                using (var clientBO = new Bayer.Pegasus.Business.ClientBO())
                {
                    List<Entities.Customer> results = clientBO.GetCustomersStatus(User, partners);
                        
                    foreach (var result in results)
                    {

                        List<string> status = new List<string>();

                        if (result.Acquired)
                            status.Add("Adquiridos");

                        if (result.Reacquired)
                            status.Add("Readquiridos");

                        if (result.Retained)
                            status.Add("Retidos");

                        if (result.Loyal)
                            status.Add("Leais");
                            
                        if (result.Lost)
                            status.Add("Perdidos");



                                    
                        excelLine++;

                        worksheet.Cells["A" + excelLine].Value = result.DocumentNumber;
                        worksheet.Cells["B" + excelLine].Value = result.Name;
                        worksheet.Cells["C" + excelLine].Value = result.TradeName;
                        worksheet.Cells["D" + excelLine].Value = result.ZipCode;
                        worksheet.Cells["E" + excelLine].Value = result.Address;
                        worksheet.Cells["F" + excelLine].Value = result.City.StateAcronym;
                        worksheet.Cells["G" + excelLine].Value = result.City.CityName;
                        worksheet.Cells["H" + excelLine].Value = String.Join(" / ", status.ToArray());

                    }
                }
                
                var fileDownloadName = "client-report.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                var fileStream = new System.IO.MemoryStream(package.GetAsByteArray());


                Response.Headers.Add("Content-Disposition", "inline; filename=" + fileDownloadName);


                return File(fileStream, contentType);
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
