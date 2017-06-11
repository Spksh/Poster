using System.Threading.Tasks;

namespace Poster.Core
{
    public interface IConfigurationProvider
    {
        Task<T> GetConfigurationSectionAsync<T>(string section) where T : class;
    }
}
