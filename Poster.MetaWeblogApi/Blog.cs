using TentacleSoftware.XmlRpc.Core;

namespace Poster.MetaWeblogApi
{
    public class Blog
    {
        [XmlRpcMember("blogid")]
        public string BlogId { get; set; }

        [XmlRpcMember("url")]
        public string Url { get; set; }

        [XmlRpcMember("blogName")]
        public string BlogName { get; set; }
    }
}
