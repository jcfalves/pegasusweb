using Bayer.Pegasus.Entities;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Bayer.Pegasus.ApiClient
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IPartnerApi
    {
        /// <summary>
        ///  EN - List all active partner  PT - Lista todos os parceiros ativos 
        /// </summary>
        /// <param name="crmCodesList">EN - List of Partner codes on CRM PT - Lista de códigos de Parceiro no CRM </param>
        /// <param name="countryCode">EN - Country Code ISO-ALPHA2. Eg. BR PT - Codigo do Pais ISO-ALPHA2. Ex. BR </param>
        /// <param name="isHeadquarter">EN - Headquarter flag PT - Flag de Matriz </param>
        /// <param name="partnerHeadquarterCodesList">EN - Array of code partner headquarter. Values must be CSV  PT - Lista de codigos da Matriz do Parceiro. Os valores devem estar no formato CSV. </param>
        /// <param name="partnerType">EN - Partner type PT - Tipo de Parceiro </param>
        /// <param name="partnerStatus">EN - Status PT - Status </param>
        /// <param name="genericSearch">EN - Search value for company name/CNPJ/SAP code PT - Pesquisa generica para Razao Social/CNPJ/Codigo SAP </param>
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param>
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param>
        /// <returns>PartnerSingleArray</returns>
        List<Partner> ListPartners(string crmCodesList, string countryCode, bool? isHeadquarter, string partnerHeadquarterCodesList, string partnerType, string partnerStatus, string genericSearch, string level, string restrictionCodes);
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class PartnerApi : IPartnerApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public PartnerApi(ApiClient apiClient = null)
        {
            if (apiClient == null) // use the default one in Configuration
                this.ApiClient = Configuration.DefaultApiClient;
            else
                this.ApiClient = apiClient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApi"/> class.
        /// </summary>
        /// <returns></returns>
        public PartnerApi(String basePath)
        {
            this.ApiClient = new ApiClient(basePath);
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <param name="basePath">The base path</param>
        /// <value>The base path</value>
        public void SetBasePath(String basePath)
        {
            this.ApiClient.BasePath = basePath;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <param name="basePath">The base path</param>
        /// <value>The base path</value>
        public String GetBasePath(String basePath)
        {
            return this.ApiClient.BasePath;
        }

        /// <summary>
        /// Gets or sets the API client.
        /// </summary>
        /// <value>An instance of the ApiClient</value>
        public ApiClient ApiClient { get; set; }

        
        /// <summary>
        ///  EN - List all active partner  PT - Lista todos os parceiros ativos 
        /// </summary>
        /// <param name="crmCodesList">EN - List of Partner codes on CRM PT - Lista de códigos de Parceiro no CRM </param> 
        /// <param name="countryCode">EN - Country Code ISO-ALPHA2. Eg. BR PT - Codigo do Pais ISO-ALPHA2. Ex. BR </param> 
        /// <param name="isHeadquarter">EN - Headquarter flag PT - Flag de Matriz </param> 
        /// <param name="partnerHeadquarterCodesList">EN - Array of code partner headquarter. Values must be CSV  PT - Lista de codigos da Matriz do Parceiro. Os valores devem estar no formato CSV. </param> 
        /// <param name="partnerType">EN - Partner type PT - Tipo de Parceiro </param> 
        /// <param name="partnerStatus">EN - Status PT - Status </param> 
        /// <param name="genericSearch">EN - Search value for company name/CNPJ/SAP code PT - Pesquisa generica para Razao Social/CNPJ/Codigo SAP </param> 
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param> 
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param> 
        /// <returns>PartnerSingleArray</returns>            
        public List<Partner> ListPartners(string crmCodesList, string countryCode, bool? isHeadquarter, 
                                          string partnerHeadquarterCodesList, string partnerType, string partnerStatus, 
                                          string genericSearch, string level, string restrictionCodes)
        {


            var path = "/partners";
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (crmCodesList != null) queryParams.Add("crmCodesList", ApiClient.ParameterToString(crmCodesList)); // query parameter
            if (countryCode != null) queryParams.Add("countryCode", ApiClient.ParameterToString(countryCode)); // query parameter
            if (isHeadquarter != null) queryParams.Add("isHeadquarter", ApiClient.ParameterToString(isHeadquarter)); // query parameter
            if (partnerHeadquarterCodesList != null) queryParams.Add("partnerHeadquarterCodesList", ApiClient.ParameterToString(partnerHeadquarterCodesList)); // query parameter
            if (partnerType != null) queryParams.Add("partnerType", ApiClient.ParameterToString(partnerType)); // query parameter
            if (partnerStatus != null) queryParams.Add("partnerStatus", ApiClient.ParameterToString(partnerStatus)); // query parameter
            if (genericSearch != null) queryParams.Add("genericSearch", ApiClient.ParameterToString(genericSearch)); // query parameter
            if (level != null) headerParams.Add("level", ApiClient.ParameterToString(level)); // header parameter
            if (restrictionCodes != null) headerParams.Add("restrictionCodes", ApiClient.ParameterToString(restrictionCodes)); // header parameter
            
            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, 
                                    formParams, fileParams);

            if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListPartners: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListPartners: " + response.ErrorMessage, response.ErrorMessage);

            return (List<Partner>)ApiClient.Deserialize(response.Content, typeof(List<Partner>), response.Headers);
        }



        public List<Partner> ListPartners(string crmCodesList, string countryCode, 
            bool? isHeadquarter, string partnerHeadquarterCodesList, string partnerType, 
            string partnerStatus, string genericSearch, string level, string restrictionCodes,
            string clientId, string token, string route)
        {

            var path = route;
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (crmCodesList != null) queryParams.Add("crmCodesList", ApiClient.ParameterToString(crmCodesList)); // query parameter
            if (countryCode != null) queryParams.Add("countryCode", ApiClient.ParameterToString(countryCode)); // query parameter
            if (isHeadquarter != null) queryParams.Add("isHeadquarter", ApiClient.ParameterToString(isHeadquarter)); // query parameter
            if (partnerHeadquarterCodesList != null) queryParams.Add("partnerHeadquarterCodesList", ApiClient.ParameterToString(partnerHeadquarterCodesList)); // query parameter
            if (partnerType != null) queryParams.Add("partnerType", ApiClient.ParameterToString(partnerType)); // query parameter
            if (partnerStatus != null) queryParams.Add("partnerStatus", ApiClient.ParameterToString(partnerStatus)); // query parameter
            if (genericSearch != null) queryParams.Add("genericSearch", ApiClient.ParameterToString(genericSearch)); // query parameter
            if (level != null) headerParams.Add("level", ApiClient.ParameterToString(level)); // header parameter
            if (restrictionCodes != null) headerParams.Add("restrictionCodes", ApiClient.ParameterToString(restrictionCodes)); // header parameter

            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient
                .CallApi(path, Method.GET, queryParams,
                postBody, headerParams, formParams, fileParams,
                clientId, token);

            if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListPartners: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListPartners: " + response.ErrorMessage, response.ErrorMessage);

            return (List<Partner>)ApiClient.Deserialize(response.Content, typeof(List<Partner>), response.Headers);
        }

    }

}
