using System;
using System.Linq;
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
                        if (publishedDocumentCollection == null || !publishedDocumentCollection.Any())
                        {
                            return null;
                        }

                        // We need to flush this out of the cache if the underlying file is changed, so we hope the IContentStore returns a decent ChangeMonitor
                        CacheItemPolicy expiry = publishedDocumentCollection.Expiry ?? new CacheItemPolicy();

                        // We support "future" publish dates
                        // Our PublishedDocumentCollection will be evicted from cache when the next "future" date is reached
                        DateTime now = DateTime.UtcNow;

                        // The collection contains potentially thousands of documents
                        // We expect the collection to be sorted by DatePublished, descending
                        // Find the first document with a DatePublished greater than now
                        PublishedDocument publishedDocument = publishedDocumentCollection.TakeWhile(p => p.DatePublished > now).FirstOrDefault();

                        // If the IContentStore has set an expiry *greater* than the next DatePublished, pull that value back into range
                        if (publishedDocument != null && publishedDocument.DatePublished < expiry.AbsoluteExpiration)
                        {
                            expiry.AbsoluteExpiration = publishedDocument.DatePublished;
                        }

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
            return "poster:publishedDocumentCollection:" + fileName.ToLowerInvariant();
        }
    }
}
