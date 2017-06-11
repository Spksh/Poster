using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TentacleSoftware.XmlRpc.Core;

namespace Poster.MetaWeblogApi
{
    // https://github.com/OpenLiveWriter/OpenLiveWriter/blob/master/src/managed/OpenLiveWriter.BlogClient/Clients/MetaweblogClient.cs
    // - 403 shows "user name or password is incorrect"
    // - 3001 shows "user name or password is incorrect"
    // http://xmlrpc.scripting.com/spec.html
    // https://codex.wordpress.org/XML-RPC_MetaWeblog_API
    // https://codex.wordpress.org/XML-RPC_WordPress_API
    // https://codex.wordpress.org/XML-RPC_MovableType_API
    public class MetaWeblogApiResponder
    {
        private IAuthenticationProvider _authenticationProvider;

        //public delegate Task<List<Blog>> GetUsersBlogsFunc(string appkey, string username, string password);

        //public delegate Task<List<Category>> GetCategoriesFunc(string blogId, string username, string password);

        //public delegate Task<string> NewCategoryFunc(string blogId, string username, string password, NewCategory category);

        //public delegate Task<List<Post>> GetRecentPostsFunc(string blogId, string username, string password, int numberOfPosts);

        //public delegate Task<Post> GetPostFunc(string postId, string username, string password);

        //public delegate Task<string> NewPostFunc(string blogId, string username, string password, NewPost content, bool publish);

        //public delegate Task<bool> EditPostFunc(string postId, string username, string password, NewPost content, bool publish);

        //public delegate Task<bool> DeletePostFunc(string appkey, string postId, string username, string password, bool publish);

        //public delegate Task<List<Author>> GetAuthorsFunc(string blogId, string username, string password);

        //public delegate Task<List<PostCategory>> GetPostCategoriesFunc(string postId, string username, string password);

        //public delegate Task<bool> SetPostCategorieFuncs(string postId, string username, string password, List<PostCategory> categories);

        //public delegate Task<MediaObject> NewMediaObjectFunc(string blogId, string username, string password, NewMediaObject data);


        //private GetUsersBlogsFunc _getUsersBlogs;
        //private GetCategoriesFunc _getCategories;
        //private NewCategoryFunc _newCategory;
        //private GetRecentPostsFunc _getRecentPosts;
        //private GetPostFunc _getPost;
        //private NewPostFunc _newPost;
        //private EditPostFunc _editPost;
        //private DeletePostFunc _deletePost;
        //private GetAuthorsFunc _getAuthors;
        //private GetPostCategoriesFunc _getPostCategories;
        //private SetPostCategorieFuncs _setPostCategorie;
        //private NewMediaObjectFunc _newMediaObject;

        //[XmlRpcIgnore]
        //public MetaWeblogApiResponder With(IBlogProvider provider)
        //{
        //    _getUsersBlogs = provider.GetUsersBlogs;
        //    _getCategories = provider.GetCategories;
        //    _newCategory = provider.NewCategory;
        //    _getRecentPosts = provider.GetRecentPosts;
        //    _getPost = provider.GetPost;
        //    _newPost = provider.NewPost;
        //    _editPost = provider.EditPost;
        //    _deletePost = provider.DeletePost;
        //    _getAuthors = provider.GetAuthors;
        //    _getPostCategories = provider.GetPostCategories;
        //    _setPostCategorie = provider.SetPostCategories;
        //    _newMediaObject = provider.NewMediaObject;

        //    return this;
        //}

        //public MetaWeblogApiResponder WithGetUsersBlogs(GetUsersBlogsFunc getUsersBlogs)
        //{
        //    _getUsersBlogs = getUsersBlogs;

        //    return this;
        //}

        public MetaWeblogApiResponder()
        {
        }

        public MetaWeblogApiResponder(IAuthenticationProvider authenticationProvider)
        {
            _authenticationProvider = authenticationProvider;
        }

        public MetaWeblogApiResponder With(IAuthenticationProvider authenticationProvider)
        {
            _authenticationProvider = authenticationProvider;

            return this;
        }

        [XmlRpcMethod("metaWeblog.getUsersBlogs")]
        [XmlRpcMethod("blogger.getUsersBlogs")]
        public async Task<List<Blog>> GetUsersBlogs(string appkey, string username, string password)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return new List<Blog>
            {
                new Blog
                {
                    BlogId = "1",
                    BlogName = "Blog 1",
                    Url = "/blog1"
                },
                new Blog
                {
                    BlogId = "2",
                    BlogName = "Blog 2",
                    Url = ""
                }
            };

            //return await _getUsersBlogs(appkey, username, password);
        }

        [XmlRpcMethod("wp.getCategories")]
        [XmlRpcMethod("metaWeblog.getCategories")]
        public async Task<List<Category>> GetCategories(string blogId, string username, string password)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return new List<Category>
            {
                new Category
                {
                    CategoryId = "1",
                    ParentId = "",
                    CategoryName = "Category 1",
                    CategoryDescription = "Description for Category 1",
                    Description = "Category 1",
                    HtmlUrl = "/category1",
                    RssUrl = "/category1/rss"
                },
                new Category
                {
                    CategoryId = "2",
                    ParentId = "",
                    CategoryName = "Category 2",
                    CategoryDescription = "Description for Category 2",
                    Description = "Category 2",
                    HtmlUrl = "/category2",
                    RssUrl = "/category2/rss"
                },
                new Category
                {
                    CategoryId = "3",
                    ParentId = "1",
                    CategoryName = "Category 3",
                    CategoryDescription = "Description for Category 3",
                    Description = "Category 3",
                    HtmlUrl = "/category3",
                    RssUrl = "/category3/rss"
                },
                new Category
                {
                    CategoryId = "4",
                    ParentId = "1",
                    CategoryName = "Category 4",
                    CategoryDescription = "Description for Category 4",
                    Description = "Category 4",
                    HtmlUrl = "/category3",
                    RssUrl = "/category3/rss"
                }
            };
        }

        [XmlRpcMethod("wp.newCategory")]
        public async Task<string> NewCategory(string blogId, string username, string password, NewCategory category)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return "5";
        }

        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        public async Task<List<Post>> GetRecentPosts(string blogId, string username, string password, int numberOfPosts)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return new List<Post>
            {
                new Post
                {
                    PostId = "1",
                    Title = "Post 1",
                    Description = "<p>This is post 1.</p>",
                    Categories = new List<string>
                    {
                        "Category 1",
                        "Category 2"
                    },
                    DateCreated = DateTime.Now,
                    Link = "/post-1",
                    AuthorId = "1",
                    AuthorDisplayName = "Author 1",
                    Keywords = "Tag1, Tag2",
                    Slug = "post-1"
                }
            };
        }

        [XmlRpcMethod("metaWeblog.getPost")]
        public async Task<Post> GetPost(string postId, string username, string password)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return new Post
            {
                PostId = "1",
                Title = "Post 1",
                Description = "<p>This is post 1.</p>",
                Categories = new List<string>
                {
                    "Category 1",
                    "Category 2"
                },
                DateCreated = DateTime.Now,
                Link = "/post-1",
                AuthorId = "1",
                AuthorDisplayName = "Author 1",
                Keywords = "Tag1, Tag2",
                Slug = "post-1"
            };
        }

        [XmlRpcMethod("metaWeblog.newPost")]
        public async Task<string> NewPost(string blogId, string username, string password, NewPost content, bool publish)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return "2";
        }

        [XmlRpcMethod("metaWeblog.editPost")]
        public async Task<bool> EditPost(string postId, string username, string password, NewPost content, bool publish)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return true;
        }

        [XmlRpcMethod("blogger.deletePost")]
        [XmlRpcMethod("metaWeblog.deletePost ")]
        public async Task<bool> DeletePost(string appkey, string postId, string username, string password, bool publish)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return true;
        }

        [XmlRpcMethod("wp.getAuthors")]
        public async Task<List<Author>> GetAuthors(string blogId, string username, string password)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return new List<Author>
            {
                new Author
                {
                    UserId = "0",
                    UserLogin = "author0",
                    DisplayName = "Author 0"
                },
                new Author
                {
                    UserId = "1",
                    UserLogin = "author1",
                    DisplayName = "Author 1"
                },
                new Author
                {
                    UserId = "2",
                    UserLogin = "author2",
                    DisplayName = "Author 2"
                }
            };
        }

        [XmlRpcMethod("mt.getPostCategories")]
        public async Task<List<PostCategory>> GetPostCategories(string postId, string username, string password)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return new List<PostCategory>
            {
                new PostCategory
                {
                    CategoryId = "1",
                    CategoryName = "Category 1",
                    IsPrimary = true
                },
                new PostCategory
                {
                    CategoryId = "2",
                    CategoryName = "Category 2",
                }
            };
        }

        [XmlRpcMethod("mt.setPostCategories")]
        public async Task<bool> SetPostCategories(string postId, string username, string password, List<PostCategory> categories)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return true;
        }

        [XmlRpcMethod("metaWeblog.newMediaObject")]
        public async Task<MediaObject> NewMediaObject(string blogId, string username, string password, NewMediaObject data)
        {
            if (!await _authenticationProvider.IsAuthenticatedAsync(username, password))
            {
                throw new XmlRpcException(403, "Forbidden");
            }

            return new MediaObject
            {
                Id = "1",
                File = data.Name,
                Type = data.Type,
                Url = "/" + data.Name
            };
        }

        // metaWeblog.getTemplate
        // metaWeblog.setTemplate
    }
}
