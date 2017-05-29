using TentacleSoftware.XmlRpc.Core;

namespace Poster.MetaWeblogApi
{
    public class NewCategory
    {
        [XmlRpcMember("name")]
        public string Name { get; set; }

        [XmlRpcMember("description")]
        public string Description { get; set; }

        /// <summary>
        /// wp.newCategory struct expects an i4 for parent_id. 
        /// In most cases, WLW/OLW uses string where "id" values are defined as i4 for WordPress methods. 
        /// In *this* case, WLW/OLW supplies us with an i4 instead. 
        /// </summary>
        [XmlRpcMember("parent_id")]
        public int ParentId { get; set; }

        [XmlRpcMember("slug")]
        public string Slug { get; set; }
    }
}
