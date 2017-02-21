using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poster.Core;

namespace Poster.DefaultDocument.Published
{
    public class PublishedDefaultDocumentProvider : IDefaultDocumentProvider
    {
        private readonly string _publishedDocumentsFile;

        public PublishedDefaultDocumentProvider(string publishedDocumentsFile)
        {
            _publishedDocumentsFile = publishedDocumentsFile;
        }

        public async Task<Core.IDefaultDocument> Get(IContentStore store, Encoding encoding)
        {
            List<PublishedDocument> published = await PublishedDocumentCollectionCache.GetCachedPublishedDocumentCollectionAsync(store, _publishedDocumentsFile, encoding);

            PublishedDefaultDocument document = new PublishedDefaultDocument();

            if (published != null && published.Any())
            {
                // We'll need to add a dependency on the list of published documents
                // We support "future" publish dates
                // Our PublishedDocumentCollection will be evicted from cache when the next "future" date is reached, or if someone edits the file
                // That will cause us to rebuild this response
                document.CacheDependencies.Add(PublishedDocumentCollectionCache.CacheKey(_publishedDocumentsFile));

                // Get the latest published document that has a published date that's not in the future
                DateTime now = DateTime.UtcNow;

                // The collection contains potentially thousands of documents
                // We expect the collection to be sorted by DatePublished, descending
                // Find the first document with a DatePublished before now
                document.FileName = published.First(p => p.DatePublished <= now).Name;
            }

            return document;
        }
    }
}
