using System;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poster.Core
{
    public static class PublishedDocumentCollectionCache
    {
        private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        public static async Task<PublishedDocumentCollection> GetCachedPublishedDocumentCollectionAsync(IContentStore store, string fileName, Encoding encoding = null)
        {
            MemoryCache cache = MemoryCache.Default;

            string publishedDocumentsKey = PublishedDocumentCollectionCache.CacheKey(fileName);

            PublishedDocumentCollection publishedDocumentCollection = cache.Get(publishedDocumentsKey) as PublishedDocumentCollection;

            if (publishedDocumentCollection == null)
            {
                await Lock.WaitAsync();

                try
                {
                    publishedDocumentCollection = cache.Get(publishedDocumentsKey) as PublishedDocumentCollection;

                    if (publishedDocumentCollection == null)
                    {
                        publishedDocumentCollection = store.ReadPublishedDocumentCollection(fileName, encoding);

                        // We don't cache responses for non-existent files 
                        if (publishedDocumentCollection == null)
                        {
                            return null;
                        }

                        // We need to flush this out of the cache if the underlying directory is changed
                        CacheItemPolicy expiry = publishedDocumentCollection.Expiry ?? new CacheItemPolicy { AbsoluteExpiration = DateTime.UtcNow.AddSeconds(15) }; // TODO: Config 

                        cache.Set(publishedDocumentsKey, publishedDocumentCollection, expiry);
                    }
                }
                finally
                {
                    Lock.Release();
                }
            }

            return publishedDocumentCollection;
        }

        public static string CacheKey(string fileName)
        {
            // MemoryCache keys are case sensitive
            // We could have collisions in MemoryCache if we don't namespace our own cache keys
            return "lazor:publishedDocumentCollection:" + fileName.ToLowerInvariant();
        }
    }
}
