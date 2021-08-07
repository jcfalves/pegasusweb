using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Bayer.Pegasus.Utils
{
    public class JsonUtils
    {
        public static JArray GetEvolutionKPIsAsJArray(List<Bayer.Pegasus.Entities.Kpis.EvolutionKPI> kpis)
        {

            JArray jArrayKpis = new JArray();

            foreach (var kpi in kpis)
            {
                jArrayKpis.Add(kpi.ToJObject());
            }

            return jArrayKpis;

        }

        public static System.Collections.Generic.List<string> GetListFilterValues(JArray filter)
        {

            System.Collections.Generic.List<string> values = new System.Collections.Generic.List<string>();

            foreach (JObject item in filter)
            {
                values.Add(item["value"].Value<string>());
            }

            return values;

        }

        public static JArray GetTopKPIsAsJArray(List<Bayer.Pegasus.Entities.Kpis.TopKPI> kpis)
        {

            JArray jArrayKpis = new JArray();

            foreach (var kpi in kpis)
            {   
                jArrayKpis.Add(kpi.ToJObject());
            }

            return jArrayKpis;

        }

    }
}
