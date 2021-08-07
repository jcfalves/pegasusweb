using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bayer.Pegasus.Business;
using Bayer.Pegasus.Entities;
using Bayer.Pegasus.Entities.Accera;
using Bayer.Pegasus.Entities.Api;
using Bayer.Pegasus.Utils;
using Bayer.Pegasus.Web.Helpers;
using CacheStrategy.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Reflection;

namespace Bayer.Pegasus.Web.Controllers
{
    public class MonitorController : Base.LoggedBaseController
    {
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RetroativoController));

        #region Private Read-Only Fields
        private readonly HttpClient _httpClient;
        private AccessTokenViewModel _accessToken;
        #endregion        

        #region Constructor

        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;


        public MonitorController(ILogger<MonitorController> logger, ITokenStore tokenStore, ILoginStore loginStore, IHttpContextAccessor accessor)
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
            model.ShowHistory = false;
            model.ShowFilter = false;

            ViewData["Title"] = "Monitoramento de Interfaces";

            if (!model.SalesStructureAccess.HasSalesStructure)
            {
                return View("AccessError", model);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult UpdateStep()
        {
            MonitorBO monitorBO = new MonitorBO();
            ETLServiceBO etlServiceBO = new ETLServiceBO();
            List<IntegrationProcess> integrationProcessList = etlServiceBO.GetIntegrationProcesses();
            List<IntegrationProcess> integrationProcessListAux = new List<IntegrationProcess>();

            foreach (IntegrationProcess integration in integrationProcessList)
            {
                integration.Steps = monitorBO.GetSteps(integration, null, null, string.Empty, string.Empty);
                if (integration.Steps.Count > 0)
                    integrationProcessListAux.Add(integration);
            }

            FeedbackService feedbackService = new FeedbackService();
            JObject result = new JObject();

            result["integrationList"] = JsonConvert.SerializeObject(integrationProcessListAux);
            result["LastProc"] = JsonConvert.SerializeObject(DateTime.Now.AddHours(1).ToString("dd/MM/yyyy HH:mm:ss"));

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult UpdateStepFilter([FromBody]JObject data)
        {
            MonitorBO monitorBO = new MonitorBO();
            List<IntegrationProcess> integrationProcessList = new List<IntegrationProcess>();
            IntegrationProcess integrationProcess = new IntegrationProcess();
            FeedbackService feedbackService = new FeedbackService();
            JObject result = new JObject();

            var integracaoResult = data.ToObject<Entities.IngracaoResult>();

            DateTime dateIni;
            DateTime dateEnd;
            DateTime dateRerenceMax = DateTime.Now;
            DateTime dateRerenceMin = DateTime.Parse("01/01/1980");

            if (!DateTime.TryParse(integracaoResult.PeriodIni, out dateIni))
            {
                feedbackService.Success = false;
                string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                feedbackService.Errors.Add(msgerro);

                return Json(feedbackService);
            }

            if (!DateTime.TryParse(integracaoResult.PeriodEnd, out dateEnd))
            {
                feedbackService.Success = false;
                string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                feedbackService.Errors.Add(msgerro);

                return Json(feedbackService);
            }

            if (dateIni < dateRerenceMin || dateIni > dateRerenceMax)
            {
                feedbackService.Success = false;
                string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                feedbackService.Errors.Add(msgerro);

                return Json(feedbackService);
            }

            if (dateEnd < dateRerenceMin || dateEnd > dateRerenceMax)
            {
                feedbackService.Success = false;
                string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                feedbackService.Errors.Add(msgerro);

                return Json(feedbackService);
            }

            if (dateIni > dateEnd)
            {
                feedbackService.Success = false;
                string msgerro = "A data de Final deve ser maior que a data Incial";
                feedbackService.Errors.Add(msgerro);

                return Json(feedbackService);
            }

            if (integracaoResult.Fl_Tipo_Execucao_A == "True" && integracaoResult.Fl_Tipo_Execucao_M == "True")
                integracaoResult.Fl_Tipo_Execucao = "T";
            else if (integracaoResult.Fl_Tipo_Execucao_A == "True")
                integracaoResult.Fl_Tipo_Execucao = "A";
            else if (integracaoResult.Fl_Tipo_Execucao_M == "True")
                integracaoResult.Fl_Tipo_Execucao = "M";

            integrationProcess.Code = integracaoResult.Cd_Integracao;
            integrationProcess.Flow = integracaoResult.Flow;

            integrationProcess.Steps = monitorBO.GetSteps(integrationProcess, dateIni,
                                                    dateEnd, integracaoResult.Fl_Tipo_Execucao, integracaoResult.Fl_Situacao);

            integrationProcessList.Add(integrationProcess);

            

            result["integrationList"] = JsonConvert.SerializeObject(integrationProcessList);
            result["LastProc"] = JsonConvert.SerializeObject(DateTime.Now.AddHours(1).ToString("dd/MM/yyyy HH:mm:ss"));

            return Json(result);
        }

        [HttpGet]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GetIntegrationProcesses()
        {
            ETLServiceBO eTLServiceBO = new ETLServiceBO();

            FeedbackService feedbackService = new FeedbackService();
            JObject result = new JObject();

            var services = eTLServiceBO.GetIntegrationProcesses();

            result["result"] = JsonConvert.SerializeObject(services);
            return Json(result);
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GetLogProcessamento([FromBody]JObject data)
        {
            MonitorBO monitorBO = new MonitorBO();
            FeedbackService feedbackService = new FeedbackService();
            var idProc = data.GetValue("ID").ToString();
            var fase = data.GetValue("FASE").ToString();
            try
            {
                List<LogResult> logResult = monitorBO.GetLogByProcess(int.Parse(idProc), int.Parse(fase));

                JObject result = new JObject();
                result["log"] = JsonConvert.SerializeObject(logResult);
                return Json(result);
            }
            catch (Exception ex)
            {
                feedbackService.Success = false;
                feedbackService.Errors.Add(ex.Message);
                return Json(feedbackService);
            }
        }

        [HttpGet]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GetIntegrationProcessesManually()
        {
            ETLServiceBO eTLServiceBO = new ETLServiceBO();

            FeedbackService feedbackService = new FeedbackService();
            JObject result = new JObject();

            var services = eTLServiceBO.GetIntegrationProcesses().Where(x => x.CanExecuteManually == "S");

            result["result"] = JsonConvert.SerializeObject(services);
            return Json(result);
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult SaveProcessesApi([FromBody]JObject data)
        {
            var integracaoResult = data.ToObject<Entities.IntegrationProcess>();
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();

            if (integracaoResult.Code!=5)
            {
                DateTime dateIni;
                DateTime dateEnd;
                DateTime dateRerenceMax = DateTime.Now;
                DateTime dateRerenceMin = DateTime.Parse("01/01/1980");

                if (!DateTime.TryParse(integracaoResult.DtPeriodIni, out dateIni))
                {
                    feedbackService.Success = false;
                    string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }

                if (!DateTime.TryParse(integracaoResult.DtPeriodEnd, out dateEnd))
                {
                    feedbackService.Success = false;
                    string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }

                if (dateIni < dateRerenceMin || dateIni > dateRerenceMax)
                {
                    feedbackService.Success = false;
                    string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }

                if (dateEnd < dateRerenceMin || dateEnd > dateRerenceMax)
                {
                    feedbackService.Success = false;
                    string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }

                if (dateIni > dateEnd)
                {
                    feedbackService.Success = false;
                    string msgerro = "A data de Final deve ser maior que a data Incial";
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }
            }

            var integracaoProcesses = eTLServiceBO.GetIntegrationProcesses().Where(x => x.CanExecuteManually == "S" &&
                                                                                   x.Code == integracaoResult.Code &&
                                                                                   x.SourceCode == integracaoResult.SourceCode);

            var json_defaultParameter = (from parameter in integracaoProcesses
                                         select new { DefaultParameter = parameter.DefaultParameter }).ToList()[0].DefaultParameter;

            string json_convert_result = string.Empty;

            if (integracaoResult.Code != 5)
            {
                DatasourceAccera results = JsonConvert.DeserializeObject<DatasourceAccera>(json_defaultParameter);

                results.filters = new List<FiltersAccera>();

                if (integracaoResult.DtPeriodIni != null)
                {
                    var filters = new FiltersAccera();
                    filters.field_name = "vendas.reg_data_inclusao";
                    filters.field_type = "date";
                    filters.filter_type = ">=";
                    var filtersvalues = new FiltersAcerraValues();
                    filtersvalues.value = Convert.ToDateTime(integracaoResult.DtPeriodIni.ToString()).ToString("yyyy-MM-dd");
                    filters.values = new List<FiltersAcerraValues>();
                    filters.values.Add(filtersvalues);
                    results.filters.Add(filters);
                }
                if (integracaoResult.DtPeriodEnd != null)
                {
                    var filters = new FiltersAccera();
                    filters.field_name = "vendas.reg_data_inclusao";
                    filters.field_type = "date";
                    filters.filter_type = "<=";
                    var filtersvalues = new FiltersAcerraValues();
                    filtersvalues.value = Convert.ToDateTime(integracaoResult.DtPeriodEnd.ToString()).ToString("yyyy-MM-dd");
                    filters.values = new List<FiltersAcerraValues>();
                    filters.values.Add(filtersvalues);
                    results.filters.Add(filters);
                }

                if (!string.IsNullOrEmpty(integracaoResult.NumCnpjDistr))
                {
                    var filters = new FiltersAccera();
                    filters.field_name = "cds.cnpj";
                    filters.field_type = "text";
                    filters.filter_type = "=";
                    var filtersvalues = new FiltersAcerraValues();
                    filtersvalues.value = integracaoResult.NumCnpjDistr;
                    filters.values = new List<FiltersAcerraValues>();
                    filters.values.Add(filtersvalues);
                    results.filters.Add(filters);
                }

                if (!string.IsNullOrEmpty(integracaoResult.NumeroNotaFiscal))
                {
                    var filters = new FiltersAccera();
                    filters.field_name = "vendas.identificador_transacao";
                    filters.field_type = "text";
                    filters.filter_type = "=";
                    var filtersvalues = new FiltersAcerraValues();
                    filtersvalues.value = integracaoResult.NumeroNotaFiscal;
                    filters.values = new List<FiltersAcerraValues>();
                    filters.values.Add(filtersvalues);
                    results.filters.Add(filters);
                }

                json_convert_result = JsonConvert.SerializeObject(results);

            }
            else if (integracaoResult.Code == 5)
            {
                DatasourceAccera results = JsonConvert.DeserializeObject<DatasourceAccera>(json_defaultParameter);
                results.filters = new List<FiltersAccera>();

                if (integracaoResult.DtPeriodIni != null)
                {
                    var filters = new FiltersAccera();
                    filters.field_name = "ultimo_estoque.data_fechamento";
                    filters.field_type = "date";
                    filters.filter_type = ">=";
                    var filtersvalues = new FiltersAcerraValues();
                    filtersvalues.value = integracaoResult.DtPeriodIni + "-01";
                    filters.values = new List<FiltersAcerraValues>();
                    filters.values.Add(filtersvalues);
                    results.filters.Add(filters);

                    //gerando a data fim
                    DateTime dt = Convert.ToDateTime(integracaoResult.DtPeriodIni + "-01");
                    dt = dt.AddMonths(1);
                    var filters1 = new FiltersAccera();
                    filters1.field_name = "ultimo_estoque.data_fechamento";
                    filters1.field_type = "date";
                    filters1.filter_type = "<";
                    var filtersvalues1 = new FiltersAcerraValues();
                    filtersvalues1.value = dt.ToString("yyyy-MM-dd") ;
                    filters1.values = new List<FiltersAcerraValues>();
                    filters1.values.Add(filtersvalues1);
                    results.filters.Add(filters1);
                }

                json_convert_result = JsonConvert.SerializeObject(results);
            }
            else
            {
                json_convert_result = json_defaultParameter;
            }

            ProcessItem processItem = new ProcessItem();
            ETLServiceBO etlServiceBO = new ETLServiceBO();

            processItem.Created = DateTime.Now;
            processItem.ExecutionType = "M";
            processItem.IntegrationProcessCode = integracaoResult.Code;
            processItem.InputParameter = json_convert_result;
            processItem.StatusCode = "P"; //Pendente

            long codeReturn = etlServiceBO.CreateProcessItem(processItem, DateTime.Now, User);

            JObject result = new JObject();
            //List<Entities.Monitor> steps = monitorBO.GetSteps(integracaoResult);
            //result["result"] = JsonConvert.SerializeObject(steps);


            if (codeReturn > 0)
            {
                feedbackService.Success = true;
                string msgerro = string.Concat("Gravado com Sucesso!");
                return Json(feedbackService);
            }
            else
            {
                feedbackService.Success = false;
                string msgerro = string.Concat("Erro ao Gravar!");
                feedbackService.Errors.Add(msgerro);
                return Json(feedbackService);
            }
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult SaveProcessesFTP([FromBody]JObject data)
        {
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            MonitorBO monitorBO = new MonitorBO();
            FeedbackService feedbackService = new FeedbackService();
            JObject result = new JObject();

            try
            {
                //int option = int.Parse(data["option"].ToString());
                int cdIntegracao = Convert.ToInt32(data["Cd_Integracao"].ToString());

                var integracaoProcesses = eTLServiceBO.GetIntegrationProcesses().Where(x => x.Code == cdIntegracao).FirstOrDefault();

                //if (option == 1)
               // {
                    ProcessItem processItem = new ProcessItem();
                    ETLServiceBO etlServiceBO = new ETLServiceBO();

                    processItem.Created = DateTime.Now;
                    processItem.ExecutionType = "M";
                    processItem.IntegrationProcessCode = integracaoProcesses.Code;
                    processItem.InputParameter = integracaoProcesses.DefaultParameter;
                    processItem.StatusCode = "P"; //Pendente

                    etlServiceBO.CreateProcessItem(processItem, DateTime.Now, User);
                //}
                /*
                if (option == 2)
                {
                    if (TempData["UploadSucessFTP"] is null || (bool)TempData["UploadSucessFTP"] == false)
                    {
                        feedbackService.Success = false;
                        string msgerro = "Por favor, realize o carregamento do arquivo!";
                        feedbackService.Errors.Add(msgerro);

                        return Json(feedbackService);
                    }

                    ProcessItem processItem = new ProcessItem();
                    ETLServiceBO etlServiceBO = new ETLServiceBO();

                    processItem.Created = DateTime.Now;
                    processItem.ExecutionType = "M";
                    processItem.IntegrationProcessCode = integracaoProcesses.Code;
                    processItem.InputParameter = "{\"source\":\"local\"}";
                    processItem.StatusCode = "P"; //Pendente

                    long codeProcess = etlServiceBO.CreateProcessItem(processItem, DateTime.Now, User);

                    ProcessItemLog processItemLog = new ProcessItemLog();

                    processItemLog.Created = DateTime.Now;
                    processItemLog.ProcessItemId = codeProcess;
                    processItemLog.Description = "Fase iniciada em " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss");
                    processItemLog.LogType = "S";
                    processItemLog.StageCode = 1;

                    etlServiceBO.AddProcessItemLog(processItemLog);

                    ServiceParameter serviceParameter = etlServiceBO.GetParameters().Where(x => x.Key == "TEMP_FLD").FirstOrDefault().Value;

                    var sourcePath = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot", "upload");

                    string fileName = data["fileName"].ToString();
                    string targetPath = serviceParameter.Value;

                    // Use Path class to manipulate file and directory paths.
                    string sourceFile = Path.Combine(sourcePath, fileName);
                    string destFile = Path.Combine(targetPath, fileName);

                    // To copy a folder's contents to a new location:
                    // Create a new target folder, if necessary.
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }

                    // To copy a file to another location and 
                    // overwrite the destination file if it already exists.
                    System.IO.File.Copy(sourceFile, destFile, true);

                    processItemLog = new ProcessItemLog();

                    processItemLog.Created = DateTime.Now;
                    processItemLog.ProcessItemId = codeProcess;
                    processItemLog.Description = "Fase finalizada em " + DateTime.Now.ToString("dd/MM/yy hh:mm:ss");
                    processItemLog.LogType = "F";
                    processItemLog.StageCode = 1;

                    etlServiceBO.AddProcessItemLog(processItemLog);
                    

                }
                */

            }
            catch (Exception ex)
            {
                feedbackService.Success = false;
                feedbackService.Errors.Add(ex.Message);
                return Json(feedbackService);
            }
            return Json(result);
        }

        [HttpPost]
        [Authorize]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadStreamingFile()
        {
            var fileName = HttpContext.Request.Headers["X-FileName-Header"].ToString();
            
            // full path to file save in location
            var filePath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot", "upload", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Request.StreamFile(stream);
            }

            TempData["UploadSucessFTP"] = true;

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok();
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