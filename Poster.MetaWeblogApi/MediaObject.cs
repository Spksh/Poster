using TentacleSoftware.XmlRpc.Core;

namespace Poster.MetaWeblogApi
{
    public class MediaObject
    {
        [XmlRpcMember("id")]
        public string Id { get; set; }

        [XmlRpcMember("file")]
        public string File { get; set; }

        [XmlRpcMember("url")]
        public string Url { get; set; }

        [XmlRpcMember("type")]
        public string Type { get; set; }
    }
}
