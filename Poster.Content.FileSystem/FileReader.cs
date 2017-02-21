using System;
using System.IO;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Poster.Core;

namespace Poster.Content.FileSystem
{
    public static class FileReader
    {
        public static async Task<T> ReadAsync<T>(string filePath, Func<Stream, Encoding, Task<T>> fromStreamAsync, Encoding encoding = null) where T : class, ICacheable
        {
            if (filePath == null)
            {
                return null;
            }

            // 404
            if (!File.Exists(filePath))
            {
                return null;
            }

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                T document = await fromStreamAsync(file, encoding);

                document.Expiry = new CacheItemPolicy();
                document.Expiry.ChangeMonitors.Add(new HostFileChangeMonitor(new[] { filePath }));

                return document;
            }
        }
    }
}
