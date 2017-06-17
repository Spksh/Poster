using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poster.MetaWeblogApi
{
    public interface IAuthorProvider
    {
        Task<List<Author>> GetAuthors(string blogId);
    }
}
