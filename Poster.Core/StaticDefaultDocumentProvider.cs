using System.Text;
using System.Threading.Tasks;

namespace Poster.Core
{
    public class StaticDefaultDocumentProvider : IDefaultDocumentProvider
    {
        private readonly string _defaultDocument;

        public StaticDefaultDocumentProvider(string defaultDocument)
        {
            _defaultDocument = defaultDocument;
        }

        public Task<IDefaultDocument> Get(IContentStore store, Encoding encoding)
        {
            return Task.FromResult(new StaticDefaultDocument { FileName = _defaultDocument } as IDefaultDocument);
        }
    }
}
