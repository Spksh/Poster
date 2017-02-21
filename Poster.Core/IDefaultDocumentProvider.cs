using System.Text;
using System.Threading.Tasks;

namespace Poster.Core
{
    public interface IDefaultDocumentProvider
    {
        Task<IDefaultDocument> Get(IContentStore store, Encoding encoding);
    }
}
