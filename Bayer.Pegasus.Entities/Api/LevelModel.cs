using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities.Api
{
    public class LevelModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("restrictionCodes")]
        public List<string> RestrictionCodes { get; set; }
    }
}
