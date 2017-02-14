using System;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poster.Core
{
    public static class DocumentCache
    {
        private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        public static async Task<Document> GetCachedDocumentAsync(IContentStore store, string documentName, Encoding encoding = null)
        {
            // Bail early if we have nothing to find
            if (string.IsNullOrWhiteSpace(documentName))
            {
                return null;
            }

            // Bail early if the Path helper methods below would explode anyway
            if (documentName.ContainsInvalidFileNameChars())
            {
                return null;
            }

            MemoryCache cache = MemoryCache.Default;

            string documentKey = DocumentCache.CacheKey(documentName);

            Document document = cache.Get(documentKey) as Document;

            if (document == null)
            {
                await Lock.WaitAsync();

                try
                {
                    document = cache.Get(documentKey) as Document;

                    if (document == null)
                    {
                        document = await store.ReadDocumentAsync(documentName, encoding);

                        // We don't cache responses for non-existent files because I don't want to write a CacheItemPolicy that has a ChangeMonitor for the entire directory
                        if (document == null)
                        {
                            return null;
                        }

                        // We need to flush this out of the cache if the underlying document file is changed
                        // The response cache also looks for changes to this cache key
                        CacheItemPolicy expiry = document.Expiry ?? new CacheItemPolicy { AbsoluteExpiration = DateTime.UtcNow.AddSeconds(15) }; // TODO: Config 

                        cache.Set(documentKey, document, expiry);
                    }
                }
                finally
                {
                    Lock.Release();
                }
            }

            return document;
        }

        public static string CacheKey(string documentName)
        {
            // MemoryCache keys are case sensitive
            // We could have collisions in MemoryCache if we don't namespace our own cache keys
            return "poster:document:" + documentName.ToLowerInvariant();
        }
    }
}
