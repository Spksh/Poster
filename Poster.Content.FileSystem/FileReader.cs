using System.IO;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Poster.Core;

namespace Poster.Content.FileSystem
{
    public static class FileReader
    {
        public static async Task<Document> ReadDocumentAsync(string filePath, Encoding encoding = null)
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
                Document document = await Document.FromStreamAsync(file, encoding);

                document.Expiry = new CacheItemPolicy();
                document.Expiry.ChangeMonitors.Add(new HostFileChangeMonitor(new[] { filePath }));

                return document;
            }
        }

        public static async Task<Template> ReadTemplateAsync(string filePath, Encoding encoding = null)
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
                Template template = await Template.FromStreamAsync(file, encoding);

                template.Expiry = new CacheItemPolicy();
                template.Expiry.ChangeMonitors.Add(new HostFileChangeMonitor(new[] { filePath }));

                return template;
            }
        }

        public static PublishedDocumentCollection ReadPublishedDocumentCollection(string filePath, Encoding encoding = null)
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
                PublishedDocumentCollection documentCollection = PublishedDocumentCollection.FromStream(file, encoding);

                documentCollection.Expiry = new CacheItemPolicy();
                documentCollection.Expiry.ChangeMonitors.Add(new HostFileChangeMonitor(new[] { filePath }));

                return documentCollection;
            }
        }
    }
}
