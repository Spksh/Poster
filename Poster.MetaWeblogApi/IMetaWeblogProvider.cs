using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poster.MetaWeblogApi
{
    public interface IMetaWeblogProvider
    {
        Task<List<Author>> GetAuthors(string blogId);

        Task<List<Blog>> GetBlogs(string username);

        List<Category> GetCategories(string blogId);

        Task<string> AddCategory(NewCategory category);

        Task<MediaObject> AddMediaObject(string blogId, NewMediaObject mediaObject);

        Task<List<Post>> GetRecentPosts(int numberOfPosts);

        Task<Post> GetPost(string postId);

        Task<string> AddPost(NewPost post, bool publish);

        Task<bool> EditPost(NewPost post, bool publish);

        Task<bool> DeletePost(string postId);

        Task<List<PostCategory>> GetPostCategories(string postId);

        Task<bool> SetPostCategories(string postId, List<PostCategory> categories);
    }
}
