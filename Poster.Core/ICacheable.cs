using System.Runtime.Caching;

namespace Poster.Core
{
    public interface ICacheable
    {
        CacheItemPolicy Expiry { get; set; }
    }
}
