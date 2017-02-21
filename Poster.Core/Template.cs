using System.IO;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Core
{
    public class Template : ICacheable
    {
        public CacheItemPolicy Expiry { get; set; }

        public string Content { get; set; }

        public static async Task<Template> FromStreamAsync(Stream stream, Encoding encoding = null)
        {
            using (StreamReader reader = new StreamReader(stream, encoding ?? Default.Encoding))
            {
                string content = await reader.ReadToEndAsync();

                return new Template
                {
                    Content = content
                };
            }
        }
    }
}
