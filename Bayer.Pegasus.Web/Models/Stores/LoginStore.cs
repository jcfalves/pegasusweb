using Bayer.Pegasus.Entities.Api;
using System.Collections.Generic;

namespace CacheStrategy.Stores
{
    public class LoginStore : ILoginStore
    {
        public List<LoginViewModel> LoginsStorage { get; set; }

        public LoginStore()
        {
        }

        public IEnumerable<LoginViewModel> List()
        {
            return LoginsStorage;
        }

        public LoginViewModel Get(LoginViewModel cwid)
        {
            throw new System.NotImplementedException();
        }

        IEnumerable<ReturnModel> ILoginStore.List()
        {
            throw new System.NotImplementedException();
        }

        public ReturnModel Get(ReturnModel cwid)
        {
            throw new System.NotImplementedException();
        }

        public void Remove()
        {
            throw new System.NotImplementedException();
        }
    }
}
