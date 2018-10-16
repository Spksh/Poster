using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Poster.Core;

namespace Poster.MetaWeblogApi
{
    public class JsonMetaWeblogProvider : IMetaWeblogProvider
    {
        private const string ConfigurationSectionName = "metaWeblog";

        public Func<Task<MetaWeblogConfiguration>> GetConfigurationAsync { get; set; }

        public JsonMetaWeblogProvider()
        {

        }

        public JsonMetaWeblogProvider(IConfigurationProvider configurationProvider)
        {
            GetConfigurationAsync = () => configurationProvider.GetConfigurationSectionAsync<MetaWeblogConfiguration>(ConfigurationSectionName);
        }

        public JsonMetaWeblogProvider With(IConfigurationProvider configurationProvider)
        {
            GetConfigurationAsync = () => configurationProvider.GetConfigurationSectionAsync<MetaWeblogConfiguration>(ConfigurationSectionName);

            return this;
        }

        public Task<List<Author>> GetAuthors(string blogId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Blog>> GetBlogs(string username)
        {
            return (await GetConfigurationAsync()).Blogs;
        }

        public List<Category> GetCategories(string blogId)
        {
            throw new NotImplementedException();
        }

        public Task<string> AddCategory(NewCategory category)
        {
            throw new NotImplementedException();
        }

        public Task<MediaObject> AddMediaObject(string blogId, NewMediaObject mediaObject)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> GetRecentPosts(int numberOfPosts)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetPost(string postId)
        {
            throw new NotImplementedException();
        }

        public Task<string> AddPost(NewPost post, bool publish)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditPost(NewPost post, bool publish)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePost(string postId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostCategory>> GetPostCategories(string postId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetPostCategories(string postId, List<PostCategory> categories)
        {
            throw new NotImplementedException();
        }
    }
}
