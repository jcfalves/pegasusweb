using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Stores;

namespace Bayer.Pegasus.Web.api
{
    [Produces("application/json")]
    [Route("api/Brand")]
    public class BrandController : Controller
    {
        private IHostingEnvironment _env;
        private ILogger<BrandController> _logger;
        private AccessTokenViewModel _accessToken;
        private readonly ITokenStore _tokenStore;

        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(BrandController));


        public BrandController(IHostingEnvironment env, ILogger<BrandController> logger
            , ITokenStore tokenStore)
        {
            _log4net.Debug($"BrandController - Construtor (Início)");
            _env = env;
            _logger = logger;
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
            _tokenStore = tokenStore;
            _log4net.Debug($"BrandController - Construtor (Fim)");
        }


        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<JArray> Get([FromBody]JObject data)
        {
            
            JArray filteredArray = new JArray();

            string _tokenBU = await _tokenStore.FetchToken(_accessToken.ClientHash);

            var brandAPi = new Pegasus.ApiClient.ProductApi(Bayer.Pegasus.Utils.Configuration.Instance.ServiceProductURL);
            brandAPi.ApiClient.WriteOnLog = Pegasus.Utils.Configuration.Instance.WriteOnLog;

            try
            {
                if (data != null)
                {
                    var search = data["search"].Value<String>();

                    if (!String.IsNullOrEmpty(search))
                    {

                        List<Entities.Brand> brands = new List<Entities.Brand>();

                        if (Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenFieldBrand)
                        {
                            _log4net.Debug($" brandAPi.ListBrand: {search}");
                            _log4net.Debug($" brandAPi.ListBrand: {this._accessToken.ClientId}");                            
                            _log4net.Debug($" brandAPi.ListBrand: {_tokenBU}");
                            _log4net.Debug($" brandAPi.ListBrand: {brandAPi.ApiClient.BasePath}");

                            try
                            {
                                brands = brandAPi.ListBrand(search, null, null, null, this._accessToken.ClientId, _tokenBU)
                                      .Where(c => c.Name.ToLower().StartsWith(search.ToLower())).OrderBy(c => c.Name).Distinct().ToList();
                            }
                            catch (Exception ex)
                            {
                                _log4net.Debug($" brandAPi.ListBrand: {ex.Message + " -- " + ex.StackTrace}");
                            }

                        }
                        else
                        {
                            using (var productBO = new Bayer.Pegasus.Business.ProductBO())
                            {
                                brands = productBO.GetBrands(search);
                            }
                        }

                        foreach (var brand in brands)
                        {
                            JObject jobject = new JObject();
                            jobject["label"] = brand.Name;
                            jobject["value"] = brand.Name;
                            filteredArray.Add(jobject);
                        }

                    }
                }

            }
            catch (Exception ex) {

                _log4net.Debug($"BrandController ex - token: {ex.StackTrace + " " + ex.Message}");

                JObject jobject = new JObject();
                jobject["value"] = "" ;
                jobject["label"] = "BrandController " + ex.StackTrace + "  -  " + ex.Message;
                filteredArray.Add(jobject);
            }


            if (Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenField)
            {
                if (brandAPi.ApiClient.WriteOnLog && _logger != null)
                {

                    if (brandAPi.ApiClient.LogInformation != null)
                    {
                        foreach (var message in brandAPi.ApiClient.LogInformation)
                        {
                            _logger.LogInformation(message);
                        }
                    }
                }
            }
                
            return filteredArray;
        }
    }

}