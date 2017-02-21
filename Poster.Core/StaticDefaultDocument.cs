using System.Collections.Generic;

namespace Poster.Core
{
    public class StaticDefaultDocument : IDefaultDocument
    {
        public string FileName { get; set; }

        public List<string> CacheDependencies { get; set; }

        public StaticDefaultDocument()
        {
            CacheDependencies = new List<string>();
        }
    }
}
