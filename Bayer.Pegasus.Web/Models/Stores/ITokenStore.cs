using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CacheStrategy.Stores
{
    public interface ITokenStore
    {
        IEnumerable<Token> List();
        Token Get(string accessToken);

        Task<string> FetchToken(string clientHash);

        Task<TokenViewModel> GetTokenFromApi(string clientHash, string accessTokenUrl);
        void Remove();

    }
}
