using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poster.MetaWeblogApi
{
    public interface IPostProvider
    {
        Task<List<Post>> GetRecentPosts(int numberOfPosts);

        Task<Post> GetPost(string postId);

        Task<string> AddPost(NewPost post, bool publish);

        Task<bool> EditPost(NewPost post, bool publish);

        Task<bool> DeletePost(string postId);

        Task<List<PostCategory>> GetCategories(string postId);

        Task<bool> SetCategories(string postId, List<PostCategory> categories);
    }
}
