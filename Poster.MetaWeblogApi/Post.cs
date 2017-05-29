using System;
using System.Collections.Generic;
using TentacleSoftware.XmlRpc.Core;

namespace Poster.MetaWeblogApi
{
    /// <summary>
    /// https://codex.wordpress.org/XML-RPC_MetaWeblog_API#metaWeblog.getPost
    /// </summary>
    public class Post
    {
        [XmlRpcMember("postid")]
        public string PostId { get; set; }

        [XmlRpcMember("title")]
        public string Title { get; set; }

        [XmlRpcMember("description")]
        public string Description { get; set; }

        [XmlRpcMember("link")]
        public string Link { get; set; }

        [XmlRpcMember("dateCreated")]
        public DateTime DateCreated { get; set; }

        [XmlRpcMember("date_created_gmt")]
        public DateTime DateCreatedGmt { get; set; }

        [XmlRpcMember("categories")]
        public List<string> Categories { get; set; }

        [XmlRpcMember("mt_keywords")]
        public string Keywords { get; set; }

        [XmlRpcMember("wp_slug")]
        public string Slug { get; set; }

        /// <summary>
        /// Same as Slug, WLW/OLW supplies both in the post struct
        /// </summary>
        [XmlRpcMember("mt_basename")]
        public string BaseName { get; set; }

        [XmlRpcMember("wp_author_id")]
        public string AuthorId { get; set; }

        [XmlRpcMember("wp_author_display_name")]
        public string AuthorDisplayName { get; set; }
    }
}
