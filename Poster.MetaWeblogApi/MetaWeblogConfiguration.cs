using System.Collections.Generic;
using Newtonsoft.Json;

namespace Poster.MetaWeblogApi
{
    public class MetaWeblogConfiguration
    {
        [JsonProperty("blogs")]
        public List<Blog> Blogs { get; set; }
    }
}
