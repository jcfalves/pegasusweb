using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bayer.Pegasus.Entities.Kpis
{
    public class ClientLocationKPI
    {
       
        [JsonProperty(PropertyName = "code_city_IBGE")]
        public string IBGECityCode
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "title")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "code_city_uf")]
        public string UF
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "qt_acquired")]
        public long Acquired
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "qt_loyal")]
        public long Loyal
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "qt_lost")]
        public long Lost
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "qt_reacquired")]
        public long Reacquired
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "qt_retained")]
        public long Retained
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "latitude")]
        public decimal Latitude
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "longitude")]
        public decimal Longitude
        {
            get;
            set;
        }
    }
}
