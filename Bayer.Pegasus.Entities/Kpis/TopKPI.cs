using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bayer.Pegasus.Entities.Kpis
{
    
    public class TopKPI
    {



        [JsonProperty(PropertyName = "type")]
        public string TypeKPI {
            get;
            set;
        }

        [JsonProperty(PropertyName = "code")]
        public string Code {
            get;
            set;
        }

        [JsonProperty(PropertyName = "description")]
        public string Description {
            get;
            set;
        }

        [JsonProperty(PropertyName = "quantity")]
        public decimal Quantity {
            get;
            set;
        }

        [JsonProperty(PropertyName = "value")]
        public decimal Value
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "percentageQuantity")]
        public decimal PercentageQuantity
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "percentageValue")]
        public decimal PercentageValue
        {
            get;
            set;
        }


        public decimal KPI {
            get
            {
                if (this.TypeKPI == "Value")
                {
                    return Value;
                }

                if (this.TypeKPI == "Quantity")
                {
                    return Quantity;
                }

                if (this.TypeKPI == "PercentageQuantity")
                {
                    return PercentageQuantity;
                }

                if (this.TypeKPI == "PercentageValue")
                {
                    return PercentageValue;
                }

                return 0;
            }
        }


        [JsonProperty(PropertyName = "formatKPI")]
        public string KPIFormatted
        {
            get {
                if (this.TypeKPI == "Value")
                {
                    return ValueFormatted;
                }

                if (this.TypeKPI == "Quantity")
                {
                    return QuantityFormatted;
                }

                return "";
            }
            
           
        } 


        [JsonProperty(PropertyName = "formatValue")]
        public string ValueFormatted
        {
            get {
                return Value.ToString("N").Replace(",00", "");
            }
        }


        [JsonProperty(PropertyName = "formatQuantity")]
        public string QuantityFormatted
        {
            get
            {
                return Quantity.ToString("N").Replace(",00", "");
            }
        }

        [JsonProperty(PropertyName = "pct")]
        public string Percentage {
            get;
            set;
        }

        public static void CalculatePercentageByQuantity(List<TopKPI> kpis) {

            if (kpis.Count == 0) {
                return;
            }

            var max = kpis.Max(p => p.Quantity);
            var onePct = max / 100;

           

            foreach (var kpi in kpis) {

                decimal pct = 0;

                if (onePct > 0)
                {
                    pct = (kpi.Quantity / onePct);
                }

                kpi.TypeKPI = "Quantity";
                kpi.Percentage = Math.Round(pct).ToString() + "%";
                
            }
        }
        public static void CalculatePercentage(List<TopKPI> kpis, string typeDataChart)
        {
            if (kpis.Count == 0)
            {
                return;
            }

            if (typeDataChart == "Value") {
                CalculatePercentageByValue(kpis);
            }

            if (typeDataChart == "Quantity")
            {
                CalculatePercentageByQuantity(kpis);
            }

        }
        public static void CalculatePercentageByValue(List<TopKPI> kpis)
        {
            if (kpis.Count == 0)
            {
                return;
            }

            var max = kpis.Max(p => p.Value);
            var onePct = max / 100;

            foreach (var kpi in kpis)
            {
                decimal pct = 0;

                if (onePct > 0) {
                    pct = (kpi.Value / onePct);
                }
                
                kpi.TypeKPI = "Value";
                kpi.Percentage = Math.Round(pct).ToString() + "%";
            }
        }

        public JObject ToJObject()
        {
            JObject jobject = new JObject();            
            jobject["code"] = Code;
            jobject["description"] = Description;
            jobject["value"] = Value;
            jobject["quantity"] = Quantity;
            jobject["kpi"] = KPI;
            jobject["pct"] = Percentage;
            jobject["pctQuatity"] = PercentageQuantity;
            jobject["pctValue"] = PercentageValue;
            return jobject;
        }
    }
}
