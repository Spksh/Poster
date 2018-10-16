using System.Collections.Generic;
using Newtonsoft.Json;

namespace Poster.Core
{
    public class AuthenticationConfiguration
    {
        [JsonProperty("users")]
        public List<User> Users { get; set; }
    }
}
