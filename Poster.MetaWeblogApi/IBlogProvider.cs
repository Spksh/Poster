using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poster.MetaWeblogApi
{
    public interface IBlogProvider
    {
        Task<List<Blog>> GetUsersBlogs(string appkey, string username, string password);

        Task<List<Category>> GetCategories(string blogId, string username, string password);

        Task<string> NewCategory(string blogId, string username, string password, NewCategory category);
        
        Task<List<Post>> GetRecentPosts(string blogId, string username, string password, int numberOfPosts);

        Task<Post> GetPost(string postId, string username, string password);

        Task<string> NewPost(string blogId, string username, string password, NewPost content, bool publish);

        Task<bool> EditPost(string postId, string username, string password, NewPost content, bool publish);

        Task<bool> DeletePost(string appkey, string postId, string username, string password, bool publish);

        Task<List<Author>> GetAuthors(string blogId, string username, string password);

        Task<List<PostCategory>> GetPostCategories(string postId, string username, string password);

        Task<bool> SetPostCategories(string postId, string username, string password, List<PostCategory> categories);

        Task<MediaObject> NewMediaObject(string blogId, string username, string password, NewMediaObject data);
    }
}
