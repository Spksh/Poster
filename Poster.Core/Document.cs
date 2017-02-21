using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using CommonMark;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Poster.Core
{
    public class Document : ICacheable
    {
        public CacheItemPolicy Expiry { get; set; }

        public Dictionary<object, object> Metadata { get; set; }

        public string Content { get; set; }

        /// <summary>
        /// We expect a text stream formatted with a YAML "document" followed by Markdown content
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<Document> FromStreamAsync(Stream stream, Encoding encoding = null)
        {
            using (StreamReader reader = new StreamReader(stream, encoding ?? Default.Encoding))
            {
                // Set up YAML parser
                // We only want to read the first YAML "document"
                // So we instruct the parser to read StreamStart, then DocumentStart
                Parser parser = new Parser(reader);
                parser.Expect<StreamStart>();
                parser.Expect<DocumentStart>();

                // Stream has been advanced to where we expect the front matter to be
                Dictionary<object, object> metadata = new DeserializerBuilder().Build().Deserialize<Dictionary<object, object>>(parser) ?? new Dictionary<object, object>();

                // This leaves the stream at the end of the front matter
                // Now we can read the remainder of the file as straight markdown from this position
                string content = await reader.ReadToEndAsync();

                return new Document
                {
                    Metadata = metadata,
                    Content = CommonMarkConverter.Convert(content)
                };
            }
        }
    }
}
