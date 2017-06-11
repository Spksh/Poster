using System.Threading.Tasks;

namespace Poster.MetaWeblogApi
{
    public interface IAuthenticationProvider
    {
        Task<bool> IsAuthenticatedAsync(string username, string password);
    }
}
