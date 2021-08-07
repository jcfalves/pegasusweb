using Bayer.Pegasus.ApiClient.Helpers;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Web.Controllers
{
    public class HealthCheckController : Base.LoggedBaseController
    {

        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RetroativoController));

        #region Private Read-Only Fields
        private readonly HttpClient _httpClient;
        private AccessTokenViewModel _accessToken;
        #endregion        

        #region Constructor

        private readonly ITokenStore _tokenStore;
        private readonly ILoginStore _loginStore;


        public HealthCheckController(ILogger<HealthCheckController> logger, ITokenStore tokenStore, ILoginStore loginStore, IHttpContextAccessor accessor)
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
            model.ShowAbaPrice = true;
            model.ShowAbaFilterHC = true;

            ViewData["Title"] = "Monitoramento - Health Check";

            if (!model.SalesStructureAccess.HasSalesStructure)
            {
                return View("AccessError", model);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GetTypeErrorHealthCheck([FromBody]JObject data)
        {
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();
            JObject result = new JObject();
            var typeErrorHealthCheck = data.ToObject<TypeErrorHealthCheck>();

            try
            {
                DateTime dateIni;
                DateTime dateEnd;
                DateTime dateRerenceMax = DateTime.Now;
                DateTime dateRerenceMin = DateTime.Parse("01/01/1980");

                if (!DateTime.TryParse(typeErrorHealthCheck.DateIni, out dateIni))
                {
                    feedbackService.Success = false;
                    string msgerro = string.Concat("Por favor, insira uma data válida entre 01/01/1980 e ", dateRerenceMax.ToString("dd/MM/yyyy"));
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }

                if (!DateTime.TryParse(typeErrorHealthCheck.DateEnd, out dateEnd))
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

                List<string> tiposErros = null;
                if (!string.IsNullOrEmpty(typeErrorHealthCheck.CheckedTypes) && typeErrorHealthCheck.CheckedTypes != "[]")
                    tiposErros = JsonConvert.DeserializeObject<List<string>>(typeErrorHealthCheck.CheckedTypes);

                HealthCheckBO healthCheckBO = new HealthCheckBO();
                List<ErrorHealthCheck> listErrorHealthCheck = healthCheckBO.GetErrorHealthCheckDashboard(Convert.ToDateTime(typeErrorHealthCheck.DateIni),
                    Convert.ToDateTime(typeErrorHealthCheck.DateEnd), typeErrorHealthCheck.ErrorCategoryId, tiposErros);

                result["dataTable"] = JsonConvert.SerializeObject(listErrorHealthCheck);
                return Json(result);
            }
            catch (Exception ex)
            {
                feedbackService.Success = false;                
                feedbackService.Errors.Add(ex.Message);
                feedbackService.Errors.Add(ex.StackTrace);
                return Json(feedbackService);
            }
            
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GetErrorHealthCheck([FromBody]JObject data)
        {
            Report report = new Report();
            JObject result = new JObject();
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();
            HealthCheckBO healthCheckBO = new HealthCheckBO();
            var terrorHealthCheck = data.ToObject<TypeErrorHealthCheck>();
            System.Security.Claims.ClaimsPrincipal UserAux = User;
            List<string> tiposErros = null;
            if (!string.IsNullOrEmpty(terrorHealthCheck.CheckedTypes) && terrorHealthCheck.CheckedTypes != "[]")
                tiposErros = JsonConvert.DeserializeObject<List<string>>(terrorHealthCheck.CheckedTypes);

            try
            {
                //ThreadPool.QueueUserWorkItem(delegate {

                Dictionary<TypeErrorHealthCheck, List<ErrorHealthCheck>> listTypeErrorHealthCheck = healthCheckBO.GetErrorHealthCheck(Convert.ToDateTime(terrorHealthCheck.DateIni),
                Convert.ToDateTime(terrorHealthCheck.DateEnd), terrorHealthCheck.ErrorCategoryId, tiposErros);

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    foreach (TypeErrorHealthCheck typeErrorHealthCheck in listTypeErrorHealthCheck.Keys)
                    {
                        var worksheet = package.Workbook.Worksheets.Add(typeErrorHealthCheck.DescriptionTyeRegister);
                        worksheet.View.ShowGridLines = false;
                        List<ErrorHealthCheck> errorHealthChecks;

                        var excelColumn = 0;
                        var excelLine = 1;
                        foreach (string columnName in typeErrorHealthCheck.Columns.Split(';'))
                        {
                            excelColumn++;
                            worksheet.Cells[excelLine, excelColumn].Value = columnName;
                        }

                        listTypeErrorHealthCheck.TryGetValue(typeErrorHealthCheck, out errorHealthChecks);

                        foreach (var errorHealthCheck in errorHealthChecks)
                        {
                            if (!string.IsNullOrEmpty(errorHealthCheck.ColumnsValues))
                            {
                                excelLine++;
                                var excelColumnValue = 0;
                                foreach (string valueRow in errorHealthCheck.ColumnsValues.Split(';'))
                                {
                                    excelColumnValue++;
                                    worksheet.Cells[excelLine, excelColumnValue].Value = valueRow;
                                }
                            }
                        }
                    }

                    report.Json = listTypeErrorHealthCheck.ToString();
                    report.SerializedContent = package.GetAsByteArray();

                    using (var reportBO = new Bayer.Pegasus.Business.ReportBO())
                    {
                        reportBO.SaveReport(UserAux, report);
                    }

                    //var subject = "Relatório de Monitoramento de Dados - Health Check";

                    //var body = EmailHelper.GetTemplateEmail(_hostingEnvironment.WebRootPath, "pegasusReport");
                    //var link = Configuration.Instance.PortalOneDomainURL + Configuration.Instance.URLPrefix + "/HealthCheck/Download";
                    //body = body.Replace("#LINK#", link);
                    //body = body.Replace("#IDENTIFIER#", report.Identifier);
                    //EmailHelper.SendEmail(terrorHealthCheck.Email, subject, body, true);

                }

                //});

                result["Identifier"] = report.Identifier;
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
        public JsonResult GetListCategoryHeathCheck()
        {
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();
            JObject result = new JObject();

            HealthCheckBO healthCheckBO = new HealthCheckBO();
            List<CategoryHeathCheck> listCategoryHealthCheck = healthCheckBO.GetListCategoryHeathCheck();
            //retornar somente inconsistencias da accer
            listCategoryHealthCheck = listCategoryHealthCheck.Where(c => c.CategoryId == 3 || c.CategoryId == 5 || c.CategoryId == 6 || c.CategoryId == 7).ToList();
            result["dataTable"] = JsonConvert.SerializeObject(listCategoryHealthCheck);
            return Json(result);
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult GetTypeErrorCategoryIdHC([FromBody]JObject data)
        {
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();
            JObject result = new JObject();

            var categoryHeath = data.ToObject<CategoryHeathCheck>();

            HealthCheckBO healthCheckBO = new HealthCheckBO();
            List<TypeErrorHealthCheck> listCategoryHealthCheck = healthCheckBO.GetTypeErrorCategoryIdHC(categoryHeath.CategoryId);
            //retornar somente inconsistencias da accera
            listCategoryHealthCheck = listCategoryHealthCheck.Where(c => c.ErrorCategoryId == 3 || c.ErrorCategoryId == 5 || c.ErrorCategoryId == 6 || c.ErrorCategoryId == 7 && c.Code != "RLC-05").ToList();
            result["dataTable"] = JsonConvert.SerializeObject(listCategoryHealthCheck);
            return Json(result);
        }     
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Download(string identifier)
        {
            using (var reportBO = new Bayer.Pegasus.Business.ReportBO())
            {
                var report = reportBO.GetReport(identifier);

                if (report.SerializedContent == null)
                    return RedirectToAction("DownloadError");

                var fileDownloadName = "errors.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileStream = new System.IO.MemoryStream(report.SerializedContent);
                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileDownloadName);
                Response.Headers.Add("X-Download-Options", "noopen");
                return File(fileStream, contentType);
            }
        }

        /* Import Price*/
        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult CheckDE([FromBody]string data)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            JObject result = new JObject();            
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

                bool notContinue = eTLServiceBO.ValidateStatusProcess(7);

                if (notContinue)
                {
                    feedbackService.Success = false;
                    string msgerro = "Aguarde alguns minutos que o processamento será realizado em segundo plano. Depois confira os resultados do processamento na grid de Price";
                    feedbackService.Errors.Add(msgerro);

                    return Json(feedbackService);
                }

                bool returnValue = eTLServiceBO.ValidateDateReference(dateRerence);

                result["hasDate"] = returnValue.ToString().ToLower();
                return Json(result);
            }
            catch (Exception ex)
            {
                feedbackService.Success = false;
                feedbackService.Errors.Add(ex.Message);
                return Json(feedbackService);
            }
        }

        [HttpPost]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JsonResult SaveDataPriceExcel([FromBody]string data)
        {
            ProductPriceBO productPriceBO = new ProductPriceBO();           
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();

            var dt = new DataTable("PrecoProduto");
            dt.Columns.Add(new DataColumn("Nr_Linha"));
            dt.Columns.Add(new DataColumn("Sg_Moeda"));
            dt.Columns.Add(new DataColumn("Nm_Diretoria"));
            dt.Columns.Add(new DataColumn("Cd_Cluster"));
            dt.Columns.Add(new DataColumn("Nm_Regional"));
            dt.Columns.Add(new DataColumn("Nm_Marca"));
            dt.Columns.Add(new DataColumn("Cd_Produto"));
            dt.Columns.Add(new DataColumn("Nm_Produto"));
            dt.Columns.Add(new DataColumn("Vl_Produto"));
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
                                "import_price_file.xlsx"))))
                {
                    //Add names from excel columns
                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                    {
                        for (int i = worksheet.Dimension.Start.Row + 2; i <= worksheet.Dimension.End.Row; i++)
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
                processItem.IntegrationProcessCode = 7;
                processItem.InputParameter = string.Empty;
                processItem.StatusCode = "I"; //Iniciado

                long codeReturn = etlServiceBO.CreateProcessItem(processItem, dateRerence, User);

                using (var package = new ExcelPackage(new FileInfo(Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot", "upload",
                                "import_price_file.xlsx"))))
                {
                    //Add names from excel columns
                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                    {
                        for (int i = worksheet.Dimension.Start.Row + 2; i <= worksheet.Dimension.End.Row; i++)
                        {
                            var row = dt.NewRow();
                            row["Nr_Linha"] = i;
                            string linhaVazia = string.Empty;
                            for (int j = worksheet.Dimension.Start.Column+1; j <= worksheet.Dimension.End.Column; j++)
                            {
                                object cellValue = worksheet.Cells[i, j].Value;
                                var value = cellValue != null && !string.IsNullOrEmpty(cellValue.ToString()) ? cellValue.ToString() : string.Empty;
                                switch (worksheet.Cells[2, j].Value.ToString().ToLower().TrimStart().TrimEnd())
                                {
                                    case "moeda":
                                        row["Sg_Moeda"] = value;
                                        break;
                                    case "dn":
                                        row["Nm_Diretoria"] = value;
                                        break;
                                    case "cluster":
                                        row["Cd_Cluster"] = value;
                                        break;
                                    case "regional":
                                        row["Nm_Regional"] = value;
                                        break;
                                    case "marca":
                                        row["Nm_Marca"] = value;
                                        break;
                                    case "material":
                                        row["Cd_Produto"] = value;
                                        break;
                                    case "descrição":
                                        row["Nm_Produto"] = value;
                                        break;
                                    case "preço de lista/sap":
                                        row["Vl_Produto"] = value.Replace(',', '.');
                                        break;                                                                       
                                    default:
                                        break;
                                }
                                linhaVazia += value.Trim().TrimStart().TrimEnd();
                            }
                            if (!string.IsNullOrEmpty(linhaVazia))
                            {
                                row["Id_Processamento"] = codeReturn;
                                row["Dt_Criacao"] = DateTime.Now;
                                row["Fl_Tratamento"] = false;
                                dt.Rows.Add(row);
                            }

                        }
                    }
                }

                JObject result = new JObject();

                productPriceBO.AddImportPrice(dt, dateRerence, User, codeReturn);
                TempData["IdProcessamento"] = codeReturn;                

                bool returnValue = eTLServiceBO.UpdateProcess((int)codeReturn, "P", null);
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
        public string GenerateExcel(List<Entities.ProductPrice> data)
        {
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
        public JsonResult SaveDataPrice()
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
        private Entities.Report GenerateExcel(List<Entities.ProductPrice> data, OfficeOpenXml.ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets.Add("Erros");
            worksheet.View.ShowGridLines = false;

            worksheet.Cells["A1:V1"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double;

            worksheet.Cells["A1:V1"].Style.Font.Bold = true;
                         
            worksheet.Cells["A1"].Value = "Moeda";
            worksheet.Cells["B1"].Value = "DN";
            worksheet.Cells["C1"].Value = "Cluster";
            worksheet.Cells["D1"].Value = "Regional";
            worksheet.Cells["E1"].Value = "Marca";
            worksheet.Cells["F1"].Value = "Material";
            worksheet.Cells["G1"].Value = "Descrição";
            worksheet.Cells["H1"].Value = "Preço de Lista/SAP";
            worksheet.Cells["I1"].Value = "Código Erro";
            worksheet.Cells["J1"].Value = "Erro";

            var excelLine = 1;

            foreach (var result in data)
            {

                excelLine++;                
                worksheet.Cells["A" + excelLine].Value = result.CoinTypes;
                worksheet.Cells["B" + excelLine].Value = result.BoardName;
                worksheet.Cells["C" + excelLine].Value = result.CodeCluster;
                worksheet.Cells["D" + excelLine].Value = result.RegionalName;
                worksheet.Cells["E" + excelLine].Value = result.TradeMark;
                worksheet.Cells["F" + excelLine].Value = result.CodeProduct;
                worksheet.Cells["G" + excelLine].Value = result.NameProduct;
                worksheet.Cells["H" + excelLine].Value = result.ValueProduct;
                worksheet.Cells["I" + excelLine].Value = result.CodeError;
                worksheet.Cells["J" + excelLine].Value = result.DescriptionError;             
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
        public ActionResult DownloadPrice(string identifier)
        {
            using (var reportBO = new Bayer.Pegasus.Business.ReportBO())
            {
                var report = reportBO.GetReport(identifier);

                if (report.SerializedContent == null)
                    return RedirectToAction("DownloadError");

                var fileDownloadName = "import_price_file.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileStream = new System.IO.MemoryStream(report.SerializedContent);
                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileDownloadName);
                Response.Headers.Add("X-Download-Options", "noopen");
                return File(fileStream, contentType);

            }

        }

        public FeedbackService CheckColumns()
        {
            FeedbackService feedbackService = new FeedbackService();

            List<string> ColumnNamesContains = new List<string>();
            List<string> ColumnNamesNotContains = new List<string>();
            List<Entities.ProductPrice> productPrice = new List<Entities.ProductPrice>();

            //TODO: MEHORAR CHECAGEM DO NOME DAS COLUNAS
            using (var package = new ExcelPackage(new FileInfo(Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot", "upload",
                            "import_price_file.xlsx"))))
            {
                //Add names from excel columns
                foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                {
                    for (int i = worksheet.Dimension.Start.Column + 1; i <= worksheet.Dimension.End.Column; i++)
                        ColumnNamesContains.Add(worksheet.Cells[2, i].Value != null ? worksheet.Cells[2, i].Value.ToString() : "");

                    //check if the names exists
                    if (!ColumnNamesContains.Any(s => s.ToLower().Contains("moeda")))
                        ColumnNamesNotContains.Add("Moeda");
                    if (!ColumnNamesContains.Any(s => s.ToLower().Contains("dn")))
                        ColumnNamesNotContains.Add("DN");
                    if (!ColumnNamesContains.Any(s => s.ToLower().Contains("cluster")))
                        ColumnNamesNotContains.Add("Cluster");
                    if (!ColumnNamesContains.Any(s => s.ToLower().Contains("regional")))
                        ColumnNamesNotContains.Add("Regional");
                    if (!ColumnNamesContains.Any(s => s.ToLower().Contains("marca")))
                        ColumnNamesNotContains.Add("Marca");
                    if (!ColumnNamesContains.Any(s => s.ToLower().Contains("material")))
                        ColumnNamesNotContains.Add("Material");
                    if (!ColumnNamesContains.Any(s => s.ToLower().Contains("descrição")))
                        ColumnNamesNotContains.Add("Descrição");
                    if (!ColumnNamesContains.Any(s => s.ToLower().Contains("preço de lista/sap")))
                        ColumnNamesNotContains.Add("Preço de Lista/SAP");
                }
            }

            if (ColumnNamesNotContains.Any())
            {
                feedbackService.Success = false;
                var totalColunas = ColumnNamesNotContains.Count();
                string msgerro = "Estão faltando as seguintes colunas: " + string.Join(",", ColumnNamesNotContains.ToArray());
                if (totalColunas >= 8)
                {
                    msgerro += ".\n<br> Planilha inválida!";
                }
                feedbackService.Errors.Add(msgerro);
            }

            return feedbackService;
        }



        [HttpPost]
        [Authorize]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadStreamingFile()
        {
            // full path to file save in location
            var filePath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot", "upload",
                            "import_price_file.xlsx");

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
        public JsonResult ValidDataPrice([FromBody]string data)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            JObject result = new JObject();
            ProductPriceBO productPriceBO = new ProductPriceBO();
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();

            var codeProcess = (int)TempData["IdProcessamento"];
            List<ProcessItem> resultsValid = productPriceBO.ValidDataPrice((long)codeProcess);
            if (resultsValid.FirstOrDefault().StatusCode == "P" ||
                resultsValid.FirstOrDefault().StatusCode == "I"
                )
            {
                var returnJson = JsonConvert.SerializeObject(resultsValid);
                result["dataStatus"] = returnJson;

            }
            else
            {
                List<ProductPrice> resultsValidDt = productPriceBO.ValidImportPrice(codeProcess);

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
        public JsonResult GetValidImportPrice([FromBody]string data)
        {
            long codeProcess = Convert.ToInt64(data);
            JObject result = new JObject();
            ProductPriceBO productPrice = new ProductPriceBO();

            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();

            List<ProductPrice> resultsValidDt = productPrice.ValidImportPrice(codeProcess);

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
            ProductPriceBO productPriceBO = new ProductPriceBO();
            ETLServiceBO eTLServiceBO = new ETLServiceBO();
            FeedbackService feedbackService = new FeedbackService();
            List<ProcessItem> listProcessItem = new List<ProcessItem>();

            ProcessItem processItem = new ProcessItem();
            processItem = productPriceBO.GetLastIntegrationProcesses(7);
            processItem.FinishedGrid = (processItem.Finished.HasValue) ? processItem.Finished.Value.ToString("dd/MM/yyyy hh:mm:ss") : string.Empty;
            listProcessItem.Add(processItem);

            var returnJson = JsonConvert.SerializeObject(listProcessItem);
            result["Data"] = returnJson;

            return Json(result);
        }

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
