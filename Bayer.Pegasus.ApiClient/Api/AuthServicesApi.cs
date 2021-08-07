using System;
using System.Collections.Generic;
using RestSharp;
using Bayer.Pegasus.Entities.Auth;

namespace Bayer.Pegasus.ApiClient
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IAuthServicesApi
    {
        /// <summary>
        /// | EN - API for alter user password   | PT - API para alterar a senha do usuário | EN - Alter Password               | PT - Alterar a senha do usuário
        /// </summary>
        /// <param name="model">| EN - Required data for alter password | PT - Dados requiridos para alterar a senha do usuário</param>
        /// <returns>AlterPasswordSwagger</returns>
        AlterPasswordSwagger AlterPassword (AlterPasswordModel model);
        /// <summary>
        /// | EN -  API for reset user password if valid and sent email to user with a new password   | PT - API para redefinir a senha e enviar por email a nova senha para o usuário | EN - Forgot My Password                | PT - Esqueci minha senha
        /// </summary>
        /// <param name="model">| EN - Required data for the method forgot my password | PT - Dados requiridos para o esqueci minha senha</param>
        /// <returns>ForgotMyPasswordSwagger</returns>
        ForgotMyPasswordSwagger ForgotMyPassword (ForgotMyPasswordModel model);
        /// <summary>
        /// | EN - List all password policies | PT - Lista todas as políticas de senha | EN - List all password policies | PT - Lista todas as políticas de senha
        /// </summary>
        /// <param name="cultureName">| EN - Optional parameter cultureName(Default is en-US)                | PT - Parâmetro obrigatório cultureName (padrão é en-US)</param>
        /// <returns>ForgotMyPasswordSwagger</returns>
        ForgotMyPasswordSwagger GetPasswordPolicy (string cultureName);
        /// <summary>
        /// | EN - List data restriction from role level  | PT - Lista as restrições de dados para um determinado nível de acesso | EN - List data restriction from role level               | PT - Lista as restrições de dados para um determinado nível de acesso
        /// </summary>
        /// <param name="appId">| EN - Application Id | PT - Id da aplicação</param>
        /// <param name="login">| EN - Your user identification | PT - Tua identificação de usuário</param>
        /// <param name="ip">| EN - User IP | PT - IP do usuário</param>
        /// <param name="role">| EN - Optional parameter role | PT - Parâmetro opcional role</param>
        /// <param name="culture"></param>
        /// <returns>LevelViewModelSwagger</returns>
        LevelViewModelSwagger GetUserDataRestriction (string appId, string login, string ip, string role, string culture);
        /// <summary>
        /// | EN - List users of a system  | PT - Lista todos os usuários de uma aplicação | EN - List users of a system               | PT - Lista todos os usuários de uma aplicação
        /// </summary>
        /// <param name="appId">| EN - Application Id | PT - Id da aplicação</param>
        /// <param name="ip">| EN - User IP | PT - IP do usuário</param>
        /// <param name="login">| EN - Your user identification | PT - Tua identificação de usuário</param>
        /// <param name="role">| EN - Optional parameter role | PT - Parâmetro opcional role</param>
        /// <param name="culture"></param>
        /// <returns>UserBySystemSwagger</returns>
        UserBySystemSwagger GetUsersBySystem (string appId, string ip, string login, string role, string culture);
        /// <summary>
        /// | EN - The user will Log-in through this API with login and password  | PT - O usuário irá se logar através desta API com login e senha | EN - Login               | PT - Login
        /// </summary>
        /// <param name="model">| EN - Required data for the login method | PT - Dados requiridos para o login</param>
        /// <returns>LoginSwagger</returns>
        LoginSwagger Login (LoginModel model);
        /// <summary>
        /// | EN - The user will Log-in through this API with login   | PT - O usuário irá se logar através desta API com login | EN - Login without password               | PT - Login sem senha
        /// </summary>
        /// <param name="model">| EN - Required data for the loginsso method | PT - Dados requiridos para o loginsso</param>
        /// <returns>LoginSwagger</returns>
        LoginSwagger Loginsso (LoginSsoModel model);
        /// <summary>
        /// | EN - The user will Log-out through this API    | PT - O usuário irá se deslogar através desta API | EN - Logout of a system               | PT - Logout de um sistema
        /// </summary>
        /// <param name="model">|EN - Required data for the logout method | PT - Dados requiridos para o logout</param>
        /// <returns>LogOffSwagger</returns>
        LogOffSwagger Logout (LogoutModel model);
    }
  
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class AuthServicesApi : IAuthServicesApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthServicesApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public AuthServicesApi(ApiClient apiClient = null)
        {
            if (apiClient == null) // use the default one in Configuration
                this.ApiClient = Configuration.DefaultApiClient; 
            else
                this.ApiClient = apiClient;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthServicesApi"/> class.
        /// </summary>
        /// <returns></returns>
        //public AuthServicesApi(String basePath)
        //{
        //    this.ApiClient = new ApiClient(basePath);
        //}


        public AuthServicesApi(String basePath)
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
        public ApiClient ApiClient {get; set;}
        public string clientId { get; set; }
        public string  token { get; set; }

        /// <summary>
        /// | EN - API for alter user password   | PT - API para alterar a senha do usuário | EN - Alter Password               | PT - Alterar a senha do usuário
        /// </summary>
        /// <param name="model">| EN - Required data for alter password | PT - Dados requiridos para alterar a senha do usuário</param> 
        /// <returns>AlterPasswordSwagger</returns>            
        public AlterPasswordSwagger AlterPassword (AlterPasswordModel model)
        {
            
            // verify the required parameter 'model' is set
            if (model == null) throw new ApiException(400, "Missing required parameter 'model' when calling AlterPassword");
            
    
            var path = "/api/AuthServices/alter-password";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;
    
            postBody = ApiClient.Serialize(model); // http body (model) parameter
    
    
            // make the HTTP request
            IRestResponse response = (IRestResponse) ApiClient.CallApi(path, Method.POST, queryParams, postBody, headerParams, formParams, fileParams);
    
            if (((int)response.StatusCode) >= 400)
                throw new ApiException ((int)response.StatusCode, "Error calling AlterPassword: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException ((int)response.StatusCode, "Error calling AlterPassword: " + response.ErrorMessage, response.ErrorMessage);
    
            return (AlterPasswordSwagger) ApiClient.Deserialize(response.Content, typeof(AlterPasswordSwagger), response.Headers);
        }
    
        /// <summary>
        /// | EN -  API for reset user password if valid and sent email to user with a new password   | PT - API para redefinir a senha e enviar por email a nova senha para o usuário | EN - Forgot My Password                | PT - Esqueci minha senha
        /// </summary>
        /// <param name="model">| EN - Required data for the method forgot my password | PT - Dados requiridos para o esqueci minha senha</param> 
        /// <returns>ForgotMyPasswordSwagger</returns>            
        public ForgotMyPasswordSwagger ForgotMyPassword (ForgotMyPasswordModel model)
        {
            
            // verify the required parameter 'model' is set
            if (model == null) throw new ApiException(400, "Missing required parameter 'model' when calling ForgotMyPassword");
            
    
            var path = "/api/AuthServices/forgot-my-password";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;
    
            postBody = ApiClient.Serialize(model); // http body (model) parameter
    
            // authentication setting, if any
            String[] authSettings = new String[] {  };
    
            // make the HTTP request
            IRestResponse response = (IRestResponse) ApiClient.CallApi(path, Method.POST, queryParams, postBody, headerParams, formParams, fileParams);
    
            if (((int)response.StatusCode) >= 400)
                throw new ApiException ((int)response.StatusCode, "Error calling ForgotMyPassword: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException ((int)response.StatusCode, "Error calling ForgotMyPassword: " + response.ErrorMessage, response.ErrorMessage);
    
            return (ForgotMyPasswordSwagger) ApiClient.Deserialize(response.Content, typeof(ForgotMyPasswordSwagger), response.Headers);
        }
    
        /// <summary>
        /// | EN - List all password policies | PT - Lista todas as políticas de senha | EN - List all password policies | PT - Lista todas as políticas de senha
        /// </summary>
        /// <param name="cultureName">| EN - Optional parameter cultureName(Default is en-US)                | PT - Parâmetro obrigatório cultureName (padrão é en-US)</param> 
        /// <returns>ForgotMyPasswordSwagger</returns>            
        public ForgotMyPasswordSwagger GetPasswordPolicy (string cultureName)
        {
            
    
            var path = "/api/AuthServices/password-policy";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;
    
             if (cultureName != null) queryParams.Add("cultureName", ApiClient.ParameterToString(cultureName)); // query parameter
                                        
            // authentication setting, if any
            String[] authSettings = new String[] {  };
    
            // make the HTTP request
            IRestResponse response = (IRestResponse) ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams);
    
            if (((int)response.StatusCode) >= 400)
                throw new ApiException ((int)response.StatusCode, "Error calling GetPasswordPolicy: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException ((int)response.StatusCode, "Error calling GetPasswordPolicy: " + response.ErrorMessage, response.ErrorMessage);
    
            return (ForgotMyPasswordSwagger) ApiClient.Deserialize(response.Content, typeof(ForgotMyPasswordSwagger), response.Headers);
        }
    
        /// <summary>
        /// | EN - List data restriction from role level  | PT - Lista as restrições de dados para um determinado nível de acesso | EN - List data restriction from role level               | PT - Lista as restrições de dados para um determinado nível de acesso
        /// </summary>
        /// <param name="appId">| EN - Application Id | PT - Id da aplicação</param> 
        /// <param name="login">| EN - Your user identification | PT - Tua identificação de usuário</param> 
        /// <param name="ip">| EN - User IP | PT - IP do usuário</param> 
        /// <param name="role">| EN - Optional parameter role | PT - Parâmetro opcional role</param> 
        /// <param name="culture"></param> 
        /// <returns>LevelViewModelSwagger</returns>            
        public LevelViewModelSwagger GetUserDataRestriction (string appId, string login, string ip, string role, string culture)
        {
            
            // verify the required parameter 'appId' is set
            if (appId == null) throw new ApiException(400, "Missing required parameter 'appId' when calling GetUserDataRestriction");
            
            // verify the required parameter 'login' is set
            if (login == null) throw new ApiException(400, "Missing required parameter 'login' when calling GetUserDataRestriction");
            
            // verify the required parameter 'ip' is set
            if (ip == null) throw new ApiException(400, "Missing required parameter 'ip' when calling GetUserDataRestriction");
            
    
            var path = "/api/AuthServices/user-data-restriction";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;
    
             if (appId != null) queryParams.Add("appId", ApiClient.ParameterToString(appId)); // query parameter
 if (login != null) queryParams.Add("login", ApiClient.ParameterToString(login)); // query parameter
 if (ip != null) queryParams.Add("ip", ApiClient.ParameterToString(ip)); // query parameter
 if (role != null) queryParams.Add("role", ApiClient.ParameterToString(role)); // query parameter
 if (culture != null) queryParams.Add("culture", ApiClient.ParameterToString(culture)); // query parameter
    
            // make the HTTP request
            IRestResponse response = (IRestResponse) ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams);


            if (((int)response.StatusCode) == 404)
                return new LevelViewModelSwagger();
            else if (((int)response.StatusCode) >= 400)
                throw new ApiException ((int)response.StatusCode, "Error calling GetUserDataRestriction: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException ((int)response.StatusCode, "Error calling GetUserDataRestriction: " + response.ErrorMessage, response.ErrorMessage);
    
            return (LevelViewModelSwagger) ApiClient.Deserialize(response.Content, typeof(LevelViewModelSwagger), response.Headers);
        }
    
        /// <summary>
        /// | EN - List users of a system  | PT - Lista todos os usuários de uma aplicação | EN - List users of a system               | PT - Lista todos os usuários de uma aplicação
        /// </summary>
        /// <param name="appId">| EN - Application Id | PT - Id da aplicação</param> 
        /// <param name="ip">| EN - User IP | PT - IP do usuário</param> 
        /// <param name="login">| EN - Your user identification | PT - Tua identificação de usuário</param> 
        /// <param name="role">| EN - Optional parameter role | PT - Parâmetro opcional role</param> 
        /// <param name="culture"></param> 
        /// <returns>UserBySystemSwagger</returns>            
        public UserBySystemSwagger GetUsersBySystem (string appId, string ip, string login, string role, string culture)
        {
            
            // verify the required parameter 'appId' is set
            if (appId == null) throw new ApiException(400, "Missing required parameter 'appId' when calling GetUsersBySystem");
            
            // verify the required parameter 'ip' is set
            if (ip == null) throw new ApiException(400, "Missing required parameter 'ip' when calling GetUsersBySystem");


            var path = "/users-by-system";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;
    
             if (appId != null) queryParams.Add("appId", ApiClient.ParameterToString(appId)); // query parameter
 if (ip != null) queryParams.Add("ip", ApiClient.ParameterToString(ip)); // query parameter
 if (login != null) queryParams.Add("login", ApiClient.ParameterToString(login)); // query parameter
 if (role != null) queryParams.Add("role", ApiClient.ParameterToString(role)); // query parameter
 if (culture != null) queryParams.Add("culture", ApiClient.ParameterToString(culture)); // query parameter
    
            // make the HTTP request
            IRestResponse response = (IRestResponse) ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams);

            var content = response.Content;

            if (((int)response.StatusCode) == 404)
                return new UserBySystemSwagger();
            else if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling GetUsersBySystem: " + content, content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling GetUsersBySystem: " + response.ErrorMessage, response.ErrorMessage);
            

            return (UserBySystemSwagger) ApiClient.Deserialize(content, typeof(UserBySystemSwagger), response.Headers);
        }
    
        /// <summary>
        /// | EN - The user will Log-in through this API with login and password  | PT - O usuário irá se logar através desta API com login e senha | EN - Login               | PT - Login
        /// </summary>
        /// <param name="model">| EN - Required data for the login method | PT - Dados requiridos para o login</param> 
        /// <returns>LoginSwagger</returns>            
        public LoginSwagger Login (LoginModel model)
        {
            
            // verify the required parameter 'model' is set
            if (model == null) throw new ApiException(400, "Missing required parameter 'model' when calling Login");
            
    
            var path = "/api/AuthServices/login";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;
    
            postBody = ApiClient.Serialize(model); // http body (model) parameter
    
            // make the HTTP request
            IRestResponse response = (IRestResponse) ApiClient.CallApi(path, Method.POST, queryParams, postBody, headerParams, formParams, fileParams);
    
            if (((int)response.StatusCode) >= 400)
                throw new ApiException ((int)response.StatusCode, "Error calling Login: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException ((int)response.StatusCode, "Error calling Login: " + response.ErrorMessage, response.ErrorMessage);
    
            return (LoginSwagger) ApiClient.Deserialize(response.Content, typeof(LoginSwagger), response.Headers);
        }
    
        /// <summary>
        /// | EN - The user will Log-in through this API with login   | PT - O usuário irá se logar através desta API com login | EN - Login without password               | PT - Login sem senha
        /// </summary>
        /// <param name="model">| EN - Required data for the loginsso method | PT - Dados requiridos para o loginsso</param> 
        /// <returns>LoginSwagger</returns>            
        public LoginSwagger Loginsso (LoginSsoModel model)
        {
            
            // verify the required parameter 'model' is set
            if (model == null) throw new ApiException(400, "Missing required parameter 'model' when calling Loginsso");
            
    
            var path = "/loginsso";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;
    
            postBody = ApiClient.Serialize(model); // http body (model) parameter
    
            // make the HTTP request
            IRestResponse response = (IRestResponse) ApiClient.CallApi(path, Method.POST, queryParams, postBody, headerParams, formParams, fileParams);


            if (((int)response.StatusCode) == 404) {
                return new LoginSwagger();
            }
            else if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling Loginsso: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling Loginsso: " + response.ErrorMessage, response.ErrorMessage);
    
            return (LoginSwagger) ApiClient.Deserialize(response.Content, typeof(LoginSwagger), response.Headers);
        }
    
        /// <summary>
        /// | EN - The user will Log-out through this API    | PT - O usuário irá se deslogar através desta API | EN - Logout of a system               | PT - Logout de um sistema
        /// </summary>
        /// <param name="model">|EN - Required data for the logout method | PT - Dados requiridos para o logout</param> 
        /// <returns>LogOffSwagger</returns>            
        public LogOffSwagger Logout (LogoutModel model)
        {
            
            // verify the required parameter 'model' is set
            if (model == null) throw new ApiException(400, "Missing required parameter 'model' when calling Logout");
            
    
            var path = "/api/AuthServices/logout";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;
    
            postBody = ApiClient.Serialize(model); // http body (model) parameter
    
            // make the HTTP request
            IRestResponse response = (IRestResponse) ApiClient.CallApi(path, Method.POST, queryParams, postBody, headerParams, formParams, fileParams);
    
            if (((int)response.StatusCode) >= 400)
                throw new ApiException ((int)response.StatusCode, "Error calling Logout: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException ((int)response.StatusCode, "Error calling Logout: " + response.ErrorMessage, response.ErrorMessage);
    
            return (LogOffSwagger) ApiClient.Deserialize(response.Content, typeof(LogOffSwagger), response.Headers);
        }
    
    }
}
