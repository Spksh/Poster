using TentacleSoftware.XmlRpc.Core;

namespace Poster.MetaWeblogApi
{
    public class Category
    {
        [XmlRpcMember("categoryId")]
        public string CategoryId { get; set; }

        [XmlRpcMember("parentId")]
        public string ParentId { get; set; }

        [XmlRpcMember("categoryName")]
        public string CategoryName { get; set; }

        [XmlRpcMember("categoryDescription")]
        public string CategoryDescription { get; set; }

        /// <summary>
        /// Name of the category, equivalent to categoryName. 
        /// </summary>
        [XmlRpcMember("description")]
        public string Description { get; set; }

        [XmlRpcMember("htmlUrl")]
        public string HtmlUrl { get; set; }

        [XmlRpcMember("rssUrl")]
        public string RssUrl { get; set; }
    }
}
