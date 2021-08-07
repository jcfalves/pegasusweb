using Bayer.Pegasus.ApiClient;
using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Web.Controllers
{

    [Route("salesstructure")]
    public sealed class SalesStructureController : Controller
    {
        public Boolean WriteOnLog
        {
            get;
            set;
        }

        public List<string> LogInformation
        {
            get;
            set;
        }

        public string BasePath { get; set; }

        private readonly Dictionary<String, String> _defaultHeaderMap = new Dictionary<String, String>();
        #region Private Read-Only Fields
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private AccessTokenViewModel _accessToken;
        private readonly ITokenStore _tokenStore;
        #endregion        

        #region Constructor

        public SalesStructureController(ITokenStore tokenStore, IMemoryCache cache) : base()
        {
            _accessToken = new AccessTokenViewModel(Utils.Configuration.Instance.ClientId, Utils.Configuration.Instance.ClientSecret);
            _httpClient = new HttpClient();
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _tokenStore = tokenStore;
        }
        #endregion

        #region Controller Actions


        [HttpGet, Route("GetSalesStructure")]
        [Consumes("application/json")]
        public async Task<SalesStructureModel> GetSalesStructure()
        {
            try
            {
                string token = await _tokenStore.FetchToken(_accessToken.ClientHash);

                var authenticationBO = new Bayer.Pegasus.Business.SalesStructureBO();
                var output = await authenticationBO.GetSalesStructure(token, _accessToken.ClientId);

                return output;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw ex;
            }
        }
        #endregion


        #region Controller Actions
        [HttpGet, Route("GetSalesStructureQry")]
        [Consumes("application/json")]
        public async Task<SalesStructureModel> GetSalesStructureQry(
                       [FromQuery]bool? withPartners,
                       [FromQuery]string salesDistrictCode,
                       [FromQuery]string salesOfficeCode,
                       [FromQuery]string salesRepCode,
                       [FromQuery]string referenceDate,
                       [FromQuery]string repMainFunctionFlag,
                       [FromQuery]string countryCode,
                       [FromQuery]string salesOrgCode,
                       [FromHeader]string level,
                       [FromHeader]string restrictionCodes)
        {
            bool withPartnersBool = Convert.ToBoolean(Convert.ToInt32(withPartners));

            try
            {
                string route = Bayer.Pegasus.Utils.Configuration.Instance.ServiceSalesStructureURL;
                string AccessTokenUrl = Utils.Configuration.Instance.UrlApiOauthToken;
                string token = await _tokenStore.FetchToken(_accessToken.ClientHash);

                var queryParams = new Dictionary<String, String>();
                var headerParams = new Dictionary<String, String>();
                var formParams = new Dictionary<String, String>();
                var fileParams = new Dictionary<String, FileParameter>();
                String postBody = null;

                if (withPartners != null) queryParams.Add("withPartners", ParameterToString(withPartners)); // query parameter
                if (salesDistrictCode != null) queryParams.Add("salesDistrictCode", ParameterToString(salesDistrictCode)); // query parameter
                if (salesOfficeCode != null) queryParams.Add("salesOfficeCode", ParameterToString(salesOfficeCode)); // query parameter
                if (salesRepCode != null) queryParams.Add("salesRepCode", ParameterToString(salesRepCode)); // query parameter
                if (referenceDate != null) queryParams.Add("referenceDate", ParameterToString(referenceDate)); // query parameter
                if (repMainFunctionFlag != null) queryParams.Add("repMainFunctionFlag", ParameterToString(repMainFunctionFlag)); // query parameter
                if (countryCode != null) queryParams.Add("countryCode", ParameterToString(countryCode)); // query parameter
                if (salesOrgCode != null) queryParams.Add("salesOrgCode", ParameterToString(salesOrgCode)); // header parameter
                if (level != null) headerParams.Add("level", ParameterToString(level)); // header parameter
                if (restrictionCodes != null) headerParams.Add("restrictionCodes", ParameterToString(restrictionCodes)); // header parameter

                IRestResponse response = (IRestResponse)CallApi(route, Method.GET, queryParams,
                       postBody, headerParams, formParams, fileParams, this._accessToken.ClientId, token);

                if (((int)response.StatusCode) >= 400)
                    throw new ApiException((int)response.StatusCode, "Error calling SalesStructure: " + response.Content, response.Content);
                else if (((int)response.StatusCode) == 0)
                    throw new ApiException((int)response.StatusCode, "Error calling SalesStructure: " + response.ErrorMessage, response.ErrorMessage);

                SalesStructureModel resultSalesStructure =
                 JsonConvert.DeserializeObject<SalesStructureModel>(response.Content);
                return resultSalesStructure;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw null;
            }
        }

        public string ParameterToString(object obj)
        {
            if (obj is DateTime)
                return ((DateTime)obj).ToString(Configuration.DateTimeFormat);
            else if (obj is List<string>)
                return String.Join(",", (obj as List<string>).ToArray());
            else
                return Convert.ToString(obj);
        }

        public static Object ConvertType(Object source, Type dest)
        {
            return Convert.ChangeType(source, dest);
        }

        public Object CallApi(String path, RestSharp.Method method, Dictionary<String, String> queryParams, String postBody,
              Dictionary<String, String> headerParams, Dictionary<String, String> formParams,
              Dictionary<String, FileParameter> fileParams, string clientId, string _token)
        {
            try
            {
                var request = new RestRequest(path, method);

                request.AddHeader("client_id", clientId);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("access_token", _token);

                // add default header, if any
                foreach (var defaultHeader in _defaultHeaderMap)
                    request.AddHeader(defaultHeader.Key, defaultHeader.Value);

                // add header parameter, if any
                foreach (var param in headerParams)
                    request.AddHeader(param.Key, param.Value);

                // add query parameter, if any
                foreach (var param in queryParams)
                    request.AddParameter(param.Key, param.Value, ParameterType.GetOrPost);

                // add form parameter, if any
                foreach (var param in formParams)
                    request.AddParameter(param.Key, param.Value, ParameterType.GetOrPost);


                if (postBody != null) // http body (model) parameter
                    request.AddParameter("application/json", postBody, ParameterType.RequestBody);

                RestClient rc = new RestClient(path);
                var response = rc.Execute(request);


                if (WriteOnLog)
                {
                    LogInformation = new List<string>();
                    LogInformation.Add("APIClient-Service:  " + BasePath + path);
                    LogInformation.Add("APIClient-Token: " + _token);

                    foreach (var defaultHeader in _defaultHeaderMap)
                        LogInformation.Add("APIClient-HeaderDefault:  " + defaultHeader.Key + "=" + defaultHeader.Value);

                    // add header parameter, if any
                    foreach (var param in headerParams)
                        LogInformation.Add("APIClient-Header:  " + param.Key + "=" + param.Value);


                    // add query parameter, if any
                    foreach (var param in queryParams)
                        LogInformation.Add("APIClient-Query:  " + param.Key + "=" + param.Value);

                    // add form parameter, if any
                    foreach (var param in formParams)
                        LogInformation.Add("APIClient-Form:  " + param.Key + "=" + param.Value);


                    if (postBody != null) // http body (model) parameter
                        LogInformation.Add("APIClient-PostBody:  " + postBody);

                    if (response != null)
                    {
                        LogInformation.Add("APIClient-ResponseCode:  " + response.StatusCode);

                        if (response.Content != null)
                            LogInformation.Add("APIClient-ResponseContent:  " + response.Content);

                        if (response.ErrorMessage != null)
                            LogInformation.Add("APIClient-ResponseError:  " + response.ErrorMessage);
                    }
                }
                return (Object)response;

            }
            catch (Exception exc)
            {
                var msg = exc.Message;
                return null;
            }
        }        
        #endregion
    }
}

