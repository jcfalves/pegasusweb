using Bayer.Pegasus.ApiClient.Helpers;
using Bayer.Pegasus.Business;
using Bayer.Pegasus.Entities.Api;
using Bayer.Pegasus.Utils;
using CacheStrategy.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Bayer.Pegasus.Web.Controllers
{
    public class PegasusReportController : Base.LoggedBaseController
    {

        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RetroativoController));
        private readonly IHostingEnvironment _hostingEnvironment;

        #region Private Read-Only Fields
        private readonly HttpClient _httpClient;
        private AccessTokenViewModel _accessToken;
        #endregion        

        #region Constructor

        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;


        public PegasusReportController(IHostingEnvironment hostingEnvironment, ILogger<PegasusReportController> logger, ITokenStore tokenStore, ILoginStore loginStore, IHttpContextAccessor accessor)
        {
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
            this._logger = logger;
            _httpClient = new HttpClient();
            _tokenStore = tokenStore;
            _loginStore = loginStore;
            this._accessor = accessor;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion




        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return AuthenticationError();

            var model = GetGenericReportModel();

            ViewData["Title"] = "Relatório de Movimentos Pegasus";

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

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult SendEmail([FromBody]JObject data)
        {
            var dataValidation = new DataValidation();

            var mailTo = data["EmailTo"].Value<string>();


            if (dataValidation.ValidateEmail("EmailTo", true, mailTo, "E-mail", 0, 200))
            {

                var subject = "Relatório de Movimento Pegasus";

                data.Remove("PivotModal");

                var body = EmailHelper.GetTemplateEmail(_hostingEnvironment.WebRootPath, "pegasusReport");


                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var report = GenerateExcel(data, false, package);

                    var link = Configuration.Instance.PortalOneDomainURL + Configuration.Instance.URLPrefix + "/PegasusReport/Download";

                    body = body.Replace("#LINK#", link);

                    body = body.Replace("#IDENTIFIER#", report.Identifier);

                    EmailHelper.SendEmail(mailTo, subject, body, true);

                }




            }

            return Json(dataValidation.FeedBackService);
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        private Entities.Report GenerateExcel(JObject data, bool pivot, OfficeOpenXml.ExcelPackage package)
        {

            var partners = GetListFilterValues((JArray)data["Partners"]);
            var units = GetListFilterValues((JArray)data["Units"]);
            var clients = GetListFilterValues((JArray)data["Clients"]);
            var brands = GetListFilterValues((JArray)data["Brands"]);
            var products = GetListFilterValues((JArray)data["Products"]);
            var cities = GetListFilterValues((JArray)data["Cities"]);
            var reportInterval = Bayer.Pegasus.Utils.DateUtils.GetIntervalDate((JObject)data["ReportDateViewModel"]);

            var worksheet = package.Workbook.Worksheets.Add("Pegasus");
            worksheet.View.ShowGridLines = false;

            worksheet.Cells["A1:V1"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double;

            worksheet.Cells["A1:V1"].Style.Font.Bold = true;





            worksheet.Cells["A1"].Value = "Matriz Código SAP";
            worksheet.Cells["B1"].Value = "Matriz CNPJ";
            worksheet.Cells["C1"].Value = "Matriz Razão Social";
            worksheet.Cells["D1"].Value = "Matriz Nome Fantasia";
            worksheet.Cells["E1"].Value = "Filial Código SAP";
            worksheet.Cells["F1"].Value = "Filial CNPJ";
            worksheet.Cells["G1"].Value = "Filial Nome Fantasia";
            worksheet.Cells["H1"].Value = "Número Nota Fiscal";
            worksheet.Cells["I1"].Value = "Nome Emitente";
            worksheet.Cells["J1"].Value = "CPF/CNPJ Emitente";
            worksheet.Cells["K1"].Value = "Data Emissão Nota Fiscal";
            worksheet.Cells["L1"].Value = "CFOP";
            worksheet.Cells["M1"].Value = "Tipo de Transação";
            worksheet.Cells["N1"].Value = "CPF/CNPJ Destinatário";
            worksheet.Cells["O1"].Value = "Nome Destinatário";
            worksheet.Cells["P1"].Value = "Cidade Destinatário";
            worksheet.Cells["Q1"].Value = "UF Destinatário";
            worksheet.Cells["R1"].Value = "Código SAP Produto";
            worksheet.Cells["S1"].Value = "Produto";
            worksheet.Cells["T1"].Value = "Quantidade Comercializada";
            worksheet.Cells["U1"].Value = "Valor Unitário Comercializado";
            worksheet.Cells["V1"].Value = "Valor Total";

            var excelLine = 1;

            using (var sellOutBO = new Bayer.Pegasus.Business.SelloutBO())
            {
                List<Entities.SelloutItem> results = sellOutBO.GetPegasusReport(User, partners, units, clients, brands, products, cities, reportInterval);
                foreach (var result in results)
                {

                    excelLine++;

                    worksheet.Cells["A" + excelLine].Value = result.Partner.ERPCode;
                    worksheet.Cells["B" + excelLine].Value = CpfCnpjUtils.FormatCNPJ(result.Partner.Cnpj);
                    worksheet.Cells["C" + excelLine].Value = result.Partner.CompanyName;
                    worksheet.Cells["D" + excelLine].Value = result.Partner.TradeName;
                    worksheet.Cells["E" + excelLine].Value = result.Unit.ERPCode;
                    worksheet.Cells["F" + excelLine].Value = CpfCnpjUtils.FormatCNPJ(result.Unit.Cnpj);
                    worksheet.Cells["G" + excelLine].Value = result.Unit.TradeName;

                    if (!String.IsNullOrEmpty(result.FiscalCode))
                    {

                        long number = 0;
                        bool parse = long.TryParse(result.FiscalCode, out number);

                        if (parse)
                        {
                            worksheet.Cells["H" + excelLine].Value = number;
                        }
                        else
                        {
                            worksheet.Cells["H" + excelLine].Value = result.FiscalCode;
                        }
                    }
                    else
                    {
                        worksheet.Cells["H" + excelLine].Value = result.FiscalCode;
                    }


                    worksheet.Cells["H" + excelLine].Style.Numberformat.Format = "0";


                    worksheet.Cells["I" + excelLine].Value = result.FiscalIssuing;
                    worksheet.Cells["J" + excelLine].Value = CpfCnpjUtils.Format(result.FiscalIssuingCnpj);
                    worksheet.Cells["K" + excelLine].Value = result.FiscalDate;
                    worksheet.Cells["K" + excelLine].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells["L" + excelLine].Value = result.CFOP.ToString();
                    worksheet.Cells["M" + excelLine].Value = result.Transaction;
                    worksheet.Cells["N" + excelLine].Value = CpfCnpjUtils.Format(result.Customer.Code);
                    worksheet.Cells["O" + excelLine].Value = result.Customer.Name;
                    worksheet.Cells["P" + excelLine].Value = result.City.CityName;
                    worksheet.Cells["Q" + excelLine].Value = result.City.StateAcronym;

                    worksheet.Cells["R" + excelLine].Value = result.Product.Code;
                    worksheet.Cells["S" + excelLine].Value = result.Product.Name;

                    worksheet.Cells["T" + excelLine].Value = result.Quantity;
                    worksheet.Cells["U" + excelLine].Value = result.Values["Unit"];
                    worksheet.Cells["V" + excelLine].Value = result.Values["Total"];

                }



            }

            if (pivot)
            {
                var wsPivot = package.Workbook.Worksheets.Add("Pivot-pegasus");
                package.Workbook.Worksheets.MoveToStart("Pivot-pegasus");
                var dataRange = worksheet.Cells[worksheet.Dimension.Address.ToString()];
                var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A3"], dataRange, "Pivotname");
                pivotTable.MultipleFieldFilters = true;
                pivotTable.RowGrandTotals = true;
                pivotTable.ColumGrandTotals = true;
                pivotTable.Compact = true;
                pivotTable.CompactData = true;
                pivotTable.GridDropZones = false;
                pivotTable.Outline = false;
                pivotTable.OutlineData = false;
                pivotTable.ShowError = true;
                pivotTable.ErrorCaption = "[error]";
                pivotTable.ShowHeaders = true;
                pivotTable.UseAutoFormatting = true;
                pivotTable.ApplyWidthHeightFormats = true;
                pivotTable.ShowDrill = true;
                pivotTable.FirstDataCol = 3;
                pivotTable.RowHeaderCaption = "Sellout";

                foreach (var row in data["PivotModal"]["Rows"])
                {
                    pivotTable.RowFields.Add(pivotTable.Fields[row["name"].Value<String>()]);
                }

                foreach (var column in data["PivotModal"]["Columns"])
                {
                    pivotTable.ColumnFields.Add(pivotTable.Fields[column["name"].Value<String>()]);
                }

                foreach (var metric in data["PivotModal"]["Metrics"])
                {
                    pivotTable.DataFields.Add(pivotTable.Fields[metric["name"].Value<String>()]).Function =
                    OfficeOpenXml.Table.PivotTable.DataFieldFunctions.Sum;
                }

                foreach (var filter in data["PivotModal"]["Criteria"])
                {
                    pivotTable.PageFields.Add(pivotTable.Fields[filter["name"].Value<String>()]);
                }


            }

            Entities.Report report = new Entities.Report();
            report.Json = data.ToString();
            report.SerializedContent = package.GetAsByteArray();

            using (var reportBO = new Bayer.Pegasus.Business.ReportBO())
            {
                reportBO.SaveReport(User, report);
            }

            return report;

        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GeneratePivot([FromBody]JObject data)
        {

            if (data == null)
                return null;

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                using (var selloutBO = new Bayer.Pegasus.Business.SelloutBO())
                {
                    var feedbackService = selloutBO.ValidatePegasusReport(User, data);

                    if (feedbackService.Success)
                    {
                        var report = GenerateExcel(data, true, package);

                        JObject result = new JObject();

                        result["Identifier"] = report.Identifier;

                        return Json(result);
                    }
                    else
                    {
                        return Json(feedbackService);
                    }
                }
            }
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GenerateExcel([FromBody]JObject data)
        {
            if (data == null)
                return null;

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                using (var selloutBO = new Bayer.Pegasus.Business.SelloutBO())
                {
                    var feedbackService = selloutBO.ValidatePegasusReport(User, data);

                    if (feedbackService.Success)
                    {
                        var report = GenerateExcel(data, false, package);

                        JObject result = new JObject();

                        result["Identifier"] = report.Identifier;

                        return Json(result);
                    }
                    else
                    {
                        return Json(feedbackService);
                    }
                }
            }
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult DownloadError()
        {
            return View();
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Download(string identifier)
        {
            using (var reportBO = new Bayer.Pegasus.Business.ReportBO())
            {
                var report = reportBO.GetReport(identifier);
                if (report.SerializedContent != null)
                {
                    var fileDownloadName = "pegasus.xlsx";
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileStream = new System.IO.MemoryStream(report.SerializedContent);
                    Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileDownloadName);
                    Response.Headers.Add("X-Download-Options", "noopen");
                    return File(fileStream, contentType);
                }
                else
                {
                    return RedirectToAction("DownloadError");
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
