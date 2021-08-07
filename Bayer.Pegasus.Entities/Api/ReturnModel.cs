using System.Collections.Generic;


namespace Bayer.Pegasus.Entities.Api
{
    public class ReturnModel
    {
        public string nameUser { get; set; }
        public string email { get; set; }
        public bool isInternal { get; set; }
        public string systemName { get; set; }
        public string cwid { get; set; }
        public object dtLastLogin { get; set; }
        public List<RoleModel> roles { get; set; }
        public List<object> complementaryFields { get; set; }
    }
}
