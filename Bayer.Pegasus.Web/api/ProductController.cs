using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Web.api
{
    [Produces("application/json")]
    [Route("api/Product")]
    public class ProductController : Controller
    {
        private IHostingEnvironment _env;
        private ILogger<ProductController> _logger;
        private readonly ITokenStore _tokenStore;
        private AccessTokenViewModel _accessToken;

        private readonly IBusinessUnitCodeStore _businessUnitCodeStore;


        public ProductController(IHostingEnvironment env, ILogger<ProductController> logger
            , ITokenStore tokenStore
            , IBusinessUnitCodeStore businessUnitCodeStore)
        {
            _env = env;
            _logger = logger;
            _tokenStore = tokenStore;
            _businessUnitCodeStore = businessUnitCodeStore;
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);

        }



        private async Task<List<string>> ListaBuc(string businessGroupCodeList)
        {
            try
            {
                string _tokenBU = await _tokenStore.FetchToken(_accessToken.ClientHash);
                var routeLegaciesBU = Bayer.Pegasus.Utils.Configuration.Instance.ServiceProductLegaciesBUURL;

                var productLegacyBUAPI = new Pegasus.ApiClient.ProductApi(Bayer.Pegasus.Utils.Configuration.Instance.ServiceProductLegaciesBUURL);
                productLegacyBUAPI.ApiClient.WriteOnLog = Pegasus.Utils.Configuration.Instance.WriteOnLog;

                List<Entities.BusinessUnitsArrayList> itemsBuc = new List<Entities.BusinessUnitsArrayList>();
                var legacieBuc = productLegacyBUAPI.ListProductsLegaciesBU(businessGroupCodeList, this._accessToken.ClientId, _tokenBU, routeLegaciesBU);

                List<Entities.BusinessUnitsList> businessUnitsList = new List<Entities.BusinessUnitsList>();

                foreach (var item in legacieBuc)
                {
                    businessUnitsList = item.businessUnitsList;
                }

                List<String> lista = new List<String>();
                foreach (var item in businessUnitsList)
                {
                    lista.Add(item.businessUnitCode);
                }
                return lista;
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return null;
            }
        }



        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<JArray> Get([FromBody]JObject data)
        {

            JArray filteredArray = new JArray();
            List<Entities.Product> items = new List<Entities.Product>();

            try
            {
                if (data != null)
                {
                    String search = data["search"].Value<String>();

                    if (!String.IsNullOrEmpty(search))
                    {
                        List<string> brands = new List<string>();

                        if (data["parents"] != null)
                        {
                            foreach (var brandItem in data["parents"])
                            {
                                var brand = brandItem["value"].Value<String>();

                                brands.Add(brand);
                            }
                        }

                        if (Bayer.Pegasus.Utils.Configuration.Instance.UseServicesOnTokenFieldProducts)
                        {
                            var lista09 = _businessUnitCodeStore.ListBUC09();
                            List<string> list09 = new List<string>();

                            if (lista09 == null)
                            {
                                lista09 = await ListaBuc("09");
                                if (lista09 != null)
                                {
                                    list09 = _businessUnitCodeStore.ListaBUC09(lista09);
                                }
                            }
                            else
                            {
                                list09 = _businessUnitCodeStore.ListaBUC09(lista09);
                            }


                            
                            var lista15 = _businessUnitCodeStore.ListBUC15();
                            List<string> list15 = new List<string>();

                            if (lista15 == null)
                            {
                                lista15 = await ListaBuc("15");
                                if (lista15 != null)
                                {
                                    list15 = _businessUnitCodeStore.ListaBUC15(lista15);
                                }
                            }
                            else
                            {
                                list15 = _businessUnitCodeStore.ListaBUC15(lista15);
                            }



                            var lista17 = _businessUnitCodeStore.ListBUC17();
                            List<string> list17 = new List<string>();

                            if (lista17 == null)
                            {
                                lista17 = await ListaBuc("17");
                                if (lista17 != null)
                                {
                                    list17 = _businessUnitCodeStore.ListaBUC17(lista17);
                                }
                            }
                            else
                            {
                                list17 = _businessUnitCodeStore.ListaBUC17(lista17);
                            }



                            var lista80 = _businessUnitCodeStore.ListBUC80();
                            List<string> list80 = new List<string>();

                            if (lista80 == null)
                            {
                                lista80 = await ListaBuc("80");
                                if (lista80 != null)
                                {
                                    list80 = _businessUnitCodeStore.ListaBUC80(lista80);
                                }
                            }
                            else
                            {
                                list80 = _businessUnitCodeStore.ListaBUC80(lista80);
                            }

                            var lista27 = _businessUnitCodeStore.ListBUC27();
                            List<string> list27 = new List<string>();

                            if (lista27 == null)
                            {
                                lista27 = await ListaBuc("27");
                                if (lista27 != null)
                                {
                                    list27 = _businessUnitCodeStore.ListaBUC27(lista27);
                                }
                            }
                            else
                            {
                                list27 = _businessUnitCodeStore.ListaBUC27(lista27);
                            }



                            var productLegacyAPI = new Pegasus.ApiClient.ProductApi(Bayer.Pegasus.Utils.Configuration.Instance.ServiceProductLegaciesURL);
                            productLegacyAPI.ApiClient.WriteOnLog = Pegasus.Utils.Configuration.Instance.WriteOnLog;

                            var productAPI = new Pegasus.ApiClient.ProductApi(Bayer.Pegasus.Utils.Configuration.Instance.ServiceProductURL);
                            productAPI.ApiClient.WriteOnLog = Pegasus.Utils.Configuration.Instance.WriteOnLog;

                            var routeLegacies = Bayer.Pegasus.Utils.Configuration.Instance.ServiceProductLegaciesURL;

                            string _token = await _tokenStore.FetchToken(_accessToken.ClientHash);

                            if (list09.Count > 0)
                            {
                                var legacie09 = productLegacyAPI.ListProductsLegacies("09", search, list09, this._accessToken.ClientId, _token, routeLegacies);

                                if (legacie09 != null)
                                {
                                    items.AddRange(legacie09);
                                }
                            }

                            if (list15.Count > 0)
                            {
                                var legacie15 = productLegacyAPI.ListProductsLegacies("15", search, list15, this._accessToken.ClientId, _token, routeLegacies);

                                if (legacie15 != null)
                                {
                                    items.AddRange(legacie15);
                                }

                            }

                            if (list17.Count > 0)
                            {
                                var legacie17 = productLegacyAPI.ListProductsLegacies("17", search, list17, this._accessToken.ClientId, _token, routeLegacies);

                                if (legacie17 != null)
                                {
                                    items.AddRange(legacie17);
                                }
                            }

                            if (list80.Count > 0)
                            {
                                var legacie80 = productLegacyAPI.ListProductsLegacies("80", search, list80, this._accessToken.ClientId, _token, routeLegacies);

                                if (legacie80 != null)
                                {
                                    items.AddRange(legacie80);
                                }
                            }

                            //------------------------------------------------------------------------
                            if (list27.Count > 0)
                            {
                                var legacie27 = productLegacyAPI.ListProductsLegacies("27", search, list27, this._accessToken.ClientId, _token, routeLegacies);


                                if (legacie27 != null)
                                {
                                    items.AddRange(legacie27);
                                }
                            }
                            //------------------------------------------------------------------------

                            if (productLegacyAPI.ApiClient.WriteOnLog && _logger != null)
                            {

                                if (productLegacyAPI.ApiClient.LogInformation != null)
                                {
                                    foreach (var message in productLegacyAPI.ApiClient.LogInformation)
                                    {
                                        _logger.LogInformation(message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var productBO = new Bayer.Pegasus.Business.ProductBO())
                            {
                                items = productBO.GetProducts(search, brands);
                            }
                        }

                        foreach (var item in items)
                        {
                            JObject jobject = new JObject();
                            jobject["value"] = item.Code;
                            jobject["label"] =    item.Name.Substring(0,24);
                            filteredArray.Add(jobject);
                        }

                    }
                }
                
            }
            catch (Exception ex)
            {
                JObject jobject = new JObject();
                jobject["value"] = "";
                jobject["label"] = "ProductController - Problema de Comunicação com o Serviço" + " - " + ex.StackTrace;
                filteredArray.Add(jobject);
            }



            return filteredArray;
        }
    }
}