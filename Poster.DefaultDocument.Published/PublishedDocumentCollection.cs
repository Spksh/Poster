using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Poster.Core;
using YamlDotNet.Serialization;

namespace Poster.DefaultDocument.Published
{
    public class PublishedDocumentCollection : List<PublishedDocument>, ICacheable
    {
        public CacheItemPolicy Expiry { get; set; }

        public static Task<PublishedDocumentCollection> FromStream(Stream stream, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(stream, encoding ?? Default.Encoding))
            {
                return Task.FromResult(new DeserializerBuilder().Build().Deserialize<PublishedDocumentCollection>(reader) ?? new PublishedDocumentCollection());
            }
        }
    }
}
