using Bayer.Pegasus.Entities.Api;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Web.Controllers
{
    public interface ITokenController
    {
        Task<string> FetchToken(string clientHash);

        Task<TokenViewModel> GetTokenFromApi(string clientHash, string accessTokenUrl);
    }
}
