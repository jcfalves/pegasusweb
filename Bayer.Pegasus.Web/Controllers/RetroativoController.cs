using Bayer.Pegasus.ApiClient.Helpers;
using Bayer.Pegasus.Business;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Entities.Api;
using Bayer.Pegasus.Utils;
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
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Web.Controllers
{
    public class RetroativoController : Base.LoggedBaseController
    {

        #region Private Read-Only Fields
        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;
        private readonly HttpClient _httpClient;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RetroativoController));
        #endregion

        private AccessTokenViewModel _accessToken;

        #region Constructor
        public RetroativoController(ILogger<RetroativoController> logger, ITokenStore tokenStore,
                                    ILoginStore loginStore, IHttpContextAccessor accessor)
        {
            _log4net.Debug($"RetroativoController - Construtor (Início)");

            _tokenStore = tokenStore;
            _loginStore = loginStore;
            _httpClient = new HttpClient();
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
            this._logger = logger;
            this._accessor = accessor;

            _log4net.Debug($"RetroativoController - Construtor (Fim)");
        }
        #endregion


        public IActionResult Index()
        {
            try
            {
                _log4net.Debug($"RetroativoController.Index (Início)");

                if (!User.Identity.IsAuthenticated)
                    return AuthenticationError();

                var model = GetGenericReportModel();
                model.ShowHistory = false;
                model.ShowFilter = false;

                ViewData["Title"] = "Arquivo retroativo";

                if (!model.SalesStructureAccess.HasSalesStructure)
                {
                    return View("AccessError", model);
                }

                _log4net.Debug($"RetroativoController.Index (Fim)");

                return View(model);
            }
            catch (Exception ex)
            {
                _log4net.Error($"RetroativoController.Index - Exception: {ex.Message} - InnerException: {ex.InnerException} - Trace: {ex.StackTrace}");
                EmailHelper.SendErrorEmail($"Erro na RetroativoController.Index", $"Exception: {ex.Message} - InnerException: {ex.InnerException} - StackTrace: {ex.StackTrace}");
                return BadRequest();
            }
        }


        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GetListArquivosRetroativos([FromBody]JObject status)
        {
            var search = status["status"].Value<String>();

            JObject result = new JObject();
            RetroativoBO retroativoBO = new RetroativoBO();
            FeedbackService feedbackService = new FeedbackService();
            try
            {
                var listRetroativosBO = retroativoBO.GetListArquivosRetroativos(search);
                result["dataTable"] = JsonConvert.SerializeObject(listRetroativosBO);
                return Json(result);
            }
            catch (Exception ex)
            {
                feedbackService.Success = false;
                string msgerro = string.Concat("Ocorreu um erro! \n", ex.Message);
                feedbackService.Errors.Add(msgerro);
                return Json(feedbackService);
            }
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GenerateExcel([FromBody]JObject status)
        {
            var search = status["status"].Value<String>();

            JObject result = new JObject();
            RetroativoBO retroativoBO = new RetroativoBO();
            FeedbackService feedbackService = new FeedbackService();
            try
            {
                var listRetroativosBO = retroativoBO.GetListArquivosRetroativos(search);
                result["Identifier"] = GenerateExcell(listRetroativosBO);
                return Json(result);
            }
            catch (Exception ex)
            {
                feedbackService.Success = false;
                string msgerro = string.Concat("Ocorreu um erro! \n", ex.Message);
                feedbackService.Errors.Add(msgerro);
                return Json(feedbackService);
            }
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public string GenerateExcell(List<Entities.Retroativo> data)
        {
            if (data == null)
                return null;

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var report = GenerateExcelll(data, package);

                return report.Identifier;
            }
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        private Entities.Report GenerateExcelll(List<Entities.Retroativo> data, OfficeOpenXml.ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets.Add("Erros");
            worksheet.View.ShowGridLines = false;

            worksheet.Cells["A1:V1"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double;

            worksheet.Cells["A1:V1"].Style.Font.Bold = true;

            worksheet.Cells["A1"].Value = "IDArquivo";
            worksheet.Cells["B1"].Value = "Arquivo";
            worksheet.Cells["C1"].Value = "Status";
            worksheet.Cells["D1"].Value = "Ação";
            worksheet.Cells["E1"].Value = "Data da Ação";

            var excelLine = 1;

            foreach (var result in data)
            {
                excelLine++;
                worksheet.Cells["A" + excelLine].Value = result.idArquivoretroativo;
                worksheet.Cells["B" + excelLine].Value = result.dsNome;
                worksheet.Cells["C" + excelLine].Value = result.dsStatus;
                worksheet.Cells["D" + excelLine].Value = result.dsAcao;
                worksheet.Cells["E" + excelLine].Value = result.dtAcao;
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
        public ActionResult Download(string identifier)
        {
            using (var reportBO = new Bayer.Pegasus.Business.ReportBO())
            {
                var report = reportBO.GetReport(identifier);

                if (report.SerializedContent == null)
                    return RedirectToAction("DownloadError");

                var fileDownloadName = "retroativo.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileStream = new System.IO.MemoryStream(report.SerializedContent);
                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileDownloadName);
                Response.Headers.Add("X-Download-Options", "noopen");
                return File(fileStream, contentType);

            }
        }


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
            catch (Exception ex)
            {
                JObject jobject = new JObject();
                jobject["value"] = "erro";
                jobject["label"] = ex.ToString();
                filteredArray.Add(jobject);
            }


            return filteredArray;

        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult Save([FromBody]JObject data)
        {
            FeedbackService feedbackService = new FeedbackService();
            try
            {
                var retroativo = data.ToObject<Entities.Retroativo>();
                RetroativoBO retroativoBO = new RetroativoBO();

                if (retroativo.idAcao > 0)
                {
                    try
                    {
                        retroativoBO.SaveArquivoRetroativo(retroativo, User);

                        feedbackService.Success = true;
                        string msgerro = string.Concat("Gravado com Sucesso!");
                        return Json(feedbackService);
                    }
                    catch (Exception ex)
                    {
                        new Exception(ex.Message);

                        feedbackService.Success = false;
                        string msgerro = string.Concat("Erro ao Gravar!");
                        feedbackService.Errors.Add(msgerro);
                        return Json(feedbackService);
                    }
                }
                else
                {
                    feedbackService.Success = false;
                    string msgerro = string.Concat("Refaça a operação.");
                    feedbackService.Errors.Add(msgerro);
                    return Json(feedbackService);
                }
            }
            catch (Exception ex)
            {
                feedbackService.Success = false;
                string msgerro = string.Concat("Ocorreu um erro no processamento.");
                feedbackService.Errors.Add(ex);
                return Json(feedbackService);

            }

        }


        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult AcaoArquivos([FromBody]JObject data)
        {
            FeedbackService feedbackService = new FeedbackService();
            try
            {
                RetroativoBO retroativoBO = new RetroativoBO();

                AcaoArquivos acaoArquivos = data.ToObject<AcaoArquivos>();
                Retroativo retroativo = new Retroativo();

                retroativo.idAcao = acaoArquivos.IdAcao;

                if (retroativo.idAcao > 0)
                {
                    try
                    {
                        foreach (int idArquivoretroativo in acaoArquivos.ArrIdArquivos)
                        {
                            retroativo.idArquivoretroativo = idArquivoretroativo;
                            retroativoBO.SaveArquivoRetroativo(retroativo, User);
                        }
                        feedbackService.Success = true;
                        string msgerro = string.Concat("Gravado com Sucesso!");
                        return Json(feedbackService);
                    }
                    catch (Exception ex)
                    {
                        new Exception(ex.Message);

                        feedbackService.Success = false;
                        string msgerro = string.Concat("Erro ao Gravar!");
                        feedbackService.Errors.Add(msgerro);
                        return Json(feedbackService);
                    }

                }
                else
                {
                    feedbackService.Success = false;
                    string msgerro = string.Concat("Refaça a operação.");
                    feedbackService.Errors.Add(msgerro);
                    return Json(feedbackService);
                }
            }
            catch (Exception ex)
            {
                feedbackService.Success = false;
                string msgerro = string.Concat("Ocorreu um erro no processamento.");
                feedbackService.Errors.Add(ex);
                return Json(feedbackService);
            }
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public override async Task SignAsUserAsync(string user)
        {
            try
            {
                _log4net.Debug($"RetroativoController.SignAsUserAsync (Início) - 3.1");
                _log4net.Debug($"user: {user}");


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
                    _log4net.Debug($"RetroativoController.SignAsUserAsync - Obtendo as Roles (Início)");


                    LoginSSOViewModel model = new LoginSSOViewModel();
                    model.AppId = Utils.Configuration.Instance.AppIdLogin;
                    model.Login = user;

                    _log4net.Debug($"RetroativoController.SignAsUserAsync - Obtendo o Token na _tokenStore.FetchToken (Início)");

                    string token = await _tokenStore.FetchToken(_accessToken.ClientHash);
                    _log4net.Debug($"RetroativoController.SignAsUserAsync - token: {token}");

                    _log4net.Debug($"RetroativoController.SignAsUserAsync - Obtendo o Token na _tokenStore.FetchToken  (Fim)");


                    _log4net.Debug($"RetroativoController.SignAsUserAsync - Autenticando o usuário na authenticationBO.LoginSSO (Início)");

                    var authenticationBO = new Bayer.Pegasus.Business.AuthenticationBO();
                    var output = await authenticationBO.LoginSSO(model, token, this._accessToken.ClientId);

                    _log4net.Debug($"output da authenticationBO.LoginSSO: {output}");

                    _log4net.Debug($"RetroativoController.SignAsUserAsync - Autenticando o usuário na authenticationBO.LoginSSO (Fim)");


                    _log4net.Debug($"RetroativoController.SignAsUserAsync - obtendo as roles do output (Início)");

                    JToken jtoken = JToken.Parse(output);
                    var rolesToken = jtoken.SelectToken("return").SelectToken("roles").ToString();


                    _log4net.Debug($"rolesToken: {rolesToken}");
                    roles = JsonConvert.DeserializeObject<List<RoleModel>>(rolesToken.ToString());

                    _log4net.Debug($"RetroativoController.SignAsUserAsync - obtendo as roles do output (Fim)");

                    _log4net.Debug($"RetroativoController.SignAsUserAsync - Obtendo as Roles (Fim)");

                    ReturnModel retmodel = new ReturnModel();
                    retmodel = JsonConvert.DeserializeObject<ReturnModel>(output.ToString());
                    var rl = _loginStore.Get(retmodel);

                    //--------------------------------------------------------------
                    if (Configuration.Instance.UseTokenLifeTime)
                    {
                        _log4net.Debug($"Token: {token}");

                        string mailTo = "roberto.lima1.ext@bayer.com; jose.alves3.ext@bayer.com; cinthia.vicentini.ext@bayer.com; marcelo.fonseca@bayer.com;";
                        string subject = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - Token: " + token;
                        string texto = "";
                        EmailHelper.SendEmail(mailTo, subject, texto, true);
                    }
                    //--------------------------------------------------------------
                }
                catch (Exception ex)
                {
                    _log4net.Error($"RetroativoController.SignAsUserAsync - Obtendo as Roles - Exception: {ex.Message} - InnerException: {ex.InnerException} - Trace: {ex.StackTrace}");
                    EmailHelper.SendErrorEmail($"Erro na RetroativoController.SignAsUserAsync - Obtendo as Roles", $"Exception: {ex.Message} - InnerException: {ex.InnerException} - StackTrace: {ex.StackTrace}");
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
                            _log4net.Debug($"role.Name: { role.Name }");
                            _logger.LogInformation(role.Name);
                        }
                    }


                    if (IAMHelper.LogInformation != null)
                    {
                        _log4net.Debug("IAMHelper.LogInformation != null");

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

                            //----------------------------------------
                            _log4net.Debug($"levelName.ToUpper(): {levelName.ToUpper()}");
                            _log4net.Debug($"role.Level.RestrictionCodes.ToArray(): {role.Level.RestrictionCodes.ToArray()}");
                            //----------------------------------------

                            claimRole.Properties["LevelName"] = levelName.ToUpper();
                            claimRole.Properties[levelName.ToUpper()] = String.Join(";", role.Level.RestrictionCodes.ToArray());

                        }
                    }

                    //----------------------------------------
                    _log4net.Debug($"claimRole: {claimRole}");
                    //----------------------------------------

                    claims.Add(claimRole);
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                };

                var userPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.User = userPrincipal;

                _log4net.Debug($"RetroativoController.SignAsUserAsync (Fim) - 3.1");

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal, authProperties);
            }
            catch (Exception ex)
            {
                _log4net.Error($"RetroativoController.SignAsUserAsync - 3.1 - Exception: {ex.Message} - InnerException: {ex.InnerException} - StackTrace: {ex.StackTrace}");
                EmailHelper.SendErrorEmail($"Erro na RetroativoController.SignAsUserAsync - 3.1", $"Exception: {ex.Message} - InnerException: {ex.InnerException} - StackTrace: {ex.StackTrace}");
            }
        }
    }

    public class AcaoArquivos
    {
        public int IdAcao { get; set; }
        public int[] ArrIdArquivos { get; set; }
    }

}