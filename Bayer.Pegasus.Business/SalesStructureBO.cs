using Bayer.Pegasus.Entities.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace Bayer.Pegasus.Business
{
    public class SalesStructureBO : BaseBO
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


        public async Task<SalesStructureModel> GetSalesStructure(string token, string clientId)
        {
            try
            {
                string route = Bayer.Pegasus.Utils.Configuration.Instance.ServiceSalesStructureURL;
                string AccessTokenUrl = Utils.Configuration.Instance.UrlApiOauthToken;
               
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", clientId);
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("access_token", token);

                    var request = await httpClient.GetAsync(route);
                    request.EnsureSuccessStatusCode();

                    var data = await request.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject(data);

                    JObject dados = JObject.Parse(data);
                    JToken jtoken = JToken.Parse(data);

                    SalesStructureModel resultRoles =
                        JsonConvert.DeserializeObject<SalesStructureModel>(jtoken.ToString());

                    return resultRoles;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw ex;
            }
        }


        public string ParameterToString(object obj)
        {
            if (obj is DateTime)
                return ((DateTime)obj).ToString(DateTimeFormat);
            else if (obj is List<string>)
                return String.Join(",", (obj as List<string>).ToArray());
            else
                return Convert.ToString(obj);
        }

        private const string ISO8601_DATETIME_FORMAT = "o";

        private static string _dateTimeFormat = ISO8601_DATETIME_FORMAT;

        public static String DateTimeFormat
        {
            get
            {
                return _dateTimeFormat;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Never allow a blank or null string, go back to the default
                    _dateTimeFormat = ISO8601_DATETIME_FORMAT;
                    return;
                }

                // Caution, no validation when you choose date time format other than ISO 8601
                // Take a look at the above links
                _dateTimeFormat = value;
            }
        }

        public static Object ConvertType(Object source, Type dest)
        {
            return Convert.ChangeType(source, dest);
        }
    }
}
