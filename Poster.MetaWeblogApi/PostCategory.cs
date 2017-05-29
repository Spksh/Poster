using TentacleSoftware.XmlRpc.Core;

namespace Poster.MetaWeblogApi
{
    public class PostCategory
    {
        [XmlRpcMember("categoryId")]
        public string CategoryId { get; set; }

        [XmlRpcMember("categoryName")]
        public string CategoryName { get; set; }

        [XmlRpcMember("isPrimary")]
        public bool IsPrimary { get; set; }
    }
}
