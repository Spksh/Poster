using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Text;
using YamlDotNet.Serialization;

namespace Poster.Core
{
    public class PublishedDocumentCollection : List<PublishedDocument>
    {
        public CacheItemPolicy Expiry { get; set; }

        public static PublishedDocumentCollection FromStream(Stream stream, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(stream, encoding ?? Default.Encoding))
            {
                return new DeserializerBuilder().Build().Deserialize<PublishedDocumentCollection>(reader) ?? new PublishedDocumentCollection();
            }
        }
    }
}
