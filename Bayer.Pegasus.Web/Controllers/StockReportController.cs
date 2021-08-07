using Bayer.Pegasus.ApiClient.Helpers;
using Bayer.Pegasus.Business;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Entities.Api;
using Bayer.Pegasus.Utils;
using CacheStrategy.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
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
    public class StockReportController : Base.LoggedBaseController
    {
        #region Private Read-Only Fields
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;
        private readonly HttpClient _httpClient;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RetroativoController));
        #endregion

        private AccessTokenViewModel _accessToken;

        public StockReportController(IHostingEnvironment hostingEnvironment, 
            ILogger<StockReportController> logger
            , IHttpContextAccessor accessor , ILoginStore loginStore, ITokenStore tokenStore)
        {
            _hostingEnvironment = hostingEnvironment;
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

            ViewData["Title"] = "Relatório de Fechamento de Estoque";

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

                var subject = "Relatório de Fechamento de Estoque";
                var body = EmailHelper.GetTemplateEmail(_hostingEnvironment.WebRootPath, "stockReport");
                
                data.Remove("PivotModal");
               

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var report = GenerateExcel(data, false, package);

                    var link = Configuration.Instance.PortalOneDomainURL + Configuration.Instance.URLPrefix + "/StockReport/Download";

                    body = body.Replace("#LINK#", link);

                    body = body.Replace("#IDENTIFIER#", report.Identifier);

                    EmailHelper.SendEmail(mailTo, subject, body, true);

                }

            }

            return Json(dataValidation.FeedBackService);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GeneratePivot([FromBody]JObject data)
        {

            if (data == null)
                return null;

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                using (var stockBO = new Bayer.Pegasus.Business.StockBO())
                {
                    var feedbackService = stockBO.ValidateStockReport(User, data);

                    if (feedbackService.Success)
                    {
                        var report = GenerateExcel(data, true, package);

                        JObject result = new JObject();

                        result["Identifier"] = report.Identifier;

                        return Json(result);

                    }
                    else {
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
                using (var stockBO = new Bayer.Pegasus.Business.StockBO())
                {
                    var feedbackService = stockBO.ValidateStockReport(User, data);

                    if (feedbackService.Success)
                    {
                        var report = GenerateExcel(data, false, package);

                        JObject result = new JObject();

                        result["Identifier"] = report.Identifier;

                        return Json(result);


                    }
                    else {
                        return Json(feedbackService);

                    }
                }       
            }
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult DownloadError() {
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
                    var fileDownloadName = "estoque.xlsx";
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileStream = new System.IO.MemoryStream(report.SerializedContent);
                    Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileDownloadName);
                    Response.Headers.Add("X-Download-Options", "noopen");
                    return File(fileStream, contentType);

                }
                else {
                    return RedirectToAction("DownloadError");
                }

            }

        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        private Entities.Report GenerateExcel(JObject data, bool pivot, OfficeOpenXml.ExcelPackage package)
        {
            
            var partners = GetListFilterValues((JArray)data["Partners"]);
            var units = GetListFilterValues((JArray)data["Units"]);
            var brands = GetListFilterValues((JArray)data["Brands"]);
            var products = GetListFilterValues((JArray)data["Products"]);
            var reportInterval = Bayer.Pegasus.Utils.DateUtils.GetIntervalDate((JObject)data["ReportDateViewModel"]);



            var worksheet = package.Workbook.Worksheets.Add("Estoque");
            worksheet.View.ShowGridLines = false;

            worksheet.Cells["A1:P1"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double;

            worksheet.Cells["A1:P1"].Style.Font.Bold = true;

            worksheet.Cells["A1"].Value = "Matriz Código SAP";
            worksheet.Cells["B1"].Value = "Matriz CNPJ";
            worksheet.Cells["C1"].Value = "Matriz Razão Social";
            worksheet.Cells["D1"].Value = "Matriz Nome Fantasia";
            worksheet.Cells["E1"].Value = "Filial Código SAP";
            worksheet.Cells["F1"].Value = "Filial CNPJ Filial";
            worksheet.Cells["G1"].Value = "Filial Nome Fantasia";
            worksheet.Cells["H1"].Value = "Código SAP Produto";
            worksheet.Cells["I1"].Value = "Produto";
            worksheet.Cells["J1"].Value = "Data";
            worksheet.Cells["K1"].Value = "Mês";
            worksheet.Cells["L1"].Value = "Quantidade Física";
            worksheet.Cells["M1"].Value = "Quantidade em AG";
            worksheet.Cells["N1"].Value = "Quantidade em Trânsito";
            worksheet.Cells["O1"].Value = "Quantidade Comprometida Venda Futura";
            worksheet.Cells["P1"].Value = "Quantidade Total de Estoque";

            var excelLine = 1;

            using (var stockBO = new Bayer.Pegasus.Business.StockBO())
            {
                List<Stock> results = stockBO.GetStockReport(User, partners, units, brands, products, reportInterval);
                foreach (var stock in results)
                {

                    excelLine++;
                    worksheet.Cells["A" + excelLine].Value = stock.Partner.ERPCode;
                    worksheet.Cells["B" + excelLine].Value = CpfCnpjUtils.FormatCNPJ(stock.Partner.Cnpj);
                    worksheet.Cells["C" + excelLine].Value = stock.Partner.CompanyName;
                    worksheet.Cells["D" + excelLine].Value = stock.Partner.TradeName;
                    worksheet.Cells["E" + excelLine].Value = stock.Unit.ERPCode;
                    worksheet.Cells["F" + excelLine].Value = CpfCnpjUtils.FormatCNPJ(stock.Unit.Cnpj);
                    worksheet.Cells["G" + excelLine].Value = stock.Unit.TradeName;
                    worksheet.Cells["H" + excelLine].Value = stock.Product.Code;
                    worksheet.Cells["I" + excelLine].Value = stock.Product.Name;
                    worksheet.Cells["J" + excelLine].Value = stock.StockDate;
                    worksheet.Cells["J" + excelLine].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells["K" + excelLine].Value = stock.StockDate.Month;
                    worksheet.Cells["K" + excelLine].Style.Numberformat.Format = "0";                    
                    worksheet.Cells["L" + excelLine].Value = stock.Quantities["H"];
                    worksheet.Cells["M" + excelLine].Value = stock.Quantities["AG"];
                    worksheet.Cells["N" + excelLine].Value = stock.Quantities["TB"];
                    worksheet.Cells["O" + excelLine].Value = stock.Quantities["VF"];
                    worksheet.Cells["P" + excelLine].Value = stock.Quantities["T"];
                }
            }

            if (pivot)
            {
                var wsPivot = package.Workbook.Worksheets.Add("Pivot-estoque");
                package.Workbook.Worksheets.MoveToStart("Pivot-estoque");
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
                pivotTable.RowHeaderCaption = "Estoque";

                foreach(var row in data["PivotModal"]["Rows"])
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
