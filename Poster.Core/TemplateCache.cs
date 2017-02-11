using System;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Poster.Template;

namespace Poster.Core
{
    public static class TemplateCache
    {
        private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        public static async Task<CompiledTemplate<TemplateContext>> GetCachedTemplateAsync(IContentStore store, string templateName, Encoding encoding = null)
        {
            // Bail early if we have nothing to find
            if (string.IsNullOrWhiteSpace(templateName))
            {
                return null;
            }

            // Bail early if the Path helper methods below would explode anyway
            if (templateName.ContainsInvalidFileNameChars())
            {
                return null;
            }

            MemoryCache cache = MemoryCache.Default;

            string templateKey = TemplateCache.CacheKey(templateName);

            CompiledTemplate<TemplateContext> compiledTemplate = cache.Get(templateKey) as CompiledTemplate<TemplateContext>;

            if (compiledTemplate == null)
            {
                await Lock.WaitAsync();

                try
                {
                    compiledTemplate = cache.Get(templateKey) as CompiledTemplate<TemplateContext>;

                    if (compiledTemplate == null)
                    {
                        Template template = await store.ReadTemplateAsync(templateName, encoding);

                        // We don't cache responses for non-existent files because I don't want to write a CacheItemPolicy that has a ChangeMonitor for the entire directory
                        if (template == null)
                        {
                            return null;
                        }

                        compiledTemplate = CompiledTemplate<TemplateContext>.Compile(template.Content);

                        // We need to flush this out of the cache if the underlying template file is changed
                        // The response cache also looks for changes to this cache key
                        CacheItemPolicy expiry = template.Expiry ?? new CacheItemPolicy { AbsoluteExpiration = DateTime.UtcNow.AddSeconds(15) }; // TODO: Config 

                        cache.Set(templateKey, compiledTemplate, expiry);
                    }
                }
                finally
                {
                    Lock.Release();
                }
            }

            return compiledTemplate;
        }

        public static string CacheKey(string templateName)
        {
            // MemoryCache keys are case sensitive
            // We could have collisions in MemoryCache if we don't namespace our own cache keys
            return "lazor:template:" + templateName.ToLowerInvariant();
        }
    }
}
