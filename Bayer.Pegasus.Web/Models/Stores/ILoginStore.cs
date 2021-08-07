using Bayer.Pegasus.Entities.Api;
using System.Collections.Generic;

namespace CacheStrategy.Stores
{
    public interface ILoginStore
    {
        IEnumerable<ReturnModel> List();
        ReturnModel Get(ReturnModel cwid);

        void Remove();
    }
}
