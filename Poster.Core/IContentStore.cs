using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Core
{
    public interface IContentStore
    {
        Task<T> ReadAsync<T>(string fileName, Func<Stream, Encoding, Task<T>> fromStreamAsync, Encoding encoding = null) where T : class, ICacheable;
    }
}
