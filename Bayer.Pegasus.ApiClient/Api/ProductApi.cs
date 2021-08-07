using System;
using System.Collections.Generic;
using RestSharp;
using Bayer.Pegasus.Entities;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Linq;

namespace Bayer.Pegasus.ApiClient
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IProductApi
    {
        /// <summary>
        ///  EN - List brands with products PT - Lista as marcas com produtos 
        /// </summary>
        /// <param name="brandNamesList">EN - Brand name list PT - List de nomes da Marca </param>
        /// <param name="salesOrgCode">EN - sales organization code PT - Codigo da organizacao </param>
        /// <param name="plantCode">EN - Plant code PT - Codigo da planta </param>
        /// <param name="distChannel">EN - Distribution channel PT - Canal de distribuicao </param>
        /// <returns>BrandArray</returns>
        List<Brand> ListBrand(string brandNamesList, string salesOrgCode, string plantCode, string distChannel);

        /// <summary>
        ///  EN - List products with domain info PT - Lista produtos com dados de domain  
        /// </summary>
        /// <param name="productCodesList">EN - product codes list PT - List de codigos do produto </param>
        /// <param name="productNamesList">EN - Product names lit, can be partial PT - Lista de nomes do Produto, pode ser parcial </param>
        /// <param name="brandNamesList">EN - Brand name list PT - List de nomes da Marca </param>
        /// <param name="salesOrgCode">EN - sales organization code PT - Codigo da organizacao </param>
        /// <param name="plantCode">EN - Plant code PT - Codigo da planta </param>
        /// <param name="distChannel">EN - Distribution channel PT - Canal de distribuicao </param>
        /// <returns>ProductSingleArray</returns>
        List<Product> ListProducts(string productCodesList, string productNamesList, string brandNamesList, string salesOrgCode, string plantCode, string distChannel);
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class ProductApi : IProductApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public ProductApi(ApiClient apiClient = null)
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
        public ProductApi(String basePath)
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
        ///  EN - List brands with products PT - Lista as marcas com produtos 
        /// </summary>
        /// <param name="brandNamesList">EN - Brand name list PT - List de nomes da Marca </param> 
        /// <param name="salesOrgCode">EN - sales organization code PT - Codigo da organizacao </param> 
        /// <param name="plantCode">EN - Plant code PT - Codigo da planta </param> 
        /// <param name="distChannel">EN - Distribution channel PT - Canal de distribuicao </param> 
        /// <returns>BrandArray</returns>            
        public List<Brand> ListBrand(string brandNamesList, string salesOrgCode, string plantCode, string distChannel)
        {
            var path = "";
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (brandNamesList != null) queryParams.Add("brandNamesList", ApiClient.ParameterToString(brandNamesList)); // query parameter
            if (salesOrgCode != null) queryParams.Add("salesOrgCode", ApiClient.ParameterToString(salesOrgCode)); // query parameter
            if (plantCode != null) queryParams.Add("plantCode", ApiClient.ParameterToString(plantCode)); // query parameter
            if (distChannel != null) queryParams.Add("distChannel", ApiClient.ParameterToString(distChannel)); // query parameter
            
            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams);

            if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListBrand: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListBrand: " + response.ErrorMessage, response.ErrorMessage);

            return (List<Brand>)ApiClient.Deserialize(response.Content, typeof(List<Brand>), response.Headers);
        }

        /// <summary>
        ///  EN - List products with domain info PT - Lista produtos com dados de domain  
        /// </summary>
        /// <param name="productCodesList">EN - product codes list PT - List de codigos do produto </param> 
        /// <param name="productNamesList">EN - Product names lit, can be partial PT - Lista de nomes do Produto, pode ser parcial </param> 
        /// <param name="brandNamesList">EN - Brand name list PT - List de nomes da Marca </param> 
        /// <param name="salesOrgCode">EN - sales organization code PT - Codigo da organizacao </param> 
        /// <param name="plantCode">EN - Plant code PT - Codigo da planta </param> 
        /// <param name="distChannel">EN - Distribution channel PT - Canal de distribuicao </param> 
        /// <returns>ProductSingleArray</returns>            
        public List<Product> ListProducts(string productCodesList, string productNamesList, string brandNamesList, string salesOrgCode, string plantCode, string distChannel)
        {


            var path = "/products";
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (productCodesList != null) queryParams.Add("productCodesList", ApiClient.ParameterToString(productCodesList)); // query parameter
            if (productNamesList != null) queryParams.Add("productNamesList", ApiClient.ParameterToString(productNamesList)); // query parameter
            if (brandNamesList != null) queryParams.Add("brandNamesList", ApiClient.ParameterToString(brandNamesList)); // query parameter
            if (salesOrgCode != null) queryParams.Add("salesOrgCode", ApiClient.ParameterToString(salesOrgCode)); // query parameter
            if (plantCode != null) queryParams.Add("plantCode", ApiClient.ParameterToString(plantCode)); // query parameter
            if (distChannel != null) queryParams.Add("distChannel", ApiClient.ParameterToString(distChannel)); // query parameter

            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams);

            if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.ErrorMessage, response.ErrorMessage);

            return (List<Product>)ApiClient.Deserialize(response.Content, typeof(List<Product>), response.Headers);
        }

        public List<Product> ListProducts(string productCodesList, string productNamesList, string brandNamesList, 
                                          string salesOrgCode, string plantCode, string distChannel,
                                          string clientId, string token, string route)
        {
            var path = route;
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (productCodesList != null) queryParams.Add("productCodesList", ApiClient.ParameterToString(productCodesList)); // query parameter
            if (productNamesList != null) queryParams.Add("productNamesList", ApiClient.ParameterToString(productNamesList)); // query parameter
            if (brandNamesList != null) queryParams.Add("brandNamesList", ApiClient.ParameterToString(brandNamesList)); // query parameter
            if (salesOrgCode != null) queryParams.Add("salesOrgCode", ApiClient.ParameterToString(salesOrgCode)); // query parameter
            if (plantCode != null) queryParams.Add("plantCode", ApiClient.ParameterToString(plantCode)); // query parameter
            if (distChannel != null) queryParams.Add("distChannel", ApiClient.ParameterToString(distChannel)); // query parameter

            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient
                .CallApi(path, Method.GET, queryParams, postBody, 
                headerParams, formParams, fileParams, clientId, token);

            if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.ErrorMessage, response.ErrorMessage);

            return (List<Product>)ApiClient.Deserialize(response.Content, typeof(List<Product>), response.Headers);
        }


        public List<Product> ListProductsLegacies(string businessGroupCode, string productNamesList, List<string> businessUnitCodesList,
                                  string clientId, string token, string route)
        {
            var path = route;
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;


            if (businessGroupCode != null) queryParams.Add("businessGroupCode", ApiClient.ParameterToString(businessGroupCode)); // query parameter
            //if (brandNamesList != null) queryParams.Add("brandNamesList", ApiClient.ParameterToString(brandNamesList)); // query parameter
            if (productNamesList != null) queryParams.Add("productNamesList", ApiClient.ParameterToString(productNamesList)); // query parameter
            if (businessUnitCodesList != null) queryParams.Add("businessUnitCodesList", ApiClient.ParameterToString(businessUnitCodesList)); // query parameter

            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient
                .CallApi(path, Method.GET, queryParams, postBody,
                headerParams, formParams, fileParams, clientId, token);

            if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.ErrorMessage, response.ErrorMessage);

            return (List<Product>)ApiClient.Deserialize(response.Content, typeof(List<Product>), response.Headers);
        }

        public List<Product> ListProductsLegaciesByBusinessGroupCode(string businessGroupCode, string search, 
                          string clientId, string token, string route)
        {
            var path = route;
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;


            if (businessGroupCode != null) queryParams.Add("businessGroupCode", ApiClient.ParameterToString(businessGroupCode)); // query parameter

            //TODO: Busca retornada pela API precisa funcionar. Falar com Sidney
            //if (search != null)
            //{
            //    queryParams.Add("code", ApiClient.ParameterToString(search)); // query parameter
            //    queryParams.Add("name", ApiClient.ParameterToString(search)); // query parameter
            //}
                
            
            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient
                .CallApi(path, Method.GET, queryParams, postBody,
                headerParams, formParams, fileParams, clientId, token);

            if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.ErrorMessage, response.ErrorMessage);


            //return (List<Product>)ApiClient.Deserialize(response.Content, typeof(List<Product>), response.Headers);
            List<Product> listProducts = new List<Product>();
            listProducts = (List<Product>)ApiClient.Deserialize(response.Content, typeof(List<Product>), response.Headers);

            //TODO: Retirar quando a busca retornada pela API funcionar
            //busca pelas propriedades "name", "code"
            if (search != null)
            {
                listProducts = listProducts.Where(p => p.Name.Contains(search) ||
                                             p.Code.Contains(search)).ToList();
            }

            return (listProducts);

        }


        public List<BusinessUnitsArrayList> ListProductsLegaciesBU(string businessGroupCodeList, string clientId, string token, string route)
        {
            try
            {

                var path = route;
                path = path.Replace("{format}", "json");

                var queryParams = new Dictionary<String, String>();
                var headerParams = new Dictionary<String, String>();
                var formParams = new Dictionary<String, String>();
                var fileParams = new Dictionary<String, FileParameter>();
                String postBody = null;

                if (businessGroupCodeList != null) queryParams.Add("businessGroupCodeList", ApiClient.ParameterToString(businessGroupCodeList)); // query parameter

                // make the HTTP request
                IRestResponse response = (IRestResponse)ApiClient
                    .CallApi(path, Method.GET, queryParams, postBody,
                    headerParams, formParams, fileParams, clientId, token);

                if (((int)response.StatusCode) >= 400)
                    throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.Content, response.Content);
                else if (((int)response.StatusCode) == 0)
                    throw new ApiException((int)response.StatusCode, "Error calling ListProducts: " + response.ErrorMessage, response.ErrorMessage);

                return (List<BusinessUnitsArrayList>)ApiClient.Deserialize(response.Content, typeof(List<BusinessUnitsArrayList>), response.Headers);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }



        /// <summary>
        ///  EN - List brands with products PT - Lista as marcas com produtos 
        /// </summary>
        /// <param name="brandNamesList">EN - Brand name list PT - List de nomes da Marca </param> 
        /// <param name="salesOrgCode">EN - sales organization code PT - Codigo da organizacao </param> 
        /// <param name="plantCode">EN - Plant code PT - Codigo da planta </param> 
        /// <param name="distChannel">EN - Distribution channel PT - Canal de distribuicao </param> 
        /// <returns>BrandArray</returns>            
        public List<Brand> ListBrand(string brandNamesList, string salesOrgCode, string plantCode, string distChannel, 
            string clientId, string _token)
        {
            var path = "";
            path = path.Replace("{format}", "json");

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (brandNamesList != null) queryParams.Add("brandNamesList", ApiClient.ParameterToString(brandNamesList)); // query parameter
            if (salesOrgCode != null) queryParams.Add("salesOrgCode", ApiClient.ParameterToString(salesOrgCode)); // query parameter
            if (plantCode != null) queryParams.Add("plantCode", ApiClient.ParameterToString(plantCode)); // query parameter
            if (distChannel != null) queryParams.Add("distChannel", ApiClient.ParameterToString(distChannel)); // query parameter

            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient.CallApi(path, Method.GET, queryParams, 
                                                                      postBody, headerParams, formParams, 
                                                                      fileParams, clientId, _token);

            if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling ListBrand: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling ListBrand: " + response.ErrorMessage, response.ErrorMessage);

            return (List<Brand>)ApiClient.Deserialize(response.Content, typeof(List<Brand>), response.Headers);
        }

    }
}
