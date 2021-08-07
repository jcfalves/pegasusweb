using System;
using System.Collections.Generic;
using RestSharp;
using Bayer.Pegasus.Entities.SalesStructure;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace Bayer.Pegasus.ApiClient
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ISalesStructureAPI
    {
        /// <summary>
        /// List all active sales districts EN - List all active sales district PT - Listagem de todas as diretorias de negocio ativas 
        /// </summary>
        /// <param name="countryCode">EN - Country Code ISO ALPHA-2. Eg. BR  PT - Codigo do pais ISO ALPHA-2. Ex. BR </param>
        /// <param name="salesOrgCode">EN - Sales Organization Code  PT - Codigo da Organizacao de Vendas </param>
        /// <param name="referenceDate">Format - date (as full-date in RFC3339). EN - Reference Date for Sales Structure Vigency - Default&#x3D; Today PT- Data de referencia para a vigencia da Estrutura de Vendas - Valor Padrao &#x3D; Hoje </param>
        /// <param name="directorCwid">EN - Director cwid PT - CWID do diretor </param>
        /// <param name="coordinatorCwid">EN - Coordinator cwid PT - CWID do coordenador </param>
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param>
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param>
        /// <returns>SalesDistrictSingleArray</returns>
        SalesDistrictSingleArray ListSalesDistrict(string countryCode, string salesOrgCode, string referenceDate, string directorCwid, string coordinatorCwid, string level, string restrictionCodes);
        /// <summary>
        /// List all active sales office EN - List all active sales office PT - Lista todas as Regionais ativas 
        /// </summary>
        /// <param name="salesDistrictCodesList">EN - Array of Sales district code to filter. Values must be CSV PT - Lista de codigos de Diretoria de Negocio para filtro. Os Valores devem estar no formato CSV </param>
        /// <param name="countryCode">EN - Country Code ISO ALPHA-2. Eg. BR  PT - Codigo do pais ISO ALPHA-2. Ex. BR </param>
        /// <param name="salesOrgCode">EN - Sales Organization Code  PT - Codigo da Organizacao de Vendas </param>
        /// <param name="referenceDate">Format - date (as full-date in RFC3339). EN - Reference Date for Sales Structure Vigency - Default&#x3D; Today PT- Data de referencia para a vigencia da Estrutura de Vendas - Valor Padrao &#x3D; Hoje </param>
        /// <param name="managerCwid">EN - Manager cwid PT - CWID do gerente </param>
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param>
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param>
        /// <returns>SalesOfficeSingleArray</returns>
        SalesOfficeSingleArray ListSalesOffice(string salesDistrictCodesList, string countryCode, string salesOrgCode, string referenceDate, string managerCwid, string level, string restrictionCodes);
        /// <summary>
        /// List all active sales representative EN - List all active sales representative PT - Lista todos os Representantes de Vendas ativos 
        /// </summary>
        /// <param name="salesDistrictCodesList">EN - Array of Sales district code to filter. Values must be CSV PT - Lista de codigos de Diretoria de Negocio para filtro. Os Valores devem estar no formato CSV </param>
        /// <param name="salesOfficeCodesList">EN - Array of code sales office. Values must be CSV PT - Lista de codigos de Reginal. Os valores devem estar no formato CSV. </param>
        /// <param name="positionStatus">EN - Position Status (Open|Active|Inative) PT - Status da posicao (Aberto|Ativo|Inativo) </param>
        /// <param name="countryCode">EN - Country Code ISO ALPHA-2. Eg. BR  PT - Codigo do pais ISO ALPHA-2. Ex. BR </param>
        /// <param name="salesOrgCode">EN - Sales Organization Code  PT - Codigo da Organizacao de Vendas </param>
        /// <param name="referenceDate">Format - date (as full-date in RFC3339). EN - Reference Date for Sales Structure Vigency - Default&#x3D; Today PT- Data de referencia para a vigencia da Estrutura de Vendas - Valor Padrao &#x3D; Hoje </param>
        /// <param name="employeeCwid">EN - Employee CWID PT - CWID do empregado </param>
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param>
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param>
        /// <returns>SalesRepresentativeSingleArray</returns>
        SalesRepresentativeSingleArray ListSalesRepresentative(string salesDistrictCodesList, string salesOfficeCodesList, string positionStatus, string countryCode, string salesOrgCode, string referenceDate, string employeeCwid, string level, string restrictionCodes);
        /// <summary>
        /// List sales structure for a given filter EN - List all active sales representative   Parameter withPartners will bring Partners list on Representative PT - Lista todos os Representantes de Vendas ativos   Parametro withPartners ira trazer lista de Parceiros no RTV 
        /// </summary>
        /// <param name="withPartners">EN - Bring partners on structure PT - Trazer parceiros na estrutura </param>
        /// <param name="salesDistrictCode">EN - Sales district code to filter. PT - codigo de Diretoria de Negocio para filtro. </param>
        /// <param name="salesOfficeCode">EN - sales office code. PT - codigo da Regional. </param>
        /// <param name="salesRepCode">EN - code sales representative code. PT - codigo de RTV. </param>
        /// <param name="referenceDate">Format - date (as full-date in RFC3339). EN - Reference Date for Sales Structure Vigency - Default&#x3D; Today PT- Data de referencia para a vigencia da Estrutura de Vendas - Valor Padrao &#x3D; Hoje </param>
        /// <param name="repMainFunctionFlag">EN - Representative main function PT - Funcao principal do RTV </param>
        /// <param name="countryCode">EN - Country Code ISO ALPHA-2. Eg. BR  PT - Codigo do pais ISO ALPHA-2. Ex. BR </param>
        /// <param name="salesOrgCode">EN - Sales Organization Code  PT - Codigo da Organizacao de Vendas </param>
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param>
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param>
        /// <returns>SalesStructure</returns>
        SalesStructure ListSalesStructure(bool? withPartners, string salesDistrictCode, string salesOfficeCode, string salesRepCode, string referenceDate, string repMainFunctionFlag, string countryCode, string salesOrgCode, string level, string restrictionCodes);
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class SalesStructureAPI : ISalesStructureAPI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public SalesStructureAPI(ApiClient apiClient = null)
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
        public SalesStructureAPI(String basePath)
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
        public string clientId { get; set; }
        public string token { get; set; }

        /// <summary>
        /// List all active sales districts EN - List all active sales district PT - Listagem de todas as diretorias de negocio ativas 
        /// </summary>
        /// <param name="countryCode">EN - Country Code ISO ALPHA-2. Eg. BR  PT - Codigo do pais ISO ALPHA-2. Ex. BR </param> 
        /// <param name="salesOrgCode">EN - Sales Organization Code  PT - Codigo da Organizacao de Vendas </param> 
        /// <param name="referenceDate">Format - date (as full-date in RFC3339). EN - Reference Date for Sales Structure Vigency - Default&#x3D; Today PT- Data de referencia para a vigencia da Estrutura de Vendas - Valor Padrao &#x3D; Hoje </param> 
        /// <param name="directorCwid">EN - Director cwid PT - CWID do diretor </param> 
        /// <param name="coordinatorCwid">EN - Coordinator cwid PT - CWID do coordenador </param> 
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param> 
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param> 
        /// <returns>SalesDistrictSingleArray</returns>            
        public SalesDistrictSingleArray ListSalesDistrict(string countryCode, string salesOrgCode, string referenceDate, string directorCwid, string coordinatorCwid, string level, string restrictionCodes)
        {

            var path = "/sales-districts";
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (countryCode != null) queryParams.Add("countryCode", ApiClient.ParameterToString(countryCode)); // query parameter
            if (salesOrgCode != null) queryParams.Add("salesOrgCode", ApiClient.ParameterToString(salesOrgCode)); // query parameter
            if (referenceDate != null) queryParams.Add("referenceDate", ApiClient.ParameterToString(referenceDate)); // query parameter
            if (directorCwid != null) queryParams.Add("directorCwid", ApiClient.ParameterToString(directorCwid)); // query parameter
            if (coordinatorCwid != null) queryParams.Add("coordinatorCwid", ApiClient.ParameterToString(coordinatorCwid)); // query parameter
            if (level != null) headerParams.Add("level", ApiClient.ParameterToString(level)); // header parameter
            if (restrictionCodes != null) headerParams.Add("restrictionCodes", ApiClient.ParameterToString(restrictionCodes)); // header parameter
            
            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams);


            if (((int)response.StatusCode) >= 404)
                return new SalesDistrictSingleArray();
            else if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesDistrict: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesDistrict: " + response.ErrorMessage, response.ErrorMessage);

            return (SalesDistrictSingleArray)ApiClient.Deserialize(response.Content, typeof(SalesDistrictSingleArray), response.Headers);
        }

        /// <summary>
        /// List all active sales office EN - List all active sales office PT - Lista todas as Regionais ativas 
        /// </summary>
        /// <param name="salesDistrictCodesList">EN - Array of Sales district code to filter. Values must be CSV PT - Lista de codigos de Diretoria de Negocio para filtro. Os Valores devem estar no formato CSV </param> 
        /// <param name="countryCode">EN - Country Code ISO ALPHA-2. Eg. BR  PT - Codigo do pais ISO ALPHA-2. Ex. BR </param> 
        /// <param name="salesOrgCode">EN - Sales Organization Code  PT - Codigo da Organizacao de Vendas </param> 
        /// <param name="referenceDate">Format - date (as full-date in RFC3339). EN - Reference Date for Sales Structure Vigency - Default&#x3D; Today PT- Data de referencia para a vigencia da Estrutura de Vendas - Valor Padrao &#x3D; Hoje </param> 
        /// <param name="managerCwid">EN - Manager cwid PT - CWID do gerente </param> 
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param> 
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param> 
        /// <returns>SalesOfficeSingleArray</returns>            
        public SalesOfficeSingleArray ListSalesOffice(string salesDistrictCodesList, string countryCode, string salesOrgCode, string referenceDate, string managerCwid, string level, string restrictionCodes)
        {
            
            var path = "/sales-offices";
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (salesDistrictCodesList != null) queryParams.Add("salesDistrictCodesList", ApiClient.ParameterToString(salesDistrictCodesList)); // query parameter
            if (countryCode != null) queryParams.Add("countryCode", ApiClient.ParameterToString(countryCode)); // query parameter
            if (salesOrgCode != null) queryParams.Add("salesOrgCode", ApiClient.ParameterToString(salesOrgCode)); // query parameter
            if (referenceDate != null) queryParams.Add("referenceDate", ApiClient.ParameterToString(referenceDate)); // query parameter
            if (managerCwid != null) queryParams.Add("managerCwid", ApiClient.ParameterToString(managerCwid)); // query parameter
            if (level != null) headerParams.Add("level", ApiClient.ParameterToString(level)); // header parameter
            if (restrictionCodes != null) headerParams.Add("restrictionCodes", ApiClient.ParameterToString(restrictionCodes)); // header parameter
            
            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams);

            if (((int)response.StatusCode) >= 404)
                return new SalesOfficeSingleArray();
            else if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesOffice: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesOffice: " + response.ErrorMessage, response.ErrorMessage);

            return (SalesOfficeSingleArray)ApiClient.Deserialize(response.Content, typeof(SalesOfficeSingleArray), response.Headers);
        }

        /// <summary>
        /// List all active sales representative EN - List all active sales representative PT - Lista todos os Representantes de Vendas ativos 
        /// </summary>
        /// <param name="salesDistrictCodesList">EN - Array of Sales district code to filter. Values must be CSV PT - Lista de codigos de Diretoria de Negocio para filtro. Os Valores devem estar no formato CSV </param> 
        /// <param name="salesOfficeCodesList">EN - Array of code sales office. Values must be CSV PT - Lista de codigos de Reginal. Os valores devem estar no formato CSV. </param> 
        /// <param name="positionStatus">EN - Position Status (Open|Active|Inative) PT - Status da posicao (Aberto|Ativo|Inativo) </param> 
        /// <param name="countryCode">EN - Country Code ISO ALPHA-2. Eg. BR  PT - Codigo do pais ISO ALPHA-2. Ex. BR </param> 
        /// <param name="salesOrgCode">EN - Sales Organization Code  PT - Codigo da Organizacao de Vendas </param> 
        /// <param name="referenceDate">Format - date (as full-date in RFC3339). EN - Reference Date for Sales Structure Vigency - Default&#x3D; Today PT- Data de referencia para a vigencia da Estrutura de Vendas - Valor Padrao &#x3D; Hoje </param> 
        /// <param name="employeeCwid">EN - Employee CWID PT - CWID do empregado </param> 
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param> 
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param> 
        /// <returns>SalesRepresentativeSingleArray</returns>            
        public SalesRepresentativeSingleArray ListSalesRepresentative(string salesDistrictCodesList, string salesOfficeCodesList, string positionStatus, string countryCode, string salesOrgCode, string referenceDate, string employeeCwid, string level, string restrictionCodes)
        {


            var path = "/sales-representatives";
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (salesDistrictCodesList != null) queryParams.Add("salesDistrictCodesList", ApiClient.ParameterToString(salesDistrictCodesList)); // query parameter
            if (salesOfficeCodesList != null) queryParams.Add("salesOfficeCodesList", ApiClient.ParameterToString(salesOfficeCodesList)); // query parameter
            if (positionStatus != null) queryParams.Add("positionStatus", ApiClient.ParameterToString(positionStatus)); // query parameter
            if (countryCode != null) queryParams.Add("countryCode", ApiClient.ParameterToString(countryCode)); // query parameter
            if (salesOrgCode != null) queryParams.Add("salesOrgCode", ApiClient.ParameterToString(salesOrgCode)); // query parameter
            if (referenceDate != null) queryParams.Add("referenceDate", ApiClient.ParameterToString(referenceDate)); // query parameter
            if (employeeCwid != null) queryParams.Add("employeeCwid", ApiClient.ParameterToString(employeeCwid)); // query parameter
            if (level != null) headerParams.Add("level", ApiClient.ParameterToString(level)); // header parameter
            if (restrictionCodes != null) headerParams.Add("restrictionCodes", ApiClient.ParameterToString(restrictionCodes)); // header parameter
            
            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient.CallApi (path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams);
            

            if (((int)response.StatusCode) >= 404)
                return new SalesRepresentativeSingleArray();
            else if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesRepresentative: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesRepresentative: " + response.ErrorMessage, response.ErrorMessage);
            
            return (SalesRepresentativeSingleArray)ApiClient.Deserialize(response.Content, typeof(SalesRepresentativeSingleArray), response.Headers);
        }

        /// <summary>
        /// List sales structure for a given filter EN - List all active sales representative   Parameter withPartners will bring Partners list on Representative PT - Lista todos os Representantes de Vendas ativos   Parametro withPartners ira trazer lista de Parceiros no RTV 
        /// </summary>
        /// <param name="withPartners">EN - Bring partners on structure PT - Trazer parceiros na estrutura </param> 
        /// <param name="salesDistrictCode">EN - Sales district code to filter. PT - codigo de Diretoria de Negocio para filtro. </param> 
        /// <param name="salesOfficeCode">EN - sales office code. PT - codigo da Regional. </param> 
        /// <param name="salesRepCode">EN - code sales representative code. PT - codigo de RTV. </param> 
        /// <param name="referenceDate">Format - date (as full-date in RFC3339). EN - Reference Date for Sales Structure Vigency - Default&#x3D; Today PT- Data de referencia para a vigencia da Estrutura de Vendas - Valor Padrao &#x3D; Hoje </param> 
        /// <param name="repMainFunctionFlag">EN - Representative main function PT - Funcao principal do RTV </param> 
        /// <param name="countryCode">EN - Country Code ISO ALPHA-2. Eg. BR  PT - Codigo do pais ISO ALPHA-2. Ex. BR </param> 
        /// <param name="salesOrgCode">EN - Sales Organization Code  PT - Codigo da Organizacao de Vendas </param> 
        /// <param name="level">EN - structure level permission PT - Nivel de permissao na estrutura </param> 
        /// <param name="restrictionCodes">EN - permission restriction codes PT - Codigos de restricao de permissao </param> 
        /// <returns>SalesStructure</returns>            
        public SalesStructure ListSalesStructure(bool? withPartners, string salesDistrictCode, string salesOfficeCode, string salesRepCode, string referenceDate, string repMainFunctionFlag, string countryCode, string salesOrgCode, string level, string restrictionCodes)
        {

            // verify the required parameter 'withPartners' is set
            if (withPartners == null) throw new ApiException(400, "Missing required parameter 'withPartners' when calling ListSalesStructure");

            var path = "/sales-structures";
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (withPartners != null) queryParams.Add("withPartners", ApiClient.ParameterToString(withPartners)); // query parameter
            if (salesDistrictCode != null) queryParams.Add("salesDistrictCode", ApiClient.ParameterToString(salesDistrictCode)); // query parameter
            if (salesOfficeCode != null) queryParams.Add("salesOfficeCode", ApiClient.ParameterToString(salesOfficeCode)); // query parameter
            if (salesRepCode != null) queryParams.Add("salesRepCode", ApiClient.ParameterToString(salesRepCode)); // query parameter
            if (referenceDate != null) queryParams.Add("referenceDate", ApiClient.ParameterToString(referenceDate)); // query parameter
            if (repMainFunctionFlag != null) queryParams.Add("repMainFunctionFlag", ApiClient.ParameterToString(repMainFunctionFlag)); // query parameter
            if (countryCode != null) queryParams.Add("countryCode", ApiClient.ParameterToString(countryCode)); // query parameter
            if (salesOrgCode != null) queryParams.Add("salesOrgCode", ApiClient.ParameterToString(salesOrgCode)); // query parameter
            if (level != null) headerParams.Add("level", ApiClient.ParameterToString(level)); // header parameter
            if (restrictionCodes != null) headerParams.Add("restrictionCodes", ApiClient.ParameterToString(restrictionCodes)); // header parameter
            
            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams);

            if (((int)response.StatusCode) >= 404)
                return new SalesStructure();
            else if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesStructure: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesStructure: " + response.ErrorMessage, response.ErrorMessage);

            return (SalesStructure)ApiClient.Deserialize(response.Content, typeof(SalesStructure), response.Headers);
        }



        public SalesStructure ListSalesStructure(bool? withPartners, string salesDistrictCode, 
            string salesOfficeCode, string salesRepCode, string referenceDate, 
               string repMainFunctionFlag, string countryCode, string salesOrgCode, 
               string level, string restrictionCodes, string clientId, string token, string route)
        {

            // verify the required parameter 'withPartners' is set
            if (withPartners == null) throw new ApiException(400, "Missing required parameter 'withPartners' when calling ListSalesStructure");

            var path = route;
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (withPartners != null) queryParams.Add("withPartners", ApiClient.ParameterToString(withPartners)); // query parameter
            if (salesDistrictCode != null) queryParams.Add("salesDistrictCode", ApiClient.ParameterToString(salesDistrictCode)); // query parameter
            if (salesOfficeCode != null) queryParams.Add("salesOfficeCode", ApiClient.ParameterToString(salesOfficeCode)); // query parameter
            if (salesRepCode != null) queryParams.Add("salesRepCode", ApiClient.ParameterToString(salesRepCode)); // query parameter
            if (referenceDate != null) queryParams.Add("referenceDate", ApiClient.ParameterToString(referenceDate)); // query parameter
            if (repMainFunctionFlag != null) queryParams.Add("repMainFunctionFlag", ApiClient.ParameterToString(repMainFunctionFlag)); // query parameter
            if (countryCode != null) queryParams.Add("countryCode", ApiClient.ParameterToString(countryCode)); // query parameter
            if (salesOrgCode != null) queryParams.Add("salesOrgCode", ApiClient.ParameterToString(salesOrgCode)); // query parameter
            if (level != null) headerParams.Add("level", ApiClient.ParameterToString(level)); // header parameter
            if (restrictionCodes != null) headerParams.Add("restrictionCodes", ApiClient.ParameterToString(restrictionCodes)); // header parameter


            IRestResponse response = (IRestResponse)ApiClient
                 .CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams,
                 clientId, token);


            if (((int)response.StatusCode) >= 404)
                return new SalesStructure();
            else if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesStructure: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListSalesStructure: " + response.ErrorMessage, response.ErrorMessage);

            return (SalesStructure)ApiClient.Deserialize(response.Content, typeof(SalesStructure), response.Headers);
        }
    }
}