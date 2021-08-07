using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Bayer.Pegasus.Business;
using Bayer.Pegasus.Entities;
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
using OfficeOpenXml;

namespace Bayer.Pegasus.Web.Controllers
{
    public class StockTransitController : Base.LoggedBaseController
    {
        #region Private Read-Only Fields
        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;
        private readonly HttpClient _httpClient;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RetroativoController));
        #endregion

        private AccessTokenViewModel _accessToken;

        public StockTransitController(ILogger<StockEvolutionController> logger, 
            IHttpContextAccessor accessor,
            ILoginStore loginStore,
            ITokenStore tokenStore)
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
            model.ShowHistory = false;
            model.ShowFilter = false;
            
            ViewData["Title"] = "Estoque em Trânsito";

            if (!model.SalesStructureAccess.HasSalesStructure)
            {
                return View("AccessError", model);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult CheckDE([FromBody]string data)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            JObject result = new JObject();
            StockBO stockBO = new StockBO();
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();

            try
            { 
                DateTime dateRerence;
                DateTime dateRerenceMax = DateTime.Now;
                DateTime dateRerenceMin = DateTime.Parse("01/01/1980");

                if (!DateTime.TryParse(data, out dateRerence))
                {
                    feedbackService.Success = false;
                    string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }

                if (dateRerence < dateRerenceMin || dateRerence > dateRerenceMax)
                {
                    feedbackService.Success = false;
                    string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }

                if (TempData["UploadSucess"] is null || (bool)TempData["UploadSucess"] == false)
                {
                    feedbackService.Success = false;
                    string msgerro = "Por favor, realize o carregamento do arquivo!";
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }

                feedbackService = CheckColumns();

                if (!feedbackService.Success)
                    return Json(feedbackService);

                bool notContinue = eTLServiceBO.ValidateStatusProcess(4);

                if(notContinue)
                {
                    feedbackService.Success = false;
                    string msgerro = "Aguarde alguns minutos que o processamento será realizado em segundo plano. Depois confira os resultados do processamento na consulta de estoques.";
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }
                
                bool returnValue = eTLServiceBO.ValidateDateReference(dateRerence);

                result["hasDate"] = returnValue.ToString().ToLower();
                return Json(result);
            }
            catch(Exception ex)
            {
                feedbackService.Success = false;
                feedbackService.Errors.Add(ex.Message);
                return Json(feedbackService);
            }
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult SaveDataTransitExcel([FromBody]string data)
        {
            StockBO stockBO = new StockBO();
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();

            var dt = new DataTable("EstoqueTransito");
            dt.Columns.Add(new DataColumn("Nr_Linha"));
            dt.Columns.Add(new DataColumn("Nr_Nota_Fiscal"));
            dt.Columns.Add(new DataColumn("Nm_Centro_Distribuicao"));
            dt.Columns.Add(new DataColumn("Cd_Sap"));
            dt.Columns.Add(new DataColumn("Nm_Destinatario"));
            dt.Columns.Add(new DataColumn("Cnpj_Cpf_Destinatario"));
            dt.Columns.Add(new DataColumn("Nm_Cidade_Destinatario"));
            dt.Columns.Add(new DataColumn("Sg_UF_Destinatario"));
            dt.Columns.Add(new DataColumn("Vl_Nota_Fiscal"));
            dt.Columns.Add(new DataColumn("Vl_Peso"));
            dt.Columns.Add(new DataColumn("Nm_Transportadora"));
            dt.Columns.Add(new DataColumn("Dt_Entrega"));
            dt.Columns.Add(new DataColumn("St_Entrega"));
            dt.Columns.Add(new DataColumn("Ds_Informacao_Complementar"));
            dt.Columns.Add(new DataColumn("Id_Processamento"));
            dt.Columns.Add(new DataColumn("Dt_Criacao"));
            dt.Columns.Add(new DataColumn("Fl_Tratamento"));

            ProcessItem processItem = new ProcessItem();
            ETLServiceBO etlServiceBO = new ETLServiceBO();
            DateTime dateRerence = DateTime.Parse(data);
            bool containsRow = false;

            try
            {
                using (var package = new ExcelPackage(new FileInfo(Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot", "upload",
                                "stock_transit_file.xlsx"))))
                {
                    //Add names from excel columns
                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                    {
                        for (int i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
                        {
                            containsRow = true;
                            break;
                        }
                    }
                }


                if (!containsRow)
                {
                    feedbackService.Success = false;
                    string msgerro = "O arquivo aparenta estar vazio, por favor, verifique o arquivo selecionado.";
                    feedbackService.Errors.Add(msgerro);
                    return Json(feedbackService);
                }

                processItem.Created = DateTime.Now;
                processItem.ExecutionType = "M";
                processItem.IntegrationProcessCode = 4;
                processItem.InputParameter = string.Empty;
                processItem.StatusCode = "I"; //Iniciado

                long codeReturn = etlServiceBO.CreateProcessItem(processItem, dateRerence, User);

                using (var package = new ExcelPackage(new FileInfo(Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot", "upload",
                                "stock_transit_file.xlsx"))))
                {
                    //Add names from excel columns
                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                    {
                        for (int i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
                        {
                            var row = dt.NewRow();
                            row["Nr_Linha"] = i;

                            for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                            {
                                object cellValue = worksheet.Cells[i, j].Value;
                                var value = cellValue != null && !string.IsNullOrEmpty(cellValue.ToString()) ? cellValue.ToString() : string.Empty;

                                switch (worksheet.Cells[1, j].Value.ToString().ToLower())
                                {
                                    case "nota fiscal":
                                        row["Nr_Nota_Fiscal"] = value;
                                        break;
                                    case "status entrega":
                                        row["St_Entrega"] = value;
                                        break;
                                    //case "cd":
                                    //    row["Nm_Centro_Distribuicao"] = value;
                                    //    break;
                                    case "código sap":
                                        row["Cd_Sap"] = value;
                                        break;
                                    case "nome destinatário":
                                        row["Nm_Destinatario"] = value;
                                        break;
                                    case "cnpj destinatário":
                                        row["Cnpj_Cpf_Destinatario"] = Utils.CpfCnpjUtils.RawValue(value);
                                        break;
                                    //case "cidade destinatário":
                                    //    row["Nm_Cidade_Destinatario"] = value;
                                    //    break;
                                    //case "uf":
                                    //    row["Sg_UF_Destinatario"] = value;
                                    //    break;
                                    case "valor nf":
                                        row["Vl_Nota_Fiscal"] = value.Replace(',', '.');
                                        break;
                                    //case "peso nf":
                                    //    row["Vl_Peso"] = value;
                                    //    break;
                                    //case "transportadora":
                                    //    row["Nm_Transportadora"] = value;
                                    //    break;
                                    case "Data de emissão":
                                        row["Dt_Entrega"] = value;
                                        break;
                                    //case "status entrega":
                                    //    row["Nr_Linha"] = i;
                                    //    stockTransit.StatusDelivery = value;
                                    //    break;
                                    //case "informação complementar":
                                    //    row["Ds_Informacao_Complementar"] = value;
                                    //    break;
                                    default:
                                        break;
                                }
                            }

                            row["Id_Processamento"] = codeReturn;
                            row["Dt_Criacao"] = DateTime.Now;
                            row["Fl_Tratamento"] = "N";
                            
                            dt.Rows.Add(row);
                        }
                    }
                }

                JObject result = new JObject();

                stockBO.AddStockTransit(dt, dateRerence, User, codeReturn);
                TempData["IdProcessamento"] = codeReturn;
                /*
                List<StockTransit> resultsValid = stockBO.ValidStockTransit(codeReturn);


                if (resultsValid.Count > 0)
                    result["Identifier"] = GenerateExcel(resultsValid);
                    if (resultsValid.Count > 1000)
                        resultsValid.RemoveRange(1000, resultsValid.Count - 1000);

                var returnJson = JsonConvert.SerializeObject(resultsValid);
                result["dataTable"] = returnJson;
                */

                bool returnValue = eTLServiceBO.UpdateProcess((int)codeReturn, "P", null);
                result["result"] = returnValue;

                return Json(result);                
            }
            catch(Exception ex)
            {
                feedbackService.Success = false;
                feedbackService.Errors.Add(ex.Message);
                return Json(feedbackService);
            }
        }

         [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public string GenerateExcel(List<Entities.StockTransit> data) {
            if (data == null)
                return null;

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var report = GenerateExcel(data, package);

                return report.Identifier;
            }
        }

        [HttpGet]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult SaveDataTransit()
        {
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();
            JObject result = new JObject();

            try
            {
                //StockTransitResult value = stockBO.SaveStockTransit((int)TempData["IdProcessamento"]);

                bool returnValue = eTLServiceBO.UpdateProcess((int)TempData["IdProcessamento"], "P", null);

                result["result"] = returnValue;
                return Json(result);
            }
            catch (Exception ex)
            {
                feedbackService.Success = false;
                feedbackService.Errors.Add(ex.Message);
                return Json(feedbackService);
            }
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        private Entities.Report GenerateExcel(List<Entities.StockTransit> data, OfficeOpenXml.ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets.Add("Erros");
            worksheet.View.ShowGridLines = false;

            worksheet.Cells["A1:V1"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double;

            worksheet.Cells["A1:V1"].Style.Font.Bold = true;


            worksheet.Cells["A1"].Value = "Nota Fiscal";
            worksheet.Cells["B1"].Value = "Data de emissão";
            worksheet.Cells["C1"].Value = "Código Sap";
            worksheet.Cells["D1"].Value = "Nome destinatário";
            worksheet.Cells["E1"].Value = "CNPJ destinatário";
            worksheet.Cells["F1"].Value = "Valor NF";
            worksheet.Cells["G1"].Value = "Erro";
            
            var excelLine = 1;

            foreach (var result in data)
            {

                excelLine++;

                worksheet.Cells["A" + excelLine].Value = result.NF;
                worksheet.Cells["B" + excelLine].Value = Convert.ToDateTime(result.IssuanceDate);
                worksheet.Cells["B" + excelLine].Style.Numberformat.Format = "dd/MM/yyyy";
                //worksheet.Cells["C" + excelLine].Value = result.CD;
                worksheet.Cells["C" + excelLine].Value = result.CDSAP;
                worksheet.Cells["D" + excelLine].Value = result.NameRecipient;
                worksheet.Cells["E" + excelLine].Value = result.CNPJRecipient;
                //worksheet.Cells["G" + excelLine].Value = result.CityRecipient;
                //worksheet.Cells["H" + excelLine].Value = result.UF;
                worksheet.Cells["F" + excelLine].Value = result.ValueNF;
               // worksheet.Cells["J" + excelLine].Value = result.WeightNF;
                //worksheet.Cells["K" + excelLine].Value = result.ShippingCompany;
                //worksheet.Cells["L" + excelLine].Value = Convert.ToDateTime(result.DeliveryDate);
                //worksheet.Cells["L" + excelLine].Style.Numberformat.Format = "dd/MM/yyyy";
                //worksheet.Cells["M" + excelLine].Value = result.ComplementaryInformation;
                worksheet.Cells["G" + excelLine].Value = result.Ds_Erro;

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

                var fileDownloadName = "stock_transit_file.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileStream = new System.IO.MemoryStream(report.SerializedContent);
                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileDownloadName);
                Response.Headers.Add("X-Download-Options", "noopen");
                return File(fileStream, contentType);

            }

        }

        public FeedbackService CheckColumns()
        {
            try
            {
                FeedbackService feedbackService = new FeedbackService();

                List<string> ColumnNamesContains = new List<string>();
                List<string> ColumnNamesNotContains = new List<string>();
                List<Entities.StockTransit> stockTransits = new List<Entities.StockTransit>();
                string msgerro = string.Empty;

                //TODO: MEHORAR CHECAGEM DO NOME DAS COLUNAS
                using (var package = new ExcelPackage(new FileInfo(Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot", "upload",
                                "stock_transit_file.xlsx"))))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    if (worksheet.Dimension.End.Column != 6)
                    {
                        feedbackService.Success = false;
                        msgerro = "Número de colunas diferentes do esperado para o arquivo! Total de colunas verificadas no arquivo " + worksheet.Dimension.End.Column.ToString();
                        feedbackService.Errors.Add(msgerro);
                        return feedbackService;
                    }
                    //Add names from excel columns

                    //check if the names exists
                    if (!worksheet.Cells[1, 1].Value.ToString().ToLower().Equals("nota fiscal"))
                        ColumnNamesNotContains.Add("Nota Fiscal");
                    if (!worksheet.Cells[1, 2].Value.ToString().ToLower().Equals("data de emissão"))
                        ColumnNamesNotContains.Add("Data de emissão");
                    //if (!worksheet.Cells[1, 3].Value.ToString().ToLower().Equals("cd"))
                    //    ColumnNamesNotContains.Add("CD");
                    if (!worksheet.Cells[1, 3].Value.ToString().ToLower().Equals("código sap"))
                        ColumnNamesNotContains.Add("Código SAP");
                    if (!worksheet.Cells[1, 4].Value.ToString().ToLower().Equals("nome destinatário"))
                        ColumnNamesNotContains.Add("Nome destinatário");
                    if (!worksheet.Cells[1, 5].Value.ToString().ToLower().Equals("cnpj destinatário"))
                        ColumnNamesNotContains.Add("CNPJ destinatário");
                    //if (!worksheet.Cells[1, 7].Value.ToString().ToLower().Equals("cidade destinatário"))
                    //    ColumnNamesNotContains.Add("Cidade destinatário");
                    //if (!worksheet.Cells[1, 8].Value.ToString().ToLower().Equals("uf"))
                    //    ColumnNamesNotContains.Add("UF");
                    //if (!worksheet.Cells[1, 9].Value.ToString().ToLower().Equals("valor nf"))
                    //    ColumnNamesNotContains.Add("Valor NF");
                    //if (!worksheet.Cells[1, 10].Value.ToString().ToLower().Equals("peso nf"))
                    //    ColumnNamesNotContains.Add("Peso NF");
                    //if (!worksheet.Cells[1, 11].Value.ToString().ToLower().Equals("transportadora"))
                    //    ColumnNamesNotContains.Add("Transportadora");
                    //if (!worksheet.Cells[1, 12].Value.ToString().ToLower().Equals("data entrega"))
                    //    ColumnNamesNotContains.Add("Data entrega");
                    if (!worksheet.Cells[1, 6].Value.ToString().ToLower().Equals("status entrega"))
                        ColumnNamesNotContains.Add("Status entrega");
                    //if (!worksheet.Cells[1, 14].Value.ToString().ToLower().Equals("informação complementar"))
                    //    ColumnNamesNotContains.Add("informação Complementar");
                }


                if (ColumnNamesNotContains.Any())
                {
                    feedbackService.Success = false;
                    var totalColunas = ColumnNamesNotContains.Count();
                    msgerro = "Não foram encontradas as colunas a seguir ou estão em posição diferente da esperada: " + string.Join(",", ColumnNamesNotContains.ToArray());
                    if (totalColunas >= 14)
                    {
                        msgerro += ".\n<br> Planilha inválida!";
                    }
                    feedbackService.Errors.Add(msgerro);
                }
                return feedbackService;
            }
            catch (IndexOutOfRangeException indexEx)
            {
                throw indexEx;
            }                        
        }        
        
        [HttpPost]
        [Authorize]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadStreamingFile()
        {
            // full path to file save in location
            var filePath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot", "upload",
                            "stock_transit_file.xlsx");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Request.StreamFile(stream);
            }

            TempData["UploadSucess"] = true;

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok();
        }

        public IActionResult DownloadError()
        {
            throw new NotImplementedException();
        }
                

        [HttpGet]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult ValidDataTransit([FromBody]string data)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            JObject result = new JObject();
            StockBO stockBO = new StockBO();
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();

            var codeProcess = (int)TempData["IdProcessamento"];
            List<ProcessItem> resultsValid = stockBO.ValidDataTransit((long)codeProcess);
            if (resultsValid.FirstOrDefault().StatusCode == "P" ||
                resultsValid.FirstOrDefault().StatusCode == "I"
                )
            {
                var returnJson = JsonConvert.SerializeObject(resultsValid);
                result["dataStatus"] = returnJson;

            }
            else
            {
                List<StockTransit> resultsValidDt = stockBO.ValidStockTransit(codeProcess);

                if (resultsValidDt.Count > 0)
                    result["Identifier"] = GenerateExcel(resultsValidDt);
                if (resultsValidDt.Count > 100)
                    resultsValidDt.RemoveRange(100, resultsValidDt.Count - 100);

                var returnJson = JsonConvert.SerializeObject(resultsValidDt);
                result["dataTable"] = returnJson;

                
                
            }
            TempData["IdProcessamento"] = codeProcess;
            return Json(result);
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GetValidStockTransit([FromBody]string data)
        {
            long codeProcess = Convert.ToInt64(data);
            JObject result = new JObject();
            StockBO stockBO = new StockBO();

            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();

            List<StockTransit> resultsValidDt = stockBO.ValidStockTransit(codeProcess);

            if (resultsValidDt.Count > 0)
                result["Identifier"] = GenerateExcel(resultsValidDt);
            
       
            return Json(result);
        }

        [HttpGet]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
  
        public JsonResult GetLastIntegrationProcesses()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            JObject result = new JObject();
            StockBO stockBO = new StockBO();
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();
            List<ProcessItem> listProcessItem = new List<ProcessItem>();

            ProcessItem processItem = new ProcessItem();
            processItem = stockBO.GetLastIntegrationProcesses(4);
            processItem.FinishedGrid = (processItem.Finished.HasValue) ? processItem.Finished.Value.ToString("dd/MM/yyyy hh:mm:ss") : string.Empty;
            listProcessItem.Add(processItem);

            var returnJson = JsonConvert.SerializeObject(listProcessItem);
            result["Data"] = returnJson;

            return Json(result);
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