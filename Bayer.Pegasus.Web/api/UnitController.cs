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
    [Route("api/Unit")]
    public class UnitController : Controller
    {

        private IHostingEnvironment _env;
        private ILogger<UnitController> _logger;

        private AccessTokenViewModel _accessToken;
        private readonly ITokenStore _tokenStore;

        public UnitController(IHostingEnvironment env, ILogger<UnitController> logger,
                              ITokenStore tokenStore)
        {
            _env = env;
            _logger = logger;
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
            _tokenStore = tokenStore;
        }


        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<JArray> Get([FromBody]JObject data)
        {

            string _tokenBU = await _tokenStore.FetchToken(_accessToken.ClientHash);

            JArray filteredArray = new JArray();

            var partnerAPI = new Pegasus.ApiClient.PartnerApi(Bayer.Pegasus.Utils.Configuration.Instance.ServicePartnerURL);
            partnerAPI.ApiClient.WriteOnLog = Pegasus.Utils.Configuration.Instance.WriteOnLog;

            try
            {
                if (data != null)
                {

                    String search = data["search"].Value<String>();
                    if (!String.IsNullOrEmpty(search))
                    {

                        List<string> partners = new List<string>(); ;

                        if (data["parents"] != null)
                        {
                            foreach (var partnerItem in data["parents"])
                            {
                                var partner = partnerItem["value"].Value<String>();

                                partners.Add(partner);
                            }
                        }

                        if (partners.Count == 0)
                        {
                            partners = null;
                        }

                        var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(User);

                        string queryPartners = null;

                        if (partners != null)
                        {
                            queryPartners = String.Join(',', partners.ToArray());
                        }

                        if (salesStructure.IsPartner)
                        {
                            queryPartners = String.Join(',', salesStructure.Partners.ToArray());
                        }

                        List<Entities.Partner> items = null;

                        if (Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenField)
                        {
                            items = partnerAPI.ListPartners(null, null, null, queryPartners, null, null, search,
                                                            salesStructure.Level, String.Join(',', salesStructure.RestrictionCodes.ToArray()),
                                                           this._accessToken.ClientId, _tokenBU,"");
                        }
                        else
                        {
                            using (var partnerBo = new Bayer.Pegasus.Business.PartnerBO())
                            {
                                string[] partnersArray = null;

                                if (salesStructure.IsPartner)
                                {
                                    partnersArray = salesStructure.Partners.ToArray();
                                }

                                if (partners != null)
                                {
                                    partnersArray = partners.ToArray();
                                }

                                items = partnerBo.GetPartners(salesStructure, search, null, partnersArray, null);
                            }
                        }

                        foreach (var item in items)
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
            catch (Exception ex)
            {
                JObject jobject = new JObject();
                jobject["value"] = "";
                jobject["label"] = "Problema de Comunicação com o Serviço";
                filteredArray.Add(jobject);
            }




            if (Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenField)
            {
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