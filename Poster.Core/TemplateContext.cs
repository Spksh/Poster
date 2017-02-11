using System.Collections.Generic;
using Poster.Template;

namespace Poster.Core
{
    public class TemplateContext : IContext
    {
        public dynamic Model { get; set; }

        public dynamic Current { get; set; }

        public List<string> CacheDependencies { get; set; }

        public TemplateContext()
        {
            CacheDependencies = new List<string>();
        }

        public static TemplateContext FromDocument(Document document)
        {
            TemplateContext context = new TemplateContext
            {
                Model = document.Metadata.ToDynamic()
            };

            context.Model.Content = document.Content;

            return context;
        }
    }
}
