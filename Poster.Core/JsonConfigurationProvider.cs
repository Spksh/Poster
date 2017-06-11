using System;
using System.IO;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Poster.Core
{
    // FUTURE: We're assuming we only read one file for the whole application; we could break up the Lock by file to gain efficiency
    public class JsonConfigurationProvider : IConfigurationProvider
    {
        private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        public string FilePath { get; set; }

        public Func<ObjectCache> GetCache { get; set; }

        public JsonConfigurationProvider()
        {
        }

        public JsonConfigurationProvider(string filePath, Func<ObjectCache> getCache = null)
        {
            GetCache = getCache ?? (() => MemoryCache.Default);
            FilePath = filePath;
        }

        public JsonConfigurationProvider With(Func<ObjectCache> getCache)
        {
            GetCache = getCache;

            return this;
        }

        public JsonConfigurationProvider With(string filePath)
        {
            FilePath = filePath;

            return this;
        }

        public async Task<TSection> GetConfigurationSectionAsync<TSection>(string name) where TSection : class
        {
            ObjectCache cache = GetCache();

            // Use FilePath as cache key to separate potentially duplicate sections
            string sectionKey = FilePath + ":" + name;

            TSection section = cache.Get(sectionKey) as TSection;

            if (section == null)
            {
                await Lock.WaitAsync();

                try
                {
                    section = cache.Get(sectionKey) as TSection;

                    if (section == null)
                    {
                        JObject configuration = cache.Get(FilePath) as JObject;

                        if (configuration == null)
                        {
                            using (FileStream file = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                            using (StreamReader reader = new StreamReader(file))
                            using (JsonReader json = new JsonTextReader(reader))
                            {
                                configuration = await JObject.LoadAsync(json);
                            }

                            // Expire cached JObject if our configuration file changes
                            CacheItemPolicy configurationPolicy = new CacheItemPolicy();
                            configurationPolicy.ChangeMonitors.Add(new HostFileChangeMonitor(new[] { FilePath }));

                            cache.Set(FilePath, configuration, configurationPolicy);
                        }

                        section = configuration[name].ToObject<TSection>();

                        // Expire cached section if our cached JObject expires
                        CacheItemPolicy sectionPolicy = new CacheItemPolicy();
                        sectionPolicy.ChangeMonitors.Add(cache.CreateCacheEntryChangeMonitor(new[] { FilePath }));

                        cache.Set(sectionKey, section, sectionPolicy);
                    }
                }
                finally
                {
                    Lock.Release();
                }
            }

            return section;
        }
    }
}
