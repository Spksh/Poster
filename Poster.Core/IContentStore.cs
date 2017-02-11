using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Core
{
    public interface IContentStore
    {
        Task<Document> ReadDocumentAsync(string fileName, Encoding encoding = null);

        Task<Template> ReadTemplateAsync(string templateName, Encoding encoding = null);

        PublishedDocumentCollection ReadPublishedDocumentCollection(string fileName, Encoding encoding = null);
    }
}
