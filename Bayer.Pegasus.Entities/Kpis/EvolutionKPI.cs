using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Bayer.Pegasus.Entities.Kpis
{
    public class EvolutionKPI
    {

        public EvolutionKPI() {
            KPIData = new System.Collections.Generic.List<Tuple<int, decimal>>();
        }


        [JsonProperty(PropertyName = "label")]
        public string Label {
            get;
            set;
        }

        [JsonProperty(PropertyName = "color")]
        public string Color {
            get;
            set;
        }


        [JsonProperty(PropertyName = "data")]
        public System.Collections.Generic.List<Tuple<int, decimal>> KPIData {
            get;
            set;
        }


        public JArray ToJArray() {
            JArray data = new JArray();

            foreach (var kpi in KPIData)
            {
                JArray dataItem = new JArray();

                dataItem.Add(kpi.Item1);
                dataItem.Add(kpi.Item2);

                data.Add(dataItem);
            }

            return data;
        }

        public static List<EvolutionKPI> FromRawData(List<Tuple<string, int, decimal>> rawData) {

            List<Entities.Kpis.EvolutionKPI> kpis = new List<EvolutionKPI>();
            var groups = rawData.Select(p => p.Item1).Distinct();

            foreach (var group in groups)
            {
                var kpi = new Bayer.Pegasus.Entities.Kpis.EvolutionKPI();
                kpi.Label = group;

                var selectItens = rawData.Where(p => p.Item1 == group).ToList();

                foreach (var item in selectItens)
                {
                    var order = item.Item2;
                    var qtd = item.Item3;

                    kpi.KPIData.Add(new Tuple<int, decimal>(order, qtd));
                }

                kpis.Add(kpi);
            }

            return kpis;
        }

        public JObject ToJObject() {
            JObject jobject = new JObject();

            if (!String.IsNullOrEmpty(Label)) {
                jobject["label"] = Label;
            }

            if (!String.IsNullOrEmpty(Color))
            {
                jobject["color"] = Color;
            }

            JArray data = new JArray();

            foreach (var kpi in KPIData) {
                JArray dataItem = new JArray();

                dataItem.Add(kpi.Item1);
                dataItem.Add(kpi.Item2);
                
                data.Add(dataItem);
            }

            

            jobject["data"] = data;

            return jobject;
        }

        

    }
}
