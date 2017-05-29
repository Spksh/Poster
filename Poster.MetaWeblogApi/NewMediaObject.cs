using TentacleSoftware.XmlRpc.Core;

namespace Poster.MetaWeblogApi
{
    public class NewMediaObject
    {
        [XmlRpcMember("name")]
        public string Name { get; set; }

        [XmlRpcMember("type")]
        public string Type { get; set; }

        [XmlRpcMember("bits")]
        public byte[] Bits { get; set; }

        [XmlRpcMember("overwrite")]
        public bool Overwrite { get; set; }
    }
}
