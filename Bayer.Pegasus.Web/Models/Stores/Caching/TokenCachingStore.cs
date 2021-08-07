using Bayer.Pegasus.ApiClient.Helpers;
using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CacheStrategy.Stores.Caching
{
    public class TokenCachingStore<T> : ITokenStore
        where T : ITokenStore
    {
        private readonly IMemoryCache _memoryCache;
        private readonly T _inner;
        private readonly ILogger<TokenCachingStore<T>> _logger;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(TokenCachingStore<T>));

        public TokenCachingStore(IMemoryCache memoryCache, T inner, ILogger<TokenCachingStore<T>> logger)
        {
            _log4net.Debug($"TokenCachingStore - Construtor (Início)");

            _memoryCache = memoryCache;
            _inner = inner;
            _logger = logger;

            _log4net.Debug($"TokenCachingStore - Construtor (Fim)");
        }


        public IEnumerable<Token> List()
        {
            _log4net.Debug($"TokenCachingStore.List (Início)");

            var key = "Tokens";
            var item = _memoryCache.Get<IEnumerable<Token>>(key);

            //_log4net.Debug($"TokenCachingStore.List - key: {key}");
            //_log4net.Debug($"TokenCachingStore.List - item: {(item == null ? "null" : item.ToString())}");

            if (item == null)
            {
                item = _inner.List();
                if (item != null)
                {
                    _memoryCache.Set(key, item, TimeSpan.FromMinutes(1));
                }
            }

            _log4net.Debug($"TokenCachingStore.List (Fim)");

            return item;
        }

        public Token Get(string name)
        {
            _log4net.Debug($"TokenCachingStore.Get (Início)");

            var key = GetKey(name.ToString());
            var item = _memoryCache.Get<Token>(key);

            if (item == null)
            {
                _logger.LogTrace("Cache miss for {cacheKey}", key);
                item = _inner.Get(name);
                if (item != null)
                {
                    _logger.LogTrace("Setting item in cache for {cacheKey}", key);
                    _memoryCache.Set(key, item, TimeSpan.FromMinutes(1));
                }
            }
            else
            {
                _logger.LogTrace("Cache hit for {cacheKey}", key);
            }

            _log4net.Debug($"TokenCachingStore.Get (Fim)");

            return item;
        }


        private static string GetKey(string key)
        {
            _log4net.Debug($"TokenCachingStore.GetKey (Início / Fim)");

            return $"{typeof(T).FullName}:{key}";

        }

        public async Task<string> FetchToken(string clientHash)
        {
            string token = string.Empty;

            try
            {
                _log4net.Debug($"TokenCachingStore.FetchToken - Obtendo o Token (Início)");

                string AccessTokenUrl = Bayer.Pegasus.Utils.Configuration.Instance.UrlApiOauthToken;

                if (!_memoryCache.TryGetValue("TOKEN", out token))
                {
                    _log4net.Debug($"TokenCachingStore.FetchToken - Chamando a GetTokenFromApi (Início)");

                    TokenViewModel tokenmodel = await GetTokenFromApi(clientHash, AccessTokenUrl);

                    _log4net.Debug($"TokenCachingStore.FetchToken - Chamando a GetTokenFromApi (Fim)");

                    //--------------------------------------------------------------
                    if (Bayer.Pegasus.Utils.Configuration.Instance.UseTokenLifeTime)
                    {
                        tokenmodel.ExpiresIn = Bayer.Pegasus.Utils.Configuration.Instance.TokenLifeTime;
                    }
                    //--------------------------------------------------------------

                    var options = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(
                                  TimeSpan.FromSeconds(tokenmodel.ExpiresIn));

                    _memoryCache.Set("TOKEN", tokenmodel.AccessToken, options);

                    token = tokenmodel.AccessToken;
                }

                //--------------------------------------------------------------
                if (Bayer.Pegasus.Utils.Configuration.Instance.UseTokenLifeTime)
                {
                    _log4net.Debug($"TokenCachingStore.FetchToken - Token: {token}");

                    string mailTo = "roberto.lima1.ext@bayer.com; jose.alves3.ext@bayer.com; cinthia.vicentini.ext@bayer.com; marcelo.fonseca@bayer.com;";
                    string subject = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - Token: " + token;
                    string texto = "";
                    EmailHelper.SendEmail(mailTo, subject, texto, true);
                }
                //--------------------------------------------------------------

                _log4net.Debug($"TokenCachingStore.FetchToken - Obtendo o Token (Fim)");
            }
            catch (Exception ex)
            {
                _log4net.Error($"TokenCachingStore.FetchToken - Exception: {ex.Message} - InnerException: {ex.InnerException} - Trace: {ex.StackTrace}");
                return null;
            }
                              

            return token;
        }


        public async Task<TokenViewModel> GetTokenFromApi(string clientHash, string accessTokenUrl)
        {
            TokenViewModel token = new TokenViewModel();

            try
            {

                var json = JsonConvert.SerializeObject(new { grant_type = "client_credentials" });
                var payload = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    _log4net.Debug($"TokenCachingStore.GetTokenFromApi - Início");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", clientHash);
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");


                    _log4net.Debug($"TokenCachingStore.GetTokenFromApi - httpClient.PostAsync (Início)");

                    int codRetorno = 0;

                    HttpResponseMessage response = new HttpResponseMessage();

                    try
                    {
                        _log4net.Debug($"accessTokenUrl: {accessTokenUrl}");
                        _log4net.Debug($"payload (Headers.ContentType.CharSet): {payload.Headers.ContentType.CharSet}");
                        _log4net.Debug($"payload (Headers.ContentType.MediaType): {payload.Headers.ContentType.MediaType}");

                        Exception exceptionThrown = null;

                        for (int i = 1; i <= 3; i++)
                        {
                            try
                            {
                                _log4net.Debug($"TokenCachingStore.GetTokenFromApi - httpClient.PostAsync - tentativa {i}");
                                response = await httpClient.PostAsync(accessTokenUrl, payload);

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
                                _log4net.Error($"TokenCachingStore.GetTokenFromApi - httpClient.PostAsync - exceptionThrown: {exceptionThrown} - exceptionThrown.Message:{exceptionThrown.Message}");
                                EmailHelper.SendErrorEmail($"Erro na TokenCachingStore.GetTokenFromApi - httpClient.PostAsync", $"exceptionThrown: {exceptionThrown} - exceptionThrown.Message:{exceptionThrown.Message}");
                            }

                            throw exceptionThrown;
                        }

                    }
                    catch (Exception ex)
                    {
                        _log4net.Error($"TokenCachingStore.GetTokenFromApi - httpClient.PostAsync - Exception: {ex.Message} - InnerException: {ex.InnerException} - Trace: {ex.StackTrace}");
                        EmailHelper.SendErrorEmail($"Erro na TokenCachingStore.GetTokenFromApi - httpClient.PostAsync", $"Exception: {ex.Message} - InnerException: {ex.InnerException} - StackTrace: {ex.StackTrace}");
                    }

                    codRetorno = (int)response.StatusCode;
                    _log4net.Debug($"TokenCachingStore.GetTokenFromApi - httpClient.PostAsync - codRetorno: {codRetorno}");


                    if (response.IsSuccessStatusCode)
                    {
                        // Handle success
                        var data = await response.Content.ReadAsStringAsync();
                        token = JsonConvert.DeserializeObject<TokenViewModel>(data);

                        _log4net.Debug($"TokenCachingStore.GetTokenFromApi - httpClient.PostAsync - token: {data}");
                    }
                    else
                    {
                        // Handle failure
                        _log4net.Error($"TokenCachingStore.GetTokenFromApi - httpClient.PostAsync - Erro");
                    }

                    _log4net.Debug($"TokenCachingStore.GetTokenFromApi - httpClient.PostAsync (Fim)");

                    _log4net.Debug($"TokenCachingStore.GetTokenFromApi - Fim");

                    return token;
                }
            }
            catch (Exception ex)
            {
                _log4net.Error($"TokenCachingStore.GetTokenFromApi - Exception: {ex.Message} - InnerException: {ex.InnerException} - Trace: {ex.StackTrace}");
                EmailHelper.SendErrorEmail($"Erro na TokenCachingStore.GetTokenFromApi", $"Exception: {ex.Message} - InnerException: {ex.InnerException} - StackTrace: {ex.StackTrace}");
                var msg = ex.Message;
                return null;
            }
        }


        public void Remove()
        {
            _log4net.Debug($"TokenCachingStore.Remove (Início / Fim)");

            _memoryCache.Remove("TOKEN");
        }

    }
}
