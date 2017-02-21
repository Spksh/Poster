using System.Collections.Generic;

namespace Poster.Core
{
    public interface IDefaultDocument
    {
        string FileName { get; }

        List<string> CacheDependencies { get; }
    }
}
