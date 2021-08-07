using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Web.api
{
    [Produces("application/json")]
    [Route("api/Partner")]
    public class PartnerController : Controller
    {
        private IHostingEnvironment _env;
        private ILogger<PartnerController> _logger;
        private readonly ITokenStore _tokenStore;
        private AccessTokenViewModel _accessToken;

        public PartnerController(IHostingEnvironment env, ILogger<PartnerController> logger, ITokenStore tokenStore)
        {
            _env = env;
            _logger = logger;
            _tokenStore = tokenStore;
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
        }

        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<JArray> Get([FromBody]JObject data)
        {

            var partnerAPI = new Pegasus.ApiClient.PartnerApi(Bayer.Pegasus.Utils.Configuration.Instance.ServicePartnerURL);
            partnerAPI.ApiClient.WriteOnLog = Pegasus.Utils.Configuration.Instance.WriteOnLog;

            JArray filteredArray = new JArray();

            try
            {
                if (data != null)
                {

                    String search = data["search"].Value<String>();
                    if (!String.IsNullOrEmpty(search))
                    {

                        var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(User);

                        List<Entities.Partner> items = null;

                        if (Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenFieldPartner) {

                            string AccessTokenUrl = Utils.Configuration.Instance.UrlApiOauthToken;
                            string _token = await _tokenStore.FetchToken(_accessToken.ClientHash);

                            var route = Bayer.Pegasus.Utils.Configuration.Instance.ServicePartnerURL;

                            items = partnerAPI.ListPartners(null, null, true, null, null, null,
                                search, salesStructure.Level, String.Join(',', salesStructure.RestrictionCodes.ToArray()),
                                this._accessToken.ClientId, _token, route);

                            //Para realizar testes direto na api de produção "pegar tokem no POSTMAN"
                            //items = partnerAPI.ListPartners(null, null, true, null, null, null,
                            //search, salesStructure.Level, String.Join(',', salesStructure.RestrictionCodes.ToArray()),
                            //"b1c8c9b4-aa61-3678-84f8-397b5445fc87", "1a80af39-7e28-386a-9f73-4ef0d55a9a01", route);

                        }
                        else
                        {
                            using (var partnerBo = new Bayer.Pegasus.Business.PartnerBO()) {
                                items = partnerBo.GetPartners(salesStructure, search, true, null, null);
                            }
                        }

                        foreach (var item in items)
                        {
                            if (item.Cnpj.Length == 14)
                            {
                                JObject jobject = new JObject();
                                jobject["cnpj"] = item.Cnpj;
                                jobject["value"] = item.Code;
                                jobject["label"] = item.TradeName;
                                filteredArray.Add(jobject);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                var msg = ex.Message;
                 JObject jobject = new JObject();
                jobject["value"] = "";
                jobject["label"] = "Problema de Comunicação com o Serviço: " + ex.Message;
                filteredArray.Add(jobject);
            }


            if (Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenField) {
                if (partnerAPI.ApiClient.WriteOnLog && _logger != null)
                {

                    if (partnerAPI.ApiClient.LogInformation != null)
                    {
                        foreach (var message in partnerAPI.ApiClient.LogInformation)
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