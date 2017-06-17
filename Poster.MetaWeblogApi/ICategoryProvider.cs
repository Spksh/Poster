using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poster.MetaWeblogApi
{
    public interface ICategoryProvider
    {
        List<Category> GetCategories(string blogId);

        Task<string> AddCategory(NewCategory category);
    }
}
