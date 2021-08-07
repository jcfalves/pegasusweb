using Bayer.Pegasus.Entities.Api;
using CacheStrategy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CacheStrategy.Stores
{
    public class TokenStore : ITokenStore
    {
        public List<Token> TokensStore { get; set; }

        public TokenStore()
        {
            
        }

        public IEnumerable<Token> List()
        {
            return TokensStore;
        }

        public Token Get(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> FetchToken(string clientHash)
        {
            throw new System.NotImplementedException();
        }

        public Task<TokenViewModel> GetTokenFromApi(string clientHash, string accessTokenUrl)
        {
            throw new System.NotImplementedException();
        }

        public void Remove()
        {
            throw new System.NotImplementedException();
        }
    }
}
