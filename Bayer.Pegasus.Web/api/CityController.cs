using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Bayer.Pegasus.Web.api
{
    [Produces("application/json")]
    [Route("api/City")]
    public class CityController : Controller
    {
        private IHostingEnvironment _env;
        public CityController(IHostingEnvironment env)
        {
            _env = env;
        }

        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JArray Get([FromBody]JObject data)
        {
            
            JArray filteredArray = new JArray();

            try
            {
                if (data != null)
                {
                    var search = data["search"].Value<String>();

                    if (!String.IsNullOrEmpty(search))
                    {

                        using (var cityBO = new Bayer.Pegasus.Business.CityBO())
                        {

                            var items = cityBO.GetCities(search);


                            foreach (var item in items)
                            {
                                JObject jobject = new JObject();
                                jobject["value"] = item.CodeIbge;
                                jobject["label"] = item.CityName;
                                filteredArray.Add(jobject);
                            }
                        }
                       
                    }
                }



            }
            catch (Exception ex) {
                JObject jobject = new JObject();
                jobject["value"] = "erro";
                jobject["label"] = ex.ToString();
                filteredArray.Add(jobject);
            }


            return filteredArray;

        }
    }

}