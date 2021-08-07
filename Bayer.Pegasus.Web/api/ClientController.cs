using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Bayer.Pegasus.Web.api
{
    [Produces("application/json")]
    [Route("api/Client")]
    public class ClientController : Controller
    {
        private IHostingEnvironment _env;
        public ClientController(IHostingEnvironment env)
        {
            _env = env;
        }

        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public JArray Get([FromBody]JObject data)
        {           
            JArray filteredArray = new JArray();

            if (data != null)
            {
                var search = data["search"].Value<String>();

                if (!String.IsNullOrEmpty(search))
                {

                    using (var clientBO = new Bayer.Pegasus.Business.ClientBO())
                    {
                        var salesStructure = Bayer.Pegasus.Entities.SalesStructureAccess.GetSalesStructureAccessByUser(User);
                        
                        List<string> partners = new List<string>(); ;

                        if (data["parents"] != null)
                        {
                            foreach (var partnerItem in data["parents"])
                            {
                                var partner = partnerItem["value"].Value<String>();

                                partners.Add(partner);
                            }
                        }

                        var items = clientBO.GetCustomers(User, partners, search);

                        foreach (var item in items)
                        {
                            JObject jobject = new JObject();
                            jobject["label"] = item.Name;
                            jobject["value"] = item.Code;
                            filteredArray.Add(jobject);
                        }
                    }
                }
            }
            return filteredArray;
        }
    }

}