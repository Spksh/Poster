using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Poster.Template;

namespace Poster.Core
{
    public class HttpResponseCache
    {
        private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        private readonly IContentStore _store;
        private readonly string _defaultTemplate;
        private readonly Encoding _encoding;

        public HttpResponseCache(IContentStore store, string defaultTemplate = null, Encoding encoding = null)
        {
            _store = store;
            _defaultTemplate = defaultTemplate ?? Default.Template;
            _encoding = encoding ?? Default.Encoding;
        }

        public async Task<HttpResponse> GetCachedResponseAsync(IOwinContext context, string requestPath)
        {
            return await GetCachedResponseAsync(context, _store, _defaultTemplate, requestPath, _encoding);
        }

        public static async Task<HttpResponse> GetCachedResponseAsync(IOwinContext context, IContentStore store, string defaultTemplate, string requestPath, Encoding encoding = null)
        {
            // Bail early if we have nothing to find
            if (requestPath == null)
            {
                return null;
            }

            // Bail early if the Path helper methods below would explode anyway
            // This also fails on directory separators in the segment because we expect only a single segment as a cache key (e.g. "/blog/something/post-one" will be looked up as "something/post-one" and will fail)
            // TODO: Currently, we only support single-segment URLs. We expect the Document identifier to be the only component of the Request Path.
            if (requestPath.ContainsInvalidFileNameChars())
            {
                return null;
            }

            MemoryCache cache = MemoryCache.Default;

            string requestPathKey = HttpResponseCache.CacheKey(requestPath);

            HttpResponse response = cache.Get(requestPathKey) as HttpResponse;

            // TODO: CurrentDictionary of locks? How to clean up crawler spam?
            // private static ConcurrentDictionary<string, object> Locks = new ConcurrentDictionary<string, object>();
            // lock (Locks.GetOrAdd(requestPathKey))

            if (response == null)
            {
                await Lock.WaitAsync();

                try
                {
                    response = cache.Get(requestPathKey) as HttpResponse;

                    if (response == null)
                    {
                        string documentName = requestPath.EndsWith(".md") ? requestPath : requestPath + ".md";

                        Document document = await DocumentCache.GetCachedDocumentAsync(store, documentName, encoding);

                        // We don't cache responses for non-existent files because I don't want to write a CacheItemPolicy that has a ChangeMonitor for the entire directory
                        // Our outgoing 404s include an expires header so clients hopefully don't spam us too much
                        if (document == null)
                        {
                            return null;
                        }

                        // Default template name
                        string templateName = defaultTemplate;

                        // ...but our document might override the default
                        if (document.Metadata.ContainsKey("Template"))
                        {
                            templateName = document.Metadata["Template"].ToString();
                        }

                        // Await for the template we want
                        // This makes sure our template cache entry exists, which prevents a race condition in our CacheEntryChangeMonitor below
                        CompiledTemplate<TemplateContext> template = await TemplateCache.GetCachedTemplateAsync(store, templateName, encoding);

                        // No template is bad!
                        if (template == null)
                        {
                            throw new NullReferenceException($"Template '{templateName}' for Document '{documentName}' was not found");
                        }

                        // Map the document and its metadata to a context object that our ViewEngine understands
                        // This is mostly about converting everything from YAML-deserialized-to-Dictionary<object, object> to a dynamic ExpandoObject
                        TemplateContext templateContext = TemplateContext.FromDocument(document);

                        // Add a dependency on our cached template
                        // If that changes, we obviously need to update all the responses that use that template
                        // NOTE: This will trigger an instant cache eviction if this cache key is *not present* when we create the ChangeMonitor (e.g if the cached template has already been evicted due to memory pressure)
                        templateContext.CacheDependencies.Add(TemplateCache.CacheKey(templateName));

                        // Add a dependency on our cached document
                        // If that changes, we obviously need to update this response
                        // NOTE: This will trigger an instant cache eviction if this cache key is *not present* when we create the ChangeMonitor (e.g if the cached document has already been evicted due to memory pressure)
                        templateContext.CacheDependencies.Add(DocumentCache.CacheKey(documentName));

                        string content = await template.EvaluateAsync(templateContext);

                        response = new HttpResponse
                        {
                            ETag = content.ToSha1Hash(encoding).ToHex(),
                            Content = content
                        };

                        // Empty policy, we rely cache expiry of our dependencies
                        CacheItemPolicy expiry = new CacheItemPolicy();

                        // Add CacheEntryChangeMonitors for all our cache dependencies
                        //  - We depend on our template and document by default
                        //  - The template evaluation may have executed a script that adds cache dependencies  (e.g. we need to depend on changes to the documents collection for "next" and "previous" links)
                        expiry.ChangeMonitors.Add(cache.CreateCacheEntryChangeMonitor(templateContext.CacheDependencies));

                        cache.Set(requestPathKey, response, expiry);
                    }
                }
                finally
                {
                    Lock.Release();
                }
            }

            return response;
        }

        public static string CacheKey(string requestPath)
        {
            // MemoryCache keys are case sensitive
            // We could have collisions in MemoryCache if we don't namespace our own cache keys
            return "lazor:httpResponse:" + requestPath.ToLowerInvariant();
        }
    }
}
