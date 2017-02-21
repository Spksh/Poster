using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Poster.Core;

namespace Poster.Content.FileSystem
{
    public class FileSystemContentStore : IContentStore
    {
        private readonly string _pathToContent;

        public FileSystemContentStore(string pathToContent)
        {
            _pathToContent = pathToContent;
        }

        public async Task<T> ReadAsync<T>(string fileName, Func<Stream, Encoding, Task<T>> fromStreamAsync, Encoding encoding = null) where T : class, ICacheable
        {
            return await FileReader.ReadAsync<T>(ResolveToPhysicalPath(fileName), fromStreamAsync, encoding);
        }

        public string ResolveToPhysicalPath(string fileName)
        {
            // Safe combine file name with physical path
            // This appends directory separators as necessary
            string filePath = Path.Combine(_pathToContent, fileName);

            // Paranoia
            if (filePath.ContainsInvalidPathChars())
            {
                return null;
            }

            // Paranoia
            // Transform forward slashes into back slashes
            // We don't use Path.GetFullPath because that actually touches the file system
            filePath = new Uri(filePath).LocalPath;

            // Paranoia
            // Bail out if we've ended up with a relative path or somehow got outside our expected root
            if (!filePath.StartsWith(_pathToContent, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Paranoia
            // Bail out if we've ended up with an incorrect file
            if (Path.HasExtension(fileName) && !Path.GetFileName(filePath).Equals(fileName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return filePath;
        }
    }
}
