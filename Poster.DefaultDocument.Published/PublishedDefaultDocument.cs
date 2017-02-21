using System.Collections.Generic;
using Poster.Core;

namespace Poster.DefaultDocument.Published
{
    public class PublishedDefaultDocument : IDefaultDocument
    {
        public string FileName { get; set; }

        public List<string> CacheDependencies { get; set; }

        public PublishedDefaultDocument()
        {
            CacheDependencies = new List<string>();
        }
    }
}
