using Newtonsoft.Json;


namespace Bayer.Pegasus.Entities.Api
{
    public class RoleModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public LevelModel Level { get; set; }
    }
}
