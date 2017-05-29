using TentacleSoftware.XmlRpc.Core;

namespace Poster.MetaWeblogApi
{
    public class Author
    {
        [XmlRpcMember("user_id")]
        public string UserId { get; set; }

        [XmlRpcMember("user_login")]
        public string UserLogin { get; set; }

        [XmlRpcMember("display_name")]
        public string DisplayName { get; set; }
    }
}
