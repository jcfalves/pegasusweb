using Bayer.Pegasus.Entities.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bayer.Pegasus.ApiClient.Helpers;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Bayer.Pegasus.Business
{
    public class AuthenticationBO
    {
        private readonly HttpClient _httpClient;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthenticationBO));

        public AuthenticationBO()
        {
            _httpClient = new HttpClient();
        }


        public async Task<String> GetLogin(LoginViewModel model, string token, string clientId)
        {
            try
            {
                string LoginUrl = Utils.Configuration.Instance.UrlApiAuthServicesLogin;

                var output = await Login(model, token, LoginUrl, clientId);

                return (output);
                //return output;
            }
            catch (Exception e)
            {
                var msg = e.Message;
                throw;
            }
        }

        public async Task<String> Login(LoginViewModel model, string token, string loginUrl, string clientId)
        {
            try
            {
                _log4net.Debug($"AuthenticationBO.Login - Início");

                var json = JsonConvert.SerializeObject(model);
                var payload = new StringContent(json, Encoding.UTF8, "application/json");
                JToken jtoken = null;

                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", clientId);
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("access_token", token);

                int codRetorno = 0;

                var response = await _httpClient.PostAsync(loginUrl, payload);

                codRetorno = (int)response.StatusCode;
                _log4net.Debug($"AuthenticationBO.Login - httpClient.PostAsync - codRetorno: {codRetorno}");

                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject(data);

                    jtoken = JToken.Parse(data);

                    _log4net.Debug($"AuthenticationBO.Login - httpClient.PostAsync - token: {data}");
                    
                }
                else
                {
                    // Handle failure
                    _log4net.Error($"AuthenticationBO.Login - httpClient.PostAsync - Erro");
                }

                _log4net.Debug($"AuthenticationBO.Login - Fim");

                return ResultLogin(jtoken, clientId, token);

            }
            catch (Exception ex)
            {
                _log4net.Error($"AuthenticationBO.Login - Exception: {ex.Message} - InnerException: {ex.InnerException} - Trace: {ex.StackTrace}");
                var msg = ex.Message;
                throw ex;
            }
        }


        public async Task<String> LoginSSO(LoginSSOViewModel model, string token, string clientId)
        {
            try
            {
                string LoginSSOUrl = Utils.Configuration.Instance.UrlApiAuthServicesLoginSSO;
                var json = JsonConvert.SerializeObject(model);
                var payload = new StringContent(json, Encoding.UTF8, "application/json");
                JToken jtoken = null;

                using (var httpClient = new HttpClient())
                {
                    _log4net.Debug($"AuthenticationBO.LoginSSO - Início");

                    //--------------------------------------------------------------
                    if (Bayer.Pegasus.Utils.Configuration.Instance.UseTokenLifeTime)
                    {

                        _log4net.Debug($"AuthenticationBO.LoginSSO - Token de Login (LoginSSO): {token}");

                        string mailTo = "roberto.lima1.ext@bayer.com; jose.alves3.ext@bayer.com; cinthia.vicentini.ext@bayer.com; marcelo.fonseca@bayer.com;";
                        string subject = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - Token de Login (LoginSSO): " + token;
                        string texto = "";
                        EmailHelper.SendEmail(mailTo, subject, texto, true);
                    }
                    //--------------------------------------------------------------

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", clientId);
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("access_token", token);

                    _log4net.Debug($"AuthenticationBO.LoginSSO - httpClient.PostAsync (Início)");

                    int codRetorno = 0;

                    HttpResponseMessage response = new HttpResponseMessage();

                    try
                    {
                        _log4net.Debug($"LoginSSOUrl: {LoginSSOUrl}");
                        _log4net.Debug($"payload (Headers.ContentType.CharSet): {payload.Headers.ContentType.CharSet}");
                        _log4net.Debug($"payload (Headers.ContentType.MediaType): {payload.Headers.ContentType.MediaType}");

                        Exception exceptionThrown = null;

                        for (int i = 1; i <= 3; i++)
                        {
                            try
                            {
                                _log4net.Debug($"AuthenticationBO.LoginSSO - httpClient.PostAsync - tentativa {i}");
                                response = await httpClient.PostAsync(LoginSSOUrl, payload);

                                break; //Request foi realizado com sucesso

                            }
                            catch (Exception ex)
                            {
                                exceptionThrown = ex;
                                Thread.Sleep(2000);
                            }
                        }

                        if (exceptionThrown != null)
                        {
                            if (exceptionThrown.GetType() == typeof(WebException) ||
                                exceptionThrown.GetType() == typeof(SocketException))
                            {
                                _log4net.Error($"AuthenticationBO.LoginSSO - httpClient.PostAsync - exceptionThrown: {exceptionThrown} - exceptionThrown.Message:{exceptionThrown.Message}");
                                EmailHelper.SendErrorEmail($"Erro na AuthenticationBO.LoginSSO - httpClient.PostAsync", $"exceptionThrown: {exceptionThrown} - exceptionThrown.Message:{exceptionThrown.Message}");
                            }

                            throw exceptionThrown;
                        }

                    }
                    catch (Exception ex)
                    {
                        _log4net.Error($"AuthenticationBO.LoginSSO - httpClient.PostAsync - Exception: {ex.Message} - InnerException: {ex.InnerException} - Trace: {ex.StackTrace}");
                        EmailHelper.SendErrorEmail($"Erro na AuthenticationBO.LoginSSO - httpClient.PostAsync", $"Exception: {ex.Message} - InnerException: {ex.InnerException} - StackTrace: {ex.StackTrace}");
                    }

                    codRetorno = (int)response.StatusCode;
                    _log4net.Debug($"AuthenticationBO.LoginSSO - httpClient.PostAsync - codRetorno: {codRetorno}");


                    if (response.IsSuccessStatusCode)
                    {
                        // Handle success
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject(data);

                        jtoken = JToken.Parse(data);

                        _log4net.Debug($"AuthenticationBO.LoginSSO - httpClient.PostAsync - token: {data}");
                    }
                    else
                    {
                        // Handle failure
                        _log4net.Error($"AuthenticationBO.LoginSSO - httpClient.PostAsync - Erro");
                    }

                    _log4net.Debug($"AuthenticationBO.LoginSSO - httpClient.PostAsync (Fim)");

                    _log4net.Debug($"AuthenticationBO.LoginSSO - Fim");

                }

                return ResultLogin(jtoken, clientId, token);
            }
            catch (Exception ex)
            {
                _log4net.Error($"AuthenticationBO.LoginSSO - Exception: {ex.Message} - InnerException: {ex.InnerException} - Trace: {ex.StackTrace}");
                var msg = ex.Message;
                throw ex;
            }
        }


        public async Task<String> Logout(LogoutViewModel model, string token, string clientId)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var payload = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", clientId);
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("access_token", token);

                    string LogoutUrl = Utils.Configuration.Instance.UrlApiAuthServicesLogout;

                    var response = await httpClient.PostAsync(LogoutUrl, payload);

                    response.EnsureSuccessStatusCode();

                    var data = await response.Content.ReadAsStringAsync();

                    var output = JsonConvert.DeserializeObject(data);

                    return JsonConvert.SerializeObject(output);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw ex;
            }
        }



        public String ResultLogin(JToken jsontoken, string ClientId, string token)
        {
            try
            {
                var res = jsontoken.SelectToken("result");
                var ret = jsontoken.SelectToken("return");
                var rol = ret.SelectToken("roles");

                AuthenticationResultModel result = JsonConvert.DeserializeObject<AuthenticationResultModel>(res.ToString());
                ReturnModel retu = JsonConvert.DeserializeObject<ReturnModel>(ret.ToString());
                List<RoleModel> rols = JsonConvert.DeserializeObject<List<RoleModel>>(rol.ToString());

                LoginInfoModel results = new LoginInfoModel();

                if (result.code == 1)
                {

                    results.Result = new InfoModel();
                    results.Result.Code = result.code;
                    results.Result.ClientId = ClientId;
                    results.Result.AccessToken = token;

                    results.Return = new ReturnModel();

                    results.Return.nameUser = retu.nameUser;
                    results.Return.cwid = retu.cwid;
                    results.Return.email = retu.email;

                    results.Return.systemName = retu.systemName;
                    results.Return.dtLastLogin = Convert.ToDateTime(retu.dtLastLogin);

                    results.Return.roles = new List<RoleModel>();
                    results.Return.roles = rols;
                }
                else if (result.code != 1)
                {
                    results.Result = new InfoModel
                    {
                        Description = "AppId não cadastrado",
                        Code = result.code,
                        ClientId = "",
                        AccessToken = ""
                    };
                }
                else
                {
                    results.Result = new InfoModel
                    {
                        Description = "Acesso não autorizado",
                        Code = result.code,
                        ClientId = "",
                        AccessToken = ""
                    };
                }

                return JsonConvert.SerializeObject(results);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                throw;
            }
        }

    }

}
