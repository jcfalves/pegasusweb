using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Bayer.Pegasus.Web.Helpers
{
    public class JObjectBodyInputFormatter : InputFormatter
    {
        public JObjectBodyInputFormatter()
        {
            this.SupportedMediaTypes.Add("application/json");
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            using (var reader = new StreamReader(request.Body))
            {
                var content = await reader.ReadToEndAsync();
                var jobject = JObject.Parse(content);

                return await InputFormatterResult.SuccessAsync(jobject);
            }
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(JObject);
        }
    }

}
